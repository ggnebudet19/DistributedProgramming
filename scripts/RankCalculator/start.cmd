@echo off
setlocal
cd /d ..\..\RankCalculator
for /l %%x in (1, 1, 3) do (
    start dotnet run
)
endlocal
