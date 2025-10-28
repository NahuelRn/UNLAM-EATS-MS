using Microsoft.EntityFrameworkCore;
using ProductosApi.Models;
using ProductosApi.Services;

var builder = WebApplication.CreateBuilder(args);

var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");

Console.WriteLine($"---> USANDO ConnectionString: {connectionString}");

builder.Services.AddDbContext<ProductoContext>(options =>
    options.UseSqlServer(connectionString, sqlServerOptionsAction: sqlOptions =>
    {
        sqlOptions.EnableRetryOnFailure(
            maxRetryCount: 5, // Intentará hasta 5 veces
            maxRetryDelay: TimeSpan.FromSeconds(30), // Espera máximo 30s entre intentos
            errorNumbersToAdd: null); // Usa los errores transitorios por defecto de SQL Server
    })); // <-- Cierre del paréntesis adicional

// Add services to the container.
builder.Services.AddScoped<IProductoService, ProductoService>();

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
