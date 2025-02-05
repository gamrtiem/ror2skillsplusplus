using RoR2;
using RoR2.UI;
using UnityEngine;

namespace SkillsPlusPlus.UI {

    class SkillUpgradeTooltipProvider : MonoBehaviour {

        public string skillName;
        public SkillIcon skillIcon;

        void Awake() {
            this.skillIcon = GetComponent<SkillIcon>();
        }

        internal static string SkillNameToToken(string skillName) {
            if (skillName == null) {
                return null;
            }
            if (skillName == "" || Language.IsTokenInvalid((skillName + "_UPGRADE_DESCRIPTION").ToUpper()))
            {
                skillName = "DEFAULT";
            }
            Logger.Debug("skill name = {0}", skillName);
            return (skillName + "_UPGRADE_DESCRIPTION").ToUpper();
        }

        internal string GetToken() {
            skillIcon = GetComponent<SkillIcon>();
            var skillIndex = (skillIcon.targetSkill?.characterBody?.skillLocator);
            Logger.Debug(((ScriptableObject)skillIcon.targetSkill?.skillDef)?.name);
            Logger.Debug(skillIndex.passiveSkill.skillNameToken);
            if (skillName != null && skillIndex != null && skillIndex.passiveSkill.skillNameToken != skillName) {
                return SkillNameToToken(skillName);
            }
            if (skillIcon)
            {
                Logger.Debug(((ScriptableObject)skillIcon.targetSkill?.skillDef)?.name);
                Logger.Debug(skillIndex.passiveSkill.skillNameToken);
                var skillName = ((ScriptableObject)skillIcon.targetSkill?.skillDef)?.name;
                if (skillName != null && skillIndex != null && skillIndex.passiveSkill.skillNameToken != skillName) {
                    return SkillNameToToken(skillName);
                }
            }
            return null;
        }

    }
}