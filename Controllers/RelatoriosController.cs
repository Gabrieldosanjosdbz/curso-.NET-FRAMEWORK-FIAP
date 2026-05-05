using MeuProjetoMVC.Data;
using MeuProjetoMVC.Filters;
using Microsoft.AspNetCore.Mvc;

namespace MeuProjetoMVC.Controllers;

/// <summary>
/// RelatoriosController — demonstra o uso de ADO.NET PURO (sem ORM)
/// para consultas analíticas, em uma view Razor convencional.
/// </summary>
[TypeFilter(typeof(LogActionFilter))]
public class RelatoriosController : Controller
{
    private readonly AdoNetProdutoRepository _repo;

    public RelatoriosController(AdoNetProdutoRepository repo)
    {
        _repo = repo;
    }

    // GET: /Relatorios  ou  /Relatorios/Index
    public async Task<IActionResult> Index(int? limiteEstoque)
    {
        // Quando o usuário não informa, usamos 5 como padrão.
        var limite = limiteEstoque ?? 5;

        // Vamos juntar dois resultados em um ViewModel "anônimo" via ViewBag/ViewData
        // ou criando uma classe — escolhemos ViewBag aqui para manter simples.
        ViewBag.LimiteEstoque = limite;
        ViewBag.Resumo        = await _repo.ObterRelatorioEstoqueAsync();
        ViewBag.ProdutosBaixo = await _repo.ListarComEstoqueAbaixoDeAsync(limite);

        return View();
    }
}
