using AISEP.Domain.Enums;

namespace AISEP.Domain.Entities;

/// <summary>
/// AI analysis result for a document
/// </summary>
public class DocumentAnalysis : BaseEntity
{
    public int DocumentId { get; private set; }
    public DocumentAnalysisStatus Status { get; private set; }
    public string? ExtractedText { get; private set; }
    public string? AnalysisResult { get; private set; } // JSON with detailed analysis
    public string? ErrorMessage { get; private set; }
    public DateTime? CompletedAt { get; private set; }

    // Navigation properties
    public Document Document { get; private set; } = null!;

    private DocumentAnalysis() { }

    public static DocumentAnalysis Create(int documentId)
    {
        return new DocumentAnalysis
        {
            DocumentId = documentId,
            Status = DocumentAnalysisStatus.Pending
        };
    }

    public void MarkAsProcessing()
    {
        Status = DocumentAnalysisStatus.Processing;
        UpdatedAt = DateTime.UtcNow;
    }

    public void Complete(string extractedText, string analysisResult)
    {
        Status = DocumentAnalysisStatus.Completed;
        ExtractedText = extractedText;
        AnalysisResult = analysisResult;
        CompletedAt = DateTime.UtcNow;
        UpdatedAt = DateTime.UtcNow;
        ErrorMessage = null;

        // TODO: Add domain event DocumentAnalysisCompletedEvent
    }

    public void Fail(string errorMessage)
    {
        Status = DocumentAnalysisStatus.Failed;
        ErrorMessage = errorMessage;
        UpdatedAt = DateTime.UtcNow;

        // TODO: Add domain event DocumentAnalysisFailedEvent
    }
}
