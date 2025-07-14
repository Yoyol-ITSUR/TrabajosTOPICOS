using Microsoft.EntityFrameworkCore;
using WebApiSCAR.Models;
using System.Text.Json.Serialization; // Importar para opciones de serializaci�n JSON

var builder = WebApplication.CreateBuilder(args);

// Configuraci�n del contexto de la base de datos con MySQL
builder.Services.AddDbContext<SCARContext>(options =>
    options.UseMySQL(
        builder.Configuration.GetConnectionString("DefaultConnection") // Obtiene la cadena de conexi�n desde appsettings.json
    )
);

// Configuraci�n de los controladores y las opciones de serializaci�n JSON.
// Esto asegura que los objetos C# se conviertan a JSON de manera consistente.
builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        // Configura el serializador para incluir propiedades con valores por defecto (ej. 0 para int, null para string).
        options.JsonSerializerOptions.DefaultIgnoreCondition = JsonIgnoreCondition.Never;
        // Maneja referencias circulares en el grafo de objetos para evitar bucles infinitos durante la serializaci�n.
        // �til en modelos con relaciones complejas (ej. Residente -> User -> Residente).
        options.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles;
        options.JsonSerializerOptions.WriteIndented = true;
    });

// Configuraci�n de Swagger/OpenAPI para la documentaci�n de la API.
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configuraci�n del pipeline de solicitudes HTTP.
// Habilita Swagger UI en entorno de desarrollo para probar la API.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// Redirecciona las solicitudes HTTP a HTTPS para seguridad.
app.UseHttpsRedirection();

// Habilita la autorizaci�n para los endpoints de la API.
app.UseAuthorization();

// Mapea los controladores a las rutas de la API.
app.MapControllers();

// Asegura que la base de datos se cree si no existe al iniciar la aplicaci�n.
// Esto es �til para el desarrollo y la configuraci�n inicial.
using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<SCARContext>();
    dbContext.Database.EnsureCreated(); // Crea la base de datos si no existe
}

// Inicia la aplicaci�n Web API.
app.Run();
