using MeuProjetoMVC.Data;
using MeuProjetoMVC.Filters;
using Microsoft.EntityFrameworkCore;

// =============================================================================
// Program.cs — ponto de entrada da aplicação ASP.NET Core (modelo "minimal hosting")
//
// A inicialização tem 2 fases:
//   1) BUILDER — registramos serviços no contêiner de DI (builder.Services).
//   2) APP     — configuramos o pipeline HTTP (middlewares).
//
// Tudo o que estiver em builder.Services é resolvido em tempo de execução por
// injeção de dependência. Tudo que estiver com app.Use... é um middleware na
// pipeline (a ordem importa!).
// =============================================================================

var builder = WebApplication.CreateBuilder(args);

// -----------------------------------------------------------------------------
// 1) MVC + API: registramos controllers (com views) e adicionamos um filtro GLOBAL.
// -----------------------------------------------------------------------------
builder.Services.AddControllersWithViews(options =>
{
    // Filtro global de logging — roda em TODAS as actions de TODOS os controllers.
    // Se precisássemos só em alguns, usaríamos [TypeFilter] no controller.
    options.Filters.Add<LogActionFilter>();
});

// -----------------------------------------------------------------------------
// 2) Entity Framework Core com PostgreSQL.
//    A connection string é lida do appsettings.json (ou variáveis de ambiente).
// -----------------------------------------------------------------------------
var connStr = builder.Configuration.GetConnectionString("DefaultConnection")
    ?? throw new InvalidOperationException("ConnectionString 'DefaultConnection' ausente.");

builder.Services.AddDbContext<AppDbContext>(options =>
{
    options.UseNpgsql(connStr);

    // Em desenvolvimento, mostrar dados sensíveis nos logs do EF (queries com valores)
    // facilita o aprendizado. Em produção, NUNCA habilite isso!
    if (builder.Environment.IsDevelopment())
    {
        options.EnableSensitiveDataLogging();
        options.EnableDetailedErrors();
    }
});

// -----------------------------------------------------------------------------
// 3) Repositório ADO.NET (não é DbContext, é nosso wrapper sobre Npgsql).
//    Como não tem estado mutável, AddScoped resolve uma instância por request —
//    AddSingleton também serviria. Aqui escolhemos Scoped por simplicidade.
// -----------------------------------------------------------------------------
builder.Services.AddScoped<AdoNetProdutoRepository>();

// -----------------------------------------------------------------------------
// 4) Filtros que dependem de DI (precisam estar registrados quando aplicados via TypeFilter).
//    O AddTransient garante uma instância nova a cada uso.
// -----------------------------------------------------------------------------
builder.Services.AddTransient<LogActionFilter>();
builder.Services.AddTransient<ApiExceptionFilter>();

// -----------------------------------------------------------------------------
// 5) Swagger / OpenAPI — gera documentação interativa para a API REST.
// -----------------------------------------------------------------------------
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new() { Title = "MeuProjetoMVC API", Version = "v1" });
});

var app = builder.Build();

// -----------------------------------------------------------------------------
// 6) Aplica migrações / cria tabelas se ainda não existem.
//    OBS: Em produção, o ideal é usar `dotnet ef migrations` versionadas.
//    Aqui mantemos simples para fins didáticos: o init.sql do docker-compose
//    cria a tabela; este bloco apenas espera o banco estar pronto.
// -----------------------------------------------------------------------------
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    // Tenta conectar com retries simples — útil quando o app sobe antes do Postgres
    // dentro do docker-compose terminar a inicialização.
    for (int tentativa = 1; tentativa <= 10; tentativa++)
    {
        try
        {
            db.Database.OpenConnection();
            db.Database.CloseConnection();
            break;
        }
        catch (Exception ex) when (tentativa < 10)
        {
            app.Logger.LogWarning("Banco indisponível (tentativa {N}): {Msg}", tentativa, ex.Message);
            Thread.Sleep(2000);
        }
    }
}

// =============================================================================
// PIPELINE HTTP — a ordem dos middlewares importa.
// Cada request passa por eles na ordem; cada middleware pode encerrar a request
// ou chamar o próximo (next()).
// =============================================================================

if (!app.Environment.IsDevelopment())
{
    // Em produção: redireciona exceções não tratadas para a action /Home/Error.
    app.UseExceptionHandler("/Home/Error");
    // Força HSTS (HTTP Strict Transport Security).
    app.UseHsts();
}
else
{
    // Em desenvolvimento: mostra a página detalhada de erros (stack trace etc.).
    app.UseDeveloperExceptionPage();
}

// Quando rodando em containers atrás de proxy reverso, normalmente desabilitamos.
// Se você for testar HTTPS local, pode reativar.
// app.UseHttpsRedirection();

app.UseStaticFiles(); // serve wwwroot/ (CSS, JS, imagens, libs)

app.UseRouting(); // resolve a rota antes de autorização e endpoints

app.UseAuthorization(); // sem [Authorize] aqui ainda, mas o middleware está pronto

// Swagger só em desenvolvimento (ou onde quisermos expor).
if (app.Environment.IsDevelopment() || app.Environment.IsEnvironment("Docker"))
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "MeuProjetoMVC API v1");
    });
}

// -----------------------------------------------------------------------------
// 7) Mapeamento de rotas
// -----------------------------------------------------------------------------

// Rota CONVENCIONAL para os controllers MVC: {controller}/{action}/{id}
// Ex: /Produtos/Edit/3 -> ProdutosController.Edit(3)
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

// Rotas via ATRIBUTO ([Route] no controller) — usadas pela API. Não precisam ser
// declaradas aqui, o MapControllers* já as descobre. MapControllerRoute cobre as
// duas. Para deixar explícito, poderíamos chamar app.MapControllers() também.

app.Run();
