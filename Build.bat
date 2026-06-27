@echo off
chcp 65001 > nul
echo Компиляция Classic Explorer...

set CSC="C:\Windows\Microsoft.NET\Framework\v4.0.30319\csc.exe"

:: Теперь компилируем ВСЕ файлы .cs в папке (*.cs)
%CSC% /nologo /target:winexe /out:InternetExplorer.exe /win32icon:icon.ico /reference:Microsoft.Web.WebView2.Core.dll,Microsoft.Web.WebView2.WinForms.dll *.cs

if %errorlevel% neq 0 (
    echo Ошибка при компиляции!
    pause
    exit /b %errorlevel%
)

echo Готово! Запускаю браузер...
start InternetExplorer.exe