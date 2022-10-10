# cheat-menu-cotl
Provides a list of cheats/utilities in an easily accessible GUI.

## Features & Usage
Press the ```M``` key to activate the cheat menu and click on any of the buttons to enable/disable the specific cheats or enter that specific cheat cateogry. ```N``` key can be used to go back from a category. Both these keys can be changed in the configuration in the ```config``` folder.

To see what cheats/utilities the mod offers see below:
- All Rituals
- Allow Shrine Creation
- Auto Clear Ritual Cooldowns
- Build All Structures
- Change Doctrines
- Change Rituals
- Clear All Doctrines
- Clear Base Rubble
- Clear Base Trees
- Clear Dead bodies
- Clear Outhouses
- Clear Poop
- Clear Vomit
- Free Building Mode
- Rename Cult
- Teleport to Cult
- Unlock All Structures
- Clear Faith
- Convert Dissenting Followers
- Kill All Followers
- Max Faith
- Remove Hunger
- Remove Sickness
- Revive All Followers
- Spawn 'Arrived' Follower
- Spawn Follower (Worker)
- Spawn Follower (Worshipper)
- Add x1 Black Heart
- Add x1 Blue Heart
- Die
- Godmode
- Heal x1
- Follower Debug
- FPS Debug
- Hide/Show UI
- Noclip
- Skip Hour
- Structure Debug
- Give Big Gift
- Give Commandment Stone
- Give Fertiziler
- Give Fish
- Give Follower Meat
- Give Follower Necklaces
- Give Food
- Give Monster Heart
- Give Resources
- Give Small Gift
- Weather: Clear
- Weather: Rain
- Weather: Windy


### Changelog 1.0.3  
#### Functionality Changes  
- Cheats are now ordered by name
- Cheats can now have sub-menus (Change All Rituals, Change All Docterines both use this)

#### Cheat Additions/Removal  
- \+ Allow Shrine Creation(Cult), Allows the Shrine to be created from the building menu
- \+ Change Doctrines(Cult), Allows the modification of selected doctrines
- \+ Change Rituals(Cult), Lets you change the selected Rituals along with unlocking not yet acquired ones
- \+ Clear All Doctrines(Cult), Clears all docterine categories and rewards (Apart from special rituals)
- \+ Clear Outhouses(Cult), Clears all outhouses of poop and adds the contents to your inventory.
- \+ Clear Base Trees(Cult), Removes all trees in the base
- \+ Clear Dead bodies(Cult), Clears any dead bodies on the floor, giving follower meat!
- \+ Clear Faith(Follower), Set the current faith to zero
- \+ Clear Poop(Cult), Clear any poop on the floor, giving the fertilizer directly!
- \+ Clear Vomit(Cult), Clear any vomit on the floor!
- \+ Complete All Quests(Misc), Complete All Quests
- \+ Convert Dissenting Followers(Follower), Converts dissenting followers back to regular followers
- \+ Give Big Gift(Resources), Gives you a 'big' gift x10
- \+ Give Commandment Stone(Resources), Gives a Commandment Stone
- \+ Give Follower Meat(Resources), Gives x10 Follower Meat
- \+ Give Follower Necklaces(Resources), Gives one of each of the various follower necklaces
- \+ Give Small Gift(Resources), Gives you a 'small' gift x10
- \+ Max Faith(Follower), Clear the cult's thoughts and gives them large positive ones
- \+ Remove Hunger(Follower), Clears starvation from any followers and maximazes satiation for all followers
- \+ Remove Sickness(Follower), Clears sickness from all followers, cleanups any vomit, poop or dead bodies and clears outhouses
- \+ Revive All Followers(Follower), Revive all currently dead followers
- \+ Turn all Followers Old(Follower), Changes the age of all followers to old
- \+ Turn all Followers Young(Follower), Changes the age of all followers to young
- \+ All Rituals(Cult), While enabled you will have access to all rituals (including both sides of every pair)
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
