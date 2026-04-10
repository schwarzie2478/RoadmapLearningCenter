# Start-All.ps1
# Starts everything via .NET Aspire: PostgreSQL, Server, Client, and Dashboard
# The Aspire Dashboard opens automatically at http://localhost:15888

$ProjectDir = "$PSScriptRoot/.."
$AppHost = Join-Path $ProjectDir "src/Learning.AppHost"

Write-Host "Starting Learning via .NET Aspire..." -ForegroundColor Cyan
Write-Host "  Launches: PostgreSQL (Docker), Server, Client, Dashboard" -ForegroundColor DarkGray
Write-Host ""
Write-Host "Browse to the Aspire Dashboard to monitor all components." -ForegroundColor Green
Write-Host ""

dotnet run --project $AppHost
