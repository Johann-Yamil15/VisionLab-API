using System.IdentityModel.Tokens.Jwt;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.RateLimiting;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using VisionLab.Api.Common;
using VisionLab.Api.Security;
using VisionLab.Application;
using VisionLab.Application.Abstractions;
using VisionLab.Infrastructure;
using VisionLab.Infrastructure.Security;

const string CorsPolicy = "Frontend";

var builder = WebApplication.CreateBuilder(args);

// --- Capas ---
builder.Services.AddApplication();
builder.Services.AddInfrastructure(builder.Configuration);

// --- Autenticación JWT: el usuario actual sale del token ---
var jwt = builder.Configuration.GetSection(JwtOptions.SectionName).Get<JwtOptions>() ?? new JwtOptions();
builder.Services.AddHttpContextAccessor();
builder.Services.AddScoped<ICurrentUser, JwtCurrentUser>();
builder.Services
    .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(o =>
    {
        o.MapInboundClaims = false; // conserva "sub" tal cual
        o.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidIssuer = jwt.Issuer,
            ValidateAudience = true,
            ValidAudience = jwt.Audience,
            ValidateLifetime = true,
            ClockSkew = TimeSpan.FromMinutes(1),
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwt.Key.Length >= 32 ? jwt.Key : new string('x', 32)))
        };
    });
builder.Services.AddAuthorization();

// --- Límites de uso: la IA cuesta y login/registro son blanco de fuerza bruta ---
builder.Services.AddRateLimiter(o =>
{
    o.RejectionStatusCode = StatusCodes.Status429TooManyRequests;
    o.AddPolicy(RateLimitPolicies.Ia, ctx => RateLimitPartition.GetFixedWindowLimiter(
        ctx.User.FindFirst(JwtRegisteredClaimNames.Sub)?.Value ?? ctx.Connection.RemoteIpAddress?.ToString() ?? "anon",
        _ => new FixedWindowRateLimiterOptions { PermitLimit = 20, Window = TimeSpan.FromMinutes(1) }));
    o.AddPolicy(RateLimitPolicies.Auth, ctx => RateLimitPartition.GetFixedWindowLimiter(
        ctx.Connection.RemoteIpAddress?.ToString() ?? "anon",
        _ => new FixedWindowRateLimiterOptions { PermitLimit = 10, Window = TimeSpan.FromMinutes(1) }));
});

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
app.UseAuthentication();
app.UseAuthorization();
app.UseRateLimiter();

app.MapControllers();
app.MapHealthChecks("/health");

app.Run();
