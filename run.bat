@ECHO OFF
setlocal
set "currentDirector=%CD%"

:MainMenu
CLS
ECHO ---- Mohammadreza Samani Angular - C#.net10 ----
ECHO.

Echo [0]. Generate Front end Permissions by Backend
ECHO [1]. Start FrontEnd dashboard
ECHO [2]. Start public site
ECHO [3]. Run open api generation Dashboard
ECHO [4]. Run open api generation PublicSite
ECHO [5]. Update database with seeds
ECHO [6]. Open dashboard in VScode
ECHO [7]. Open public site in VScode
ECHO [8]. Start Host api
ECHO [100]. Exit
ECHO.




set /p choice=Enter your choice:
ECHO.


:: Note - list ERRORLEVELS in decreasing order
IF "%choice%" == "100" GOTO ExitCMD
IF "%choice%" == "8" GOTO StartHostApi
IF "%choice%" == "7" GOTO OpenPublicVsCode
IF "%choice%" == "6" GOTO OpenDashboardVsCode
IF "%choice%" == "5" GOTO UpdateDbWithSeed
IF "%choice%" == "4" GOTO RunOpenApiGenPublicSite
IF "%choice%" == "3" GOTO RunOpenApiGenDashboard
IF "%choice%" == "2" GOTO StartPublicSite
IF "%choice%" == "1" GOTO StartDashboardFrontEnd
IF "%choice%" == "0" GOTO GenrateFrontEndPermissionsByBackEnd
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
:RunOpenApiGenDashboard
ECHO *** Generate Services and model from swagger ***
CD /D "%currentDirector%\Frontend\SamaniCrm"
call npm run apiGen
GOTO End

::---------------------------------------------------------
:RunOpenApiGenPublicSite
ECHO *** Generate Services and model from swagger ***
CD /D "%currentDirector%\Frontend\SamaniPublicSite"
call npm run generate-api
GOTO End
::---------------------------------------------------------
:StartDashboardFrontEnd
ECHO *** Serve Frontend Dashboard***
CD /D "%currentDirector%\Frontend\SamaniCrm"
call npm run start
GOTO End
::---------------------------------------------------------
:GenrateFrontEndPermissionsByBackEnd
ECHO *** Genrate FrontEnd Permissions By BackEnd ***
CD /D "%currentDirector%\Frontend\SamaniCrm"
call npm run generate:app-permissions
GOTO End
::---------------------------------------------------------
:StartHostApi
ECHO *** Start Dotnet Host Api***
CD /D "%currentDirector%\BackEnd\SamaniCrm.Api"
call dotnet run --project ./SamaniCrm.Api.csproj --launch-profile https
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
