#r "Z:\CultOfTheLambModding\mods\CheatMenu\bin\cheat_menu.dll"
using cheat_menu;
using System.Reflection;

//Used for creating a seperate MD file used in the readme
//to describe all the cheats currently supported by the mod!

List<string> cheatFnNames = new List<string>();
List<string> lines = new List<string>();

if(File.Exists("cheats.md")){
    File.Delete("cheats.md");
}

if(File.Exists("CheatNames.text")){
    File.Delete("CheatNames.txt");
}

List<MethodInfo> cheatMethods = Definitions.getAllCheatMethods();

lines.Add("## Available Cheats  \n---  \n");

foreach(MethodInfo cheatMethod in cheatMethods){
    CheatDetails details = Definitions.HasAttribute<CheatDetails>(cheatMethod);
    CheatWIP cheatWip = Definitions.HasAttribute<CheatWIP>(cheatMethod);
    CheatFlag cheatFlag = Definitions.HasAttribute<CheatFlag>(cheatMethod);

    if(cheatWip == null){
        //Skip WIP cheats for now

        string type = cheatFlag == null ? "Simple" : "Mode";

        lines.Add($"### **{details.Title}**  ");
        lines.Add($"**Type**: {type}  ");
        lines.Add($"**Description**: {details.Description}  ");
        lines.Add("\n");
        lines.Add("---  ");
        lines.Add("\n");

        cheatFnNames.Add(cheatMethod.Name);
    }
}

lines.Add("\n[Back to Home](../README.md)");

cheatFnNames.Sort();

File.WriteAllLines("cheats.md", lines);
File.WriteAllLines("cheatNames.txt", cheatFnNames);
