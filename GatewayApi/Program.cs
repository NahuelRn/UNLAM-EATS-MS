using Ocelot.DependencyInjection;
using Ocelot.Middleware;

var builder = WebApplication.CreateBuilder(args);

// 1. Carga el archivo ocelot.json como configuraci�n
builder.Configuration.AddJsonFile("ocelot.json", optional: false, reloadOnChange: true);

// 2. A�ade los servicios de Ocelot al contenedor de inyecci�n de dependencias
builder.Services.AddOcelot(builder.Configuration);

var app = builder.Build();

// 3. Configura el pipeline para que use Ocelot
// Esto reemplaza a app.MapControllers(), Swagger, etc.
app.UseOcelot().Wait();

app.Run();