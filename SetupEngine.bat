@echo off
chcp 65001 > nul
echo Скачиваю движок Chromium (WebView2)...

:: Скачиваем архив
powershell -Command "[Net.ServicePointManager]::SecurityProtocol = [Net.SecurityProtocolType]::Tls12; Invoke-WebRequest -Uri 'https://www.nuget.org/api/v2/package/Microsoft.Web.WebView2' -OutFile 'webview2.zip'"

echo Распаковываю файлы...
powershell -Command "Expand-Archive -Path 'webview2.zip' -DestinationPath 'webview2_temp' -Force"

echo Достаю нужные библиотеки...
:: Умный поиск файлов по всем распакованным папкам
for /R "webview2_temp\lib" %%F in (Microsoft.Web.WebView2.Core.dll) do copy "%%F" . > nul
for /R "webview2_temp\lib" %%F in (Microsoft.Web.WebView2.WinForms.dll) do copy "%%F" . > nul
copy "webview2_temp\build\native\x64\WebView2Loader.dll" . > nul

echo Убираю мусор...
rmdir /S /Q "webview2_temp"
del "webview2.zip"

echo.
if exist "Microsoft.Web.WebView2.Core.dll" (
    echo УСПЕШНО: Все нужные файлы на месте! Можно запускать Build.bat
) else (
    echo ОШИБКА: Файлы не скопировались.
)
pause