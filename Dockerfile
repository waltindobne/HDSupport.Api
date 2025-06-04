# Base image
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 5000

# Build image
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release

WORKDIR /src

# Copia o arquivo de projeto e restaura as dependências
COPY HD-Support-API/HD-Support-API.csproj ./HD-Support-API/
RUN dotnet restore "hd-support-api/HD-Support-API.csproj"

# Copia o restante do código e faz o build
COPY HD-Support-API/. ./HD-Support-API/
WORKDIR /src/HD-Support-API
RUN dotnet build "HD-Support-API.csproj" -c $BUILD_CONFIGURATION -o /app/build

# Publica a aplicação
FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "HD-Support-API.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

# Final
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "HD-Support-API.dll"]
