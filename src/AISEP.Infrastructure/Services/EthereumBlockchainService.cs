using AISEP.Application.Configuration;
using AISEP.Domain.Interfaces;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Nethereum.Web3;
using Nethereum.Web3.Accounts;
using Nethereum.Contracts;
using Nethereum.RPC.Eth.DTOs;
using Nethereum.Hex.HexTypes;
using System.Numerics;

namespace AISEP.Infrastructure.Services;

/// <summary>
/// Real Ethereum blockchain service implementation using Nethereum.
/// Interacts with the DocumentHashRegistry smart contract on Ethereum Sepolia testnet.
/// </summary>
public class EthereumBlockchainService : IBlockchainService
{
    private readonly ILogger<EthereumBlockchainService> _logger;
    private readonly EthereumSettings _settings;
    private readonly Web3 _web3;
    private readonly Account _account;
    private readonly Contract _contract;

    public EthereumBlockchainService(
        ILogger<EthereumBlockchainService> logger,
        IOptions<EthereumSettings> settings)
    {
        _logger = logger;
        _settings = settings.Value;

        // Initialize account from private key
        _account = new Account(_settings.PrivateKey, new BigInteger(int.Parse(_settings.ChainId)));
        
        // Initialize Web3 with account
        _web3 = new Web3(_account, _settings.RpcUrl);
        
        // Initialize contract
        _contract = _web3.Eth.GetContract(_settings.ContractAbi, _settings.ContractAddress);

        _logger.LogInformation(
            "EthereumBlockchainService initialized: RpcUrl={RpcUrl}, ContractAddress={ContractAddress}, Account={Account}",
            _settings.RpcUrl, _settings.ContractAddress, _account.Address);
    }

    /// <summary>
    /// Submit a file hash to the blockchain smart contract.
    /// Normalizes hash (adds 0x prefix and converts to lowercase).
    /// Estimates gas and adds 20% buffer.
    /// Waits for transaction confirmation (max 60 attempts, 3s interval).
    /// </summary>
    public async Task<string> SubmitHashAsync(string fileHash, BlockchainSubmitMeta metadata, CancellationToken ct = default)
    {
        try
        {
            // Normalize hash: add 0x prefix if missing and convert to lowercase
            var normalizedHash = NormalizeHash(fileHash);

            _logger.LogInformation(
                "Submitting hash to blockchain: DocID={DocumentID}, Hash={Hash}, NormalizedHash={NormalizedHash}",
                metadata.DocumentID, fileHash, normalizedHash);

            // Get the storeHash function
            var storeHashFunction = _contract.GetFunction("storeHash");

            // Estimate gas with 20% buffer
            var gasEstimate = await storeHashFunction.EstimateGasAsync(_account.Address, null, null, normalizedHash);
            var gasLimit = new HexBigInteger(gasEstimate.Value * 120 / 100); // Add 20% buffer

            _logger.LogInformation(
                "Gas estimation: Estimated={Estimated}, WithBuffer={WithBuffer}",
                gasEstimate.Value, gasLimit.Value);

            // Send transaction
            var txHash = await storeHashFunction.SendTransactionAsync(
                _account.Address,
                gasLimit,
                null, // gasPrice (null = use network default)
                null, // value
                normalizedHash);

            _logger.LogInformation(
                "Transaction sent: TxHash={TxHash}, waiting for confirmation...",
                txHash);

            // Wait for transaction to be mined (max 60 attempts x 3 seconds = 3 minutes)
            const int maxAttempts = 60;
            const int delaySeconds = 3;
            
            for (int i = 0; i < maxAttempts; i++)
            {
                ct.ThrowIfCancellationRequested();

                var receipt = await _web3.Eth.Transactions.GetTransactionReceipt.SendRequestAsync(txHash);
                
                if (receipt != null)
                {
                    _logger.LogInformation(
                        "Transaction confirmed: TxHash={TxHash}, BlockNumber={BlockNumber}, Status={Status}",
                        txHash, receipt.BlockNumber?.Value, receipt.Status?.Value);

                    if (receipt.Status?.Value == 0)
                    {
                        _logger.LogError("Transaction failed: TxHash={TxHash}", txHash);
                        throw new Exception($"Transaction failed: {txHash}");
                    }

                    return txHash;
                }

                _logger.LogDebug(
                    "Waiting for confirmation... Attempt {Attempt}/{MaxAttempts}",
                    i + 1, maxAttempts);

                await Task.Delay(TimeSpan.FromSeconds(delaySeconds), ct);
            }

            _logger.LogWarning(
                "Transaction not confirmed after {MaxAttempts} attempts: TxHash={TxHash}",
                maxAttempts, txHash);

            // Return txHash even if not confirmed yet - it's still pending
            return txHash;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex,
                "Error submitting hash to blockchain: DocID={DocumentID}, Hash={Hash}",
                metadata.DocumentID, fileHash);
            throw;
        }
    }

    /// <summary>
    /// Verify that a file hash exists on-chain by calling the verifyHash view function.
    /// Returns true if timestamp > 0, false otherwise.
    /// </summary>
    public async Task<bool> VerifyHashAsync(string fileHash, CancellationToken ct = default)
    {
        try
        {
            var normalizedHash = NormalizeHash(fileHash);

            _logger.LogInformation(
                "Verifying hash on blockchain: Hash={Hash}, NormalizedHash={NormalizedHash}",
                fileHash, normalizedHash);

            // Call verifyHash view function
            var verifyHashFunction = _contract.GetFunction("verifyHash");
            var timestamp = await verifyHashFunction.CallAsync<BigInteger>(normalizedHash);

            var exists = timestamp > 0;

            _logger.LogInformation(
                "Hash verification result: Hash={Hash}, Exists={Exists}, Timestamp={Timestamp}",
                normalizedHash, exists, timestamp);

            return exists;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error verifying hash on blockchain: Hash={Hash}", fileHash);
            throw;
        }
    }

    /// <summary>
    /// Check the status of a previously submitted transaction.
    /// </summary>
    public async Task<BlockchainTxStatusResult> GetTxStatusAsync(string txHash, CancellationToken ct = default)
    {
        try
        {
            _logger.LogInformation("Checking transaction status: TxHash={TxHash}", txHash);

            var receipt = await _web3.Eth.Transactions.GetTransactionReceipt.SendRequestAsync(txHash);

            if (receipt == null)
            {
                // Transaction not yet mined
                return new BlockchainTxStatusResult
                {
                    Status = "Pending",
                    BlockNumber = null,
                    ConfirmedAt = null
                };
            }

            // Transaction mined
            var status = receipt.Status?.Value == 1 ? "Confirmed" : "Failed";
            var blockNumber = receipt.BlockNumber?.Value.ToString();

            // Try to get block timestamp
            DateTime? confirmedAt = null;
            if (receipt.BlockNumber != null)
            {
                try
                {
                    var block = await _web3.Eth.Blocks.GetBlockWithTransactionsByNumber
                        .SendRequestAsync(receipt.BlockNumber);
                    
                    if (block?.Timestamp != null)
                    {
                        var unixTimestamp = (long)block.Timestamp.Value;
                        confirmedAt = DateTimeOffset.FromUnixTimeSeconds(unixTimestamp).UtcDateTime;
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogWarning(ex, "Failed to get block timestamp for block {BlockNumber}", blockNumber);
                }
            }

            _logger.LogInformation(
                "Transaction status: TxHash={TxHash}, Status={Status}, BlockNumber={BlockNumber}",
                txHash, status, blockNumber);

            return new BlockchainTxStatusResult
            {
                Status = status,
                BlockNumber = blockNumber,
                ConfirmedAt = confirmedAt
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting transaction status: TxHash={TxHash}", txHash);
            throw;
        }
    }

    /// <summary>
    /// Get the account address
    /// </summary>
    public string GetAccountAddress()
    {
        return _account.Address;
    }

    /// <summary>
    /// Get the account balance in Wei and ETH
    /// </summary>
    public async Task<(BigInteger Wei, decimal Eth)> GetAccountBalanceAsync(CancellationToken ct = default)
    {
        try
        {
            var balance = await _web3.Eth.GetBalance.SendRequestAsync(_account.Address);
            var balanceInWei = balance.Value;
            var balanceInEth = Web3.Convert.FromWei(balanceInWei);

            _logger.LogInformation(
                "Account balance: Address={Address}, Wei={Wei}, ETH={Eth}",
                _account.Address, balanceInWei, balanceInEth);

            return (balanceInWei, balanceInEth);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting account balance: Address={Address}", _account.Address);
            throw;
        }
    }

    /// <summary>
    /// Get the timestamp when a hash was stored on-chain.
    /// Returns 0 if hash doesn't exist.
    /// </summary>
    public async Task<BigInteger> GetHashTimestampAsync(string fileHash, CancellationToken ct = default)
    {
        try
        {
            var normalizedHash = NormalizeHash(fileHash);

            var verifyHashFunction = _contract.GetFunction("verifyHash");
            var timestamp = await verifyHashFunction.CallAsync<BigInteger>(normalizedHash);

            return timestamp;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting hash timestamp: Hash={Hash}", fileHash);
            throw;
        }
    }

    /// <summary>
    /// Normalize hash: add 0x prefix if missing and convert to lowercase
    /// </summary>
    private static string NormalizeHash(string hash)
    {
        if (string.IsNullOrWhiteSpace(hash))
            return hash;

        hash = hash.Trim();
        
        if (!hash.StartsWith("0x", StringComparison.OrdinalIgnoreCase))
            hash = "0x" + hash;

        return hash.ToLowerInvariant();
    }
}
