REM Requires ILMerge from http://www.microsoft.com/download/en/confirmation.aspx?id=17630
REM makes fnr2.exe that includes fnr.exe and both assemblies. Can be renamed to fnr.exe

set RELEASE_DIR=C:\Users\Jonathan\Desktop\Z\GitHub\findandreplace\src\FindAndReplace.App\bin\Release

cd %RELEASE_DIR%
"C:\Program Files (x86)\Microsoft\ILMerge\ilmerge.exe" /log:log.txt /targetplatform:4 /out:fnr2.exe fnr.exe FindAndReplace.dll CommandLine.dll EncodingTools.dll