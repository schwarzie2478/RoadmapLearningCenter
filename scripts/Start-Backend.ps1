# Start-Backend.ps1
# Builds and starts the ASP.NET Core backend server

$ProjectDir = "$PSScriptRoot/.."
$ServerProject = Join-Path $ProjectDir "src/Learning.Server"
$Port = "http://localhost:5200"
$ClientOrigin = "http://localhost:5035"

Write-Host "Building backend..." -ForegroundColor Cyan

# Restore and build in one pass
$buildResult = & dotnet build $ServerProject --nologo -v q 2>&1
if ($LASTEXITCODE -ne 0) {
    Write-Host "Build failed:" -ForegroundColor Red
    $buildResult | Where-Object { $_ -is [System.Management.Automation.ErrorRecord] } | ForEach-Object { Write-Host $_ -ForegroundColor Red }
    exit 1
}

Write-Host "Build succeeded." -ForegroundColor Green
Write-Host "Starting API server on $Port..." -ForegroundColor Yellow
Write-Host `"CORS origin set to: $ClientOrigin`" -ForegroundColor White
Write-Host ""
Write-Host "Endpoints:" -ForegroundColor Cyan
Write-Host "  API     : $Port" -ForegroundColor White
Write-Host "  Swagger : $Port/swagger" -ForegroundColor White
Write-Host ""
Write-Host "Press Ctrl+C to stop." -ForegroundColor DarkGray

# Run the server (blocks until stopped)
dotnet run --project $ServerProject --no-build --urls $Port
