using System.Reflection;
using Microsoft.Extensions.Options;
using Microsoft.OpenApi.Models;
using MongoDB.Driver;
using VisionLabApi.Infraestructura;
using VisionLabApi.Models;
using VisionLabApi.Services;
using VisionLabApi.Services.VisionLabApi.Services;

namespace VisionLabApi
{
    public class Program
    {
        public static void Main(string[] args)
        {
            try
            {
                var builder = WebApplication.CreateBuilder(args);

                // --- REGISTRO DE INFRAESTRUCTURA DE GAMIFICACIÓN ---
                builder.Services.AddSingleton<IUsuarioRepository, UsuarioRepository>();
                builder.Services.AddSingleton<ILogroRepository, LogroRepository>();
                builder.Services.AddSingleton<INivelRepository, NivelRepository>();
                // REGISTRO DE SERVICIO DE AUTENTICACIÓN
                builder.Services.AddSingleton<IAuthService, AuthService>();

                // --- REGISTRO DE SERVICIO DE GAMIFICACIÓN ---
                builder.Services.AddSingleton<IGamificationService, GamificationService>();

                // ... (El servicio de Hallazgos que usa el de Gamificación)
                builder.Services.AddSingleton<IHallazgoService, HallazgoService>();

                // CONFIGURAR CORS (IMPORTANTE)
                // ============================================
                builder.Services.AddCors(options =>
                {
                    options.AddPolicy("AllowAngularApp", policy =>
                    {
                        policy.WithOrigins("http://localhost:4200") // URL de Angular
                              .AllowAnyHeader()
                              .AllowAnyMethod()
                              .AllowCredentials();
                    });
                });

                // Configurar opciones de MongoDB
                builder.Services.Configure<VisionLabDatabaseSettings>(
                    builder.Configuration.GetSection("VisionLabDatabase")
                );

                // Registrar cliente MongoDB
                builder.Services.AddSingleton<IMongoClient>(sp =>
                {
                    var settings = sp.GetRequiredService<IOptions<VisionLabDatabaseSettings>>().Value;
                    return new MongoClient(settings.ConnectionString);
                });

                builder.Services.AddControllers();
                builder.Services.AddEndpointsApiExplorer();

                // Configurar Swagger con documentación detallada
                builder.Services.AddSwaggerGen(options =>
                {
                    options.SwaggerDoc("v1", new OpenApiInfo
                    {
                        Version = "v1",
                        Title = "VisionLab API",
                        Description = "API para la gestión de hallazgos, usuarios, niveles y logros del sistema VisionLab",
                        Contact = new OpenApiContact
                        {
                            Name = "Tu Nombre o Equipo",
                            Email = "tuemail@ejemplo.com",
                            Url = new Uri("https://tuportafolio.com")
                        },
                        License = new OpenApiLicense
                        {
                            Name = "MIT License",
                            Url = new Uri("https://opensource.org/licenses/MIT")
                        }
                    });

                    // Incluir comentarios XML para documentación (opcional)
                    var xmlFilename = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
                    var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFilename);
                    if (File.Exists(xmlPath))
                    {
                        options.IncludeXmlComments(xmlPath);
                    }
                });

                var app = builder.Build();

                // ============================================
                // ⚠️ USAR CORS - ESTO FALTABA! ⚠️
                // Debe ir ANTES de UseAuthorization y MapControllers
                // ============================================
                app.UseCors("AllowAngularApp");

                if (app.Environment.IsDevelopment())
                {
                    app.UseSwagger();
                    app.UseSwaggerUI(options =>
                    {
                        options.SwaggerEndpoint("/swagger/v1/swagger.json", "VisionLab API v1");
                        options.RoutePrefix = string.Empty;
                        options.DocumentTitle = "VisionLab API Documentation";
                    });
                }

                app.UseHttpsRedirection();
                app.UseAuthorization();
                app.MapControllers();

                app.Run();
            }
            catch (ReflectionTypeLoadException ex)
            {
                Console.WriteLine("=== ReflectionTypeLoadException Detalles ===");
                Console.WriteLine($"Mensaje: {ex.Message}");

                if (ex.LoaderExceptions != null)
                {
                    foreach (var loaderEx in ex.LoaderExceptions)
                    {
                        Console.WriteLine($"\nLoader Exception: {loaderEx?.Message}");
                        if (loaderEx?.InnerException != null)
                        {
                            Console.WriteLine($"Inner Exception: {loaderEx.InnerException.Message}");
                        }
                    }
                }
                throw;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error general: {ex.GetType().Name}");
                Console.WriteLine($"Mensaje: {ex.Message}");
                Console.WriteLine($"StackTrace: {ex.StackTrace}");
                throw;
            }
        }
    }
}