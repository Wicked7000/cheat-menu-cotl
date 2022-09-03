xcopy /y .\bin\cheat_menu.dll "%CULT_OF_THE_LAMB_PATH%\BepInEx\plugins\cheat_menu.dll"
xcopy /y .\bin\cheat_menu.dll "%CULT_OF_THE_LAMB_PATH%\BepInEx\scripts\cheat_menu.dll"

.\tools\pdb2mdb.exe .\bin\cheat_menu.dll
xcopy /y .\bin\cheat_menu.dll.mdb "%CULT_OF_THE_LAMB_PATH%\BepInEx\plugins\cheat_menu.mdb"
xcopy /y .\bin\cheat_menu.dll.mdb "%CULT_OF_THE_LAMB_PATH%\BepInEx\scripts\cheat_menu.mdb"