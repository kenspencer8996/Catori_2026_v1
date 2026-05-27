@echo off
setlocal

set "SOURCE=%~dp0."
set "DEST=C:\Development\Gaming\Catori2026\Catori_2026_v1\DLLs"

if not exist "%DEST%" mkdir "%DEST%"

robocopy "%SOURCE%" "%DEST%" /E
if %ERRORLEVEL% LEQ 7 exit /B 0
exit /B %ERRORLEVEL%
