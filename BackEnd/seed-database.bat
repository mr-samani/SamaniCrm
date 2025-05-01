@echo off
cls

cd SamaniCrm.Infrastructure
dotnet ef database update -- --seed
