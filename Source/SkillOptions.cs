using System;
using System.Linq;
using System.Reflection;
using ExtraSkillSlots;
using MonoMod.RuntimeDetour;
using Rewired;
using Rewired.Data;
using Rewired.Data.Mapping;
using RoR2;
using RoR2.UI;
using UnityEngine;
using Object = UnityEngine.Object;

namespace SkillsPlusPlus
{
    internal class SkillOptions
    {
        public static RewiredAction hotkey { get; set; }
        internal static void SetupGameplayOptions()
        {
            hotkey = new RewiredAction
            {
                ActionId = 400,
                Name = "SKILLS_GAMEPAD_BUY_BTN",
                DisplayToken = "SKILLS_GAMEPAD_BUY_BTN",
                DefaultKeyboardKey = KeyboardKeyCode.None,
                DefaultJoystickKey = 16
            };
            InputCatalog.actionToToken[hotkey] = "SKILLS_GAMEPAD_BUY_BTN";
            var userDataInit = typeof(UserData).GetMethod(nameof(UserData.gLOOAxUFAvrvUufkVjaYyZoeLbLE), BindingFlags.NonPublic | BindingFlags.Instance);
            new Hook(userDataInit, (Action<Action<UserData>, UserData>) AddCustomActions);
            
            On.RoR2.UI.SettingsPanelController.Start += (orig, self) =>
            {
                orig(self);
                SettingsPanelControllerAwake(self);
            };
        }
        
        //taken from extra skill slots sorr y!!!!
        internal static void AddCustomActions(Action<UserData> orig, UserData self)
        {
            self.actions?.Add(hotkey);

            var joystickMap = self.joystickMaps?.FirstOrDefault();
            var keyboardMap = self.keyboardMaps?.FirstOrDefault();
            
            if (joystickMap != null && joystickMap.actionElementMaps.All(map => map.actionId != hotkey.ActionId))
            {
                joystickMap.actionElementMaps.Add(hotkey.DefaultJoystickMap);
            }

            if (keyboardMap != null && keyboardMap.actionElementMaps.All(map => map.actionId != hotkey.ActionId))
            {
                keyboardMap.actionElementMaps.Add(hotkey.DefaultKeyboardMap);
            }

            orig(self);
        }
        private static void SettingsPanelControllerAwake(SettingsPanelController settingsPanelController)
        {
            Logger.Debug(settingsPanelController.name);
            if (settingsPanelController.name == "SettingsSubPanel, Controls (M&KB)" || settingsPanelController.name == "SettingsSubPanel, Controls (Gamepad)")
            {
                var jumpBindingTransform = settingsPanelController.transform.Find("Scroll View/Viewport/VerticalLayout/SettingsEntryButton, Binding (Jump)");
                var inputBindingObject = Object.Instantiate(jumpBindingTransform, jumpBindingTransform.parent);
                var inputBindingControl = inputBindingObject.GetComponent<InputBindingControl>();
                
                inputBindingControl.actionName = "SKILLS_GAMEPAD_BUY_BTN";
                inputBindingControl.Awake();
                Logger.Debug("added option !!");
            }
        }
    }
}
