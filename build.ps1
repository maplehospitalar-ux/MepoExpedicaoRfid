Write-Host "=====================================" -ForegroundColor Cyan
Write-Host "Build Final - MepoExpedicaoRfid" -ForegroundColor Cyan
Write-Host "=====================================" -ForegroundColor Cyan

Set-Location "C:\MepoExpedicaoRfid"

Write-Host "`nLimpando build anterior..." -ForegroundColor Yellow
& dotnet clean --configuration Debug 2>&1 | Out-Null

Write-Host "Compilando projeto..." -ForegroundColor Yellow
$buildResult = & dotnet build --configuration Debug --no-restore 2>&1

# Mostrar últimas linhas do build
$buildResult[-10..-1] | ForEach-Object { Write-Host $_ }

# Verificar sucesso
if ($LASTEXITCODE -eq 0) {
    Write-Host "`n✅ BUILD SUCESSO!" -ForegroundColor Green
    
    $exe = "src/MepoExpedicaoRfid/bin/Debug/net8.0-windows/win-x86/MepoExpedicaoRfid.exe"
    if (Test-Path $exe) {
        $size = (Get-Item $exe).Length / 1024 / 1024
        Write-Host "Executável: $exe ($([math]::Round($size, 2)) MB)" -ForegroundColor Green
    }
    
    Write-Host "`n✅ PRONTO PARA EXECUTAR!" -ForegroundColor Green
} else {
    Write-Host "`n❌ BUILD FALHOU!" -ForegroundColor Red
    exit 1
}

Write-Host "`nPronto! Execute: .\MepoExpedicaoRfid.exe" -ForegroundColor Green
