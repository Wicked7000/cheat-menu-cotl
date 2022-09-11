#r "nuget: Newtonsoft.Json, 13.0.1"

using Newtonsoft.Json;

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

if(File.Exists("../src/Plugin.cs")){
    Manifest data = LoadJsonManifest();
    List<string> pluginLines = new List<string>(File.ReadAllLines("../src/Plugin.cs"));
    for(int i = 0; i < pluginLines.Count; i += 1){
        if(pluginLines[i].Contains("[BepInPlugin")){
            pluginLines[i] = $"[BepInPlugin(\"org.wicked.cheat_menu\", \"Cheat Menu\", \"{data.version_number}\")]";
        }
    }
    File.WriteAllLines("../src/Plugin.cs", pluginLines);
    Console.WriteLine("Plugin.cs updated for new version!");
} else {
    throw new Exception("Can't find plugin file? was it removed or renamed?");
}
