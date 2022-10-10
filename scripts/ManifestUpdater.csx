#r "nuget: Newtonsoft.Json, 13.0.1"
using Newtonsoft.Json;

public bool IsNumber(char value){
    return int.TryParse(value.ToString(), out int i);
}

public string ValidateAndCollectInput(){
    string newVersion = Console.ReadLine();
    int numberOfSections = 0;
    int sectionLength = 0;
    for(var i = 0; i < newVersion.Length; i++){
        if(IsNumber(newVersion[i])){
            sectionLength += 1;
            continue;
        } else if(newVersion[i] == '.') {
            if(sectionLength == 0){
                throw new Exception($"section {numberOfSections} has incorrect length of zero!");
            }
            numberOfSections += 1;
            if(numberOfSections > 2){
                throw new Exception($"Version only permits two sections please follow the format X.X.X");
            }
        }
    }
    return newVersion;
}

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

public void SaveManifest(Manifest data){
    string jsonContent = JsonConvert.SerializeObject(data);
    File.Delete("../manifest.json");
    File.WriteAllText("../manifest.json", jsonContent);
}

public void Main(){
    Manifest data = LoadJsonManifest();
    try {
        WriteLine("Please enter the new version number in X.X.X format:");
        string validVersionNumber = ValidateAndCollectInput();
        data.version_number = validVersionNumber;
        SaveManifest(data);
        WriteLine("Manifest updated!");
    } catch(Exception e){
        WriteLine($"ERROR: {e.Message}");
        Main();
    }
}

Main();



