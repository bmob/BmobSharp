@echo on

set config=%1
if "%config%" == "" (
   set config=Release
)

REM Build
%WINDIR%\Microsoft.NET\Framework\v4.0.30319\msbuild.exe core/BmobCore.Desktop.csproj /p:Configuration=%config% /m /v:M /fl /flp:LogFile=msbuild.Desktop.log;Verbosity=Normal /nr:true /p:BuildInParallel=true /p:RestorePackages=true /t:Clean,Rebuild
if not "%errorlevel%"=="0" goto failure

%WINDIR%\Microsoft.NET\Framework\v4.0.30319\msbuild.exe core/BmobCore.Unity.csproj /p:Configuration=%config% /m /v:M /fl /flp:LogFile=msbuild.Unity.log;Verbosity=Normal /nr:true /p:BuildInParallel=true /p:RestorePackages=true /t:Clean,Rebuild
if not "%errorlevel%"=="0" goto failure

%WINDIR%\Microsoft.NET\Framework\v4.0.30319\msbuild.exe core/BmobCore.Win8_1.csproj /p:Configuration=%config% /m /v:M /fl /flp:LogFile=msbuild.Win8_1.log;Verbosity=Normal /nr:true /p:BuildInParallel=true /p:RestorePackages=true /t:Clean,Rebuild
if not "%errorlevel%"=="0" goto failure

%WINDIR%\Microsoft.NET\Framework\v4.0.30319\msbuild.exe core/BmobCore.WP8.csproj /p:Configuration=%config% /m /v:M /fl /flp:LogFile=msbuild.WP8.log;Verbosity=Normal /nr:true /p:BuildInParallel=true /p:RestorePackages=true /t:Clean,Rebuild
if not "%errorlevel%"=="0" goto failure

REM delete the old stuff

rd target /s /q  
rd bmob-demo-csharp\examples\bmob-desktop-demo\lib\      /s /q 
rd bmob-demo-csharp\examples\bmob-unity-demo\Assets\libs /s /q 
rd bmob-demo-csharp\examples\bmob-windowsphone-demo\lib\ /s /q 

if not exist target\Unity mkdir target\Unity\
if not exist target\Windows mkdir target\Windows\
if not exist target\WindowsPhone8 mkdir target\WindowsPhone8\
if not exist target\Win8_1 mkdir target\Win8_1\

if not exist bmob-demo-csharp\examples\bmob-desktop-demo\lib\      mkdir bmob-demo-csharp\examples\bmob-desktop-demo\lib\
if not exist bmob-demo-csharp\examples\bmob-unity-demo\Assets\libs mkdir bmob-demo-csharp\examples\bmob-unity-demo\Assets\libs
if not exist bmob-demo-csharp\examples\bmob-windowsphone-demo\lib\ mkdir bmob-demo-csharp\examples\bmob-windowsphone-demo\lib\

rem unity

copy Core\bin\Release\Bmob-Unity.dll target\Unity\

copy Core\bin\Release\Bmob-Unity.XML target\Unity

rem desktop

copy Core\bin\Release\Bmob-Windows.dll target\Windows\

copy Core\bin\Release\Bmob-Windows.XML target\Windows\

rem wp8

copy Core\bin\Release\Bmob-WP8.dll target\WindowsPhone8\

copy Core\bin\Release\Bmob-WP8.XML target\WindowsPhone8\

rem win8_1

copy Core\bin\Release\Bmob-Win8_1.dll target\Win8_1\

copy Core\bin\Release\Bmob-Win8_1.XML target\Win8_1\

copy Core\bin\Release\Bmob-Unity.dll    bmob-demo-csharp\examples\bmob-unity-demo\Assets\libs\
copy Core\bin\Release\Bmob-WP8.dll      bmob-demo-csharp\examples\bmob-windowsphone-demo\lib\
copy Core\bin\Release\Bmob-Windows.dll  bmob-demo-csharp\examples\bmob-desktop-demo\lib\

:success

REM compile success

goto end

:failure

REM compile fail

goto end

:end

pause