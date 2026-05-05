# =============================================================================
# Dockerfile multi-stage para a aplicação ASP.NET Core.
#
# Multi-stage = usamos uma imagem grande (com SDK) para COMPILAR, e depois
# copiamos só o resultado para uma imagem pequena (com runtime). A imagem final
# fica enxuta e segura.
# =============================================================================

# -----------------------------------------------------------------------------
# Stage 1: BUILD — compila o projeto e publica os artefatos.
# -----------------------------------------------------------------------------
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Variáveis de ambiente que ajudam o NuGet em redes restritas/instáveis:
#   - DOTNET_NOLOGO: silencia o banner.
#   - NUGET_XMLDOC_MODE: economiza tempo desabilitando download de XML docs.
#   - NUGET_HTTP_REQUEST_TIMEOUT_SECONDS: aumenta o timeout dos downloads.
ENV DOTNET_NOLOGO=1 \
    NUGET_XMLDOC_MODE=skip \
    NUGET_HTTP_REQUEST_TIMEOUT_SECONDS=300

# Copia só o csproj primeiro para aproveitar o cache de layers do Docker:
# se nada mudar no csproj, o "restore" não é refeito a cada build.
COPY MeuProjetoMVC.csproj ./
# --disable-parallel reduz contenção de rede e dá logs mais legíveis quando
# há problemas. Útil em ambientes com DNS/proxy lento.
RUN dotnet restore MeuProjetoMVC.csproj --disable-parallel

# Agora copia o resto do código e publica em modo Release.
# --no-restore: já restauramos acima, evita refazer.
COPY . .
RUN dotnet publish MeuProjetoMVC.csproj -c Release -o /app/publish \
    --no-restore /p:UseAppHost=false

# -----------------------------------------------------------------------------
# Stage 2: RUNTIME — imagem final, só com o necessário para rodar.
# -----------------------------------------------------------------------------
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime
WORKDIR /app

# Copia os artefatos publicados do stage anterior.
COPY --from=build /app/publish ./

# A porta padrão do Kestrel em containers é 8080 no .NET 8. Documentamos aqui
# para clareza (EXPOSE não publica a porta, só sinaliza qual porta o app usa).
EXPOSE 8080
ENV ASPNETCORE_URLS=http://+:8080
ENV ASPNETCORE_ENVIRONMENT=Docker

# ENTRYPOINT define o comando que roda quando o container inicia.
ENTRYPOINT ["dotnet", "MeuProjetoMVC.dll"]
