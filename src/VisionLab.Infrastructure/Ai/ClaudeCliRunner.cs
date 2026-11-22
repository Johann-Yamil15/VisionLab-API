using System.Diagnostics;
using System.Text;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using VisionLab.Application.Abstractions;

namespace VisionLab.Infrastructure.Ai;

public sealed class ClaudeCliOptions
{
    public const string SectionName = "ClaudeCli";

    /// <summary>Ejecutable. En Windows debe ser el .exe (p. ej. la ruta completa a claude.exe).</summary>
    public string Command { get; set; } = "claude";

    /// <summary>Tiempo máximo por llamada; pasado esto se mata el proceso y se responde 503.</summary>
    public int TimeoutSeconds { get; set; } = 60;
}

/// <summary>Ejecuta <c>claude -p</c> como proceso hijo, con el prompt por stdin y sin shell.</summary>
public sealed class ClaudeCliRunner(IOptions<ClaudeCliOptions> options, ILogger<ClaudeCliRunner> logger)
{
    /// <param name="prompt">Texto que se envía por stdin.</param>
    /// <param name="tools">Herramientas que se le permiten al CLI ("Read" o "" para ninguna).</param>
    /// <param name="workDir">Directorio de trabajo (carpeta temporal de la petición).</param>
    /// <exception cref="IaNoDisponibleException">No arrancó, falló, tardó demasiado o no respondió nada.</exception>
    public async Task<string> RunAsync(string prompt, string tools, string workDir, CancellationToken ct)
    {
        var opts = options.Value;
        var psi = new ProcessStartInfo(opts.Command)
        {
            WorkingDirectory = workDir,
            RedirectStandardInput = true,
            RedirectStandardOutput = true,
            RedirectStandardError = true,
            UseShellExecute = false,
            CreateNoWindow = true,
            StandardInputEncoding = new UTF8Encoding(false),
            StandardOutputEncoding = Encoding.UTF8,
            StandardErrorEncoding = Encoding.UTF8,
        };
        // Solo lectura: --tools limita las herramientas disponibles (sin Bash, Edit ni Write).
        psi.ArgumentList.Add("-p");
        psi.ArgumentList.Add("--tools");
        psi.ArgumentList.Add(tools);

        using var timeout = CancellationTokenSource.CreateLinkedTokenSource(ct);
        timeout.CancelAfter(TimeSpan.FromSeconds(opts.TimeoutSeconds));

        Process process;
        try
        {
            process = Process.Start(psi) ?? throw new InvalidOperationException("No se pudo iniciar el proceso.");
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "No se pudo ejecutar el CLI de Claude ({Command})", opts.Command);
            throw new IaNoDisponibleException("La IA no está disponible en este momento.", ex);
        }

        using (process)
        {
            try
            {
                var stdout = process.StandardOutput.ReadToEndAsync(timeout.Token);
                var stderr = process.StandardError.ReadToEndAsync(timeout.Token);

                await process.StandardInput.WriteAsync(prompt.AsMemory(), timeout.Token);
                process.StandardInput.Close();

                await process.WaitForExitAsync(timeout.Token);
                var salida = (await stdout).Trim();

                if (process.ExitCode != 0 || salida.Length == 0)
                {
                    logger.LogWarning("El CLI de Claude terminó con código {Code}: {Error}", process.ExitCode, await stderr);
                    throw new IaNoDisponibleException("La IA no pudo responder. Inténtalo de nuevo.");
                }

                return salida;
            }
            catch (OperationCanceledException) when (!ct.IsCancellationRequested)
            {
                logger.LogWarning("El CLI de Claude superó {Seconds}s; se cancela", opts.TimeoutSeconds);
                Matar(process);
                throw new IaNoDisponibleException("La IA tardó demasiado. Inténtalo de nuevo.");
            }
            catch (OperationCanceledException)
            {
                Matar(process); // el cliente cerró la conexión
                throw;
            }
            catch (IOException ex)
            {
                Matar(process);
                throw new IaNoDisponibleException("La IA no pudo responder. Inténtalo de nuevo.", ex);
            }
        }
    }

    private static void Matar(Process process)
    {
        try { if (!process.HasExited) process.Kill(entireProcessTree: true); }
        catch (InvalidOperationException) { /* ya terminó */ }
    }
}
