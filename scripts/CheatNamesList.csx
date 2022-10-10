#r "nuget: Wicked.UnityAnnotationHelpers, 1.0.0"
#r "..\bin\cheat_menu.dll"

using CheatMenu;

if(File.Exists("../doc/cheatNames.txt")){
    File.Delete("../doc/cheatNames.txt");
}


List<Definition> cheatMethods = DefinitionManager.GetAllCheatMethods();

List<string> cheatFnNames = new();

foreach(var cheatMethod in cheatMethods){
    cheatFnNames.Add(cheatMethod.MethodInfo.Name);
}

cheatFnNames.Sort();
File.WriteAllLines("../doc/cheatNames.txt", cheatFnNames);