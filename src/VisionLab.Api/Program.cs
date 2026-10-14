using System.Text.Json.Serialization;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.OpenApi.Models;
using VisionLab.Api.Security;
using VisionLab.Application;
using VisionLab.Application.Abstractions;
using VisionLab.Infrastructure;

const string CorsPolicy = "Frontend";

var builder = WebApplication.CreateBuilder(args);

// --- Capas ---
builder.Services.AddApplication();
builder.Services.AddInfrastructure(builder.Configuration);

// --- Usuario actual (temporal: demo; se reemplaza por JWT) ---
builder.Services.Configure<DemoUserOptions>(builder.Configuration.GetSection(DemoUserOptions.SectionName));
builder.Services.AddScoped<ICurrentUser, DemoCurrentUser>();

// --- API ---
builder.Services.AddControllers()
    .AddJsonOptions(o =>
        // Gemini a veces devuelve números como texto ("95"); se aceptan ambos.
        o.JsonSerializerOptions.NumberHandling = JsonNumberHandling.AllowReadingFromString);
builder.Services.AddProblemDetails();

builder.Services.AddCors(o => o.AddPolicy(CorsPolicy, policy => policy
    .WithOrigins(builder.Configuration.GetSection("Cors:AllowedOrigins").Get<string[]>() ?? [])
    .AllowAnyHeader()
    .AllowAnyMethod()));

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(o =>
{
    o.SwaggerDoc("v1", new OpenApiInfo
    {
        Version = "v1",
        Title = "VisionLab API",
        Description = "Microscopio digital para niños: hallazgos, usuarios y gamificación."
    });
    var xml = Path.Combine(AppContext.BaseDirectory, "VisionLab.Api.xml");
    if (File.Exists(xml)) o.IncludeXmlComments(xml);
});

// Detrás de un proxy / balanceador (Docker, nginx, nube) respeta el esquema e IP originales.
builder.Services.Configure<ForwardedHeadersOptions>(o =>
{
    o.ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto;
    o.KnownNetworks.Clear();
    o.KnownProxies.Clear();
});

var app = builder.Build();

app.UseForwardedHeaders();
app.UseExceptionHandler();
app.UseStatusCodePages();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(o =>
    {
        o.SwaggerEndpoint("/swagger/v1/swagger.json", "VisionLab API v1");
        o.RoutePrefix = string.Empty;
        o.DocumentTitle = "VisionLab API";
    });
}

app.UseHttpsRedirection();
app.UseCors(CorsPolicy);
app.UseAuthorization();

app.MapControllers();
app.MapHealthChecks("/health");

app.Run();
