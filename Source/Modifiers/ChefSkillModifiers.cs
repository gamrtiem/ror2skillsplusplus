using System.Collections.Generic;
using System.Linq;
using UnityEngine;

using EntityStates;
using EntityStates.Chef;

using RoR2;
using RoR2.Projectile;
using RoR2.Skills;
using SkillsPlusPlus.Modifiers;
using static R2API.RecalculateStatsAPI;

namespace SkillsPlusPlus.Source.Modifiers
{
    [SkillLevelModifier("ChefDice", typeof(Dice))]
    class ChefDiceSkillModifier : BaseSkillModifier
    {
        private int locallevel;
        public override void OnSkillLeveledUp(int level, CharacterBody characterBody, SkillDef skillDef)
        {
            base.OnSkillLeveledUp(level, characterBody, skillDef);
        }

        public override void SetupSkill()
        {
            base.SetupSkill();
            
            On.RoR2.Projectile.CleaverProjectile.ChargeCleaver += CleaverProjectileOnChargeCleaver;
        }

        private void CleaverProjectileOnChargeCleaver(On.RoR2.Projectile.CleaverProjectile.orig_ChargeCleaver orig, CleaverProjectile self)
        {
            if (!self.charged)
            {
                //increase projectile size of returning dices by 30%
                self.projectileOverlapAttack.transform.localScale *= AdditiveScaling(1, .3f, locallevel);
            }
            orig(self);
        }

        public override void OnSkillEnter(BaseState skillState, int level)
        {
            base.OnSkillEnter(skillState, level);
            if (skillState is Dice dice)
            {
                Logger.Debug("Dice");
                
                //hasboost is used with yes chef !!
                if (!dice.hasBoost)
                {
                    Logger.Debug($"charge time be4 {dice.cleaverController.holdChargeTime}");
                    Logger.Debug($"charge damage coeff {dice.cleaverController.chargedDamageCoefficient}");
                    Logger.Debug($"prjectile damage  {dice.damageCoefficient}");
                    Logger.Debug($"travel distance {dice.cleaverController.maxTravelDistance}");
                    
                    //dice.cleaverController.holdChargeTime = MultScaling(0.33f, 0.80f, level); // base 0.33
                    dice.cleaverController.maxTravelDistance = AdditiveScaling(55f, 10f, level); //base 55
                    dice.damageCoefficient = AdditiveScaling(2f, .5f, level); // base 2 (200% dmg)
                    
                    Logger.Debug($"travel distance after {dice.cleaverController.maxTravelDistance}");
                    Logger.Debug($"damage after {dice.damageCoefficient}");

                    locallevel = level;
                }
            }
        }
    }
    
    

    [SkillLevelModifier("ChefSear", typeof(Sear))]
    class ChefSearSkillModifier : BaseSkillModifier
    {

        public override void OnSkillLeveledUp(int level, CharacterBody characterBody, SkillDef skillDef)
        {
            base.OnSkillLeveledUp(level, characterBody, skillDef);
        }

        public override void OnSkillEnter(BaseState skillState, int level)
        {
            base.OnSkillEnter(skillState, level);

            if (skillState is Sear)
            {
                Logger.Debug("Sear");
            }
        }
    }

    [SkillLevelModifier("ChefRolyPoly", typeof(RolyPoly), typeof(RolyPolyWeaponBlockingState),
        typeof(RolyPolyBoostedProjectileTimer))]
    class ChefRolyPolySkillModifier : BaseSkillModifier
    {

        public override void OnSkillLeveledUp(int level, CharacterBody characterBody, SkillDef skillDef)
        {
            base.OnSkillLeveledUp(level, characterBody, skillDef);
        }

        public override void OnSkillEnter(BaseState skillState, int level)
        {
            base.OnSkillEnter(skillState, level);

            if (skillState is RolyPoly)
            {
                Logger.Debug("RolyPoly");
            }
            else if (skillState is RolyPolyWeaponBlockingState)
            {
                Logger.Debug("RolyPolyWeaponBlockingState");
            }
        }
    }

    [SkillLevelModifier("ChefGlaze", typeof(Glaze))]
    class ChefGlazeSkillModifier : BaseSkillModifier
    {

        public override void OnSkillLeveledUp(int level, CharacterBody characterBody, SkillDef skillDef)
        {
            base.OnSkillLeveledUp(level, characterBody, skillDef);
        }

        public override void OnSkillEnter(BaseState skillState, int level)
        {
            base.OnSkillEnter(skillState, level);

            if (skillState is Glaze)
            {
                Logger.Debug("Glaze");
            }
        }
    }

    [SkillLevelModifier("YesChef", typeof(YesChef))]
    class YesChefSkillModifier : BaseSkillModifier
    {

        public override void OnSkillLeveledUp(int level, CharacterBody characterBody, SkillDef skillDef)
        {
            base.OnSkillLeveledUp(level, characterBody, skillDef);
        }

        public override void OnSkillEnter(BaseState skillState, int level)
        {
            base.OnSkillEnter(skillState, level);

            if (skillState is YesChef)
            {
                Logger.Debug("YesChef");
            }
        }
    }
    
    [SkillLevelModifier("ChefOilSpill", typeof(OilSpillBase), typeof(OilSpillV1), typeof(OilSpillV2))]
    class ChefOilSpillSkillModifier : BaseSkillModifier
    {

        public override void OnSkillLeveledUp(int level, CharacterBody characterBody, SkillDef skillDef)
        {
            base.OnSkillLeveledUp(level, characterBody, skillDef);
        }

        public override void OnSkillEnter(BaseState skillState, int level)
        {
            base.OnSkillEnter(skillState, level);

            if (skillState is OilSpillV1)
            {
                Logger.Debug("OilSpillV1");
            } 
            else if (skillState is OilSpillV2)
            {
                Logger.Debug("OilSpillV2");
            } else if (skillState is OilSpillBase)
            {
                Logger.Debug("OilSpillBase");
            } 
        }
    }
    
    [SkillLevelModifier("ChefIceBox", typeof(IceBox))]
    class ChefIceBoxSkillModifier : BaseSkillModifier
    {

        public override void OnSkillLeveledUp(int level, CharacterBody characterBody, SkillDef skillDef)
        {
            base.OnSkillLeveledUp(level, characterBody, skillDef);
        }

        public override void OnSkillEnter(BaseState skillState, int level)
        {
            base.OnSkillEnter(skillState, level);

            if (skillState is IceBox)
            {
                Logger.Debug("IceBox");
            } 
        }
    }
}
