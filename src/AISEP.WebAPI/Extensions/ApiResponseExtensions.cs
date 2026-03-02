using AISEP.Application.DTOs.Common;
using Microsoft.AspNetCore.Mvc;

namespace AISEP.WebAPI.Extensions;

/// <summary>
/// Backward-compatible extensions that now output <see cref="ApiEnvelope{T}"/>
/// instead of raw <see cref="ApiResponse{T}"/>.
/// Delegates error-code mapping to <see cref="ApiEnvelopeExtensions"/>.
/// </summary>
public static class ApiResponseExtensions
{
    /// <summary>Success → 200 envelope; Fail → mapped-status error envelope.</summary>
    public static IActionResult ToActionResult<T>(this ApiResponse<T> result)
        => result.ToEnvelope();

    /// <summary>Fail → error envelope with mapped status code.</summary>
    public static IActionResult ToErrorResult<T>(this ApiResponse<T> result)
        => result.ToErrorEnvelope();
}
