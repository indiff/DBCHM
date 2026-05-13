@echo off
cd /d %~dp0
rem v1.0.3.4	2025-01-24 19:54:38 
rem v1.0.3.3	2025-01-01  1:49:02
rem v1.0.3.2	2024-12-19 23:36:14
rem v1.0.3.1	2024-12-18 16:59:41
rem v1.0.3.0	2024-12-17 23:31:29
rem v1.0.2.9	2024-12-16 10:48:50
rem v1.0.2.8
rem v1.0.2.7
rem v1.0.2.1 v1.0.2.2 v1.0.2.3 v1.0.2.4 v1.0.2.5 v1.0.2.6
set VZ=v1.8.0.5
set SUFFIX=.zip
del /f /q "%cd%\DBChm\bin\Release\DBCHM_%VZ%%SUFFIX%" 

"C:\Program Files\Bandizip\bz.exe" a -r -aoa -l:9 "%cd%\DBChm\bin\Release\DBCHM_%VZ%%SUFFIX%" "%cd%\DBChm\bin\Release\*.exe" ^
	"%cd%\DBChm\bin\Release\TplFile"  ^
	"%cd%\DBChm\bin\Release\*.dll"

pause	