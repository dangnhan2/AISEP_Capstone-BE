using AISEP.Domain.Enums;

namespace AISEP.Domain.Entities;

/// <summary>
/// Blockchain transaction record for document hash storage
/// </summary>
public class BlockchainTransaction : BaseEntity
{
    public int DocumentId { get; private set; }
    public string TransactionHash { get; private set; } = string.Empty;
    public long? BlockNumber { get; private set; }
    public BlockchainTransactionStatus Status { get; private set; }
    public decimal? GasUsed { get; private set; }
    public DateTime? ConfirmedAt { get; private set; }
    public string? ErrorMessage { get; private set; }

    // Navigation properties
    public Document Document { get; private set; } = null!;

    private BlockchainTransaction() { }

    public static BlockchainTransaction Create(int documentId, string transactionHash)
    {
        if (string.IsNullOrWhiteSpace(transactionHash))
            throw new ArgumentException("Transaction hash is required", nameof(transactionHash));

        return new BlockchainTransaction
        {
            DocumentId = documentId,
            TransactionHash = transactionHash,
            Status = BlockchainTransactionStatus.Pending
        };
    }

    public void Confirm(long blockNumber, decimal gasUsed)
    {
        if (Status != BlockchainTransactionStatus.Pending)
            throw new InvalidOperationException("Only pending transactions can be confirmed");

        Status = BlockchainTransactionStatus.Confirmed;
        BlockNumber = blockNumber;
        GasUsed = gasUsed;
        ConfirmedAt = DateTime.UtcNow;
        UpdatedAt = DateTime.UtcNow;
        ErrorMessage = null;

        // TODO: Add domain event BlockchainTransactionConfirmedEvent
    }

    public void Fail(string errorMessage)
    {
        if (Status != BlockchainTransactionStatus.Pending)
            throw new InvalidOperationException("Only pending transactions can be marked as failed");

        Status = BlockchainTransactionStatus.Failed;
        ErrorMessage = errorMessage;
        UpdatedAt = DateTime.UtcNow;

        // TODO: Add domain event BlockchainTransactionFailedEvent
    }
}
