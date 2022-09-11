using static cheat_menu.Singleton;

namespace cheat_menu;

[CheatCategory(CheatCategoryEnum.CULT)]
public class CultDefinitions : IDefinition{
    [CheatDetails("Teleport to Cult", "Teleports the player to the Base")]
    public static void TeleportToBase(){
        Instance.cheatConsoleInstance.Method("ReturnToBase").GetValue();
    }

    [CheatDetails("Rename Cult", "Bring up the UI to rename the cult")]
    public static void RenameCult(){
        CultUtils.RenameCult();
    }

    [CheatDetails("Clear Base Rubble", "Removes any stones and large rubble")]
    public static void ClearBaseRubble(){
        Instance.cheatConsoleInstance.Method("ClearRubble").GetValue();
    }

    [CheatDetails("Clear Vomit", "Clear any vomit on the floor!")]
    public static void ClearVomit(){
        foreach(var vomit in StructureManager.GetAllStructuresOfType(FollowerLocation.Base, StructureBrain.TYPES.VOMIT)){
            vomit.Remove();
        }
        CultUtils.PlayNotification("Vomit cleared!");
    }

    [CheatDetails("Clear Poop", "Clear any poop on the floor, giving the fertilizer directly!")]
    public static async void ClearPoop(){
        foreach(var poop in StructureManager.GetAllStructuresOfType(FollowerLocation.Base, StructureBrain.TYPES.POOP)){
            poop.Remove();
            
        }
        await AsyncHelper.WaitSeconds(1);
        foreach(var pickup in PickUp.PickUps){
            if(pickup.type == InventoryItem.ITEM_TYPE.POOP){
                pickup.PickMeUp();
            }
        }
        CultUtils.PlayNotification("Poop cleared!");
    }

    [CheatDetails("Clear Dead bodies", "Clears any dead bodies on the floor, giving follower meat!")]
    public static void ClearDeadBodies(){
        foreach(DeadWorshipper deadWorshipper in DeadWorshipper.DeadWorshippers){
            CultUtils.AddInventoryItem(InventoryItem.ITEM_TYPE.FOLLOWER_MEAT, 5);
            CultUtils.AddInventoryItem(InventoryItem.ITEM_TYPE.BONE, 2);
		    if (deadWorshipper.followerInfo.Necklace != InventoryItem.ITEM_TYPE.NONE)
		    {
                CultUtils.AddInventoryItem(deadWorshipper.followerInfo.Necklace, 1);
		    }
		    deadWorshipper.followerInfo.Necklace = InventoryItem.ITEM_TYPE.NONE;
            StructureManager.RemoveStructure(deadWorshipper.Structure.Brain);
        }
        CultUtils.PlayNotification("Dead bodies cleared!");
    }

    //TODO: Possibly allow the patching to happen within the below function
    [CheatDetails("Max Faith Mode", "Keeps the faith of the cult maxed out")]
    [CheatFlag(CheatFlags.MaxFaithMode)]
    [CheatWIP]
    public static void MaxFaithMode(){}

    //TODO: Possibly allow the patching to happen within the below function
    [CheatDetails("Auto Clear Ritual Cooldowns", "Set ritual cooldowns to zero while active")]
    [CheatFlag(CheatFlags.NoRitualCooldown)]
    public static void ZeroRitualCooldown(){}

    [CheatDetails("Free Building Mode", "Buildings can be placed for free")]
    [CheatFlag(CheatFlags.FreeBuildingMode)]
    public static void FreeBuildingMode(bool flag){
        Instance.cheatConsoleInstance.Field("BuildingsFree").SetValue(flag);
    }

    [CheatDetails("Build All Structures", "Instantly build all structures")]
    public static void BuildAllStructures(){
        Instance.cheatConsoleInstance.Method("BuildAll").GetValue();
    }

    [CheatDetails("Unlock All Structures", "Unlocks all buildings")]
    public static void UnlockAllStructures(){
        Instance.cheatConsoleInstance.Method("UnlockAllStructures").GetValue();
    }

    //Currently you can't pick which out the 'pairs' to unlock so leave as WIP for now.
    [CheatDetails("Unlock All Rituals", "Unlocks all rituals")]
    [CheatWIP]
    public static void UnlockAllRituals(){
        //WIP
    }
}