using System;
using BepInEx;
using BepInEx.Configuration;
using R2API.Utils;
using RiskOfOptions;
using RiskOfOptions.OptionConfigs;
using RiskOfOptions.Options;
using RoR2;
using RoR2.UI;

using SkillsPlusPlus.Modifiers;
using SkillsPlusPlus.UI;
using UnityEngine;
using UnityEngine.Networking;

using Rebindables;
using Rewired;
using Rewired.Data;
using System.Linq;
using System.Reflection;
using MonoMod.RuntimeDetour;
using Rewired.Data.Mapping;
using SkillsPlusPlus.Source;
using UnityEngine.AddressableAssets;

//using UnityHotReloadNS;

namespace SkillsPlusPlus
{
    [BepInDependency(R2API.R2API.PluginGUID)]
    [BepInDependency("com.KingEnderBrine.ExtendedLoadout", BepInDependency.DependencyFlags.SoftDependency)] //Soft-dependency to make Skills++ load after ExtendedLoadout
    [BepInPlugin("com.cwmlolzlz.skills", "Skills", "0.6.4")]
    [BepInDependency("pseudopulse.Rebindables")]
    public sealed class SkillsPlugin : BaseUnityPlugin
    {
        public static SkillsPlugin Instance;

        private void Awake()
        {
            //   _____  _     _  _  _
            //  / ____|| |   (_)| || |        _      _
            // | (___  | | __ _ | || | ___  _| |_  _| |_
            //  \___ \ | |/ /| || || |/ __||_   _||_   _|
            //  ____) ||   < | || || |\__ \  |_|    |_|
            // |_____/ |_|\_\|_||_||_||___/
            SkillsPlusPlus.Logger.Init(Logger);
            Instance = this;

            //this is required to ensure that you can actually upgrade skills .,.,
            //GameObject playerMasterPrefab = LegacyResourcesAPI.Load<GameObject>("prefabs/charactermasters/CommandoMaster");
            Addressables.LoadAssetAsync<GameObject>(RoR2BepInExPack.GameAssetPaths.Version_1_39_0.RoR2_Base_Core.PlayerMaster_prefab).Completed += handle =>
            {
                handle.Result.EnsureComponent<SkillPointsController>();
            };
            
            SkillModifierManager.LoadSkillModifiers();
            SkillOptions.InitConfig();

            R2API.RecalculateStatsAPI.GetStatCoefficients += LunarModifiers.RecalculateStats_GetLunarStats;

            HUD.onHudTargetChangedGlobal += HUD_onHudTargetChangedGlobal;

            On.RoR2.UI.TooltipController.SetTooltipProvider += TooltipController_SetTooltipProvider;
            On.RoR2.UI.LoadoutPanelController.Row.FromSkillSlot += Row_FromSkillSlot;
        }

        private static void TooltipController_SetTooltipProvider(On.RoR2.UI.TooltipController.orig_SetTooltipProvider orig, TooltipController self, TooltipProvider provider)
        {
            orig(self, provider);

            if (provider.TryGetComponent(out SkillUpgradeTooltipProvider tooltipProvider))
            {
                var tooltipController = self.EnsureComponent<SkillsPlusPlusTooltipController>();
                tooltipController.skillUpgradeToken = tooltipProvider.GetToken();
            }
        }

        private static LoadoutPanelController.Row Row_FromSkillSlot(On.RoR2.UI.LoadoutPanelController.Row.orig_FromSkillSlot orig, LoadoutPanelController owner, BodyIndex bodyIndex, int skillSlotIndex, GenericSkill skillSlot)
        {
            var row = orig(owner, bodyIndex, skillSlotIndex, skillSlot);

            if (row == null) return null;
                
            var buttons = row.rowData;

            for (int i = 0; i < buttons.Count; i++)
            {
                SkillsPlusPlus.Logger.Debug($"Ensuring SkillsPlusPlusTooltipProvider({i})");
                var button = buttons[i];
                var skillDef = skillSlot?.skillFamily?.variants[i].skillDef;
                if (skillDef != null)
                {
                    var provider = button.button.gameObject.EnsureComponent<SkillUpgradeTooltipProvider>();
                    provider.skillName = ((ScriptableObject)skillDef)?.name;
                    SkillsPlusPlus.Logger.Debug(((ScriptableObject)skillDef)?.name);
                }
            }

            return row;
        }

        [SystemInitializer(typeof(SurvivorCatalog))]
        private static void OnCatalogInitializer()
        {
            foreach (var survivorDef in SurvivorCatalog.allSurvivorDefs)
            {
                Instance.PrepareSurvivor(survivorDef);
            }
        }

        private void PrepareSurvivor(SurvivorDef survivorDef)
        {
            if (!survivorDef || !survivorDef.bodyPrefab)
                return;

            var upgrades = survivorDef.bodyPrefab.GetComponents<SkillUpgrade>();
            for (int i = 0; i < upgrades.Length; i++)
            {
                Destroy(upgrades[i]);
            }

            foreach (var genericSkill in survivorDef.bodyPrefab.GetComponents<GenericSkill>())
            {
                if (genericSkill == null)
                {
                    continue;
                }

                var skillUpgrade = survivorDef.bodyPrefab.AddComponent<SkillUpgrade>();
                skillUpgrade.targetGenericSkill = genericSkill;
            }
        }

        private static void HUD_onHudTargetChangedGlobal(HUD hud)
        {
            foreach (var skillIcon in hud.GetComponentsInChildren<SkillIcon>(true))
            {
                skillIcon.EnsureComponent<SkillUpgradeTooltipProvider>();
                skillIcon.EnsureComponent<SkillLevelIconController>();
            }
        }
        
        // void Update()
        // {
        //     if (Input.GetKeyUp(KeyCode.F4))
        //     {
        //         UnityHotReload.LoadNewAssemblyVersion(
        //             typeof(SkillsPlugin).Assembly, // The currently loaded assembly to replace.
        //             System.IO.Path.Combine(System.IO.Path.GetDirectoryName(Info.Location), "Skills.dll")  // The path to the newly compiled DLL.
        //         );
        //     }
        // }
    }
}