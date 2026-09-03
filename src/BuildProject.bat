@echo off
setlocal

set "SRC_DIR=%~dp0"
set "BUILD_DIR=%SRC_DIR%FindAndReplace.App\bin\Release"
set "PACKAGE_DIR=%SRC_DIR%release"
set "MSBUILD=C:\Program Files\Microsoft Visual Studio\2022\Community\MSBuild\Current\Bin\MSBuild.exe"
set "ILMERGE=C:\Program Files (x86)\Microsoft\ILMerge\ilmerge.exe"

if not exist "%MSBUILD%" (
    echo ERROR: MSBuild was not found at "%MSBUILD%".
    exit /b 1
)

if not exist "%ILMERGE%" (
    echo ERROR: ILMerge was not found at "%ILMERGE%".
    exit /b 1
)

echo Building Release executable...
"%MSBUILD%" "%SRC_DIR%FindAndReplace.sln" /t:Build /p:Configuration=Release /p:Platform="Mixed Platforms" /v:minimal
if errorlevel 1 exit /b 1

if not exist "%PACKAGE_DIR%" mkdir "%PACKAGE_DIR%"

echo Creating one-file package...
"%ILMERGE%" /ndebug /targetplatform:4 /out:"%PACKAGE_DIR%\fnr.exe" "%BUILD_DIR%\fnr.exe" "%BUILD_DIR%\FindAndReplace.dll" "%BUILD_DIR%\CommandLine.dll" "%BUILD_DIR%\EncodingTools.dll"
if errorlevel 1 exit /b 1

echo.
echo Done: "%PACKAGE_DIR%\fnr.exe"
endlocal
