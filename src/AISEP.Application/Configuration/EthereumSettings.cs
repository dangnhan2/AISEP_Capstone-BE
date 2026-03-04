namespace AISEP.Application.Configuration;

/// <summary>
/// Ethereum blockchain configuration settings
/// </summary>
public class EthereumSettings
{
    public string RpcUrl { get; set; } = null!;
    public string PrivateKey { get; set; } = null!;
    public string ChainId { get; set; } = null!;
    public string ContractAddress { get; set; } = null!;
    public string ContractAbi { get; set; } = null!;
}
