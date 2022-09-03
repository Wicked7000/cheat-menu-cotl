# cheat-menu-cotl

Provides a list of cheats/utilities in an easily accessible GUI.

## Features & Usage
Press the ```M``` key to activate the cheat menu and click on any of the buttons to enable/disable the specific cheats.md. This key can be changed in the configuration in the ```config``` folder.

- Godmode
- Noclip
- Heal x1
- Add x1 Blue Heart
- Add x1 Black Heart
- Rename Cult
- Clear Base Rubble
- Weather: Rain
- Weather: Windy
- Weather: Clear
- Give Resources
- Give Monster Heart
- Give Food
- Give Fish
- Give Fertiziler
- Hide/Show UI
- Skip Hour
- Teleport to Base
- Die
- Spawn Follower (Worker)
- Spawn Follower (Worshipper)
- Spawn 'Arrived' Follower
- Build All Structures
- Kill All Followers
- Auto Clear Ritual Cooldowns
- Free Building Mode
- Unlock All Structures
- FPS Debug
- Follower Debug
- Structure Debug



## Installation
- Make sure you have BepInEx installed ([Guide](https://docs.bepinex.dev/articles/user_guide/installation/index.html))
- Download the DLL the latest DLL from the releases tab
- Copy the DLL file to BepInEx/Plugins folder
- **Important**: Some COTL mods require that the BepInEx configuration has a diferent entrypoint. Download this [file]() and place it in BepInEx/config
- Start the game and enjoy the mod!

### Dependencies
[BepInEx 5](https://github.com/BepInEx/BepInEx/releases/tag/v5.4.21)

### Developer Dependencies  
- [Gaze](https://github.com/wtetsu/gaze)  
    - Used for file watching to run the build when changes occur.
- [dotnet-script](https://github.com/filipw/dotnet-script)
    - Used to run the readme generator
- [pdb2mdb](https://gist.github.com/jbevain/ba23149da8369e4a966f)
    - Used to create visual studio debugging files

### Developer Setup
- Ensure dotnet is in your ```PATH```
- Install [Gaze](https://github.com/wtetsu/gaze) and put it in your ```PATH```
- Download [pdb2mdb](https://gist.github.com/jbevain/ba23149da8369e4a966f) and put it in the ```tools``` directory (even if you don't need the visual studio debugging symbols).
- Set environment variable ```CULT_OF_THE_LAMB_PATH``` to the root directory of the game
- Run either ```./build.bat``` or ```./watch.bat``` under scripts 

### License / Credits
Feel free to look around the code and modify for personal use, if you want to release a version of your code please reach out to me first!

If you just want to add a specific 'Cheat' to the mode feel free to open a pull request or open an issue.

