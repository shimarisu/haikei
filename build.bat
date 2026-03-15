@echo off
setlocal

:: Windows標準の.NET Framework csc.exeを探す
set CSC_PATH=%WINDIR%\Microsoft.NET\Framework64\v4.0.30319\csc.exe

if not exist "%CSC_PATH%" (
    echo The 64-bit version of csc.exe could not be found. Searching for the 32-bit version...
    set CSC_PATH=%WINDIR%\Microsoft.NET\Framework\v4.0.30319\csc.exe
)

if not exist "%CSC_PATH%" (
    echo Error: The 32-bit version of csc.exe could not be found.
    pause
    exit /b 1
)

echo Compiler: Use %CSC_PATH% .
echo Compiling...

:: ソースコードのパスを指定してコンパイル (Releaseモード: 最適化有効 / デバッグ情報なし)
:: リファレンスとして System.dll, System.Drawing.dll, System.Windows.Forms.dll を追加
"%CSC_PATH%" /optimize+ /debug- /target:winexe /out:haikei.exe /win32icon:haikei\Haikei.ico /r:System.dll,System.Drawing.dll,System.Windows.Forms.dll haikei\Program.cs haikei\HaikeiForm.cs haikei\HaikeiForm.Designer.cs

if %ERRORLEVEL% == 0 (
    echo.
    echo ------------------------------------------
    echo Compiled: Generated haikei.exe successfully.
    echo ------------------------------------------
) else (
    echo.
    echo Compilation failed.
    pause
)
endlocal
