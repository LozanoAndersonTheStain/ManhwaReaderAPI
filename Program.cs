using ManhwaReaderAPI.Domain.Interfaces;
using ManhwaReaderAPI.Application.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new Microsoft.OpenApi.Models.OpenApiInfo
    {
        Title = "ManhwaReader API",
        Version = "v1",
        Description = "API para la aplicación ManhwaReader",
        Contact = new Microsoft.OpenApi.Models.OpenApiContact
        {
            Name = "Equipo ManhwaReader",
            Email = "contact@manhwareader.com"
        }
    });
});

// Configure CORS
builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(builder =>
    {
        builder.AllowAnyOrigin()
               .AllowAnyMethod()
               .AllowAnyHeader();
    });
});

// Register services
builder.Services.AddScoped<IManhwaService, ManhwaService>();
builder.Services.AddControllers();

// Add DbConfig as a singleton service
builder.Services.AddSingleton<ManhwaReaderAPI.Infrastructure.Data.Config.DbConfig>();

var app = builder.Build();

// Test database connection at startup
var dbConfig = app.Services.GetRequiredService<ManhwaReaderAPI.Infrastructure.Data.Config.DbConfig>();
if (dbConfig.ValidateConnection(out string message))
{
    app.Logger.LogInformation(message);
}
else
{
    app.Logger.LogError(message);
}

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "ManhwaReader API V1");
        c.RoutePrefix = string.Empty; // Para que Swagger UI sea la página de inicio
    });
}

// Enable CORS
app.UseCors();

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

app.Run();
