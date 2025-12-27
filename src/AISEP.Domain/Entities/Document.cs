using AISEP.Domain.Enums;

namespace AISEP.Domain.Entities;

/// <summary>
/// Document uploaded by startup for AI analysis
/// </summary>
public class Document : BaseEntity
{
    public int StartupProfileId { get; private set; }
    public string Title { get; private set; } = string.Empty;
    public DocumentType Type { get; private set; }
    public string FilePath { get; private set; } = string.Empty;
    public string FileName { get; private set; } = string.Empty;
    public long FileSize { get; private set; }
    public string ContentHash { get; private set; } = string.Empty; // SHA-256
    public int Version { get; private set; } = 1;

    // Navigation properties
    public StartupProfile StartupProfile { get; private set; } = null!;
    public DocumentAnalysis? Analysis { get; private set; }
    public BlockchainTransaction? BlockchainTransaction { get; private set; }

    private Document() { }

    public static Document Create(
        int startupProfileId,
        string title,
        DocumentType type,
        string filePath,
        string fileName,
        long fileSize,
        string contentHash)
    {
        const long maxFileSize = 25 * 1024 * 1024; // 25MB
        if (fileSize > maxFileSize)
            throw new ArgumentException($"File size exceeds maximum allowed size of {maxFileSize} bytes", nameof(fileSize));

        if (string.IsNullOrWhiteSpace(contentHash))
            throw new ArgumentException("Content hash is required", nameof(contentHash));

        var document = new Document
        {
            StartupProfileId = startupProfileId,
            Title = title,
            Type = type,
            FilePath = filePath,
            FileName = fileName,
            FileSize = fileSize,
            ContentHash = contentHash,
            Version = 1
        };

        // TODO: Add domain event DocumentUploadedEvent
        return document;
    }

    public void UpdateVersion(string filePath, string fileName, long fileSize, string contentHash)
    {
        FilePath = filePath;
        FileName = fileName;
        FileSize = fileSize;
        ContentHash = contentHash;
        Version++;
        UpdatedAt = DateTime.UtcNow;

        // TODO: Add domain event DocumentVersionUpdatedEvent
    }
}
