# Scripts

Start the project in order:

1. **Database** → `Set-ExecutionPolicy -Scope Process -ExecutionPolicy Bypass; .\Start-Database.ps1`
2. **Backend**  → `Set-ExecutionPolicy -Scope Process -ExecutionPolicy Bypass; .\Start-Backend.ps1`
3. **Frontend** → `Set-ExecutionPolicy -Scope Process -ExecutionPolicy Bypass; .\Start-Frontend.ps1`

Each script:
- Builds its target project (Backend: `Learning.Server`, Frontend: `Learning.Client`)
- Runs it with the correct port
- Blocks until you press **Ctrl+C** to stop

Ports:
- Database → `5432`
- Backend  → `5200` (Swagger: http://localhost:5200/swagger)
- Frontend → `5035` (Browse: http://localhost:5035)
