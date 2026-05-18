#!/bin/bash

# ═══════════════════════════════════════════════════════════════
# Mohammadreza Samani - Angular & C#.NET10 Launcher
# Linux Version (Zorin OS)
# ═══════════════════════════════════════════════════════════════

currentDirectory=$(pwd)

# ───────────────────────────────────────────────────────────────
# Functions
# ───────────────────────────────────────────────────────────────
startDashboardFrontEnd() {
    echo "*** Serve Frontend Dashboard ***"
    cd "$currentDirectory/Frontend/SamaniCrm"
    npm run start
}

GenrateFrontEndPermissionsByBackEnd() {
    echo "*** Genrate FrontEnd Permissions By BackEnd ***"
    cd "$currentDirectory/Frontend/SamaniCrm"
    npm run generate:app-permissions
}

startPublicSite() {
    echo "*** Serve Frontend Public Site ***"
    cd "$currentDirectory/Frontend/SamaniPublicSite"
    npm run start
}

runOpenApiGenDashboard() {
    echo "*** Generate Services and Model from Swagger (Dashboard) ***"
    cd "$currentDirectory/Frontend/SamaniCrm"
    npm run apiGen
}

runOpenApiGenPublicSite() {
    echo "*** Generate Services and Model from Swagger (PublicSite) ***"
    cd "$currentDirectory/Frontend/SamaniPublicSite"
    npm run generate-api
}

updateDbWithSeed() {
    echo "*** Run Migrations on Database with Seed Data ***"
    cd "$currentDirectory/BackEnd"
    if [ -f "seed-database.sh" ]; then
        chmod +x seed-database.sh
        ./seed-database.sh
    else
        echo "Error: seed-database.sh not found!"
    fi
}

openDashboardVsCode() {
    echo "*** Opening VS Code for Frontend DASHBOARD ***"
    cd "$currentDirectory/Frontend/SamaniCrm"
    code .
}

openPublicVsCode() {
    echo "*** Opening VS Code for Frontend PUBLIC SITE ***"
    cd "$currentDirectory/Frontend/SamaniPublicSite"
    code .
}

startHostApi() {
    echo "*** Start Dotnet Host API ***"
    cd "$currentDirectory/BackEnd/SamaniCrm.Api"
    dotnet run --project ./SamaniCrm.Api.csproj --launch-profile https
}

# ───────────────────────────────────────────────────────────────
# Main Menu
# ───────────────────────────────────────────────────────────────

showMenu() {
    clear
    echo "╔══════════════════════════════════════════════════════════╗"
    echo "║     Mohammadreza Samani - Angular & C#.NET9            ║"
    echo "╚══════════════════════════════════════════════════════════╝"
    echo ""
    echo "  [0]  Generate Front end Permissions by Backend"
    echo "  [1]  Start FrontEnd Dashboard"
    echo "  [2]  Start Public Site"
    echo "  [3]  Run OpenAPI Generation (Dashboard)"
    echo "  [4]  Run OpenAPI Generation (PublicSite)"
    echo "  [5]  Update Database with Seeds"
    echo "  [6]  Open Dashboard in VSCode"
    echo "  [7]  Open Public Site in VSCode"
    echo "  [8]  Start Host API"
    echo "  [9]  Exit"
    echo ""
}

# ───────────────────────────────────────────────────────────────
# Main Loop
# ───────────────────────────────────────────────────────────────

while true; do
    showMenu
    read -p "  Enter your choice [1-9]: " choice
    echo ""

    case $choice in
        0) GenrateFrontEndPermissionsByBackEnd ;;
        1) startDashboardFrontEnd ;;
        2) startPublicSite ;;
        3) runOpenApiGenDashboard ;;
        4) runOpenApiGenPublicSite ;;
        5) updateDbWithSeed ;;
        6) openDashboardVsCode ;;
        7) openPublicVsCode ;;
        8) startHostApi ;;
        9) 
            echo "*** Goodbye! ***"
            exit 0
            ;;
        *)
            echo "Invalid choice! Please enter 1-9"
            ;;
    esac

    echo ""
    read -p "Press ENTER to return to menu..."
done
