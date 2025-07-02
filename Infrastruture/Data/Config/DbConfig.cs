using DotNetEnv;
using Npgsql;
using System;

namespace ManhwaReaderAPI.Infrastructure.Data.Config
{
    public class DbConfig
    {
        public string ConnectionString { get; private set; }

        public DbConfig()
        {
            try
            {
                Env.Load();

                // Primero intenta obtener el connection string directo
                ConnectionString = Environment.GetEnvironmentVariable("DB_CONNECTION_STRING") ?? string.Empty;

                if (string.IsNullOrEmpty(ConnectionString))
                {
                    // Construye el string de conexión desde variables individuales
                    var host = Environment.GetEnvironmentVariable("DB_HOST")
                        ?? throw new InvalidOperationException("DB_HOST no configurado");
                    var port = Environment.GetEnvironmentVariable("DB_PORT")
                        ?? throw new InvalidOperationException("DB_PORT no configurado");
                    var database = Environment.GetEnvironmentVariable("DB_NAME")
                        ?? throw new InvalidOperationException("DB_NAME no configurado");
                    var username = Environment.GetEnvironmentVariable("DB_USERNAME")
                        ?? throw new InvalidOperationException("DB_USERNAME no configurado");
                    var password = Environment.GetEnvironmentVariable("DB_PASSWORD")
                        ?? throw new InvalidOperationException("DB_PASSWORD no configurado");

                    // Verifica si se está ejecutando en Docker
                    if (Environment.GetEnvironmentVariable("DOTNET_RUNNING_IN_CONTAINER") == "true")
                    {
                        host = Environment.GetEnvironmentVariable("DB_HOST_IN_DOCKER") ?? "db";
                    }

                    // Add SSL Mode for Azure PostgreSQL
                    var sslMode = Environment.GetEnvironmentVariable("SSL_MODE") ?? "Require";
                    ConnectionString = $"Host={host};Port={port};Database={database};Username={username};Password={password};SSL Mode={sslMode};Trust Server Certificate=true;";
                }

                ValidateConnectionString();
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException($"Error de configuración de base de datos: {ex.Message}", ex);
            }
        }

        private void ValidateConnectionString()
        {
            if (string.IsNullOrEmpty(ConnectionString))
            {
                throw new InvalidOperationException("El string de conexión está vacío");
            }
        }

        public bool ValidateConnection(out string message)
        {
            try
            {
                using var connection = new NpgsqlConnection(ConnectionString);
                connection.Open();
                message = "Conexión a base de datos exitosa";
                return true;
            }
            catch (Exception ex)
            {
                message = $"Falló la conexión a base de datos: {ex.Message}";
                return false;
            }
        }
    }
}