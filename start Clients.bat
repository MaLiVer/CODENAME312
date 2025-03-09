@echo off
cd TestWPF\bin\Debug
xcopy "net8.0-windows\" "net8.0-windows2\" /E /H /C /Y
xcopy "net8.0-windows\" "net8.0-windows3\" /E /H /C /Y
cd net8.0-windows
start TestWPF.exe user1 password1
cd ..\net8.0-windows2
start TestWPF.exe user2 password2
cd ..\net8.0-windows3
start TestWPF.exe admin admin123