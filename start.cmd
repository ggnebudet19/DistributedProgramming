@echo off

cd Chain
start cmd /k dotnet run 1234 localhost 1235 true
start cmd /k dotnet run 1235 localhost 1236
start cmd /k dotnet run 1236 localhost 1237
start cmd /k dotnet run 1237 localhost 1234