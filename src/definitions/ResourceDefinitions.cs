using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using HarmonyLib;

namespace CheatMenu;

[CheatCategory(CheatCategoryEnum.RESOURCE)]
public class ResourceDefinitions : IDefinition{

    //TODO: Update this to not use cheatConsole
    [CheatDetails("Give Resources", "Gives 100 of the main primary resources")]
    public static void GiveResources(){
        Traverse.Create(typeof(CheatConsole)).Method("GiveResources").GetValue();
    }

    [CheatDetails("Give Commandment Stone", "Gives a Commandment Stone")]
    public static void GiveCommandmentStone(){
        UnityEngine.Debug.Log("hi");
        CultUtils.GiveDocterineStone();
    }

    [CheatDetails("Give Monster Heart", "Gives a heart of the heretic")]
    public static void GiveMonsterHeart(){
        CultUtils.AddInventoryItem(InventoryItem.ITEM_TYPE.MONSTER_HEART, 10);
    }

    [CheatDetails("Give Food", "Gives all farming based foods")]
    public static void GiveFarmingFood(){
        CultUtils.AddInventoryItem(InventoryItem.ITEM_TYPE.CAULIFLOWER, 10);
        CultUtils.AddInventoryItem(InventoryItem.ITEM_TYPE.BERRY, 10);
        CultUtils.AddInventoryItem(InventoryItem.ITEM_TYPE.BEETROOT, 10);
        CultUtils.AddInventoryItem(InventoryItem.ITEM_TYPE.PUMPKIN, 10);
        CultUtils.AddInventoryItem(InventoryItem.ITEM_TYPE.MUSHROOM_SMALL, 10);
    }

    [CheatDetails("Give Fish", "Gives all types of fish (x10)")]
    public static void GiveFish(){
        CultUtils.AddInventoryItem(InventoryItem.ITEM_TYPE.FISH_BIG, 10);
        CultUtils.AddInventoryItem(InventoryItem.ITEM_TYPE.FISH_CRAB, 10);
        CultUtils.AddInventoryItem(InventoryItem.ITEM_TYPE.FISH_BLOWFISH, 10);
        CultUtils.AddInventoryItem(InventoryItem.ITEM_TYPE.FISH_LOBSTER, 10);
        CultUtils.AddInventoryItem(InventoryItem.ITEM_TYPE.FISH_OCTOPUS, 10);
        CultUtils.AddInventoryItem(InventoryItem.ITEM_TYPE.FISH_SMALL, 10);
        CultUtils.AddInventoryItem(InventoryItem.ITEM_TYPE.FISH_SQUID, 10);
        CultUtils.AddInventoryItem(InventoryItem.ITEM_TYPE.FISH_SWORDFISH, 10);
    }

    [CheatDetails("Give Fertiziler", "Gives x100 Fertiziler (Poop)")]
    public static void GivePoop(){
        CultUtils.AddInventoryItem(InventoryItem.ITEM_TYPE.POOP, 100);
    }

    [CheatDetails("Give Follower Meat", "Gives x10 Follower Meat")]
    public static void GiveFollowerMeat(){
        CultUtils.AddInventoryItem(InventoryItem.ITEM_TYPE.FOLLOWER_MEAT, 10);
    }

    [CheatDetails("Give Follower Necklaces", "Gives one of each of the various follower necklaces")]
    public static void GiveGifts(){
        CultUtils.AddInventoryItem(InventoryItem.ITEM_TYPE.Necklace_1, 1);
        CultUtils.AddInventoryItem(InventoryItem.ITEM_TYPE.Necklace_2, 1);
        CultUtils.AddInventoryItem(InventoryItem.ITEM_TYPE.Necklace_3, 1);
        CultUtils.AddInventoryItem(InventoryItem.ITEM_TYPE.Necklace_4, 1);
        CultUtils.AddInventoryItem(InventoryItem.ITEM_TYPE.Necklace_5, 1);
    }

    [CheatDetails("Give Small Gift", "Gives you a 'small' gift x10")]
    public static void GiveSmallGift(){
        CultUtils.AddInventoryItem(InventoryItem.ITEM_TYPE.GIFT_SMALL, 10);
    }

    [CheatDetails("Give Big Gift", "Gives you a 'big' gift x10")]
    public static void GiveBigGift(){
        CultUtils.AddInventoryItem(InventoryItem.ITEM_TYPE.GIFT_MEDIUM, 10);
    }
}