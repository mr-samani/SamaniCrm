@ECHO OFF
setlocal
set "currentDirector=%CD%"

:MainMenu
CLS
ECHO ---- Mohammadreza Samani Angular & C#.net9 ----
ECHO.

ECHO [1]. Start FrontEnd dashboard
ECHO [2]. Start public site
ECHO [3]. Run open api generation
ECHO [4]. Update database with seeds
ECHO [5]. Open dashboard in VScode
ECHO [6]. Open public site in VScode
ECHO [7]. Exit
ECHO.

CHOICE /C 1234567 /M "Enter your choice:"

ECHO.

:: Note - list ERRORLEVELS in decreasing order
IF ERRORLEVEL 7 GOTO ExitCMD
IF ERRORLEVEL 6 GOTO OpenPublicVsCode
IF ERRORLEVEL 5 GOTO OpenDashboardVsCode
IF ERRORLEVEL 4 GOTO UpdateDbWithSeed
IF ERRORLEVEL 3 GOTO RunOpenApiGen
IF ERRORLEVEL 2 GOTO StartPublicSite
IF ERRORLEVEL 1 GOTO StartDashboardFrontEnd

::---------------------------------------------------------
:ExitCMD
ECHO *** Exit... ***
GOTO PromptClose

::---------------------------------------------------------
:OpenPublicVsCode
ECHO *** Opening VS Code for frontend PUBLIC SITE***
CD /D "%currentDirector%\Frontend\SamaniPublicSite"
call code .
GOTO End
::---------------------------------------------------------
:OpenDashboardVsCode
ECHO *** Opening VS Code for frontend DASHBOARD***
CD /D "%currentDirector%\Frontend\SamaniCrm"
call code .
GOTO End


::---------------------------------------------------------
:UpdateDbWithSeed
ECHO *** Run migrations on database with seed data ***
CD /D "%currentDirector%\BackEnd"
call seed-database.bat
GOTO End

::---------------------------------------------------------
:RunOpenApiGen
ECHO *** Generate Services and model from swagger ***
CD /D "%currentDirector%\Frontend\SamaniCrm"
call npm run generate-api
GOTO End

::---------------------------------------------------------
:StartDashboardFrontEnd
ECHO *** Serve Frontend Dashboard***
CD /D "%currentDirector%\Frontend\SamaniCrm"
call npm run start
GOTO End

::---------------------------------------------------------
:StartPublicSite
ECHO *** Serve Frontend Public site ***
CD /D "%currentDirector%\Frontend\SamaniPublicSite"
call npm run start
GOTO End
::---------------------------------------------------------
:End
GOTO PromptClose

::---------------------------------------------------------
:PromptClose
SET /P AREYOUSURE=close (Y/[N])?
IF /I "%AREYOUSURE%" EQU "Y" GOTO ENDApp
GOTO MainMenu

::---------------------------------------------------------
:ENDApp
endlocal
exit
