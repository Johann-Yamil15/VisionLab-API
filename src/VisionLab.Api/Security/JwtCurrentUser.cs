using System.IdentityModel.Tokens.Jwt;
using VisionLab.Application.Abstractions;

namespace VisionLab.Api.Security;

/// <summary>Usuario actual leído del claim "sub" del JWT validado. Nunca del body ni de la URL.</summary>
public sealed class JwtCurrentUser(IHttpContextAccessor accessor) : ICurrentUser
{
    public string? UserId => accessor.HttpContext?.User.FindFirst(JwtRegisteredClaimNames.Sub)?.Value;
}
