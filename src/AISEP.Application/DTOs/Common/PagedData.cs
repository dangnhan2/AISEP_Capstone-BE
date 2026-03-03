namespace AISEP.Application.DTOs.Common;

/// <summary>
/// Paged collection wrapper returned inside <see cref="ApiEnvelope{T}"/>.
/// Shape: { page, pageSize, total, data[] }
/// </summary>
public class PagedData<T>
{
    public int Page { get; set; }
    public int PageSize { get; set; }
    public int Total { get; set; }
    public List<T> Data { get; set; } = new();
}
