using System.IdentityModel.Tokens.Jwt;
using System.Text;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using VisionLab.Application.Abstractions;
using VisionLab.Domain.Usuarios;

namespace VisionLab.Infrastructure.Security;

internal sealed class JwtTokenService(IOptions<JwtOptions> options) : ITokenService
{
    public string Crear(Usuario usuario)
    {
        var o = options.Value;
        var credenciales = new SigningCredentials(
            new SymmetricSecurityKey(Encoding.UTF8.GetBytes(o.Key)), SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(
            issuer: o.Issuer,
            audience: o.Audience,
            claims:
            [
                new System.Security.Claims.Claim(JwtRegisteredClaimNames.Sub, usuario.Id),
                new System.Security.Claims.Claim(JwtRegisteredClaimNames.Name, usuario.NombreUsuario),
                new System.Security.Claims.Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString("N"))
            ],
            expires: DateTime.UtcNow.AddDays(o.DiasValidez),
            signingCredentials: credenciales);

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}
