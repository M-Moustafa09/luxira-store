using System.Net;
using FluentValidation;
using Microsoft.AspNetCore.Mvc;

namespace Luxira.API.Middleware;

public class ExceptionHandlingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<ExceptionHandlingMiddleware> _logger;

    public ExceptionHandlingMiddleware(RequestDelegate next, ILogger<ExceptionHandlingMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (ValidationException ex)
        {
            _logger.LogWarning(ex, "Validation failed for {Method} {Path}", context.Request.Method, context.Request.Path);

            var problemDetails = new ValidationProblemDetails(
                ex.Errors
                    .GroupBy(e => e.PropertyName)
                    .ToDictionary(g => g.Key, g => g.Select(e => e.ErrorMessage).ToArray()))
            {
                Status = (int)HttpStatusCode.BadRequest,
                Title = "بيانات غير صالحة",
                Instance = context.Request.Path
            };

            await WriteProblemDetails(context, problemDetails);
        }
        catch (UnauthorizedAccessException ex)
        {
            _logger.LogWarning(ex, "Missing identity for {Method} {Path}", context.Request.Method, context.Request.Path);

            var problemDetails = new ProblemDetails
            {
                Status = (int)HttpStatusCode.Unauthorized,
                Title = "غير مصرح",
                Detail = ex.Message,
                Instance = context.Request.Path
            };

            await WriteProblemDetails(context, problemDetails);
        }
        catch (KeyNotFoundException ex)
        {
            _logger.LogWarning(ex, "Resource not found for {Method} {Path}", context.Request.Method, context.Request.Path);

            var problemDetails = new ProblemDetails
            {
                Status = (int)HttpStatusCode.NotFound,
                Title = "العنصر غير موجود",
                Detail = ex.Message,
                Instance = context.Request.Path
            };

            await WriteProblemDetails(context, problemDetails);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unhandled exception while processing {Method} {Path}", context.Request.Method, context.Request.Path);

            var problemDetails = new ProblemDetails
            {
                Status = (int)HttpStatusCode.InternalServerError,
                Title = "حدث خطأ غير متوقع",
                Detail = "الرجاء المحاولة مرة أخرى لاحقاً",
                Instance = context.Request.Path
            };

            await WriteProblemDetails(context, problemDetails);
        }
    }

    private static Task WriteProblemDetails(HttpContext context, ProblemDetails problemDetails)
    {
        context.Response.ContentType = "application/problem+json";
        context.Response.StatusCode = problemDetails.Status!.Value;

        // Serialize by runtime type (not the ProblemDetails-typed parameter) so
        // subtypes like ValidationProblemDetails don't have their extra
        // properties (e.g. Errors) silently sliced off by System.Text.Json.
        return context.Response.WriteAsJsonAsync(problemDetails, problemDetails.GetType());
    }
}
