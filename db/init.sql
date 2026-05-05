-- =============================================================================
-- init.sql — script executado UMA ÚNICA VEZ na primeira inicialização do
-- container do Postgres (quando o volume "pgdata" ainda está vazio).
--
-- Cria a tabela "produtos" e popula com alguns registros de exemplo.
-- A estrutura espelha o que o EF Core esperaria a partir do Models/Produto.cs:
--   nomes em snake_case, NUMERIC para preço, TIMESTAMP com default NOW().
--
-- Em projetos reais, a recomendação é usar MIGRATIONS do EF Core
-- (`dotnet ef migrations add`/`dotnet ef database update`). Aqui usamos SQL
-- direto pra você ver claramente o esquema que o ORM mapeia.
-- =============================================================================

CREATE TABLE IF NOT EXISTS produtos (
    id          SERIAL PRIMARY KEY,
    nome        VARCHAR(100) NOT NULL UNIQUE,
    descricao   VARCHAR(500),
    preco       NUMERIC(12, 2) NOT NULL CHECK (preco > 0),
    estoque     INTEGER NOT NULL DEFAULT 0 CHECK (estoque >= 0),
    criado_em   TIMESTAMP NOT NULL DEFAULT NOW()
);

-- Dados de exemplo (idempotente: ON CONFLICT evita erro se rodar de novo).
INSERT INTO produtos (nome, descricao, preco, estoque) VALUES
    ('Caderno Universitário', 'Caderno 200 folhas, 10 matérias',  29.90, 12),
    ('Caneta Esferográfica',  'Tinta azul, ponta 1.0mm',           3.50,  3),
    ('Mochila Escolar',       'Mochila de costas, 25L',          189.90,  7),
    ('Lápis HB',              'Lápis grafite n.º 2',               1.20, 50),
    ('Borracha Branca',       'Borracha plástica macia',           2.00,  1)
ON CONFLICT (nome) DO NOTHING;
