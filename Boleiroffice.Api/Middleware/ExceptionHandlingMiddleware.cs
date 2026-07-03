using System.Diagnostics;
using Boleiroffice.Application.Exceptions;
using FluentValidation;

namespace Boleiroffice.Api.Middleware;

public sealed class ExceptionHandlingMiddleware
{
    private readonly ILogger<ExceptionHandlingMiddleware> _logger;
    private readonly RequestDelegate _next;

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
            var errors = ex.Errors
                .GroupBy(x => string.IsNullOrWhiteSpace(x.PropertyName) ? "request" : x.PropertyName)
                .ToDictionary(
                    group => group.Key,
                    group => group.Select(item => item.ErrorMessage).Distinct().ToArray());

            await WriteErrorAsync(context, StatusCodes.Status400BadRequest, "Falha de validacao.", errors);
        }
        catch (NotFoundException ex)
        {
            await WriteErrorAsync(context, StatusCodes.Status404NotFound, ex.Message);
        }
        catch (BusinessException ex)
        {
            await WriteErrorAsync(context, StatusCodes.Status400BadRequest, ex.Message);
        }
        catch (UnauthorizedAccessException ex)
        {
            await WriteErrorAsync(context, StatusCodes.Status401Unauthorized, ex.Message);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro nao tratado na requisicao.");
            await WriteErrorAsync(context, StatusCodes.Status500InternalServerError, "Ocorreu um erro interno no servidor.");
        }
    }

    private static Task WriteErrorAsync(
        HttpContext context,
        int statusCode,
        string message,
        IReadOnlyDictionary<string, string[]>? errors = null)
    {
        context.Response.StatusCode = statusCode;

        return context.Response.WriteAsJsonAsync(new
        {
            message,
            errors,
            traceId = Activity.Current?.Id ?? context.TraceIdentifier
        });
    }
}
