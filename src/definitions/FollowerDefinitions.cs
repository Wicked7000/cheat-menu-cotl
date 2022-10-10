namespace CheatMenu;

[CheatCategory(CheatCategoryEnum.FOLLOWER)]
public class FollowerDefinitions : IDefinition{
    [CheatDetails("Spawn Follower (Worker)", "Spawns and auto-indoctrinates a follower as a worker")]
    public static void SpawnWorkerFollower(){
        CultUtils.SpawnFollower(FollowerRole.Worker);
    }

    [CheatDetails("Spawn Follower (Worshipper)", "Spawns and auto-indoctrinates a follower as a worshipper")]
    public static void SpawnWorkerWorshipper(){
        CultUtils.SpawnFollower(FollowerRole.Worshipper);
    }

    [CheatDetails("Spawn 'Arrived' Follower", "Spawns a follower ready for indoctrination")]
    public static void SpawnArrivedFollower(){
        FollowerManager.CreateNewRecruit(FollowerLocation.Base, NotificationCentre.NotificationType.NewRecruit);
    }

    [CheatDetails("Turn all Followers Young", "Changes the age of all followers to young")]
    [CheatWIP]
    public static void TurnAllFollowersYoung(){
        var followers = DataManager.Instance.Followers;
        foreach(var follower in followers)
        {
            CultUtils.TurnFollowerYoung(follower);
        }
    }

    [CheatDetails("Turn all Followers Old", "Changes the age of all followers to old")]
    [CheatWIP]
    public static void TurnAllFollowersOld(){
        var followers = DataManager.Instance.Followers;
        foreach(var follower in followers)
        {
           CultUtils.TurnFollowerOld(follower);
        }
        CultUtils.PlayNotification("All followers are old now!");
    }

    [CheatDetails("Kill All Followers", "Kills all followers at the Base")]
    public static void KillAllFollowers(){
        var followers = DataManager.Instance.Followers;
        foreach (var follower in followers)
        {
            CultUtils.KillFollower(CultUtils.GetFollower(follower), false);
        }
    }

    [CheatDetails("Revive All Followers", "Revive all currently dead followers")]
    public static void ReviveAllFollowers(){
        var followers = CheatUtils.CloneList(DataManager.Instance.Followers_Dead);
        foreach (var follower in followers)
        {
            CultUtils.ReviveFollower(follower);
        }
    }

    [CheatDetails("Remove Sickness", "Clears sickness from all followers, cleanups any vomit, poop or dead bodies and clears outhouses")]
    public static void RemoveSickness(){
        CultUtils.ClearPoop();
        CultUtils.ClearBodies();
        CultUtils.ClearVomit();
        CultUtils.ClearOuthouses();
        foreach (var follower in DataManager.Instance.Followers)
        {
            CultUtils.CureIllness(follower);
        }
        CultUtils.PlayNotification("Cured all followers :)");
    }

    [CheatDetails("Convert Dissenting Followers", "Converts dissenting followers back to regular followers")]
    public static void ConvertAllDissenting(){
        foreach (var follower in DataManager.Instance.Followers)
        {
            CultUtils.ConvertDissenting(follower);
        }
        CultUtils.PlayNotification("Converted all followers :)");
    }

    [CheatDetails("Clear Faith", "Set the current faith to zero")]
    public static void ClearFaith(){
        CultUtils.ModifyFaith(0f, "Cleared faith :)");
    }

    [CheatDetails("Remove Hunger", "Clears starvation from any followers and maximazes satiation for all followers")]
    public static void RemoveHunger(){
        foreach (var follower in DataManager.Instance.Followers)
        {
            CultUtils.MaximizeSatiationAndRemoveStarvation(follower);
        }
        CultUtils.PlayNotification("Everyone is full! :)");
    }

    [CheatDetails("Max Faith", "Clear the cult's thoughts and gives them large positive ones")]
    public static void MaxFaith(){
        CultUtils.ClearAndAddPositiveFollowerThought();
    }
}