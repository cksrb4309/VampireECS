@echo off
setlocal

set ROOT=%~1
if "%ROOT%"=="" set ROOT=.

where py >nul 2>&1
if %errorlevel%==0 (
  set PY=py -3
) else (
  set PY=python
)

%PY% "%~dp0graphify-refresh.py" %ROOT%
exit /b %errorlevel%
