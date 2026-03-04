using AISEP.Application.DTOs.Common;
using AISEP.Application.DTOs.Hash;
using AISEP.Infrastructure.Services;
using Microsoft.AspNetCore.Mvc;
using System.Security.Cryptography;
using System.Text;
using iText.Kernel.Pdf;

namespace AISEP.WebAPI.Controllers;

/// <summary>
/// Hash computation and blockchain storage operations.
/// Supports SHA-256 hash computation from text and PDF files,
/// and stores/verifies hashes on Ethereum blockchain.
/// </summary>
[ApiController]
[Route("api/hash")]
[Tags("Hash & Blockchain")]
public class HashController : ControllerBase
{
    private readonly ILogger<HashController> _logger;
    private readonly EthereumBlockchainService _blockchainService;

    public HashController(
        ILogger<HashController> logger,
        EthereumBlockchainService blockchainService)
    {
        _logger = logger;
        _blockchainService = blockchainService;
    }

    /// <summary>
    /// Get Ethereum account information (address and balance)
    /// </summary>
    /// <remarks>
    /// Returns the Ethereum wallet address and current balance in Wei and ETH.
    /// </remarks>
    [HttpGet("account")]
    [ProducesResponseType(typeof(ApiEnvelope<AccountInfoResponse>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAccount(CancellationToken ct)
    {
        try
        {
            _logger.LogInformation("Getting Ethereum account info");

            var address = _blockchainService.GetAccountAddress();
            var (balanceWei, balanceEth) = await _blockchainService.GetAccountBalanceAsync(ct);

            var response = new AccountInfoResponse
            {
                Address = address,
                Balance = balanceWei.ToString(),
                BalanceInEth = balanceEth.ToString("F18")
            };

            return Ok(ApiEnvelope<AccountInfoResponse>.Success(
                response,
                "Account information retrieved successfully"));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting account info");
            return StatusCode(
                StatusCodes.Status500InternalServerError,
                ApiEnvelope<AccountInfoResponse>.Error(
                    "Failed to retrieve account information",
                    StatusCodes.Status500InternalServerError));
        }
    }

    /// <summary>
    /// Compute SHA-256 hash from text input
    /// </summary>
    /// <remarks>
    /// Computes the SHA-256 hash of the provided text string.
    /// Returns hash with 0x prefix in lowercase.
    /// </remarks>
    [HttpPost("compute")]
    [ProducesResponseType(typeof(ApiEnvelope<ComputeHashResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiEnvelope<ComputeHashResponse>), StatusCodes.Status400BadRequest)]
    public IActionResult ComputeHashFromText([FromBody] ComputeHashRequest request)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(request.Text))
            {
                return BadRequest(ApiEnvelope<ComputeHashResponse>.Error(
                    "Text cannot be empty",
                    StatusCodes.Status400BadRequest));
            }

            _logger.LogInformation("Computing hash from text (length: {Length})", request.Text.Length);

            var hash = ComputeSha256Hash(request.Text);

            var response = new ComputeHashResponse
            {
                Hash = hash,
                Algorithm = "SHA-256"
            };

            return Ok(ApiEnvelope<ComputeHashResponse>.Success(
                response,
                "Hash computed successfully"));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error computing hash from text");
            return StatusCode(
                StatusCodes.Status500InternalServerError,
                ApiEnvelope<ComputeHashResponse>.Error(
                    "Failed to compute hash",
                    StatusCodes.Status500InternalServerError));
        }
    }

    /// <summary>
    /// Compute SHA-256 hash from PDF file
    /// </summary>
    /// <remarks>
    /// Computes the SHA-256 hash of the uploaded PDF file content.
    /// Returns hash with 0x prefix in lowercase.
    /// </remarks>
    [HttpPost("compute-pdf")]
    [ProducesResponseType(typeof(ApiEnvelope<ComputeHashResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiEnvelope<ComputeHashResponse>), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> ComputeHashFromPdf(IFormFile file, CancellationToken ct)
    {
        try
        {
            if (file == null || file.Length == 0)
            {
                return BadRequest(ApiEnvelope<ComputeHashResponse>.Error(
                    "PDF file is required",
                    StatusCodes.Status400BadRequest));
            }

            if (!file.ContentType.Equals("application/pdf", StringComparison.OrdinalIgnoreCase) &&
                !file.FileName.EndsWith(".pdf", StringComparison.OrdinalIgnoreCase))
            {
                return BadRequest(ApiEnvelope<ComputeHashResponse>.Error(
                    "File must be a PDF",
                    StatusCodes.Status400BadRequest));
            }

            _logger.LogInformation(
                "Computing hash from PDF file: {FileName}, Size: {Size} bytes",
                file.FileName, file.Length);

            string hash;
            
            using (var stream = file.OpenReadStream())
            {
                // Validate PDF and compute hash
                try
                {
                    using var pdfReader = new PdfReader(stream);
                    using var pdfDocument = new PdfDocument(pdfReader);
                    
                    // Reset stream for hash computation
                    stream.Position = 0;
                }
                catch (Exception ex)
                {
                    _logger.LogWarning(ex, "Invalid PDF file: {FileName}", file.FileName);
                    return BadRequest(ApiEnvelope<ComputeHashResponse>.Error(
                        "Invalid PDF file",
                        StatusCodes.Status400BadRequest));
                }

                // Compute hash from file bytes
                stream.Position = 0;
                using var sha256 = SHA256.Create();
                var hashBytes = await sha256.ComputeHashAsync(stream, ct);
                hash = "0x" + BitConverter.ToString(hashBytes).Replace("-", "").ToLowerInvariant();
            }

            var response = new ComputeHashResponse
            {
                Hash = hash,
                Algorithm = "SHA-256"
            };

            return Ok(ApiEnvelope<ComputeHashResponse>.Success(
                response,
                "Hash computed successfully from PDF"));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error computing hash from PDF");
            return StatusCode(
                StatusCodes.Status500InternalServerError,
                ApiEnvelope<ComputeHashResponse>.Error(
                    "Failed to compute hash from PDF",
                    StatusCodes.Status500InternalServerError));
        }
    }

    /// <summary>
    /// Store hash on Ethereum blockchain
    /// </summary>
    /// <remarks>
    /// Submits the hash to the smart contract on Ethereum Sepolia testnet.
    /// Waits for transaction confirmation (max 3 minutes).
    /// Hash will be normalized (0x prefix + lowercase).
    /// </remarks>
    [HttpPost("store")]
    [ProducesResponseType(typeof(ApiEnvelope<StoreHashResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiEnvelope<StoreHashResponse>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiEnvelope<StoreHashResponse>), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> StoreHash([FromBody] StoreHashRequest request, CancellationToken ct)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(request.Hash))
            {
                return BadRequest(ApiEnvelope<StoreHashResponse>.Error(
                    "Hash cannot be empty",
                    StatusCodes.Status400BadRequest));
            }

            _logger.LogInformation("Storing hash on blockchain: {Hash}", request.Hash);

            // Check if hash already exists
            var timestamp = await _blockchainService.GetHashTimestampAsync(request.Hash, ct);
            if (timestamp > 0)
            {
                var existingStoredAt = DateTimeOffset.FromUnixTimeSeconds((long)timestamp).UtcDateTime;
                
                _logger.LogWarning(
                    "Hash already exists on blockchain: {Hash}, StoredAt: {StoredAt}",
                    request.Hash, existingStoredAt);

                return BadRequest(ApiEnvelope<StoreHashResponse>.Error(
                    $"Hash already exists on blockchain (stored at {existingStoredAt:yyyy-MM-dd HH:mm:ss} UTC)",
                    StatusCodes.Status400BadRequest));
            }

            // Store hash on blockchain
            var metadata = new Domain.Interfaces.BlockchainSubmitMeta
            {
                DocumentID = 0,
                StartupID = 0,
                DocumentType = "Hash",
                FileName = "Manual Hash Submission"
            };

            var txHash = await _blockchainService.SubmitHashAsync(request.Hash, metadata, ct);

            // Get transaction status
            var txStatus = await _blockchainService.GetTxStatusAsync(txHash, ct);

            var response = new StoreHashResponse
            {
                Hash = request.Hash,
                TransactionHash = txHash,
                Status = txStatus.Status,
                BlockNumber = txStatus.BlockNumber ?? "Pending"
            };

            return Ok(ApiEnvelope<StoreHashResponse>.Success(
                response,
                "Hash stored on blockchain successfully"));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error storing hash on blockchain: {Hash}", request.Hash);
            return StatusCode(
                StatusCodes.Status500InternalServerError,
                ApiEnvelope<StoreHashResponse>.Error(
                    "Failed to store hash on blockchain: " + ex.Message,
                    StatusCodes.Status500InternalServerError));
        }
    }

    /// <summary>
    /// Verify hash exists on Ethereum blockchain
    /// </summary>
    /// <remarks>
    /// Checks if the hash exists in the smart contract on Ethereum Sepolia testnet.
    /// Returns existence status and storage timestamp if found.
    /// Hash will be normalized (0x prefix + lowercase).
    /// </remarks>
    [HttpGet("verify/{hash}")]
    [ProducesResponseType(typeof(ApiEnvelope<VerifyHashResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiEnvelope<VerifyHashResponse>), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> VerifyHash(string hash, CancellationToken ct)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(hash))
            {
                return BadRequest(ApiEnvelope<VerifyHashResponse>.Error(
                    "Hash cannot be empty",
                    StatusCodes.Status400BadRequest));
            }

            _logger.LogInformation("Verifying hash on blockchain: {Hash}", hash);

            // Get timestamp from blockchain
            var timestamp = await _blockchainService.GetHashTimestampAsync(hash, ct);
            var exists = timestamp > 0;
            
            DateTime? storedAt = null;
            if (exists)
            {
                storedAt = DateTimeOffset.FromUnixTimeSeconds((long)timestamp).UtcDateTime;
            }

            var response = new VerifyHashResponse
            {
                Hash = hash,
                Exists = exists,
                Timestamp = (long)timestamp,
                StoredAt = storedAt
            };

            var message = exists
                ? $"Hash found on blockchain (stored at {storedAt:yyyy-MM-dd HH:mm:ss} UTC)"
                : "Hash not found on blockchain";

            return Ok(ApiEnvelope<VerifyHashResponse>.Success(response, message));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error verifying hash on blockchain: {Hash}", hash);
            return StatusCode(
                StatusCodes.Status500InternalServerError,
                ApiEnvelope<VerifyHashResponse>.Error(
                    "Failed to verify hash on blockchain: " + ex.Message,
                    StatusCodes.Status500InternalServerError));
        }
    }

    /// <summary>
    /// Compute SHA-256 hash from text string
    /// </summary>
    private static string ComputeSha256Hash(string text)
    {
        using var sha256 = SHA256.Create();
        var bytes = Encoding.UTF8.GetBytes(text);
        var hashBytes = sha256.ComputeHash(bytes);
        return "0x" + BitConverter.ToString(hashBytes).Replace("-", "").ToLowerInvariant();
    }
}
