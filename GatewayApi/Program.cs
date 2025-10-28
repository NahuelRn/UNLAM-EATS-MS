using Ocelot.DependencyInjection;
using Ocelot.Middleware;

var builder = WebApplication.CreateBuilder(args);

// 1. Carga el archivo ocelot.json como configuración
builder.Configuration.AddJsonFile("ocelot.json", optional: false, reloadOnChange: true);

// 2. Añade los servicios de Ocelot al contenedor de inyección de dependencias
builder.Services.AddOcelot(builder.Configuration);

var app = builder.Build();

// 3. Configura el pipeline para que use Ocelot
// Esto reemplaza a app.MapControllers(), Swagger, etc.
app.UseOcelot().Wait();

app.Run();