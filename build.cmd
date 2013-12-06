:: Build the package zip
C:\WINDOWS\Microsoft.NET\Framework\v4.0.30319\msbuild.exe /m /t:Package /p:"Configuration=Release";"MinUmbracoVersion=6.1.0";"PackageVersion=6.0.1-build";"BuildInParallel=true" package\package.proj

@IF %ERRORLEVEL% NEQ 0 GOTO err
@EXIT /B 0
:err
@PAUSE
@EXIT /B 1