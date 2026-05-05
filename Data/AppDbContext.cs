using MeuProjetoMVC.Models;
using Microsoft.EntityFrameworkCore;

namespace MeuProjetoMVC.Data;

/// <summary>
/// DbContext: o "coração" do Entity Framework Core (ORM).
///
/// É a classe que representa uma sessão com o banco. Funciona como:
///   - Unit of Work (acumula alterações até chamar SaveChanges()).
///   - Repository genérico (cada DbSet&lt;T&gt; é uma tabela acessível por LINQ).
///
/// Registramos no Program.cs com builder.Services.AddDbContext&lt;AppDbContext&gt;(...).
/// Aí o ASP.NET injeta uma instância nova por request nos controllers.
/// </summary>
public class AppDbContext : DbContext
{
    // O construtor recebe as opções (string de conexão, provider, etc.) via DI.
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    /// <summary>
    /// Cada DbSet representa uma tabela.
    /// Operações comuns:
    ///   _ctx.Produtos.ToListAsync()         -> SELECT *
    ///   _ctx.Produtos.FindAsync(id)         -> SELECT por PK
    ///   _ctx.Produtos.Add(p)                -> marca para INSERT
    ///   _ctx.Produtos.Update(p)             -> marca para UPDATE
    ///   _ctx.Produtos.Remove(p)             -> marca para DELETE
    ///   _ctx.SaveChangesAsync()             -> efetiva tudo no banco
    /// </summary>
    public DbSet<Produto> Produtos => Set<Produto>();

    /// <summary>
    /// OnModelCreating é onde configuramos o mapeamento "via Fluent API",
    /// uma alternativa às DataAnnotations. Aqui usamos as duas:
    /// as anotações ficam nos models, e ajustes finos ficam aqui.
    /// </summary>
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Convenção: nomes em snake_case (padrão no Postgres).
        // Já fizemos isso via [Table] e [Column] no Produto, mas aqui poderíamos
        // configurar índices, relações, valor default etc.
        modelBuilder.Entity<Produto>(e =>
        {
            // Garante que o nome do produto seja único no banco.
            e.HasIndex(p => p.Nome).IsUnique();

            // CriadoEm: deixar o banco preencher com NOW() quando não for informado.
            e.Property(p => p.CriadoEm)
                .HasDefaultValueSql("NOW()");
        });
    }
}
