using MeuProjetoMVC.Models;
using Npgsql;

namespace MeuProjetoMVC.Data;

/// <summary>
/// Repositório usando ADO.NET PURO (sem ORM), via driver Npgsql.
///
/// Por que existir, se já temos EF Core?
///   - Para você ENTENDER o que está acontecendo "embaixo do capô".
///   - Em cenários de alta performance, relatórios pesados ou stored procedures,
///     às vezes vale escrever SQL na mão.
///   - Toda a estrutura (Connection, Command, Parameter, DataReader) é a base
///     histórica do .NET para acesso a dados — e funciona igual para SQL Server,
///     Oracle, MySQL, etc., só mudando o driver.
///
/// Princípios importantes que aparecem aqui:
///   - "using" para garantir Dispose() (fecha conexão mesmo com exceção).
///   - Sempre usar PARÂMETROS (@nome). NUNCA concatenar strings em SQL — risco de SQL Injection.
///   - Async/await em IO para não travar threads.
/// </summary>
public class AdoNetProdutoRepository
{
    private readonly string _connectionString;

    // A connection string vem do appsettings.json via IConfiguration (injetada).
    public AdoNetProdutoRepository(IConfiguration configuration)
    {
        _connectionString = configuration.GetConnectionString("DefaultConnection")
            ?? throw new InvalidOperationException("ConnectionString 'DefaultConnection' não configurada.");
    }

    /// <summary>
    /// Exemplo de relatório "agregado" — algo que faria sentido em SQL puro.
    /// Retorna: total de produtos, soma do estoque, valor total em estoque, preço médio.
    /// </summary>
    public async Task<RelatorioEstoque> ObterRelatorioEstoqueAsync()
    {
        // 1) Conexão. O "await using" garante que será fechada/disposta no fim do bloco.
        await using var conn = new NpgsqlConnection(_connectionString);
        await conn.OpenAsync();

        // 2) Comando SQL. Note que aqui não há parâmetros pq não há entrada do usuário.
        const string sql = @"
            SELECT
                COUNT(*)             AS total_produtos,
                COALESCE(SUM(estoque), 0)         AS soma_estoque,
                COALESCE(SUM(preco * estoque), 0) AS valor_total,
                COALESCE(AVG(preco), 0)           AS preco_medio
            FROM produtos;";

        await using var cmd = new NpgsqlCommand(sql, conn);

        // 3) ExecuteReaderAsync devolve um cursor sobre as linhas.
        await using var reader = await cmd.ExecuteReaderAsync();

        if (await reader.ReadAsync())
        {
            // Lemos cada coluna pelo índice. Existe também GetOrdinal("nome_coluna").
            return new RelatorioEstoque
            {
                TotalProdutos = reader.GetInt32(0),
                SomaEstoque   = reader.GetInt64(1),
                ValorTotal    = reader.GetDecimal(2),
                PrecoMedio    = reader.GetDecimal(3),
            };
        }

        return new RelatorioEstoque();
    }

    /// <summary>
    /// Exemplo de SELECT com parâmetro de entrada.
    /// Retorna produtos com estoque abaixo do limite (típico "produtos a repor").
    /// </summary>
    public async Task<List<Produto>> ListarComEstoqueAbaixoDeAsync(int limite)
    {
        var resultado = new List<Produto>();

        await using var conn = new NpgsqlConnection(_connectionString);
        await conn.OpenAsync();

        // Sempre usar @param (PARAMETRIZADO) — nunca concatenar valor na string!
        const string sql = @"
            SELECT id, nome, descricao, preco, estoque, criado_em
            FROM produtos
            WHERE estoque < @limite
            ORDER BY estoque ASC;";

        await using var cmd = new NpgsqlCommand(sql, conn);
        cmd.Parameters.AddWithValue("limite", limite);

        await using var reader = await cmd.ExecuteReaderAsync();
        while (await reader.ReadAsync())
        {
            resultado.Add(new Produto
            {
                Id        = reader.GetInt32(0),
                Nome      = reader.GetString(1),
                // GetString quebra se for NULL — IsDBNull primeiro.
                Descricao = reader.IsDBNull(2) ? null : reader.GetString(2),
                Preco     = reader.GetDecimal(3),
                Estoque   = reader.GetInt32(4),
                CriadoEm  = reader.GetDateTime(5),
            });
        }

        return resultado;
    }
}

/// <summary>
/// DTO simples para carregar o resultado do relatório.
/// Sem [Table] — não é uma entidade do EF, é só um objeto de transporte.
/// </summary>
public class RelatorioEstoque
{
    public int TotalProdutos { get; set; }
    public long SomaEstoque { get; set; }
    public decimal ValorTotal { get; set; }
    public decimal PrecoMedio { get; set; }
}
