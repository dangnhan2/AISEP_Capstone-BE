namespace AISEP.Application.DTOs.Common;

/// <summary>
/// Auth success payload returned inside <see cref="ApiEnvelope{T}"/>.
/// Shape: { data: userDto, accessToken }
/// </summary>
public class AuthPayload<TUser>
{
    public TUser Data { get; set; } = default!;
    public string AccessToken { get; set; } = null!;
}
