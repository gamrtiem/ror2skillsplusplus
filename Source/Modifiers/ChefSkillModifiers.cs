using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using UnityEngine;

using EntityStates;
using EntityStates.Chef;
using MonoMod.Cil;
using R2API;
using RoR2;
using RoR2.Projectile;
using RoR2.Skills;
using SkillsPlusPlus.Modifiers;
using UnityEngine.AddressableAssets;
using static R2API.RecalculateStatsAPI;
using HealthComponent = On.RoR2.HealthComponent;
using OpCodes = Mono.Cecil.Cil.OpCodes;

namespace SkillsPlusPlus.Source.Modifiers
{
    [SkillLevelModifier(new[] { "ChefDice", "ChefDiceBoosted" }, typeof(Dice))]
    class ChefDiceSkillModifier : BaseSkillModifier
    {
        private int locallevel;
        public static int[] ChargedDiceArray = { 8, 4, 4 }; // this might cause issues in multiplayer due to being static,.., look into ,,.,,

        public override void OnSkillLeveledUp(int level, CharacterBody characterBody, SkillDef skillDef)
        {
            base.OnSkillLeveledUp(level, characterBody, skillDef);
        }

        public override void SetupSkill()
        {
            base.SetupSkill();

            On.RoR2.Projectile.CleaverProjectile.ChargeCleaver += CleaverProjectileOnChargeCleaver;
            IL.EntityStates.Chef.Dice.TrySpawnCleavers += DiceOnTrySpawnCleavers;
        }

        private void DiceOnTrySpawnCleavers(ILContext il)
        {
            ILCursor c = new ILCursor(il);
            if (c.TryGotoNext(
                    //x => x.MatchLdarg(0),
                    //x => x.MatchCallvirt<EntityStates.EntityState>("get_skillLocator"),
                    x => x.MatchLdfld<RoR2.SkillLocator>("primary"),
                    x => x.MatchLdcI4(out _)
                ))
            {
                c.Index -= 10; // boosted cleaver if statement eright after array creation
                Logger.Debug(c);
                
                c.Emit(OpCodes.Ldloc_2); //load array 
                c.Emit(OpCodes.Pop); //kill array
                c.Emit(OpCodes.Ldsfld, //add reference to 
                    typeof(ChefDiceSkillModifier)
                        .GetField(nameof(ChargedDiceArray)));
                c.Emit(OpCodes.Stloc_2);
            }
            else 
            {
                Logger.Error(il.Method.Name + " IL Hook failed!");
            }

            //Logger.Debug(il.ToString());
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
                Logger.Debug($"charge time be4 {dice.cleaverController.holdChargeTime}");
                Logger.Debug($"charge damage coeff {dice.cleaverController.chargedDamageCoefficient}");
                Logger.Debug($"prjectile damage  {dice.damageCoefficient}");
                Logger.Debug($"boost prjectile damage  {dice.boostedDamageCoefficient}"); // this is in cleaverprojectile.chargeddamagecoeff i think ?
                Logger.Debug($"travel distance {dice.cleaverController.maxTravelDistance}");

                level += dice.characterBody.GetBuffCount(YesChefSkillModifier.levelupBuff);
                locallevel = level;
                
                dice.cleaverController.maxTravelDistance = AdditiveScaling(55f, 10f, level); //base 55
                dice.damageCoefficient = AdditiveScaling(2f, .5f, level); // base 2 (200% dmg)
                dice.boostedDamageCoefficient = AdditiveScaling(4f, .5f, level); // base 4 (400% dmg)

                Logger.Debug($"travel distance after {dice.cleaverController.maxTravelDistance}");
                Logger.Debug($"damage after {dice.damageCoefficient}");
                
                ChargedDiceArray[0] = 8 + level;
                ChargedDiceArray[1] = 4 + level;
                ChargedDiceArray[2] = 4 + level;
            }
        }

        public override void OnSkillExit(BaseState skillState, int level)
        {
            base.OnSkillExit(skillState, level);
            
            skillState.characterBody.SetBuffCount(YesChefSkillModifier.levelupBuff.buffIndex, 0);
        }
    }



    [SkillLevelModifier( new[] {"ChefSear", "ChefSearBoosted"}, typeof(Sear))]
    class ChefSearSkillModifier : BaseSkillModifier
    {
        //heal on ignites, attack faster, damage
        //boosted add extra oil globs and 
        private BuffDef ChefAttackSpeedBuff;

        private DamageTypeCombo SearDamageTypeCombo = new DamageTypeCombo(DamageType.IgniteOnHit, DamageTypeExtended.ChefSource, DamageSource.Secondary);
        
        private DamageTypeCombo BoostedSearDamageTypeCombo = new DamageTypeCombo(DamageType.IgniteOnHit, DamageTypeExtended.ChefSource | DamageTypeExtended.IgniteChefOilBoosted, DamageSource.Secondary);
        
        public override void OnSkillLeveledUp(int level, CharacterBody characterBody, SkillDef skillDef)
        {
            base.OnSkillLeveledUp(level, characterBody, skillDef);
        }

        public override void OnSkillEnter(BaseState skillState, int level)
        {
            base.OnSkillEnter(skillState, level); 

            if (skillState is Sear sear)
            {
                Logger.Debug($"Sear {sear.tickDamageCoefficient}");
                Logger.Debug($"Sear {sear.damageStat}");
                Logger.Debug($"Sear {sear.boostedDamage}");
                //Logger.Debug($"Sear {sear.dams}");
                Logger.Debug($"Sear {sear.tickDamageCoefficient}");
                Logger.Debug($"hasboost {sear.hasBoost}");
                
                level += sear.characterBody.GetBuffCount(YesChefSkillModifier.levelupBuff);
                
                if (sear.characterBody.GetBuffCount(YesChefSkillModifier.levelupBuff) > 0)
                {
                    sear.characterBody.SetBuffCount(ChefAttackSpeedBuff.buffIndex, level);
                    sear.characterBody.RecalculateStats();
                    
                    sear.boostedProjectilesFired = level * -1;
                }
                else
                {
                    sear.characterBody.SetBuffCount(ChefAttackSpeedBuff.buffIndex, level);
                    sear.characterBody.RecalculateStats();
                    //Sear.totalDamageCoefficient = AdditiveScaling(1.2f, 0.6f, level); // base 1.2 
                }
            }
        }

        public override void SetupSkill()
        {
            base.SetupSkill();

            RegisterChefAttackSpeedBuff();
            GetStatCoefficients += RecalculateStatsAPIOnGetStatCoefficients;
            HealthComponent.TakeDamage += HealthComponentOnTakeDamage;
        }

        private void HealthComponentOnTakeDamage(HealthComponent.orig_TakeDamage orig, RoR2.HealthComponent self, DamageInfo damageInfo)
        {
            orig(self, damageInfo);

            var damagetypecheck = (damageInfo.damageType == SearDamageTypeCombo || damageInfo.damageType == BoostedSearDamageTypeCombo);
            if (!damagetypecheck) return;
            
            var surv = damageInfo.attacker?.GetComponent<CharacterBody>();
            
            if (surv != null && surv.GetBuffCount(ChefAttackSpeedBuff) > 0 && damagetypecheck)
            {
                Logger.Debug(damageInfo.damageType);
                
                surv.healthComponent.Heal(surv.healthComponent.fullHealth * 0.005f * surv.GetBuffCount(ChefAttackSpeedBuff), default);
            }
            
        }

        public void RegisterChefAttackSpeedBuff()
        {
            BuffDef buffDef = ScriptableObject.CreateInstance<BuffDef>();

            buffDef.buffColor = new Color(0.9f, 0.5f, 0.5f);
            buffDef.eliteDef = null;
            buffDef.canStack = true;
            buffDef.iconSprite = Addressables.LoadAssetAsync<BuffDef>("RoR2/DLC2/Chef/Buffs/bdBoosted.asset").WaitForCompletion().iconSprite;
            buffDef.isDebuff = false;
            buffDef.name = "ChefAttackSpeedBuff";

            ChefAttackSpeedBuff = buffDef;
            ContentAddition.AddBuffDef(buffDef);
        }

        private void RecalculateStatsAPIOnGetStatCoefficients(CharacterBody sender, StatHookEventArgs args)
        {
            var buffcount = sender.GetBuffCount(ChefAttackSpeedBuff);
            
            if (buffcount > 0)
            {
                args.attackSpeedMultAdd += .25f * buffcount;
            }
        }

        public override void OnSkillExit(BaseState skillState, int level)
        {
            base.OnSkillExit(skillState, level);
            
            if (skillState is Sear)
            {
                Logger.Debug("Sear");
                skillState.characterBody.SetBuffCount(ChefAttackSpeedBuff.buffIndex, 0);
                skillState.characterBody.RecalculateStats();
                
                skillState.characterBody.SetBuffCount(YesChefSkillModifier.levelupBuff.buffIndex, 0);
            }
        }
    }

    [SkillLevelModifier(new[] {"ChefRolyPoly", "ChefRolyPolyBoosted"}, typeof(RolyPoly), typeof(RolyPolyWeaponBlockingState),
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

            if (skillState is RolyPoly rolypoly)
            {
                Logger.Debug("RolyPoly");
                Logger.Debug($"duration {rolypoly.duration}");
                Logger.Debug($"speedMultiplier {rolypoly.speedMultiplier}");
                Logger.Debug($"chargeDuration {rolypoly.chargeDuration}");
                Logger.Debug($"gearCharge {rolypoly.gearCharge}");
                Logger.Debug($"baseDuration {rolypoly.baseDuration}");
                Logger.Debug($"baseDurationLvlUp {rolypoly.baseDurationLvlUp}");
                Logger.Debug($"explosionDmgCoefficient {rolypoly.explosionDmgCoefficient}");
                Logger.Debug($"RolyPoly");
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

            if (skillState is Glaze unknownglaze)
            {
                Logger.Debug("Glaze");
                Logger.Debug($"duration {unknownglaze.duration}");
                Logger.Debug($"grenadeCountmax {Glaze.grenadeCountMax}");
                unknownglaze.grenadeCount = level * 4 * -1; //Glaze.grenadeCountMax is 3, if we make the instance variable negative we can make it bigger ,., 
                Logger.Debug($"damageStat {unknownglaze.damageStat}");
                Logger.Debug($"fireTimer {unknownglaze.fireTimer}");
                Logger.Debug($"duration {unknownglaze.duration}");
            }
        }
    }

    [SkillLevelModifier("YesChef", typeof(YesChef))]
    class YesChefSkillModifier : BaseSkillModifier
    {
        public static BuffDef levelupBuff;

        public override void OnSkillLeveledUp(int level, CharacterBody characterBody, SkillDef skillDef)
        {
            base.OnSkillLeveledUp(level, characterBody, skillDef);
        }

        public override void OnSkillEnter(BaseState skillState, int level)
        {
            base.OnSkillEnter(skillState, level);

            if (skillState is YesChef yessir)
            {
                Logger.Debug("YesChef");
                yessir.characterBody.SetBuffCount(levelupBuff.buffIndex, level);
            }
        }

        public override void OnSkillExit(BaseState skillState, int level)
        {
            base.OnSkillExit(skillState, level);
            
            if (skillState is YesChef yessir)
            {
                Logger.Debug("YesChef");
                if (yessir.characterBody.HasBuff(levelupBuff))
                {
                    yessir.characterBody.SetBuffCount(levelupBuff.buffIndex, 0);
                }
            }
        }

        public override void SetupSkill()
        {
            base.SetupSkill();

            RegisterYesChefLevelTemp();
        }

        public void RegisterYesChefLevelTemp()
        {
            BuffDef buffDef = ScriptableObject.CreateInstance<BuffDef>();

            buffDef.buffColor = new Color(0.9f, 0.9f, 0.5f);
            buffDef.eliteDef = null;
            buffDef.canStack = true;
            buffDef.iconSprite = Addressables.LoadAssetAsync<BuffDef>("RoR2/DLC2/Chef/Buffs/bdBoosted.asset").WaitForCompletion().iconSprite;
            buffDef.isDebuff = false;
            buffDef.name = "ChefTemporaryLevelup";

            levelupBuff = buffDef;
            ContentAddition.AddBuffDef(buffDef);
        }
    }
    
    [SkillLevelModifier(new[] {"ChefOilSpill", "ChefOilSpillBoosted"}, typeof(OilSpillBase), typeof(OilSpillV1), typeof(OilSpillV2))]
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
    
    [SkillLevelModifier(new[] {"ChefIceBox", "ChefIceBoxBoosted"}, typeof(IceBox))]
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
