namespace AISEP.Domain.Entities;

/// <summary>
/// System configuration entity for dynamic settings without code changes
/// Allows administrators to configure system parameters at runtime
/// </summary>
public class SystemConfig : BaseEntity
{
    public string Key { get; private set; } = string.Empty;
    public string Value { get; private set; } = string.Empty;
    public string Description { get; private set; } = string.Empty;
    public string Category { get; private set; } = string.Empty;
    public string DataType { get; private set; } = "String";
    public bool IsEncrypted { get; private set; } = false;
    public bool IsEditable { get; private set; } = true;
    public int? ModifiedByUserId { get; private set; }

    // Navigation properties
    public User? ModifiedByUser { get; private set; }

    private SystemConfig() { }

    public static SystemConfig Create(
        string key,
        string value,
        string description,
        string category,
        string dataType = "String",
        bool isEncrypted = false,
        bool isEditable = true)
    {
        return new SystemConfig
        {
            Key = key,
            Value = value,
            Description = description,
            Category = category,
            DataType = dataType,
            IsEncrypted = isEncrypted,
            IsEditable = isEditable
        };
    }

    public void UpdateValue(string newValue, int modifiedByUserId)
    {
        if (!IsEditable)
        {
            throw new InvalidOperationException($"Configuration key '{Key}' is not editable");
        }

        Value = newValue;
        ModifiedByUserId = modifiedByUserId;
        UpdatedAt = DateTime.UtcNow;
    }

    public T GetValue<T>()
    {
        if (IsEncrypted)
        {
            // TODO: Implement decryption logic
            throw new NotImplementedException("Decryption not implemented yet");
        }

        return (T)Convert.ChangeType(Value, typeof(T));
    }

    public bool TryGetValue<T>(out T? result)
    {
        try
        {
            result = GetValue<T>();
            return true;
        }
        catch
        {
            result = default;
            return false;
        }
    }
}
