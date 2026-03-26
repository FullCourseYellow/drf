using ErrorOr;
using Microsoft.AspNetCore.Mvc;

namespace Company.ProjectName.Api.Extensions;

public static class ErrorOrExtensions
{
    /// <summary>
    /// Maps an ErrorOr result to an IResult HTTP response.
    /// Success → 200 OK with value.
    /// Errors → ProblemDetails with appropriate status code.
    /// </summary>
    public static IResult MatchToResult<T>(this ErrorOr<T> errorOr) =>
        errorOr.Match(
            value => Results.Ok(value),
            errors =>
            {
                var first = errors.First();
                return first.Type switch
                {
                    ErrorType.NotFound => Results.Problem(new ProblemDetails
                    {
                        Title = "Resource not found",
                        Detail = first.Description,
                        Status = StatusCodes.Status404NotFound,
                        Type = "https://tools.ietf.org/html/rfc9110#section-15.5.5"
                    }),
                    ErrorType.Validation => Results.Problem(new ProblemDetails
                    {
                        Title = "Validation failed",
                        Detail = first.Description,
                        Status = StatusCodes.Status422UnprocessableEntity,
                        Type = "https://tools.ietf.org/html/rfc4918#section-11.2"
                    }),
                    ErrorType.Conflict => Results.Problem(new ProblemDetails
                    {
                        Title = "Conflict",
                        Detail = first.Description,
                        Status = StatusCodes.Status409Conflict,
                        Type = "https://tools.ietf.org/html/rfc9110#section-15.5.10"
                    }),
                    _ => Results.Problem(new ProblemDetails
                    {
                        Title = "An unexpected error occurred",
                        Detail = first.Description,
                        Status = StatusCodes.Status500InternalServerError,
                        Type = "https://tools.ietf.org/html/rfc9110#section-15.6.1"
                    })
                };
            });
}
