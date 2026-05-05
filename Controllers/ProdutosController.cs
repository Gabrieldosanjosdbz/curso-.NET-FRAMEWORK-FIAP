using MeuProjetoMVC.Data;
using MeuProjetoMVC.Filters;
using MeuProjetoMVC.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace MeuProjetoMVC.Controllers;

/// <summary>
/// ProdutosController — controller WEB MVC clássico (retorna Views Razor).
/// CRUD completo de produtos usando EF Core (ORM).
///
/// Convenções importantes do scaffolding MVC:
///   - GET   /Produtos          -> Index()       lista
///   - GET   /Produtos/Details/5-> Details(5)    visualizar
///   - GET   /Produtos/Create   -> Create()      formulário
///   - POST  /Produtos/Create   -> Create(model) submissão
///   - GET   /Produtos/Edit/5   -> Edit(5)       formulário pré-preenchido
///   - POST  /Produtos/Edit/5   -> Edit(5,model) submissão
///   - GET   /Produtos/Delete/5 -> Delete(5)     confirmação
///   - POST  /Produtos/Delete/5 -> DeleteConfirmed(5)
///
/// O [TypeFilter] aplica nosso LogActionFilter em TODAS as actions deste controller.
/// </summary>
[TypeFilter(typeof(LogActionFilter))]
public class ProdutosController : Controller
{
    private readonly AppDbContext _ctx;

    // O AppDbContext é injetado pelo DI (registrado em Program.cs).
    public ProdutosController(AppDbContext ctx)
    {
        _ctx = ctx;
    }

    // GET: /Produtos
    public async Task<IActionResult> Index()
    {
        // ToListAsync() é uma extensão do EF Core: gera SELECT * FROM produtos.
        var produtos = await _ctx.Produtos
            .OrderBy(p => p.Nome)
            .ToListAsync();
        return View(produtos);
    }

    // GET: /Produtos/Details/5
    public async Task<IActionResult> Details(int? id)
    {
        if (id is null) return NotFound();

        // FirstOrDefaultAsync gera SELECT ... WHERE id = @id LIMIT 1
        var produto = await _ctx.Produtos.FirstOrDefaultAsync(p => p.Id == id);
        if (produto is null) return NotFound();

        return View(produto);
    }

    // GET: /Produtos/Create
    public IActionResult Create()
    {
        // Apenas devolve a View vazia. O form na View renderiza um Produto novo.
        return View();
    }

    // POST: /Produtos/Create
    // [ValidateAntiForgeryToken] é uma proteção contra CSRF — a view Razor
    // gera um token escondido e o servidor valida que o POST veio de lá.
    [HttpPost]
    [ValidateAntiForgeryToken]
    // [Bind] limita quais campos podem vir do formulário (evita "over-posting":
    // alguém mandar Id ou CriadoEm via fetch malicioso). Boa prática de segurança.
    public async Task<IActionResult> Create([Bind("Nome,Descricao,Preco,Estoque")] Produto produto)
    {
        // ModelState.IsValid roda as DataAnnotations definidas no Produto.cs.
        // Se algum [Required], [Range], etc. falhar, IsValid = false.
        if (!ModelState.IsValid)
        {
            // Retornamos a mesma View com o produto, mostrando os erros.
            return View(produto);
        }

        _ctx.Produtos.Add(produto);                 // marca para INSERT
        await _ctx.SaveChangesAsync();              // efetiva no banco

        // Padrão "Post-Redirect-Get": após sucesso de POST, redirecionamos para evitar
        // que F5 reenvie o formulário.
        return RedirectToAction(nameof(Index));
    }

    // GET: /Produtos/Edit/5
    public async Task<IActionResult> Edit(int? id)
    {
        if (id is null) return NotFound();
        var produto = await _ctx.Produtos.FindAsync(id);
        if (produto is null) return NotFound();
        return View(produto);
    }

    // POST: /Produtos/Edit/5
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, [Bind("Id,Nome,Descricao,Preco,Estoque,CriadoEm")] Produto produto)
    {
        if (id != produto.Id) return NotFound();

        if (!ModelState.IsValid) return View(produto);

        try
        {
            _ctx.Update(produto);          // marca todos os campos para UPDATE
            await _ctx.SaveChangesAsync();
        }
        catch (DbUpdateConcurrencyException)
        {
            // Se outro usuário deletou/alterou o produto entre o GET e o POST,
            // o EF lança esta exceção. Aqui apenas tratamos como "não encontrado".
            if (!await _ctx.Produtos.AnyAsync(e => e.Id == id))
                return NotFound();
            throw;
        }

        return RedirectToAction(nameof(Index));
    }

    // GET: /Produtos/Delete/5
    public async Task<IActionResult> Delete(int? id)
    {
        if (id is null) return NotFound();
        var produto = await _ctx.Produtos.FirstOrDefaultAsync(p => p.Id == id);
        if (produto is null) return NotFound();
        return View(produto);
    }

    // POST: /Produtos/Delete/5
    // ActionName força a rota a ser "Delete" mesmo o método chamando-se "DeleteConfirmed".
    // Isso é necessário porque já temos um GET Delete acima.
    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        var produto = await _ctx.Produtos.FindAsync(id);
        if (produto is not null)
        {
            _ctx.Produtos.Remove(produto);
            await _ctx.SaveChangesAsync();
        }
        return RedirectToAction(nameof(Index));
    }
}
