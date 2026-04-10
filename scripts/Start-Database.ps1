# Start-Database.ps1
# Starts PostgreSQL in Docker for the learning project

$ContainerName = "learning-db"
$Image = "tensorchord/pgvecto-rs:pg14-v0.2.0"
$Port = "5432:5432"
$DbName = "learning_dev"
$DbUser = "postgres"
$DbPass = "dev"

Write-Host "Checking for existing container..." -ForegroundColor Cyan
$existing = docker ps -a --filter "name=$ContainerName" --format "{{.Names}}"

if ($existing -eq $ContainerName) {
    $state = docker inspect -f "{{.State.Status}}" $ContainerName
    if ($state -eq "running") {
        Write-Host "Container is already running." -ForegroundColor Green
        exit 0
    } else {
        Write-Host "Starting existing container..." -ForegroundColor Yellow
        docker start $ContainerName
    }
} else {
    Write-Host "Creating new PostgreSQL container..." -ForegroundColor Yellow
    docker run -d --name $ContainerName `
        -e POSTGRES_DB=$DbName `
        -e POSTGRES_USER=$DbUser `
        -e POSTGRES_PASSWORD=$DbPass `
        -p $Port `$Image
}

# Wait for readiness
Write-Host "Waiting for database to be ready..." -ForegroundColor Yellow
$attempts = 0
$maxAttempts = 20

while ($attempts -lt $maxAttempts) {
    $healthy = docker exec $ContainerName pg_isready -U $DbUser 2>$null
    if ($LASTEXITCODE -eq 0) {
        Write-Host "Database is ready!" -ForegroundColor Green
        Write-Host "  Host:     localhost" -ForegroundColor White
        Write-Host "  Port:     5432" -ForegroundColor White
        Write-Host "  Database: $DbName" -ForegroundColor White
        Write-Host "  User:     $DbUser" -ForegroundColor White
        exit 0
    }
    Start-Sleep -Seconds 2
    $attempts++
}

Write-Host "Timed out waiting for database." -ForegroundColor Red
exit 1
