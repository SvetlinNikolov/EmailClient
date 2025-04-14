using EmailService.Domain.Errors;
using Microsoft.AspNetCore.Mvc;

namespace EmailService.Domain.Results;

public static class ResultExtensions
{
    public static IActionResult BuildResult(this Result result)
    {
        if (result.IsSuccess)
        {
            return new OkResult();
        }

        var error = result.Error ?? new Error("Unknown", "An unknown error occurred", System.Net.HttpStatusCode.InternalServerError);

        return new ObjectResult(error)
        {
            StatusCode = (int)error.StatusCode!
        };
    }
}