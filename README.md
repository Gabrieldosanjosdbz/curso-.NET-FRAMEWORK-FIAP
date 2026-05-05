# MeuProjetoMVC — projeto de estudos ASP.NET Core 8

Projeto didático que reúne, em uma única solução pequena, os principais pilares
do ASP.NET Core MVC + Web API:

| Tópico | Onde olhar |
|---|---|
| Models + Validação (DataAnnotations) | `Models/Produto.cs` |
| Controllers MVC (Views Razor) | `Controllers/ProdutosController.cs`, `Controllers/HomeController.cs` |
| Controller de API REST (JSON) | `Controllers/Api/ProdutosApiController.cs` |
| Views Razor + Layout + Tag Helpers | `Views/**/*.cshtml`, `Views/Shared/_Layout.cshtml` |
| Rotas convencional + por atributo | `Program.cs` (`MapControllerRoute`), `[Route]` no controller |
| ORM com EF Core | `Data/AppDbContext.cs` |
| ADO.NET puro com Npgsql | `Data/AdoNetProdutoRepository.cs`, `Controllers/RelatoriosController.cs` |
| Filtros (action + exception) | `Filters/LogActionFilter.cs`, `Filters/ApiExceptionFilter.cs` |
| Injeção de dependência | `Program.cs` (`builder.Services.Add...`) |
| Banco PostgreSQL via Docker | `docker-compose.yml`, `Dockerfile`, `db/init.sql` |
| Swagger / OpenAPI | `Program.cs` (`AddSwaggerGen`), rota `/swagger` |

---

## Pré-requisitos

- [.NET 8 SDK](https://dotnet.microsoft.com/download) (apenas se rodar fora do Docker)
- [Docker](https://www.docker.com/) e Docker Compose

---

## Como rodar

### Opção 1 — Tudo no Docker (recomendado)

```bash
docker compose up --build
```

Isso sobe:
- **Postgres 16** em `localhost:5433` (user `postgres`, senha `postgres`, db `meuprojeto`)
  - Internamente, o container expõe `5432`; mapeamos para `5433` no host para não conflitar com um Postgres local.
  - Se você não tem Postgres local, pode editar o `docker-compose.yml` para `"5432:5432"`.
- **App ASP.NET** em `http://localhost:8080`

URLs principais:
- Site MVC: <http://localhost:8080>
- Swagger (API REST): <http://localhost:8080/swagger>
- API direto: <http://localhost:8080/api/produtos>

Para parar: `docker compose down`. Para resetar o banco: `docker compose down -v`.

### Opção 2 — App local + banco no Docker

Sobe apenas o banco:
```bash
docker compose up db
```

Em outro terminal:
```bash
dotnet run
```

A `ConnectionString` do `appsettings.json` já aponta para `localhost:5432`.

---

## Mapa do projeto

```
MeuProjetoMVC/
├── Controllers/
│   ├── HomeController.cs              # rota raiz, página de erro
│   ├── ProdutosController.cs          # CRUD MVC com Views, usando EF Core
│   ├── RelatoriosController.cs        # exemplo com ADO.NET puro
│   └── Api/
│       └── ProdutosApiController.cs   # API REST (JSON), usando EF Core
├── Data/
│   ├── AppDbContext.cs                # ORM (EF Core)
│   └── AdoNetProdutoRepository.cs     # ADO.NET puro com Npgsql
├── Filters/
│   ├── LogActionFilter.cs             # action filter (log antes/depois)
│   └── ApiExceptionFilter.cs          # exception filter para a API
├── Models/
│   ├── Produto.cs                     # entidade + validações
│   └── ErrorViewModel.cs
├── Views/
│   ├── Shared/_Layout.cshtml          # layout master
│   ├── Home/...                       # views da home
│   ├── Produtos/...                   # CRUD de produtos
│   └── Relatorios/Index.cshtml        # relatório ADO.NET
├── wwwroot/                           # estáticos (CSS, JS, libs do bootstrap)
├── db/init.sql                        # cria tabela e popula no 1º start do Postgres
├── Dockerfile                         # imagem da aplicação
├── docker-compose.yml                 # orquestra app + db
├── Program.cs                         # bootstrap, DI, pipeline HTTP
├── appsettings.json                   # configurações
└── MeuProjetoMVC.csproj               # referências NuGet
```

---

## Conceitos rápidos para revisar

- **MVC**: o cliente faz uma requisição → o **roteamento** descobre qual *Controller/Action* tratar → a action interage com o **Model** (dados) e devolve uma **View** (HTML) ou um JSON.
- **Razor**: motor de templates `.cshtml` onde se misturam HTML e C# (com `@`).
- **Tag Helpers**: atributos `asp-...` que geram HTML/URLs/forms respeitando as rotas e o ModelState.
- **Model Binding**: o framework copia dados do request (rota, querystring, formulário, JSON) para os parâmetros das actions.
- **DataAnnotations**: atributos como `[Required]`, `[Range]`, `[StringLength]` que validam tanto no servidor (`ModelState.IsValid`) quanto no cliente (via jQuery Validate).
- **EF Core**: ORM. `DbContext` + `DbSet<T>` + LINQ → SQL automático.
- **ADO.NET**: API de mais baixo nível para falar SQL puro. Útil para performance crítica e relatórios.
- **Filtros**: interceptadores do pipeline MVC. Tipos: Authorization, Resource, Action, Exception, Result.
- **Injeção de dependência**: o ASP.NET Core já vem com um contêiner DI nativo. Basta registrar em `Program.cs` e declarar no construtor.
- **Middleware**: a pipeline `app.Use*(...)` que processa cada request. A ordem importa.

---

## Endpoints da API REST

| Método | URL | Ação |
|---|---|---|
| GET | `/api/produtos` | Lista todos |
| GET | `/api/produtos/{id}` | Obtém por id |
| POST | `/api/produtos` | Cria (corpo JSON) |
| PUT | `/api/produtos/{id}` | Atualiza |
| DELETE | `/api/produtos/{id}` | Remove |

Exemplo:

```bash
curl -X POST http://localhost:8080/api/produtos \
  -H "Content-Type: application/json" \
  -d '{"nome":"Estojo","descricao":"Com 3 divisórias","preco":39.90,"estoque":15}'
```
