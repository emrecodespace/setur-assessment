using ContactReports.Application.Common;
using Microsoft.AspNetCore.Mvc;

namespace ContactReports.Api.Helpers;

public sealed record ApiError(string? Error, int? StatusCode);

public static class ApiResponseExtensions
{
    private static IActionResult ToActionResultInternal(ControllerBase controller, string? errorMessage, int? statusCode)
    {
        return statusCode switch
        {
            400 => controller.BadRequest(new ApiError(errorMessage, 400)),
            404 => controller.NotFound(new ApiError(errorMessage, 404)),
            500 => controller.StatusCode(500, new ApiError(errorMessage, 500)),
            _   => controller.StatusCode(statusCode ?? 500, new ApiError(errorMessage, statusCode ?? 500))
        };
    }

    public static IActionResult ToActionResult<T>(this ControllerBase controller, Result<T> result)
    {
        return result.IsSuccess ? controller.Ok(result.Value) 
            : ToActionResultInternal(controller, result.ErrorMessage, result.StatusCode);
    }

    public static IActionResult ToActionResult(this ControllerBase controller, Result result)
    {
        return result.IsSuccess ? controller.Ok()
            : ToActionResultInternal(controller, result.ErrorMessage, result.StatusCode);
    }
}