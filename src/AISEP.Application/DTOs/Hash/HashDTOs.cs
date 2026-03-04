namespace AISEP.Application.DTOs.Hash;

/// <summary>
/// Response containing Ethereum account information
/// </summary>
public class AccountInfoResponse
{
    public string Address { get; set; } = null!;
    public string Balance { get; set; } = null!;
    public string BalanceInEth { get; set; } = null!;
}

/// <summary>
/// Request to compute hash from text
/// </summary>
public class ComputeHashRequest
{
    public string Text { get; set; } = null!;
}

/// <summary>
/// Response containing computed hash
/// </summary>
public class ComputeHashResponse
{
    public string Hash { get; set; } = null!;
    public string Algorithm { get; set; } = "SHA-256";
}

/// <summary>
/// Request to store hash on blockchain
/// </summary>
public class StoreHashRequest
{
    public string Hash { get; set; } = null!;
}

/// <summary>
/// Response after storing hash on blockchain
/// </summary>
public class StoreHashResponse
{
    public string Hash { get; set; } = null!;
    public string TransactionHash { get; set; } = null!;
    public string Status { get; set; } = null!;
    public string BlockNumber { get; set; } = null!;
}

/// <summary>
/// Response from verifying hash on blockchain
/// </summary>
public class VerifyHashResponse
{
    public string Hash { get; set; } = null!;
    public bool Exists { get; set; }
    public long Timestamp { get; set; }
    public DateTime? StoredAt { get; set; }
}
