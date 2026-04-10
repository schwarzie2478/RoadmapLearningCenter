param(
    [string]$TestName = "frontend",
    [string]$Url = "http://localhost:5035",
    [int]$TimeoutSeconds = 10
)

$ErrorActionPreference = "Stop"
$ProgressPreference = "SilentlyContinue"

Write-Host "Testing $TestName at $Url..." -ForegroundColor Cyan

try {
    $sw = [System.Diagnostics.Stopwatch]::StartNew()
    $response = Invoke-WebRequest -Uri $Url -UseBasicParsing -TimeoutSec $TimeoutSeconds
    $sw.Stop()
    Write-Host "  Status Code : $($response.StatusCode)" -ForegroundColor Green
    Write-Host "  Response ms : $($sw.ElapsedMilliseconds)" -ForegroundColor White
    if ($response.Content.Length -gt 0) {
        Write-Host "  Content len : $($response.Content.Length) bytes" -ForegroundColor White
    }
    if ($response.Content -match '<title>([^<]+)</title>') {
        Write-Host "  Page Title  : $($matches[1])" -ForegroundColor White
    }
    
    # For API endpoints, also show body preview
    if ($Url -match '/api/') {
        $preview = $response.Content.Substring(0, [Math]::Min(200, $response.Content.Length))
        Write-Host "  Body preview: $preview" -ForegroundColor DarkGray
    }
    
    Write-Host ""
    Write-Host "OK" -ForegroundColor Green
    exit 0
} catch {
    $ex = $_.Exception
    
    # Unwrap WebException for nested details
    $webEx = $ex
    while ($webEx.InnerException -ne $null) { $webEx = $webEx.InnerException }
    
    $status = if ($webEx -is [System.Net.WebException]) {
        $resp = $webEx.Response
        if ($resp -is [System.Net.HttpWebResponse]) {
            "$([int]$resp.StatusCode) $($resp.StatusCode)"
        } else { "No response" }
    } else { $ex.Message }
    
    Write-Host "  Error  : $status" -ForegroundColor Red
    exit 1
}
