mkdir .\bin\CheatMenu 
.\tools\pdb2mdb.exe .\bin\cheat_menu.dll
echo f | xcopy /y .\bin\cheat_menu.dll.mdb .\bin\CheatMenu\cheat_menu.mdb
echo f | xcopy /y .\bin\cheat_menu.dll .\bin\CheatMenu\cheat_menu.dll
echo f | xcopy /y .\bin\UnityAnnotationHelpers.dll .\bin\CheatMenu\UnityAnnotationHelpers.dll

echo d | xcopy /y .\bin\CheatMenu "%CULT_OF_THE_LAMB_PATH%\BepInEx\plugins\CheatMenu"
echo f | xcopy /y .\bin\CheatMenu\cheat_menu.dll "%CULT_OF_THE_LAMB_PATH%\BepInEx\scripts\cheat_menu.dll"
