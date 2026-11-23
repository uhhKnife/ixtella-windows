@echo off
rem Builds the game. Usage: build.bat [x64|x86|all]   (default: x64)
rem Set UNITY to your Unity.exe if it isn't at the default path below.
setlocal

if "%UNITY%"=="" set "UNITY=D:\Unity\Unity\6000.6.10f1\Editor\Unity.exe"
if not exist "%UNITY%" (
    echo Unity not found at "%UNITY%"
    echo Install Unity 6000.6.10f1 or run: set UNITY=C:\path\to\Unity.exe
    exit /b 1
)

set "ROOT=%~dp0"
set "TARGET=%~1"
if "%TARGET%"=="" set "TARGET=x64"
if not exist "%ROOT%logs" mkdir "%ROOT%logs"

if /i "%TARGET%"=="x64" (
    call :build Win64 x64 || exit /b 1
    goto :done
)
if /i "%TARGET%"=="x86" (
    call :build Win32 x86 || exit /b 1
    goto :done
)
if /i "%TARGET%"=="all" (
    call :build Win64 x64 || exit /b 1
    call :build Win32 x86 || exit /b 1
    goto :done
)
echo Unknown target "%TARGET%". Use x64, x86 or all.
exit /b 1

:build
set "LOG=%ROOT%logs\build-%2.log"
echo Building %2 (takes a few minutes, close the Unity editor first)...
"%UNITY%" -batchmode -quit -projectPath "%ROOT%project" -executeMethod Builder.Build%1 -logFile "%LOG%"
if errorlevel 1 (
    echo.
    echo %2 build FAILED. Errors:
    findstr /c:"error CS" /c:"[Builder] FAILED" /c:"another Unity instance" "%LOG%"
    echo Full log: %LOG%
    exit /b 1
)
echo %2 build OK: %ROOT%build\win-%2\IkustellaLink.exe
exit /b 0

:done
endlocal
