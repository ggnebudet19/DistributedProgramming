@echo off

cd "../Valuator"
start cmd /k dotnet run --urls "http://0.0.0.0:5001"

cd "../RankCalculator"
start cmd /k dotnet run

cd "../EventsLogger"
start cmd /k dotnet run

cd "../nats-server-v2.9.2-windows-amd64"
start nats-server.exe

cd "../../../nginx-1.25.4"
start nginx.exe