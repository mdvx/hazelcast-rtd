@echo off

echo Restoring NuGet packages
nuget restore .\hazelcast-rtd.sln

if errorlevel 1 pause

echo Building release
"%VS150COMCOMNTOOLS%\..\..\MSBuild\15.0\Bin\MSBuild.exe"  /p:Configuration=Release /v:minimal /nologo .\hazelcast-rtd.sln

if errorlevel 1 pause