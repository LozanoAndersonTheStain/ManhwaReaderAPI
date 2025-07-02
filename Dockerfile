# Etapa de construcción
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src

# Copiar y restaurar dependencias
COPY ["ManhwaReaderAPI.csproj", "./"]
RUN dotnet restore

# Copiar el resto del código y publicar
COPY . .
RUN dotnet publish -c Release -o /app/publish

# Etapa de ejecución
FROM mcr.microsoft.com/dotnet/aspnet:9.0
WORKDIR /app
COPY --from=build /app/publish .

# Crear directorio para keys con permisos adecuados
RUN mkdir -p /app/keys && \
    chown -R 1000:1000 /app/keys && \
    chmod -R 700 /app/keys

EXPOSE 5193

ENTRYPOINT ["dotnet", "ManhwaReaderAPI.dll"]