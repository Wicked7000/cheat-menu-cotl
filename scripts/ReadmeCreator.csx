#r "nuget: Wicked.UnityAnnotationHelpers, 1.0.0"
#r "..\bin\cheat_menu.dll"
#r "nuget: Newtonsoft.Json, 13.0.1"

using Newtonsoft.Json;
using CheatMenu;
using System.Reflection;

//Used for creating a seperate MD file used in the readme
//to describe all the cheats currently supported by the mod!

#pragma warning disable 0649
public class Manifest {
    public string name;
    public string version_number;
    public string website_url;
    public string description;
    public string[] dependencies;
}
#pragma warning restore 0649

public Manifest LoadJsonManifest(){
    Manifest data;
    using(StreamReader r = new("../manifest.json")){
        string content = r.ReadToEnd();
        data = JsonConvert.DeserializeObject<Manifest>(content);
    }
    return data;
}

Manifest data = LoadJsonManifest();
List<string> cheatTitles = new();
List<string> lines = new();

if(File.Exists("../doc/cheats.md")){
    File.Delete("../doc/cheats.md");
}

if(File.Exists("../doc/thunderstoreReadme.md")){
    File.Delete("../doc/thunderstoreReadme.md");
}


List<Definition> cheatMethods = DefinitionManager.GetAllCheatMethods();
Dictionary<CheatCategoryEnum, List<Definition>> cheatGroups = DefinitionManager.GroupCheatsByCategory(cheatMethods);

lines.Add("## Available Cheats  \n---  \n");

foreach(var cheatGroup in cheatGroups){
    foreach(Definition cheatMethod in cheatGroup.Value){
        if(!cheatMethod.IsWIPCheat){
            //Skip WIP cheats for now

            string type = cheatMethod.IsModeCheat ? "Mode" : "Simple";

            lines.Add($"### **{cheatMethod.Details.Title}**  ");
            lines.Add($"**Category**: {cheatGroup.Key.GetCategoryName()}  ");
            lines.Add($"**Type**: {type}  ");
            lines.Add($"**Description**: {cheatMethod.Details.Description}  ");
            lines.Add("\n");
            lines.Add("---  ");
            lines.Add("\n");

            cheatTitles.Add($"- {cheatMethod.Details.Title}");

        }
    }
}

//Creates the thunderstore specific readme (md links don't work in thunderstore)
string readmeOriginal = File.ReadAllText("../README.md");
List<string> newReadmeFile = new(File.ReadAllLines("../README.md"));
List<string> changelog = new(File.ReadAllLines($"../doc/changelogs/{data.version_number}.md"));
newReadmeFile.InsertRange(9, changelog);

newReadmeFile[7] = "";
newReadmeFile[8] = "";
newReadmeFile.InsertRange(7, cheatTitles);

File.WriteAllLines("../doc/thunderstoreReadme.md", newReadmeFile);

//Adds the most recent changelog link to the main readme
List<string> updateMainReadmeFile = new(File.ReadAllLines("../README.md"));
updateMainReadmeFile[9] = $"Latest changes: [{data.version_number}](doc/changelogs/{data.version_number}.md)";

if(File.Exists("../README.md")){
    File.Delete("../README.md");
    File.WriteAllLines("../README.md", updateMainReadmeFile);
}

lines.Add("\n[Back to Home](../README.md)");
File.WriteAllLines("../doc/cheats.md", lines);
