using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using MeuProjetoMVC.Models;

namespace MeuProjetoMVC.Controllers;

/// <summary>
/// HomeController — controller padrão do template MVC.
///
/// CONCEITOS:
///   - Toda classe que termina em "Controller" e herda de Controller é descoberta
///     automaticamente pelo ASP.NET via convenção de nome.
///   - Cada método público vira uma "action". Por padrão, retorna uma View com
///     o mesmo nome do método (Index() -> Views/Home/Index.cshtml).
///   - A rota padrão configurada no Program.cs é {controller=Home}/{action=Index}/{id?}
///     ou seja: GET / -> HomeController.Index().
/// </summary>
public class HomeController : Controller
{
    // Logger é injetado pelo container DI do ASP.NET (não precisamos instanciar).
    private readonly ILogger<HomeController> _logger;

    public HomeController(ILogger<HomeController> logger)
    {
        _logger = logger;
    }

    // GET /  ou  GET /Home  ou  GET /Home/Index
    public IActionResult Index()
    {
        // View() procura Views/Home/Index.cshtml.
        return View();
    }

    // GET /Home/Privacy
    public IActionResult Privacy()
    {
        return View();
    }

    // [ResponseCache] é um exemplo de filtro embutido — força o cliente a NÃO cachear.
    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}
