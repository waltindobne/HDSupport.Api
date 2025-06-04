# Etapa 1: Build
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /app

# Copia o .csproj e restaura as dependências
COPY *.sln .
COPY HD-Support-API/*.csproj ./HD-Support-API/
RUN dotnet restore

# Copia o restante do código
COPY HD-Support-API/. ./HD-Support-API/
WORKDIR /app/HD-Support-API

# Compila em modo Release
RUN dotnet publish -c Release -o out

# Etapa 2: Runtime
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime
WORKDIR /app
COPY --from=build /app/HD-Support-API/out ./

# Variável de ambiente opcional para produção
ENV ASPNETCORE_URLS=http://+:5000

# Expõe a porta
EXPOSE 5000

# Executa a aplicação
ENTRYPOINT ["dotnet", "HD-Support-API.dll"]