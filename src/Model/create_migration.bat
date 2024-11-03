@echo off

set /p name="Name for the migration? "
dotnet.exe ef migrations add %name%
pause