@echo off
rem script to extract minor build version number from AssemblyInfo.cs
set _BuildVersion=
setlocal EnableDelayedExpansion
rem skip  look for comma's with 3rd field to get minor version number
for /f "delims=. tokens=3" %%G in (Properties\AssemblyInfo.cs) do set _buildNo=%%G
endlocal & set _BuildVersion=%_buildNo%
