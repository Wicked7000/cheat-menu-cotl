#r "..\bin\cheat_menu.dll"
using cheat_menu;
using System.Reflection;

//Used for creating a seperate MD file used in the readme
//to describe all the cheats currently supported by the mod!

List<string> cheatTitles = new List<string>();
List<string> cheatFnNames = new List<string>();
List<string> lines = new List<string>();

if(File.Exists("cheats.md")){
    File.Delete("cheats.md");
}

if(File.Exists("thunderstoreReadme.md")){
    File.Delete("thunderstoreReadme.md");
}

if(File.Exists("CheatNames.txt")){
    File.Delete("CheatNames.txt");
}

List<MethodInfo> cheatMethods = Definitions.getAllCheatMethods();

lines.Add("## Available Cheats  \n---  \n");

foreach(MethodInfo cheatMethod in cheatMethods){
    CheatDetails details = ReflectionHelper.HasAttribute<CheatDetails>(cheatMethod);
    CheatWIP cheatWip = ReflectionHelper.HasAttribute<CheatWIP>(cheatMethod);
    CheatFlag cheatFlag = ReflectionHelper.HasAttribute<CheatFlag>(cheatMethod);

    if(cheatWip == null){
        //Skip WIP cheats for now

        string type = cheatFlag == null ? "Simple" : "Mode";

        lines.Add($"### **{details.Title}**  ");
        lines.Add($"**Type**: {type}  ");
        lines.Add($"**Description**: {details.Description}  ");
        lines.Add("\n");
        lines.Add("---  ");
        lines.Add("\n");

        cheatTitles.Add($"- {details.Title}");
        cheatFnNames.Add(cheatMethod.Name);
    }
}

string readmeOriginal = File.ReadAllText("../README.md");
List<string> newReadmeFile = new List<string>(readmeOriginal.Split('\n'));
newReadmeFile[7] = "";
newReadmeFile[8] = "";
newReadmeFile.InsertRange(7, cheatTitles);

File.WriteAllLines("thunderstoreReadme.md", newReadmeFile);

lines.Add("\n[Back to Home](../README.md)");

cheatFnNames.Sort();

File.WriteAllLines("cheats.md", lines);
File.WriteAllLines("cheatNames.txt", cheatFnNames);
