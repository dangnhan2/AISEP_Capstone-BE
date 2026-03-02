namespace AISEP.Application.DTOs.Common;

/// <summary>
/// Standard API response envelope.
/// Every endpoint returns this shape: { message, isSuccess, statusCode, data }.
/// </summary>
public class ApiEnvelope<T>
{
    public string Message { get; set; } = string.Empty;
    public bool IsSuccess { get; set; }
    public int StatusCode { get; set; }
    public T? Data { get; set; }

    // ── Builders ──────────────────────────────────────────────

    public static ApiEnvelope<T> Success(T? data, string message = "Success", int statusCode = 200)
        => new() { IsSuccess = true, StatusCode = statusCode, Message = message, Data = data };

    public static ApiEnvelope<T> Error(string message, int statusCode = 400)
        => new() { IsSuccess = false, StatusCode = statusCode, Message = message, Data = default };
}
