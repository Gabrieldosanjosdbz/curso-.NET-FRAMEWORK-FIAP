using MeuProjetoMVC.Data;
using MeuProjetoMVC.Filters;
using MeuProjetoMVC.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace MeuProjetoMVC.Controllers.Api;

/// <summary>
/// API REST de Produtos.
///
/// Diferenças vs MVC com Views:
///   - Herda de ControllerBase (não Controller). ControllerBase não tem helpers de View
///     (View(), PartialView, etc.) — só os de HTTP/JSON. Mais leve, ideal pra API.
///   - [ApiController] habilita comportamentos automáticos:
///       * Validação automática de ModelState — se inválido, devolve 400 + ProblemDetails
///         sem precisar checar manualmente.
///       * Inferência de binding (parâmetro complexo vem do body, simples da rota/query).
///       * Respostas de erro padronizadas (RFC 7807 ProblemDetails).
///   - [Route("api/[controller]")] usa attribute routing (não a rota convencional do MVC).
///     [controller] vira "produtos" (sem o sufixo "Api"... é o token do nome da classe
///     SEM "Controller"; aqui ficaria "ProdutosApi"). Por isso forçamos abaixo.
///
/// REST clássico:
///   GET    /api/produtos          -> lista
///   GET    /api/produtos/{id}     -> obtém um
///   POST   /api/produtos          -> cria
///   PUT    /api/produtos/{id}     -> atualiza inteiro
///   DELETE /api/produtos/{id}     -> remove
/// </summary>
[ApiController]
[Route("api/produtos")]
[TypeFilter(typeof(ApiExceptionFilter))] // captura exceções e devolve JSON padronizado
[Produces("application/json")]
public class ProdutosApiController : ControllerBase
{
    private readonly AppDbContext _ctx;
    private readonly ILogger<ProdutosApiController> _logger;

    public ProdutosApiController(AppDbContext ctx, ILogger<ProdutosApiController> logger)
    {
        _ctx = ctx;
        _logger = logger;
    }

    /// <summary>Lista todos os produtos.</summary>
    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<Produto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IEnumerable<Produto>>> Listar()
    {
        var produtos = await _ctx.Produtos.AsNoTracking().ToListAsync();
        // AsNoTracking: como não vamos editar, dizemos ao EF para não monitorar
        // mudanças — economiza memória e CPU em endpoints de leitura.
        return Ok(produtos);
    }

    /// <summary>Obtém um produto pelo Id.</summary>
    [HttpGet("{id:int}")]
    [ProducesResponseType(typeof(Produto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<Produto>> ObterPorId(int id)
    {
        var produto = await _ctx.Produtos.FindAsync(id);
        if (produto is null) return NotFound();
        return Ok(produto);
    }

    /// <summary>Cria um novo produto.</summary>
    [HttpPost]
    [ProducesResponseType(typeof(Produto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<Produto>> Criar([FromBody] Produto produto)
    {
        // Graças a [ApiController], se o body for inválido o framework já devolve 400
        // antes mesmo de entrar aqui. Mas se você quiser regras extras, pode adicionar:
        if (await _ctx.Produtos.AnyAsync(p => p.Nome == produto.Nome))
        {
            ModelState.AddModelError(nameof(Produto.Nome), "Já existe um produto com este nome.");
            return ValidationProblem(ModelState); // retorna 400 + ProblemDetails
        }

        _ctx.Produtos.Add(produto);
        await _ctx.SaveChangesAsync();

        // CreatedAtAction retorna 201 + header Location apontando pra GET /api/produtos/{id}.
        return CreatedAtAction(nameof(ObterPorId), new { id = produto.Id }, produto);
    }

    /// <summary>Atualiza um produto inteiro.</summary>
    [HttpPut("{id:int}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Atualizar(int id, [FromBody] Produto produto)
    {
        if (id != produto.Id) return BadRequest("Id da URL difere do corpo.");

        var existe = await _ctx.Produtos.AnyAsync(p => p.Id == id);
        if (!existe) return NotFound();

        _ctx.Entry(produto).State = EntityState.Modified; // marca todos campos como alterados
        await _ctx.SaveChangesAsync();

        // 204 No Content é a resposta REST canônica para PUT bem-sucedido sem body.
        return NoContent();
    }

    /// <summary>Remove um produto.</summary>
    [HttpDelete("{id:int}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Remover(int id)
    {
        var produto = await _ctx.Produtos.FindAsync(id);
        if (produto is null) return NotFound();

        _ctx.Produtos.Remove(produto);
        await _ctx.SaveChangesAsync();
        return NoContent();
    }
}
