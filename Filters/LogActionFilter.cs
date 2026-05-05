using Microsoft.AspNetCore.Mvc.Filters;

namespace MeuProjetoMVC.Filters;

/// <summary>
/// FILTROS no ASP.NET Core MVC são interceptadores que rodam em pontos específicos
/// do pipeline da request. Existem 5 tipos principais (ordem de execução):
///
///   1) Authorization Filters  — controlam acesso (ex: [Authorize]).
///   2) Resource Filters       — rodam logo após autorização (cache, throttling).
///   3) Action Filters         — rodam ANTES e DEPOIS do método do controller.
///   4) Exception Filters      — capturam exceções não tratadas.
///   5) Result Filters         — rodam ANTES e DEPOIS do IActionResult ser executado.
///
/// Esta classe é um Action Filter que loga cada chamada de action.
/// Pode ser aplicado:
///   - [TypeFilter(typeof(LogActionFilter))] em um controller/método específico
///   - Globalmente via Program.cs: options.Filters.Add&lt;LogActionFilter&gt;()
/// </summary>
public class LogActionFilter : IActionFilter
{
    private readonly ILogger<LogActionFilter> _logger;

    // Filtros podem receber dependências via construtor (precisa registrar com TypeFilter
    // ou Filters.Add<T>(); injection automática só funciona dessa forma).
    public LogActionFilter(ILogger<LogActionFilter> logger)
    {
        _logger = logger;
    }

    /// <summary>
    /// Chamado ANTES da action executar.
    /// Aqui você pode: validar algo, modificar argumentos, abortar (atribuindo Result).
    /// </summary>
    public void OnActionExecuting(ActionExecutingContext context)
    {
        var controller = context.RouteData.Values["controller"];
        var action     = context.RouteData.Values["action"];
        _logger.LogInformation("➡️  Executando {Controller}/{Action}", controller, action);
    }

    /// <summary>
    /// Chamado DEPOIS da action executar (mas antes do Result ser processado).
    /// Útil para medir tempo, logar resultado, capturar exceções (context.Exception).
    /// </summary>
    public void OnActionExecuted(ActionExecutedContext context)
    {
        var controller = context.RouteData.Values["controller"];
        var action     = context.RouteData.Values["action"];

        if (context.Exception is not null)
        {
            _logger.LogError(context.Exception,
                "❌ Erro em {Controller}/{Action}", controller, action);
        }
        else
        {
            _logger.LogInformation("✅ Concluído {Controller}/{Action}", controller, action);
        }
    }
}
