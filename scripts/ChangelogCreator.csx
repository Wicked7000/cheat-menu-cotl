#r "..\bin\cheat_menu.dll"
#r "nuget: Newtonsoft.Json, 13.0.1"

using cheat_menu;
using Newtonsoft.Json;
using System.Linq;
using System.Reflection;

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
    using(StreamReader r = new StreamReader("../manifest.json")){
        string content = r.ReadToEnd();
        data = JsonConvert.DeserializeObject<Manifest>(content);
    }
    return data;
}

Manifest manifest = LoadJsonManifest();
List<string> oldFileLines = File.ReadLines("../doc/old/cheatNames.txt").ToList();
List<string> newFileLines = File.ReadLines("../doc/cheatNames.txt").ToList();

List<string> changelogFile = new List<string>();
HashSet<int> oldFoundIndexes = new HashSet<int>();

int newIndex = 0;
int oldIndex = 0;

changelogFile.Add($"### Changelog {manifest.version_number}  ");
changelogFile.Add("#### Functionality Changes  ");
changelogFile.Add("  ");
changelogFile.Add("#### Cheat Additions/Removal  ");

List<Definition> cheatMethods = DefinitionManager.GetAllCheatMethods();
Dictionary<string, Definition> cheatFunctionToDetails = DefinitionManager.CheatFunctionToDetails(cheatMethods);

public void addNewCheat(string fnName){
    Definition cheatDetails;
    if(cheatFunctionToDetails.TryGetValue(fnName, out cheatDetails)){
        changelogFile.Add($"- \\+ {cheatDetails.Details.Title}({cheatDetails.CategoryName}), {cheatDetails.Details.Description}");
    } else {
        throw new Exception("Couldn't find cheat details, mismatch somewhere!");
    }
}

while(newIndex < newFileLines.Count || oldIndex < oldFileLines.Count){
    //Reached end of 'new' file
    if(newIndex == newFileLines.Count){
        //Everything 'left' in the old file was removed or renamed.
        if(!oldFoundIndexes.Contains(oldIndex)){
            changelogFile.Add($"- {oldFileLines[oldIndex]} removed/renamed!");
        }
        oldIndex += 1;
    } 
    //Reached the end of 'old' file
    else if(oldIndex == oldFileLines.Count){
        //Everything 'left' in the new file is additional things
        addNewCheat(newFileLines[newIndex]);
        newIndex += 1;
    } 
    else {
        string newItem = newFileLines[newIndex];
        string oldItem = oldFileLines[oldIndex];

        if(newItem == oldItem){
            //No change contine 
            newIndex += 1;
            oldIndex += 1;
            continue;
        } else {
            bool itemFound = false;
            for(var oldInnerIndex = oldIndex; oldInnerIndex < oldFileLines.Count; oldInnerIndex += 1){
                string oldInnerItem = oldFileLines[oldInnerIndex];
                if(newItem == oldInnerItem){
                    //No change! (Item has just moved)
                    oldFoundIndexes.Add(oldInnerIndex);
                    itemFound = true;
                }
            }

            if(!itemFound){
                addNewCheat(newItem);
            }
            newIndex += 1;
        }
    }
}

if(File.Exists($"../doc/changelogs/{manifest.version_number}.md")){
    File.Delete($"../doc/changelogs/{manifest.version_number}.md");
}

File.WriteAllLines($"../doc/changelogs/{manifest.version_number}.md", changelogFile);