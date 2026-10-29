using Microsoft.Extensions.Options;
using VisionLab.Application.Abstractions;

namespace VisionLab.Api.Security;

public sealed class DemoUserOptions
{
    public const string SectionName = "DemoUser";
    public string? Id { get; set; }
}

/// <summary>
/// Implementación TEMPORAL de <see cref="ICurrentUser"/>: devuelve el usuario demo configurado,
/// igual que el comportamiento heredado. La feature de seguridad la reemplaza por una que lee
/// el claim del JWT; los casos de uso no cambian.
/// </summary>
public sealed class DemoCurrentUser(IOptions<DemoUserOptions> options) : ICurrentUser
{
    public string? UserId { get; } = string.IsNullOrWhiteSpace(options.Value.Id) ? null : options.Value.Id;
}
