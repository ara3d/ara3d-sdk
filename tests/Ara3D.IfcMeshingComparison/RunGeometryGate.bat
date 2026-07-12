@echo off
rem One-click IFC mesher geometry gate (correctness fast tier).
rem Usage: RunGeometryGate.bat
setlocal
set ROOT=%~dp0..\..\
call "%ROOT%test.bat" ifcmesher fast
exit /b %ERRORLEVEL%
