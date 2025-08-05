namespace SkillsPlusPlus.Source.Modifiers
{
    /*[SkillLevelModifier("ChefDice", typeof(Dice))]
    class ChefDiceSkillModifier : BaseSkillModifier
    {

        public override void OnSkillLeveledUp(int level, CharacterBody characterBody, SkillDef skillDef)
        {
            base.OnSkillLeveledUp(level, characterBody, skillDef);
        }

        public override void OnSkillEnter(BaseState skillState, int level)
        {
            base.OnSkillEnter(skillState, level);
            if (skillState is Dice dice)
            {
                Logger.Debug("Dice");
                //dice.force = AdditiveScaling(4, 3, level);
                //chargeState.baseChargeDuration = MultScaling(chargeState.baseChargeDuration, -0.15f, level); // +15% charge spee
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
                Logger.Debug("FireCorruptHandBeam");
            }
        }
    }

    [SkillLevelModifier("ChefRolyPoly", typeof(RolyPoly), typeof(ChargeRolyPoly), typeof(RolyPolyWeaponBlockingState),
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
            else if (skillState is ChargeRolyPoly)
            {
                Logger.Debug("ChargeRolyPoly");
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
    class ChefYesChefSkillModifier : BaseSkillModifier
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
    }*/
}
