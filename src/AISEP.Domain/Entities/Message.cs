namespace AISEP.Domain.Entities;

/// <summary>
/// Message entity for direct communication between users
/// Separates messaging functionality from Connection entity
/// </summary>
public class Message : BaseEntity
{
    public int SenderId { get; private set; }
    public int ReceiverId { get; private set; }
    public int? ConnectionId { get; private set; }
    public string Content { get; private set; } = string.Empty;
    public bool IsRead { get; private set; } = false;
    public DateTime? ReadAt { get; private set; }
    public string? AttachmentUrl { get; private set; }
    public string? AttachmentType { get; private set; }
    public int? ReplyToMessageId { get; private set; }

    // Navigation properties
    public User Sender { get; private set; } = null!;
    public User Receiver { get; private set; } = null!;
    public Connection? Connection { get; private set; }
    public Message? ReplyToMessage { get; private set; }

    private Message() { }

    public static Message Create(
        int senderId,
        int receiverId,
        string content,
        int? connectionId = null,
        string? attachmentUrl = null,
        string? attachmentType = null,
        int? replyToMessageId = null)
    {
        if (string.IsNullOrWhiteSpace(content) && string.IsNullOrWhiteSpace(attachmentUrl))
        {
            throw new ArgumentException("Message must have either content or attachment");
        }

        return new Message
        {
            SenderId = senderId,
            ReceiverId = receiverId,
            Content = content,
            ConnectionId = connectionId,
            AttachmentUrl = attachmentUrl,
            AttachmentType = attachmentType,
            ReplyToMessageId = replyToMessageId,
            IsRead = false
        };
    }

    public void MarkAsRead()
    {
        if (IsRead)
        {
            return;
        }

        IsRead = true;
        ReadAt = DateTime.UtcNow;
        UpdatedAt = DateTime.UtcNow;
    }

    public void MarkAsUnread()
    {
        IsRead = false;
        ReadAt = null;
        UpdatedAt = DateTime.UtcNow;
    }

    public bool HasAttachment()
    {
        return !string.IsNullOrWhiteSpace(AttachmentUrl);
    }

    public bool IsReply()
    {
        return ReplyToMessageId.HasValue;
    }
}
