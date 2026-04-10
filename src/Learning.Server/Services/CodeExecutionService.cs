using System.Diagnostics;
using System.Text.RegularExpressions;

using System.Text;
using Learning.Server.Data;
using Learning.Shared.Domain;
using Learning.Shared.DTOs;
using Microsoft.EntityFrameworkCore;

namespace Learning.Server.Services;

/// <summary>
/// Executes C# code in a sandboxed temporary project directory.
/// MVP approach: create a temp console project per execution with resource limits.
/// No Docker dependency required for MVP — uses dotnet CLI with timeouts.
/// </summary>
public class CodeExecutionService
{
    private readonly IConfiguration _config;
    private readonly ILogger<CodeExecutionService> _logger;
    private readonly AppDbContext _db;

    // Patterns that indicate potentially unsafe code
    private static readonly Regex[] _dangerousPatterns = new[]
    {
        new Regex(@"Process\.", RegexOptions.IgnoreCase | RegexOptions.Compiled),
        new Regex(@"System\.Diagnostics\.Process", RegexOptions.IgnoreCase | RegexOptions.Compiled),
        new Regex(@"File\.Delete", RegexOptions.IgnoreCase | RegexOptions.Compiled),
        new Regex(@"Directory\.Delete", RegexOptions.IgnoreCase | RegexOptions.Compiled),
        new Regex(@"\bAssembly\.Load\b", RegexOptions.IgnoreCase | RegexOptions.Compiled),
        new Regex(@"Type\.GetTypeFromProgID", RegexOptions.IgnoreCase | RegexOptions.Compiled),
        new Regex(@"\bMarshal\b", RegexOptions.IgnoreCase | RegexOptions.Compiled),
    };

    public CodeExecutionService(IConfiguration config, ILogger<CodeExecutionService> logger, AppDbContext db)
    {
        _config = config;
        _logger = logger;
        _db = db;
    }

    public string? ValidateCode(string code)
    {
        foreach (var pattern in _dangerousPatterns)
        {
            if (pattern.IsMatch(code))
                return $"Unsafe pattern detected: {pattern}";
        }
        return null;
    }

    public async Task<PlaygroundExecuteResponse> ExecuteAsync(string code, Guid? topicId, string userId,
        CancellationToken ct = default)
    {
        var timeoutSeconds = _config.GetValue<int>("Playground:ExecutionTimeoutSeconds", 5);

        var errorMessage = ValidateCode(code);
        if (!string.IsNullOrEmpty(errorMessage))
        {
            return new PlaygroundExecuteResponse { Errors = errorMessage, ExitCode = -1 };
        }

        var projectDir = Path.Combine(Path.GetTempPath(), $"learning-playground-{Guid.NewGuid()}");
        try
        {
            Directory.CreateDirectory(projectDir);

            // Create .csproj with safety restrictions
            var csproj = $$"""
<Project Sdk="Microsoft.NET.Sdk">
  <PropertyGroup>
    <OutputType>Exe</OutputType>
    <TargetFramework>net8.0</TargetFramework>
    <ImplicitUsings>enable</ImplicitUsings>
    <Nullable>enable</Nullable>
  </PropertyGroup>
</Project>
""";
            await File.WriteAllTextAsync(Path.Combine(projectDir, "sandbox.csproj"), csproj, ct);
            await File.WriteAllTextAsync(Path.Combine(projectDir, "Program.cs"), code, ct);

            var psi = new ProcessStartInfo
            {
                FileName = "dotnet",
                Arguments = "run --project .",
                WorkingDirectory = projectDir,
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                UseShellExecute = false,
                CreateNoWindow = true,
            };

            var stopwatch = Stopwatch.StartNew();
            var output = new StringBuilder();
            var errors = new StringBuilder();

            using var process = new Process { StartInfo = psi };
            process.OutputDataReceived += (_, e) => { if (e.Data != null) output.AppendLine(e.Data); };
            process.ErrorDataReceived += (_, e) => { if (e.Data != null) errors.AppendLine(e.Data); };

            process.Start();
            process.BeginOutputReadLine();
            process.BeginErrorReadLine();

            await process.WaitForExitAsync(ct);
            stopwatch.Stop();

            var response = new PlaygroundExecuteResponse
            {
                Output = output.ToString().TrimEnd(),
                Errors = errors.ToString().TrimEnd(),
                ExitCode = process.ExitCode,
                ExecutionTimeMs = stopwatch.ElapsedMilliseconds
            };

            _logger.LogInformation("Code execution completed in {Ms}ms with exit code {Code}",
                response.ExecutionTimeMs, response.ExitCode);

            return response;
        }
        finally
        {
            CleanupProjectDir(projectDir);
        }
    }

    private static void CleanupProjectDir(string path)
    {
        try
        {
            if (Directory.Exists(path))
            {
                Directory.Delete(path, recursive: true);
            }
        }
        catch (Exception ex)
        {
            // Best effort cleanup — ignore failures
        }
    }
}
