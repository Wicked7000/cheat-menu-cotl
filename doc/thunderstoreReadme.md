# cheat-menu-cotl
Provides a list of cheats/utilities in an easily accessible GUI.

## Features & Usage
Press the ```M``` key to activate the cheat menu and click on any of the buttons to enable/disable the specific cheats or enter that specific cheat cateogry. ```N``` key can be used to go back from a category. Both these keys can be changed in the configuration in the ```config``` folder.

To see what cheats/utilities the mod offers see below:
- Teleport to Cult
- Rename Cult
- Clear Base Rubble
- Clear Vomit
- Clear Poop
- Clear Dead bodies
- Auto Clear Ritual Cooldowns
- Free Building Mode
- Build All Structures
- Unlock All Structures
- Spawn Follower (Worker)
- Spawn Follower (Worshipper)
- Spawn 'Arrived' Follower
- Kill All Followers
- Revive All Followers
- Clear Faith
- Max Faith
- Godmode
- Heal x1
- Add x1 Blue Heart
- Add x1 Black Heart
- Die
- Noclip
- FPS Debug
- Follower Debug
- Structure Debug
- Hide/Show UI
- Skip Hour
- Give Resources
- Give Monster Heart
- Give Food
- Give Fish
- Give Fertiziler
- Give Follower Meat
- Give Follower Necklaces
- Give Small Gift
- Give Big Gift
- Weather: Rain
- Weather: Windy
- Weather: Clear


### Changelog 1.0.2  
#### Functionality Changes  
- Cheats are now displayed within categories for organisation purposes.
  
#### Cheat Additions/Removal  
- \+ Clear Vomit(Cult), Clear any vomit on the floor!
- \+ Clear Poop(Cult), Clear any poop on the floor, giving the fertilizer directly!
- \+ Clear Dead bodies(Cult), Clears any dead bodies on the floor, giving follower meat!
- \+ Max Faith Mode(Cult), Keeps the faith of the cult maxed out
- \+ Unlock All Rituals(Cult), Unlocks all rituals
- \+ Turn all Followers Young(Follower), Changes the age of all followers to young
- \+ Turn all Followers Old(Follower), Changes the age of all followers to old
- \+ Revive All Followers(Follower), Revive all currently dead followers
- \+ Clear Faith(Follower), Set the current faith to zero
- \+ Max Faith(Follower), Clear the cult's thoughts and gives them large positive ones
- \+ Complete All Objectives(Misc), Complete All Current quest objectives
- \+ Give Follower Meat(Resources), Gives x10 Follower Meat
- \+ Give Follower Necklaces(Resources), Gives one of each of the various follower necklaces
- \+ Give Small Gift(Resources), Gives you a 'small' gift x10
- \+ Give Big Gift(Resources), Gives you a 'big' gift x10
Latest changes: [1.0.2](doc/changelogs/1.0.2.md)

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

### Ethics
Cheats that unlock DLC content or content that is intended to be locked will not be added to this mod.
