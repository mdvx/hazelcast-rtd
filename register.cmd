@echo off

echo Registering RTD Server
%SystemRoot%\Microsoft.NET\Framework\v4.0.30319\RegAsm.exe %~dp0\rtd-client\bin\Release\HazelcastRTD.dll /codebase

if errorlevel 1 pause
