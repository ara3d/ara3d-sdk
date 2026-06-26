@echo off
rem Commit changes without pushing.
rem Usage: save.bat "commit message"
setlocal EnableExtensions

set ROOT=%~dp0
set MSG=%~1
if "%MSG%"=="" (
  echo Usage: save.bat "commit message"
  exit /b 1
)

pushd "%ROOT%" || exit /b 1
git status
git add .
git diff --cached --quiet
if errorlevel 1 (
  git commit -m "%MSG%"
) else (
  echo No staged changes to commit.
)
git status
popd
exit /b %ERRORLEVEL%
