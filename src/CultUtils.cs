using Lamb.UI;
using System;
using src.Extensions;
using System.Collections.Generic;
using UnityEngine;
using System.Reflection;

namespace CheatMenu;

using DoctrinePairs =  Dictionary<SermonCategory, List<Tuple<DoctrineUpgradeSystem.DoctrineType, DoctrineUpgradeSystem.DoctrineType>>>;

internal class CultUtils {
    public static bool IsInGame(){
        return SaveAndLoad.Loaded;
    }

    public static void GiveDocterineStone(){
        // Used to make sure the user can declare doctrines before even doing a run
        DataManager.Instance.FirstDoctrineStone = true;
        DataManager.Instance.ForceDoctrineStones = true;
        DataManager.Instance.CompletedDoctrineStones += 1;

        //Needed to update the UI
        PlayerDoctrineStone.OnIncreaseCount?.Invoke();
        CultUtils.PlayNotification("Commandment stone given!");
    }

    public static void CompleteObjective(ObjectivesData objective){
        objective.Complete();
        
        Action onCompleteDelegate = delegate {
            FieldInfo objectiveEvent = typeof(ObjectiveManager).GetField("OnObjectiveCompleted", BindingFlags.Static | BindingFlags.NonPublic);
            if(objectiveEvent != null && objectiveEvent.GetValue(null) != null){
                var eventHandler = objectiveEvent.GetValue(null);
                if(eventHandler != null && eventHandler is ObjectiveManager.ObjectiveUpdated handler){
                    handler(objective);
                }
            }
            return;
        };

        MethodInfo invokeOrQueue = typeof(ObjectiveManager).GetMethod("InvokeOrQueue", BindingFlags.Static | BindingFlags.NonPublic);
        invokeOrQueue?.Invoke(null, new object[]{onCompleteDelegate});
        
        DataManager.Instance.Objectives.Remove(objective);
        DataManager.Instance.CompletedObjectives.Add(objective);
    }

    public static void CompleteAllQuests(){
        if(DataManager.Instance.Objectives.Count > 0)
        {
            foreach(var objective in DataManager.Instance.Objectives.ToArray())
            {
                CompleteObjective(objective);
            }
        }
    }

    public static void ClearBaseTrees(){
        foreach(var tree in StructureManager.GetAllStructuresOfType(FollowerLocation.Base, StructureBrain.TYPES.TREE)){
            if (tree is Structures_Tree treeStruct)
            {
                treeStruct.OnTreeComplete(true);
            }
        }
    }

    public static void ClearAllDocterines(){
        ClearUnlockedRituals();
        DataManager.Instance.CultTraits.Clear();
        DoctrineUpgradeSystem.UnlockedUpgrades.Clear();

        foreach(var category in GetAllSermonCategories()){
            DoctrineUpgradeSystem.SetLevelBySermon(category, 0);
        }
        UpgradeSystem.UnlockAbility(UpgradeSystem.PrimaryRitual1);

        DoctrineUpgradeSystem.UnlockAbility(DoctrineUpgradeSystem.DoctrineType.Special_Bonfire);
        DoctrineUpgradeSystem.UnlockAbility(DoctrineUpgradeSystem.DoctrineType.Special_ReadMind);
        DoctrineUpgradeSystem.UnlockAbility(DoctrineUpgradeSystem.DoctrineType.Special_Sacrifice);
        PlayNotification("Cleared all docterines!");
    }

    //Avoids removing special and player related upgrades
    public static void RemoveDocterineUpgrades(){
        List<DoctrineUpgradeSystem.DoctrineType> copiedUnlockedUpgrades = new();
        foreach(var unlock in DoctrineUpgradeSystem.UnlockedUpgrades){
            copiedUnlockedUpgrades.Add(unlock);
        }

        foreach(var unlock in DoctrineUpgradeSystem.UnlockedUpgrades){
            var unlockCategory = DoctrineUpgradeSystem.GetCategory(unlock);
            if(unlockCategory != SermonCategory.Special && unlockCategory != SermonCategory.PlayerUpgrade){
                copiedUnlockedUpgrades.Remove(unlock);
            }
        }
        DoctrineUpgradeSystem.UnlockedUpgrades = copiedUnlockedUpgrades;
    }

    // These are the 'public' sermon categories that are visible to the user
    public static List<SermonCategory> GetAllSermonCategories(){
        List<SermonCategory> sermonCategories = new();
        foreach(var value in Enum.GetValues(typeof(SermonCategory))){
            SermonCategory sermonCategory = (SermonCategory)value;
            string innerCategoryName = DoctrineUpgradeSystem.GetSermonCategoryLocalizedName(sermonCategory);
            if(innerCategoryName.StartsWith("DoctrineUpgradeSystem")){
                continue;
            } else {
                sermonCategories.Add(sermonCategory);
            }
        }
        return sermonCategories;
    }

    public static int[] GetDoctrineCategoryState(SermonCategory category, List<DoctrineUpgradeSystem.DoctrineType> upgrades = null){
        List<DoctrineUpgradeSystem.DoctrineType> innerUpgrades = upgrades;
        if(innerUpgrades == null){
            innerUpgrades = DoctrineUpgradeSystem.UnlockedUpgrades;
        }
    
        var doctrinePairs = GetAllDoctrinePairs();
        int[] pairStates = new int[4];

        for(int tupleIdx = 0; tupleIdx < pairStates.Length; tupleIdx++){
            var tupleSet = doctrinePairs[category][tupleIdx];                    
            if(innerUpgrades.Contains(tupleSet.Item1)){
                pairStates[tupleIdx] = 1;
            } else if(innerUpgrades.Contains(tupleSet.Item2)){
                pairStates[tupleIdx] = 2;
            } else {
                pairStates[tupleIdx] = 0;
            }
        }
        return pairStates;
    }

    public static void ReapplyAllDoctrinesWithChanges(SermonCategory overridenCategory, int[] stateMap){
        DataManager.Instance.CultTraits.Clear();       
        List<DoctrineUpgradeSystem.DoctrineType> copiedUnlockedUpgrades = new();
        foreach(var unlock in DoctrineUpgradeSystem.UnlockedUpgrades){
            copiedUnlockedUpgrades.Add(unlock);
        }
        RemoveDocterineUpgrades();

        var categories = GetAllSermonCategories();
        var doctrinePairs = GetAllDoctrinePairs();
        foreach(var category in categories){
            int[] innerStateMap = category == overridenCategory ? stateMap : GetDoctrineCategoryState(category, copiedUnlockedUpgrades);
            for(int pairIdx = 0; pairIdx < innerStateMap.Length; pairIdx++){
                int state = innerStateMap[pairIdx];
                if(state == 1){
                    DoctrineUpgradeSystem.UnlockAbility(doctrinePairs[category][pairIdx].Item1);
                } else if(state == 2){
                    DoctrineUpgradeSystem.UnlockAbility(doctrinePairs[category][pairIdx].Item2);
                }

                if(state != 0){
                    DoctrineUpgradeSystem.SetLevelBySermon(category, pairIdx+1);
                }
            }

        }
    }

    public static void ClearAllCultTraits(){
        foreach(var trait in DataManager.Instance.CultTraits){
            UnityEngine.Debug.Log($"{FollowerTrait.GetLocalizedTitle(trait)}");
        }

        DataManager.Instance.CultTraits.Clear();
    }

    public static Dictionary<UpgradeSystem.Type, UpgradeSystem.Type> GetDictionaryRitualPairs(){
        Dictionary<UpgradeSystem.Type, UpgradeSystem.Type> ritualPairs = new();
        for(int ritualIdx = 0; ritualIdx < UpgradeSystem.SecondaryRitualPairs.Length-1; ritualIdx += 2){
            var item1 = UpgradeSystem.SecondaryRitualPairs[ritualIdx];
            var item2 = UpgradeSystem.SecondaryRitualPairs[ritualIdx+1];

            ritualPairs[item1] = item2;
            ritualPairs[item2] = item1;
        }
        return ritualPairs;
    }

    public static void ClearUnlockedRituals(){
        foreach(var ritual in UpgradeSystem.SecondaryRitualPairs){
            UpgradeSystem.UnlockedUpgrades.Remove(ritual);
        }
        
        foreach(var ritual in UpgradeSystem.SecondaryRituals){
            UpgradeSystem.UnlockedUpgrades.Remove(ritual);
        }

        UpgradeSystem.UnlockedUpgrades.Remove(UpgradeSystem.PrimaryRitual1);
    }

    public static void PlayNotification(string message){
        if(NotificationCentre.Instance){
            NotificationCentre.Instance.PlayGenericNotification(message);
        }
    }

    public static void ClearVomit(){
        foreach(var vomit in StructureManager.GetAllStructuresOfType(FollowerLocation.Base, StructureBrain.TYPES.VOMIT)){
            vomit.Remove();
        }
        PlayNotification("Vomit cleared!");
    }

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
        PlayNotification("Poop cleared!");
    }

    public static void SetFollowerFaith(FollowerInfo followerInfo, float value){
        followerInfo.Faith = UnityEngine.Mathf.Clamp(value, 0, 100);
    }

    public static void SetFollowerSatiation(FollowerInfo followerInfo, float value){
        followerInfo.Satiation = UnityEngine.Mathf.Clamp(value, 0, 100);
    }
       
    public static void SetFollowerStarvation(FollowerInfo followerInfo, float value){
        FollowerBrainStats.StatStateChangedEvent onStarvationStateChanged = FollowerBrainStats.OnStarvationStateChanged;
        if(value > 0){
            followerInfo.Starvation = UnityEngine.Mathf.Clamp(value, 0, 75);
            followerInfo.IsStarving = true;
        } else {
            followerInfo.Starvation = 0f;
            followerInfo.IsStarving = false;
        }
        onStarvationStateChanged(followerInfo.ID, FollowerStatState.On, FollowerStatState.Off);
    }

    public static void ConvertDissenting(FollowerInfo followerInfo){
        Follower thisFollower = GetFollowerFromInfo(followerInfo);
        if(followerInfo.HasThought(Thought.Dissenter)){
            thisFollower.RemoveCursedState(Thought.Dissenter);
            SetFollowerFaith(followerInfo, 100);
        }
    }

    public static DoctrinePairs GetAllDoctrinePairs(){
        DoctrinePairs pairs = new();
        var enumValues = Enum.GetValues(typeof(SermonCategory));
        foreach(var enumVal in enumValues){
            SermonCategory category = (SermonCategory)enumVal;
            List<Tuple<DoctrineUpgradeSystem.DoctrineType, DoctrineUpgradeSystem.DoctrineType>> innerPair = new();
            if(DoctrineUpgradeSystem.GetSermonReward(category, 1, true) != DoctrineUpgradeSystem.DoctrineType.None){
                for(int level = 1; level <= 4; level++){
                    innerPair.Add(Tuple.Create(
                        DoctrineUpgradeSystem.GetSermonReward((SermonCategory)enumVal, level, true),
                        DoctrineUpgradeSystem.GetSermonReward((SermonCategory)enumVal, level, false)
                    ));
                }                
            }
            pairs[category] = innerPair;
        }
        return pairs;
    }

    public static void ClearOuthouses(){
        List<StructureBrain> outhouse1 = StructureManager.GetAllStructuresOfType(FollowerLocation.Base, StructureBrain.TYPES.OUTHOUSE);
        List<StructureBrain> outhouse2 = StructureManager.GetAllStructuresOfType(FollowerLocation.Base, StructureBrain.TYPES.OUTHOUSE_2);
        StructureBrain[] outhouses = CheatUtils.Concat(outhouse1.ToArray(), outhouse2.ToArray());
        foreach(var outhouse in outhouses){
            if (outhouse is Structures_Outhouse outhouseStructure)
            {
                AddInventoryItem(InventoryItem.ITEM_TYPE.POOP, outhouseStructure.GetPoopCount());
                outhouseStructure.Data.Inventory.Clear();
            }
        }
        PlayNotification("Outhouses cleared!");
    }

    public static void MaximizeSatiationAndRemoveStarvation(FollowerInfo followerInfo){
        Follower thisFollower = GetFollowerFromInfo(followerInfo);
        SetFollowerSatiation(followerInfo, 100);
        SetFollowerStarvation(followerInfo, 0);
        if(followerInfo.HasThought(Thought.BecomeStarving)){
            thisFollower.RemoveCursedState(Thought.BecomeStarving);
        }
    }

    public static void AddInventoryItem(InventoryItem.ITEM_TYPE type, int amount){
        Inventory.AddItem(type, amount, false);
    }

    public static float CalculateCurrentFaith()
    {
        float totalFaith = 0f;
        foreach (ThoughtData thoughtData in CultFaithManager.Thoughts)
        {
            int index = 0;
            float thoughtFaith = -1;
            while (index <= thoughtData.Quantity)
            {
                if(index == 0)
                {
                    thoughtFaith += thoughtData.Modifier;
                } else
                {
                    thoughtFaith += thoughtData.StackModifier;
                }
                index += 1;
            }
            totalFaith += thoughtFaith;
        }
        return totalFaith;
    }

    public static float GetCurrentFaith()
    {
        return CultFaithManager.CurrentFaith;
    }

    public static ThoughtData HasThought(FollowerInfo follower, Thought thoughtType)
    {
        foreach(var thought in follower.Thoughts)
        {
            if(thought.ThoughtType == thoughtType)
            {
                return thought;
            }
        }
        return null;
    }

    public static List<Tuple<UpgradeSystem.Type, UpgradeSystem.Type>> GetRitualPairs(){
        List<Tuple<UpgradeSystem.Type, UpgradeSystem.Type>> pairs = new();
        for(int i = 0; i < UpgradeSystem.SecondaryRitualPairs.Length; i+=2){
            pairs.Add(new(UpgradeSystem.SecondaryRitualPairs[i], UpgradeSystem.SecondaryRitualPairs[i+1]));
        }
        return pairs;
    }

    public static void RenameCult(Action<string> onNameConfirmed = null)
    {
        UICultNameMenuController cultNameMenuInstance = MonoSingleton<UIManager>.Instance.CultNameMenuTemplate.Instantiate<UICultNameMenuController>();
        cultNameMenuInstance.Show(true);

        cultNameMenuInstance.OnNameConfirmed = delegate (string newName)
        {
            DataManager.Instance.CultName = newName;
        };
        if (onNameConfirmed != null)
        {
            cultNameMenuInstance.OnNameConfirmed = (Action<string>)Action.Combine(cultNameMenuInstance.OnNameConfirmed, onNameConfirmed);
        }
        cultNameMenuInstance.OnHide = delegate () { };
        cultNameMenuInstance.OnHidden = delegate () { };
    }

    public static void TurnFollowerYoung(FollowerInfo follower){
        var thisFollower = CultUtils.GetFollowerFromInfo(follower);
        thisFollower.RemoveCursedState(Thought.OldAge);
        thisFollower.Brain.ClearThought(Thought.OldAge);
        follower.Age = 0;
        follower.OldAge = false;
        thisFollower.Brain.CheckChangeState();
        DataManager.Instance.Followers_Elderly_IDs.Remove(follower.ID);        
        if(thisFollower.Outfit.CurrentOutfit == FollowerOutfitType.Old){
            thisFollower.SetOutfit(FollowerOutfitType.Follower, false, Thought.None);
        }
    }

    public static void TurnFollowerOld(FollowerInfo follower){
        Follower thisFollower = CultUtils.GetFollowerFromInfo(follower);
        CultFaithManager.RemoveThought(Thought.OldAge);
        thisFollower.Brain.ApplyCurseState(Thought.OldAge);
    }

    public static FollowerInfo GetFollowerInfo(Follower follower)
    {
        return follower.Brain._directInfoAccess;
    }

    public static Follower GetFollowerFromInfo(FollowerInfo follower)
    {
        return FollowerManager.FindFollowerByID(follower.ID);
    }


    public static void SetFollowerIllness(FollowerInfo follower, float value)
    {
        follower.Illness = UnityEngine.Mathf.Clamp(value, 0f, 100f);
    }

    public static void ClearAllThoughts()
    {
        CultFaithManager.Thoughts.Clear();
        CultFaithManager.GetFaith(0f, 0f, true, NotificationBase.Flair.Positive, "Cleared follower thoughts!", -1);
    }

    public static void ClearAndAddPositiveFollowerThought()
    {
        CultFaithManager.Thoughts.Clear();
        foreach(var follower in DataManager.Instance.Followers){
            CultFaithManager.AddThought(Thought.TestPositive, follower.ID, 999);
        }
        ThoughtData data = FollowerThoughts.GetData(Thought.TestPositive);
        CultFaithManager.GetFaith(0f, data.Modifier, true, NotificationBase.Flair.Positive, "Cleared follower thoughts and added positive test thougtht!", -1);
    }

    public static void SetFollowerHunger(FollowerInfo follower, float value)
    {
        follower.Satiation = UnityEngine.Mathf.Clamp(value, 0f, 100f);
    }

    public static Follower GetFollower(FollowerInfo followerInfo)
    {
        return FollowerManager.FindFollowerByID(followerInfo.ID);
    }

    public static void KillFollower(Follower follower,
                                    bool withNotification = false)
    {
        NotificationCentre.NotificationType notifType = withNotification ? NotificationCentre.NotificationType.Died : NotificationCentre.NotificationType.None;
        follower.Die(notifType, false, 1, "dead", null, true);
    }

    //Similar to the revive that is performed by ritual but makes sure they aren't ill / hungry
    public static void ReviveFollower(FollowerInfo follower)
    {
        if (DataManager.Instance.Followers_Dead_IDs.Contains(follower.ID))
        {
            DataManager.Instance.Followers_Dead.Remove(follower);
            DataManager.Instance.Followers_Dead_IDs.Remove(follower.ID);

            Follower revivedFollower = FollowerManager.CreateNewFollower(follower, PlayerFarming.Instance.transform.position, false);
            if (follower.Age > follower.LifeExpectancy)
            {
                follower.LifeExpectancy = follower.Age + UnityEngine.Random.Range(20, 30);
            }
            else
            {
                follower.LifeExpectancy += UnityEngine.Random.Range(20, 30);
            }
            revivedFollower.Brain.ResetStats();
        }
    }

    public static void SpawnFollower(FollowerRole role)
    {
        Follower follower = FollowerManager.CreateNewFollower(PlayerFarming.Location, PlayerFarming.Instance.transform.position, false);
        follower.Brain.Info.FollowerRole = role;
        follower.Brain.Info.Outfit = FollowerOutfitType.Follower;
        follower.SetOutfit(FollowerOutfitType.Follower, false, Thought.None);

        if (role == FollowerRole.Worker)
        {
            follower.Brain.Info.WorkerPriority = WorkerPriority.Rubble;
            follower.Brain.Stats.WorkerBeenGivenOrders = true;
            follower.Brain.CheckChangeTask();
        }

        FollowerInfo newFollowerInfo = GetFollowerInfo(follower);
        SetFollowerIllness(newFollowerInfo, 0f);
        SetFollowerHunger(newFollowerInfo, 100f);
    }

    public static void ClearBodies(){
        foreach(DeadWorshipper deadWorshipper in DeadWorshipper.DeadWorshippers){
            AddInventoryItem(InventoryItem.ITEM_TYPE.FOLLOWER_MEAT, 5);
            AddInventoryItem(InventoryItem.ITEM_TYPE.BONE, 2);
		    if (deadWorshipper.followerInfo.Necklace != InventoryItem.ITEM_TYPE.NONE)
		    {
                AddInventoryItem(deadWorshipper.followerInfo.Necklace, 1);
		    }
		    deadWorshipper.followerInfo.Necklace = InventoryItem.ITEM_TYPE.NONE;
            StructureManager.RemoveStructure(deadWorshipper.Structure.Brain);
        }
        PlayNotification("Dead bodies cleared!");
    }

    public static void CureIllness(FollowerInfo follower){
        follower.Illness = 0f;
        GetFollowerFromInfo(follower).RemoveCursedState(Thought.Ill);
    }

    public static void ModifyFaith(float value, string notifMessage, bool shouldNotify = true)
    {
        NotificationBase.Flair flair = NotificationBase.Flair.Positive;
        float currentFaithValue = CultUtils.GetCurrentFaith();
        if (currentFaithValue > value)
        {
            flair = NotificationBase.Flair.Negative;
        }
        float currentFollowerFaith = CalculateCurrentFaith();
        float staticFaith = currentFaithValue < value ? value - currentFollowerFaith : currentFollowerFaith - value;


        NotificationData data = shouldNotify ? new NotificationData(notifMessage, 0f, -1, flair, new string[] { }) : null;

        CultFaithManager.StaticFaith = staticFaith;
        CultFaithManager.Instance.BarController.SetBarSize(value / 85f, true, true, data);
    }
}

