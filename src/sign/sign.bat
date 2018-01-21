set PATH_TO_SIGN_EXE="C:\Program Files (x86)\Windows Kits\8.0\bin\x64\signtool.exe"

set SOLUTION_DIR=F:\ENTechSolutions\FindAndREplace\

set PATH_TO_ASSEMBLY=src\FindAndReplace.App\bin\Release\

set PASSWORD="entech1234"

%PATH_TO_SIGN_EXE% sign /f "%SOLUTION_DIR%sign\Certificate.pfx" /p %PASSWORD% "%SOLUTION_DIR%%PATH_TO_ASSEMBLY%FindAndReplace.dll"

%PATH_TO_SIGN_EXE% sign /f "%SOLUTION_DIR%sign\Certificate.pfx" /p %PASSWORD% "%SOLUTION_DIR%%PATH_TO_ASSEMBLY%fnr.exe"

pause