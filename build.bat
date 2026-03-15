@echo off
setlocal

:: Windows標準の.NET Framework csc.exeを探す
set CSC_PATH=%WINDIR%\Microsoft.NET\Framework64\v4.0.30319\csc.exe

if not exist "%CSC_PATH%" (
    echo 64bit版のcsc.exeが見つかりません。32bit版を探します...
    set CSC_PATH=%WINDIR%\Microsoft.NET\Framework\v4.0.30319\csc.exe
)

if not exist "%CSC_PATH%" (
    echo エラー: .NET Framework の csc.exe が見つかりません。
    pause
    exit /b 1
)

echo コンパイラ: %CSC_PATH% を使用します。
echo コンパイル中...

:: ソースコードのパスを指定してコンパイル (Releaseモード: 最適化有効 / デバッグ情報なし)
:: リファレンスとして System.dll, System.Drawing.dll, System.Windows.Forms.dll を追加
"%CSC_PATH%" /optimize+ /debug- /target:winexe /out:haikei.exe /win32icon:haikei\Haikei.ico /r:System.dll,System.Drawing.dll,System.Windows.Forms.dll haikei\Program.cs haikei\HaikeiForm.cs haikei\HaikeiForm.Designer.cs

if %ERRORLEVEL% == 0 (
    echo.
    echo ------------------------------------------
    echo コンパイル成功！ haikei.exe が生成されました。
    echo ------------------------------------------
) else (
    echo.
    echo コンパイル失敗。
    pause
)
endlocal
