using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace MeuProjetoMVC.Filters;

/// <summary>
/// EXCEPTION FILTER específico para a API REST.
///
/// Quando aplicado em controllers de API, qualquer exceção não tratada vira
/// um JSON padronizado com HTTP 500 — em vez de devolver HTML feio do
/// "Developer Exception Page" ou um stacktrace puro pro cliente.
///
/// Aplicação preferencial: nos próprios controllers de API com [TypeFilter].
/// Para controllers MVC (que retornam Views), o app.UseExceptionHandler("/Home/Error")
/// no Program.cs já faz o trabalho correspondente.
/// </summary>
public class ApiExceptionFilter : IExceptionFilter
{
    private readonly ILogger<ApiExceptionFilter> _logger;
    private readonly IHostEnvironment _env;

    public ApiExceptionFilter(ILogger<ApiExceptionFilter> logger, IHostEnvironment env)
    {
        _logger = logger;
        _env = env;
    }

    public void OnException(ExceptionContext context)
    {
        _logger.LogError(context.Exception,
            "Exceção não tratada na API: {Message}", context.Exception.Message);

        // Em DEV expomos a mensagem real; em PROD escondemos detalhes (princípio de
        // segurança: não vazar stack trace pra fora).
        var detail = _env.IsDevelopment()
            ? context.Exception.ToString()
            : "Ocorreu um erro inesperado.";

        // ProblemDetails é o formato padrão (RFC 7807) que o ASP.NET usa pra erros HTTP.
        var problem = new ProblemDetails
        {
            Status = StatusCodes.Status500InternalServerError,
            Title  = "Erro interno do servidor",
            Detail = detail,
            Type   = "https://tools.ietf.org/html/rfc7231#section-6.6.1",
        };

        // Atribuir context.Result evita que a exceção continue propagando.
        context.Result = new ObjectResult(problem)
        {
            StatusCode = StatusCodes.Status500InternalServerError
        };
        context.ExceptionHandled = true;
    }
}
