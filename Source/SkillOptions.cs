using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BepInEx.Configuration;
using Rebindables;
using RiskOfOptions;
using RiskOfOptions.OptionConfigs;
using RiskOfOptions.Options;
using RoR2;
using UnityEngine;

namespace SkillsPlusPlus.Source
{
    internal class SkillOptions
    {
        public static ModKeybind hotkey { get; set; }
        public static ConfigEntry<int> levelsPerSkillPoint;
        public static ConfigEntry<bool> disableInput;
        public static ConfigEntry<bool> multScalingLinear;
        public static ConfigEntry<bool> debugLogging;
        public static ConfigEntry<List<string>> disabledSurvivors;
        
        public static void InitConfig()
        {
            {
                hotkey = RebindAPI.RegisterModKeybind(new ModKeybind(
                    "SKILLS_GAMEPAD_BUY_BTN", 
                    KeyCode.None, 
                    16, 
                    "Jump" 
                ));
            }
            
            {
                levelsPerSkillPoint = SkillsPlugin.Instance.Config.Bind("Skills++",
                    "Levels per skill point",
                    5,
                    "The number of levels to reach to be rewarded with a skillpoint. Changes will not be applied during a run. In multiplayer runs the host's setting is used");
                ModSettingsManager.AddOption(new IntSliderOption(levelsPerSkillPoint, new IntSliderConfig
                {
                    max = 30,
                    min = 1,
                }));
            }

            {
                disableInput = SkillsPlugin.Instance.Config.Bind("Skills++",
                    "Disable Skills While Buying",
                    true,
                    "Should skills be disabled while the Buy Skills Input is pressed. (Disable this if you find yourself hitting the key by mistake)");
                ModSettingsManager.AddOption(new CheckBoxOption(disableInput));
            }

            {
                multScalingLinear = SkillsPlugin.Instance.Config.Bind("Skills++",
                    "Linear Skill Multipliers",
                    false,
                    "Should Multiplicative (+%) skill values use a linear value rather than an exponential one. (Useful for playing with low \"Levels per skill point\" values). In multiplayer runs the host's setting is used");
                ModSettingsManager.AddOption(new CheckBoxOption(multScalingLinear));
            }

            {
                disabledSurvivors = SkillsPlugin.Instance.Config.Bind("Skills++",
                    "Disabled Survivors",
                    new List<string>(),
                    "Survivors which shouldn't recieve skill upgrades. Can be modified in-game using the \"spp_disable_survivor\" and \"spp_enable_survivor\" commands respectively.");
                ModSettingsManager.AddOption(new CheckBoxOption(debugLogging));
            }
            
            {
                debugLogging = SkillsPlugin.Instance.Config.Bind("Skills++",
                    "Debug Logging",
                    false,
                    "Enables debug logging.");
                ModSettingsManager.AddOption(new CheckBoxOption(debugLogging));
            }
        }
        
        [ConCommand(commandName = "spp_disable_survivor", flags = ConVarFlags.None, helpText = "spp_disable_survivor <survivor name>\n  Disables Skills++ for the named survivor.")]
        public static void CCDisableSurvivor(ConCommandArgs args)
        {
            if (args.Count < 1)
            {
                Debug.Log("Could not parse a survivor name. Did you specify a survivor name?");
                return;
            }
            
            string survivorName = args[0];
            SurvivorDef[] survivorDefs = SurvivorCatalog.allSurvivorDefs.Where(surv => surv.cachedName == survivorName).ToArray();
            if (survivorDefs.Length == 0)
            {
                Debug.Log($"Could not find any survivor named {survivorName}. Are you sure you specified the internal survivor name?");
                return;
            }

            SurvivorDef survivorDef = survivorDefs[0];
            if (disabledSurvivors.Value.Contains(survivorDef.cachedName))
            {
                Debug.Log($"{survivorDef.cachedName} has already been disabled.");
                return;
            }
            
            disabledSurvivors.Value.Add(survivorDef.cachedName);
            Debug.Log($"Disabled survivors: '{disabledSurvivors.Value}'"); 
        }

        [ConCommand(commandName = "spp_enable_survivor", flags = ConVarFlags.None, helpText = "spp_enable_survivor <survivor name>\n  Re-enables Skills++ for the named survivor.")]
        public static void CCEnableSurvivor(ConCommandArgs args)
        {
            if (args.Count < 1)
            {
                Debug.Log("Could not parse a survivor name. Did you specify a survivor name?");
                return;
            }
            
            string survivorName = args[0];
            SurvivorDef[] survivorDefs = SurvivorCatalog.allSurvivorDefs.Where(surv => surv.cachedName == survivorName).ToArray();
            if (survivorDefs.Length == 0)
            {
                Debug.Log($"Could not find any survivor named {survivorName}. Are you sure you specified the internal survivor name?");
                return;
            }
            
            disabledSurvivors.Value.Remove(survivorDefs[0].cachedName);
            Debug.Log($"Disabled survivors: '{disabledSurvivors.Value}'");
        }
    }
}
