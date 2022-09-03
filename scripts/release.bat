dotnet build -c RELEASE ../CheatMenu.csproj
dotnet-script ./ReadmeCreator.csx
move ./cheats.md ../doc/
move ./thunderstoreReadme.md ../doc/
move ./cheatNames.txt ../doc/
dotnet-script ./ZipCreator.csx