@echo off
cd TestWPF\bin\Debug
xcopy "net8.0-windows10.0.26100.0\" "net8.0-windows10.0.26100.0_2\" /E /H /C /Y
xcopy "net8.0-windows10.0.26100.0\" "net8.0-windows10.0.26100.0_3\" /E /H /C /Y
cd net8.0-windows10.0.26100.0
start TestWPF.exe user1 password1
cd ..\net8.0-windows10.0.26100.0_2
start TestWPF.exe user2 password2
cd ..\net8.0-windows10.0.26100.0_3
start TestWPF.exe admin admin123