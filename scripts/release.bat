dotnet build -c RELEASE ../CheatMenu.csproj
dotnet-script ./ManifestUpdater.csx --no-cache
dotnet-script ./CheatNamesList.csx --no-cache
dotnet-script ./ChangelogCreator.csx --no-cache
echo "Please modify the changelog file to reflect functional additions".
pause
dotnet-script ./PatchPluginFile.csx --no-cache
dotnet-script ./ReadmeCreator.csx --no-cache
dotnet-script ./ZipCreator.csx --no-cache