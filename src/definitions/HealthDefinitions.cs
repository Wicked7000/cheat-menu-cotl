using UnityEngine;
using static cheat_menu.Singleton;

namespace cheat_menu;

[CheatCategory(CheatCategoryEnum.HEALTH)]
public class HealthDefinitions : IDefinition{

    [CheatDetails("Godmode", "Gives Invincibility")]
    [CheatFlag(CheatFlags.GodMode)]
    public static void GodMode(){
        Instance.cheatConsoleInstance.Method("ToggleGodMode").GetValue();
    }
    
    [CheatDetails("Heal x1", "Heals a Red Heart of the Player")]
    public static void HealRed(){
        GameObject gameObject = GameObject.FindWithTag("Player");
        if (gameObject != null)
        {
            gameObject.GetComponent<Health>().Heal(2f);
        }
    }

    [CheatDetails("Add x1 Blue Heart", "Adds a Blue Heart to the Player")]
    public static void AddBlueHeart(){
        GameObject gameObject = GameObject.FindWithTag("Player");
        if (gameObject != null)
        {
            gameObject.GetComponent<Health>().BlueHearts += 2;
        }
    }

    [CheatDetails("Add x1 Black Heart", "Adds a Black Heart to the Player")]
    public static void AddBlackHeart(){
        GameObject gameObject = GameObject.FindWithTag("Player");
        if (gameObject != null)
        {
            gameObject.GetComponent<Health>().BlackHearts += 2;
        }
    }

    [CheatDetails("Die", "Kills the Player")]
    public static void Die(){
        GameObject gameObject = GameObject.FindWithTag("Player");
        if (gameObject != null)
        {
            Health healthComp = gameObject.GetComponent<Health>();
            healthComp.DealDamage(9999f, gameObject, gameObject.transform.position, false, Health.AttackTypes.Melee, false, (Health.AttackFlags)0);
        }
    }
}