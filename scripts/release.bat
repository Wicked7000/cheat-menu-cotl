dotnet build -c RELEASE ../CheatMenu.csproj
cd "readme"
dotnet-script ./ReadmeCreator.csx
move ./cheats.md ../../doc/
move ./cheatNames.txt ../../doc/
cd ..