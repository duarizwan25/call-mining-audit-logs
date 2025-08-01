# PowerShell script to run the audit trigger SQL script
# Run this script to create the audit trigger for DemoRequests table

param(
    [Parameter(Mandatory=$false)]
    [string]$ConnectionString = "Server=localhost;Database=CallMiningDB;Trusted_Connection=true;TrustServerCertificate=true;"
)

Write-Host "Creating audit trigger for DemoRequests table..." -ForegroundColor Green

try {
    # Get the script path
    $scriptPath = Join-Path $PSScriptRoot "03_CreateAuditTrigger.sql"
    
    if (-not (Test-Path $scriptPath)) {
        Write-Error "SQL script not found at: $scriptPath"
        exit 1
    }
    
    # Execute the SQL script using sqlcmd
    Write-Host "Executing SQL script: $scriptPath" -ForegroundColor Yellow
    
    $result = sqlcmd -S "localhost" -d "CallMiningDB" -E -i $scriptPath
    
    if ($LASTEXITCODE -eq 0) {
        Write-Host "Audit trigger created successfully!" -ForegroundColor Green
        Write-Host "Result: $result" -ForegroundColor Cyan
    } else {
        Write-Error "Failed to create audit trigger. Exit code: $LASTEXITCODE"
        Write-Host "Output: $result" -ForegroundColor Red
    }
}
catch {
    Write-Error "Error executing SQL script: $_"
}

Write-Host "`nAudit trigger setup completed." -ForegroundColor Magenta
