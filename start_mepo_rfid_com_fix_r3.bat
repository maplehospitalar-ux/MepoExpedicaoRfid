@echo off
setlocal

set APP="C:\Program Files (x86)\MepoExpedicaoRfid\MepoExpedicaoRfid.exe"
set TOOL_DIR="C:\MepoExpedicaoRfid\base fabrica\UHFAPP\tools\r3-reset-tool"
set TOOL=%TOOL_DIR%\R3ResetTool.exe

echo [1/3] Iniciando MEPO Desktop RFID...
start "MepoExpedicaoRfid" %APP%

echo [2/3] Aguardando 3s e aplicando configuracao do R3 (USB)...
timeout /t 3 /nobreak >nul

if not exist %TOOL% (
  echo ERRO: Tool nao encontrado em %TOOL%
  pause
  exit /b 1
)

pushd %TOOL_DIR%
%TOOL% --ant1 --region usa --power 30 --rflink 0 --save 1
popd

echo [3/3] Fix aplicado. Se ainda nao ler, verifique cabo/antena/porta USB.
endlocal
