@echo off

cd Chat\Server
start cmd /k dotnet run 7000

cd ..\Client
start cmd /k dotnet run localhost 7000 "Hi!"