using System.IO;
using System.IO.Compression;
using System.Reflection;

//Used for creating a seperate MD file used in the readme
//to describe all the cheats currently supported by the mod!

Directory.CreateDirectory("./temp");

File.Copy("../doc/thunderstoreReadme.md", "./temp/README.md", true);
File.Copy("../doc/icon.png", "./temp/icon.png", true);
File.Copy("../manifest.json", "./temp/manifest.json", true);
File.Copy("../bin/cheat_menu.dll", "./temp/cheat_menu.dll", true);

if(File.Exists("../release.zip")){
    File.Delete("../release.zip");
}

ZipFile.CreateFromDirectory("./temp", "../release.zip");

Directory.Delete("./temp", true);