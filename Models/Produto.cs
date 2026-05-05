using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MeuProjetoMVC.Models;

/// <summary>
/// Model que representa a entidade Produto. Em ASP.NET MVC, "Model" tem dois papéis:
///   1) Entidade de domínio (mapeia para tabela do banco via ORM).
///   2) ViewModel (dados que a View precisa para renderizar).
/// Aqui simplificamos juntando os dois — é comum em projetos pequenos.
///
/// As anotações [Required], [StringLength], [Range], etc. são "DataAnnotations".
/// Elas servem para DUAS coisas ao mesmo tempo:
///   - VALIDAÇÃO no servidor (ModelState.IsValid no Controller).
///   - VALIDAÇÃO no cliente (gera atributos data-val no HTML, validados pelo
///     jQuery Validate via _ValidationScriptsPartial).
/// </summary>
[Table("produtos")] // mapeia explicitamente para a tabela "produtos" (snake_case usual no Postgres)
public class Produto
{
    /// <summary>
    /// Chave primária. Por convenção do EF Core, "Id" ou "{ClassName}Id" vira PK automaticamente.
    /// O [Column] é só pra forçar o nome em snake_case no Postgres.
    /// </summary>
    [Column("id")]
    public int Id { get; set; }

    [Required(ErrorMessage = "O nome é obrigatório.")]
    [StringLength(100, MinimumLength = 2,
        ErrorMessage = "O nome deve ter entre {2} e {1} caracteres.")]
    [Display(Name = "Nome do Produto")] // rótulo usado pelas Tag Helpers (asp-for) nas Views
    [Column("nome")]
    public string Nome { get; set; } = string.Empty;

    [StringLength(500, ErrorMessage = "Descrição muito longa (máx 500).")]
    [Display(Name = "Descrição")]
    [Column("descricao")]
    public string? Descricao { get; set; }

    [Required(ErrorMessage = "O preço é obrigatório.")]
    [Range(0.01, 1_000_000.00,
        ErrorMessage = "O preço deve estar entre {1} e {2}.")]
    [DataType(DataType.Currency)] // dica para a View formatar como moeda
    [Display(Name = "Preço (R$)")]
    [Column("preco", TypeName = "numeric(12,2)")] // NUMERIC no Postgres = decimal exato (dinheiro)
    public decimal Preco { get; set; }

    [Required(ErrorMessage = "Informe o estoque.")]
    [Range(0, int.MaxValue, ErrorMessage = "Estoque não pode ser negativo.")]
    [Display(Name = "Em Estoque")]
    [Column("estoque")]
    public int Estoque { get; set; }

    [Display(Name = "Criado em")]
    [DataType(DataType.DateTime)]
    [Column("criado_em")]
    // O EF Core usará o valor default do banco (NOW()) quando inserirmos via init.sql,
    // mas podemos definir aqui também caso o registro venha pelo MVC.
    public DateTime CriadoEm { get; set; } = DateTime.UtcNow;
}
