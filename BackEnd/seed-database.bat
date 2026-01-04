@echo off
cls

cd SamaniCrm.Infrastructure
dotnet ef database update -- --seed





@REM create migration
@REM dotnet ef migrations add CreateDushboardTable
