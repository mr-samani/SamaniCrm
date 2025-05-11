@ECHO OFF

CLS
ECHO ---- Mohammadreza Samani Angular & C#.net9 ----
ECHO.
set currentDirector=%CD%


ECHO [1]. Start FrontEnd
ECHO [2]. Run open api generation
ECHO [3]. Update database with seeds
ECHO [4]. Open frontend in VScode
ECHO [5]. Exit
ECHO.

CHOICE /C 12345 /M "Enter your choice:"

ECHO.

:: Note - list ERRORLEVELS in decreasing order
IF ERRORLEVEL 5 GOTO ExitCMD
IF ERRORLEVEL 4 GOTO OpenVsCode
IF ERRORLEVEL 3 GOTO UpdateDbWithSeed
IF ERRORLEVEL 2 GOTO RunOpenApiGen
IF ERRORLEVEL 1 GOTO StartFrontEnd

::---------------------------------------------------------

:ExitCMD
ECHO *** Exit... ***
call exit
ECHO.
GOTO End

::---------------------------------------------------------

:OpenVsCode
ECHO *** Opening VS Code for frontend ***
CD /D  %currentDirector%\Frontend\SamaniCrm
call code .
ECHO.
GOTO End


::---------------------------------------------------------
:UpdateDbWithSeed
ECHO *** Run migrations on database with seed data ***
CD /D  %currentDirector%\BackEnd
call seed-database.bat
ECHO.
GOTO End

::---------------------------------------------------------
:RunOpenApiGen
ECHO *** Generate Services and model from swagger ***
CD /D  %currentDirector%\Frontend\SamaniCrm
call npm run generate-api
ECHO.
GOTO End


::---------------------------------------------------------
:StartFrontEnd
ECHO *** Serve Frontend ***
CD /D  %currentDirector%\Frontend\SamaniCrm
call npm run start
ECHO.
GOTO End




:End
setlocal
:PROMPT
SET /P AREYOUSURE=close (Y/[N])?
IF /I "%AREYOUSURE%" EQU "Y" GOTO ENDApp

echo ...  ...


:WaitApp
cmd /k


:ENDApp
endlocal
