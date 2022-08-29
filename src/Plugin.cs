using BepInEx;
using HarmonyLib;
using Lamb.UI.MainMenu;
using Lamb.UI.Rituals;
using System;
using UnityEngine;
using System.Diagnostics;
using UnityEngine.SceneManagement;
using UnityEditor;
using System.Management.Instrumentation;
using UnityEngine.UIElements;
using System.Collections.Generic;
using Map;
using src.Extensions;
using static cheat_menu.Singleton;
using Lamb.UI;

namespace cheat_menu;

[BepInPlugin("org.wicked.zero_cooldown_ritual", "Zero cooldown ritual", "1.0.0")]
public class Plugin : BaseUnityPlugin
{
    private GUIStyle buttonStyle = null;
    private Rect windowRect = new Rect(100, 500, 500, 500);
    public Vector2 scrollPosition = Vector2.zero;

    private GUIStyle GetGUIButtonStyle()
    {
        if(buttonStyle != null)
        {
            return buttonStyle;
        }

        //Default styling
        var styleObj = new GUIStyle();
        GUIStyleState normalStyle = new GUIStyleState();
        normalStyle.background = GetSolidTexture(new Color(0.3f, 0.3f, 0.3f, 1f));
        normalStyle.background.hideFlags = HideFlags.DontUnloadUnusedAsset;
        normalStyle.textColor = Color.white;
        styleObj.normal = normalStyle;
        styleObj.onNormal = normalStyle;

        //Hover styling
        Texture2D hoverColor = GetSolidTexture(new Color(0.2f, 0.2f, 0.2f, 1f));
        hoverColor.hideFlags = HideFlags.DontUnloadUnusedAsset;
        GUIStyleState hoverStyle = new GUIStyleState();
        hoverStyle.textColor = Color.white;
        hoverStyle.background = hoverColor;
        styleObj.hover = hoverStyle;
        styleObj.onHover = hoverStyle;

        //Clicked styling
        Texture2D activeColor = GetSolidTexture(new Color(0.1f, 0.1f, 0.1f, 1f));
        activeColor.hideFlags = HideFlags.DontUnloadUnusedAsset;
        GUIStyleState activeStyle = new GUIStyleState();
        activeStyle.textColor = Color.white;
        activeStyle.background = activeColor;
        styleObj.active = activeStyle;
        styleObj.onActive = activeStyle;

        styleObj.alignment = TextAnchor.MiddleCenter;

        buttonStyle = styleObj;
        return styleObj;
    }

    private static Texture2D GetSolidTexture(Color color)
    {
        Texture2D tex = new Texture2D(100, 100);
        Color[] pixels = tex.GetPixels();
        for (var i = 0; i < pixels.Length; i += 1)
        {
            pixels[i] = color;
        }
        tex.SetPixels(pixels);
        tex.Apply();
        return tex;
    }

    private void OnDisable()
    {
        //Kill all textures
        Destroy(GetGUIButtonStyle().normal.background);
        Destroy(GetGUIButtonStyle().hover.background);
        Destroy(GetGUIButtonStyle().active.background);

        //Unpatch all
        Patching.UnpatchAll();

        buttonStyle = null;
    }


    private void Awake()
    {
        Singleton.Instance.Init();
        Patching.PatchAll();
        UnityEngine.Debug.Log("CheatMenu patching and loading completed!");
    }

    private void OnGUI()
    {
        if (Instance.guiEnabled)
        {
            windowRect = GUI.Window(0, windowRect, CheatWindow, "Cheat Menu - InsaneWickedTV");
        }
    }

    void CheatWindow(int windowID)
    {
        var currentButtonY = 0;
        var totalWindowCalculatedHeight = 20;

        var Button = delegate (String buttonText, Action handler)
        {
            var buttonHeight = 50;
            var btn = GUI.Button(new Rect(0, currentButtonY, 480, buttonHeight), buttonText, GetGUIButtonStyle());
            totalWindowCalculatedHeight += buttonHeight;
            currentButtonY += buttonHeight;
            if (btn)
            {
                handler();
            };
            return btn;
        };

        //Cheats that are based on flags (aka modes)
        var ButtonWithFlag = delegate (String onText, String offText, CheatFlags flagID, Action<bool> handler)
        {
            bool flag = Instance.IsFlagEnabled(flagID);
            var btnText = flag ? onText : offText;
            var btn = Button(btnText, delegate { handler(flag); });
            if (btn)
            {
                Instance.FlipFlagValue(flagID);
            }
            return btn;
        };

        //Shorthand variation for mode type cheats
        var ButtonWithFlagS = delegate (String text, CheatFlags flagID, Action<bool> handler)
        {
            return ButtonWithFlag($"{text} (ON)", $"{text} (OFF)", flagID, handler);
        };

        //Shorthand for flags that have no implementation (patcher based)
        var ButtonWithFlagP = delegate (String text, CheatFlags flagID)
        {
            return ButtonWithFlagS(text, flagID, delegate { });
        };

        //Make titlebar of the window draggable
        GUI.DragWindow(new Rect(0, 0, 10000, 20));

        scrollPosition = GUI.BeginScrollView(new Rect(5, 20, 490, 493), scrollPosition, new Rect(0, 0, 490, Instance.totalWindowSize), false, true);

        ButtonWithFlagS("Godmode", CheatFlags.GodMode, delegate (bool flag)
        {
            Instance.cheatConsoleInstance.Method("ToggleGodMode").GetValue();
        });

        ButtonWithFlagS("Noclip", CheatFlags.NoClip, delegate (bool flag)
        {
            Instance.cheatConsoleInstance.Method("ToggleNoClip").GetValue();
        });

        Button("Complete All Current Quests", delegate
        {
            if(DataManager.Instance.Objectives.Count > 0)
            {
                UnityEngine.Debug.Log("Hello?");
                foreach(var quest in DataManager.Instance.Objectives)
                {
                    Objectives_Custom customObjective = quest as Objectives_Custom;
                    if (quest != null)
                    {
                        customObjective.ResultFollowerID = -1;
                        ObjectiveManager.UpdateObjective(customObjective);
                    }
                }
            }
        });

        Button("Rename Cult", delegate
        {
            CultUtils.RenameCult();
        });

        Button("Skip Tutorials", delegate
        {
            DataManager privateInstance = Traverse.Create(typeof(DataManager)).Field("instance").GetValue<DataManager>();

            DataManager.Instance.AllowSaving = true;
            DataManager.Instance.EnabledHealing = true;
            DataManager.Instance.BuildShrineEnabled = true;
            privateInstance.CookedFirstFood = true;
            privateInstance.XPEnabled = true;
            DataManager.Instance.Tutorial_Second_Enter_Base = false;
            DataManager.Instance.AllowBuilding = true;
            DataManager.Instance.ShowLoyaltyBars = true;
            DataManager.Instance.RatExplainDungeon = false;
            DataManager.Instance.ShowCultFaith = true;
            DataManager.Instance.ShowCultHunger = true;
            DataManager.Instance.ShowCultIllness = true;
            DataManager.Instance.UnlockBaseTeleporter = false;
            DataManager.Instance.BonesEnabled = false;
            privateInstance.PauseGameTime = true;
            privateInstance.ShownDodgeTutorial = true;
            privateInstance.ShownInventoryTutorial = true;
            privateInstance.HasEncounteredTarot = true;
            DataManager.Instance.CurrentGameTime = 244f;
            DataManager.Instance.HasBuiltShrine1 = false;
            DataManager.Instance.OnboardedHomeless = false;
            DataManager.Instance.ForceDoctrineStones = false;
            DataManager.Instance.Tutorial_First_Indoctoring = true;
            privateInstance.HadInitialDeathCatConversation = false;
            privateInstance.PlayerHasBeenGivenHearts = true;
            privateInstance.BaseGoopDoorLocked = false;
            privateInstance.InTutorial = false;

            //Skip the indoctrinate phase
            CultUtils.SpawnFollower(FollowerRole.Worker);
            Onboarding.Instance.Rat1Indoctrinate.SetActive(false);
            Onboarding.CurrentPhase = DataManager.OnboardingPhase.Devotion;

            //Name the Cult
            CultUtils.RenameCult(delegate { DataManager.Instance.OnboardedCultName = true; });

            //
            StructureManager.BuildStructure(Structures_Shrine.FindPlacementRegion)
        });

        Button("Clear Base Rubble", delegate
        {
            Instance.cheatConsoleInstance.Method("ClearRubble").GetValue();
        });

        Button("Clear Base Weeds", delegate
        {
            Instance.cheatConsoleInstance.Method("ClearWeed").GetValue();
        });

        Button("Set Raining Weather", delegate
        {
            WeatherController.Instance.SetRain();
        });

        Button("Set Windy Weather", delegate
        {
            WeatherController.Instance.SetWind();
        });

        Button("Set Clear Weather", delegate
        {
            WeatherController.isRaining = false;
            Traverse.Create(WeatherController.Instance).Field("isRaining").SetValue(false);
            Traverse.Create(WeatherController.Instance).Field("RainIntensity").SetValue(0f);
            Traverse.Create(WeatherController.Instance).Field("windSpeed").SetValue(0f);
            Traverse.Create(WeatherController.Instance).Field("windDensity").SetValue(0f);
            Traverse.Create(WeatherController.Instance).Field("IsActive").SetValue(false);
            Traverse.Create(WeatherController.Instance).Field("weatherChanged").SetValue(true);
            WeatherController.Instance.CheckWeather();
        });

        Button("Give Resources", delegate
        {
            Instance.cheatConsoleInstance.Method("GiveResources").GetValue();
        });

        Button("Give Monster Heart", delegate
        {
            Instance.cheatConsoleInstance.Method("MonsterHeart").GetValue();
        });

        Button("Give Food", delegate
        {
            Instance.cheatConsoleInstance.Method("GiveFood").GetValue();
        });

        Button("Give Fish", delegate
        {
            Instance.cheatConsoleInstance.Method("Fish").GetValue();
        });

        Button("Give Food", delegate
        {
            Instance.cheatConsoleInstance.Method("GiveFood").GetValue();
        });

        Button("Give Poop", delegate
        {
            Instance.cheatConsoleInstance.Method("GivePoop").GetValue();
        });

        ButtonWithFlag("Show UI", "Hide UI", CheatFlags.HidingUI, delegate (bool flag)
        {
            if (flag)
            {
                Instance.cheatConsoleInstance.Method("ShowUI").GetValue();
            }
            else
            {
                Instance.cheatConsoleInstance.Method("HideUI").GetValue();
            }
        });

        Button("Skip Hour", delegate
        {
            Instance.cheatConsoleInstance.Method("SkipHour").GetValue();
        });

        Button("Teleport to Base", delegate
        {
            Instance.cheatConsoleInstance.Method("ReturnToBase").GetValue();
        });

        Button("Die", delegate
        {
            GameObject gameObject = GameObject.FindWithTag("Player");
            if (gameObject != null)
            {
                Health healthComp = gameObject.GetComponent<Health>();
                healthComp.DealDamage(9999f, gameObject, gameObject.transform.position, false, Health.AttackTypes.Melee, false, (Health.AttackFlags)0);
            }
        });

        Button("Heal x1 (Red Heart)", delegate
        {
            GameObject gameObject = GameObject.FindWithTag("Player");
            if (gameObject != null)
            {
                gameObject.GetComponent<Health>().Heal(2f);
            }
        });


        Button("Add x1 (Blue Heart)", delegate
        {
            GameObject gameObject = GameObject.FindWithTag("Player");
            if (gameObject != null)
            {
                gameObject.GetComponent<Health>().BlueHearts += 2;
            }
        });

        Button("Add x1 (Black Heart)", delegate
        {
            GameObject gameObject = GameObject.FindWithTag("Player");
            if (gameObject != null)
            {
                gameObject.GetComponent<Health>().BlackHearts += 2;
            }
        });

        Button("Spawn Follower (Worker)", delegate
        {
            CultUtils.SpawnFollower(FollowerRole.Worker);
        });

        Button("Spawn Follower (Worshipper)", delegate
        {
            CultUtils.SpawnFollower(FollowerRole.Worshipper);
        });

        Button("Spawn Arrived Follower", delegate
        {
           FollowerManager.CreateNewRecruit(FollowerLocation.Base, NotificationCentre.NotificationType.None);
        });

        Button("Build All Structures", delegate
        {
            Instance.cheatConsoleInstance.Method("BuildAll").GetValue();
        });

        Button("Revive all followers", delegate
        {
            var followers = Utils.cloneList(DataManager.Instance.Followers_Dead);
            foreach (var follower in followers)
            {
                CultUtils.ReviveFollower(follower);
            }
        });


        Button("Turn All Followers Young", delegate
        {
            var followers = DataManager.Instance.Followers;
            foreach(var follower in followers)
            {
                ThoughtData thought = CultUtils.HasThought(follower, Thought.OldAge);
                if (thought != null)
                {
                    follower.Thoughts.Remove(thought);
                    DataManager.Instance.Followers_Elderly_IDs.Remove(follower.ID);
                }
            }
        });

        Button("Kill All Followers", delegate
        {
            var followers = DataManager.Instance.Followers;
            foreach (var follower in followers)
            {
                CultUtils.KillFollower(CultUtils.GetFollower(follower), false, false);
            }
        });

        Button("Clear Faith", delegate
        {
            CultUtils.ModifyFaith(0f, "Modifying faith to 0 (ClearFaith)");
        });

        ButtonWithFlagP("Max Faith Mode", CheatFlags.MaxFaithMode);

        ButtonWithFlagP("Auto-clear Ritual Cooldowns", CheatFlags.NoRitualCooldown);

        Button("Unlock All Structures", delegate
        {
            Instance.cheatConsoleInstance.Method("UnlockAllStructures").GetValue();
        });

        ButtonWithFlagS("Unlock All Rituals", CheatFlags.RitualsAllUnlock, delegate (bool flag)
        {
            Instance.cheatConsoleInstance.Method("FPS").GetValue();
        });

        ButtonWithFlagS("FPS Debug", CheatFlags.HidingUI, delegate (bool flag)
        {
            Instance.cheatConsoleInstance.Method("FPS").GetValue();
        });

        ButtonWithFlagS("Follower Debug", CheatFlags.FollowerDebug, delegate (bool flag)
        {
            Instance.cheatConsoleInstance.Method("FollowerDebug").GetValue();
        });

        ButtonWithFlagS("Structure Debug", CheatFlags.StructureDebug, delegate (bool flag)
        {
            Instance.cheatConsoleInstance.Method("StructureDebug").GetValue();
        });

        ButtonWithFlagS("Free Building Mode", CheatFlags.FreeBuildingMode, delegate (bool flag)
        {
            Instance.cheatConsoleInstance.SetValue(flag);
        });

        GUI.EndScrollView();

        if(Instance.totalWindowSize == 0)
        {
            Instance.totalWindowSize = totalWindowCalculatedHeight;
        }
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.M))
        {
            Instance.guiEnabled = !Instance.guiEnabled;
        }

        if(Instance.guiEnabled && Input.GetKeyDown(KeyCode.Escape))
        {
            Instance.guiEnabled = false;
        }
    }
}
