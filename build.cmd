@echo off

echo Restoring NuGet packages
nuget restore .\hazelcast-rtd.sln

if errorlevel 1 pause

SET BP="C:\Program Files\Microsoft Visual Studio\2022\Community\MSBuild\Current\Bin\"

echo Building release
"%BP%msbuild.exe" /p:Configuration=Release /v:minimal /nologo .\hazelcast-rtd.sln

if errorlevel 1 pause