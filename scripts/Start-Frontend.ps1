# Start-Frontend.ps1
# Builds and starts the Blazor WASM frontend

$ProjectDir = "$PSScriptRoot/.."
$ClientProject = Join-Path $ProjectDir "src/Learning.Client"
$Port = "http://localhost:5035"
$ApiUrl = "http://localhost:5200"

Write-Host "Building frontend..." -ForegroundColor Cyan

# Set environment variable for API base URL before build/start
$env:ApiBaseUrl = $ApiUrl

$buildResult = & dotnet build $ClientProject --nologo -v q 2>&1
if ($LASTEXITCODE -ne 0) {
    Write-Host "Build failed:" -ForegroundColor Red
    $buildResult | Where-Object { $_ -is [System.Management.Automation.ErrorRecord] } | ForEach-Object { Write-Host $_ -ForegroundColor Red }
    exit 1
}

Write-Host "Build succeeded." -ForegroundColor Green
Write-Host "Starting frontend on $Port..." -ForegroundColor Yellow
Write-Host "  API proxy: $ApiUrl" -ForegroundColor White
Write-Host ""
Write-Host "Browse to: $Port" -ForegroundColor Green
Write-Host ""
Write-Host "Press Ctrl+C to stop." -ForegroundColor DarkGray

# Run the client (blocks until stopped)
dotnet run --project $ClientProject --no-build --urls $Port
