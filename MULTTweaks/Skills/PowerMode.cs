using RoR2.Skills;
using R2API;
using RoR2;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace HIFUMultTweaks
{
    public class PowerMode : TweakBase<PowerMode>
    {
        public static BuffDef armorBuff;
        public static BuffDef slowBuff;
        public static float armor;
        public static int count;
        public static float speedReduc;
        public override string Name => "Special :: Power Mode";

        public override string SkillToken => "special_alt";

        public override string DescText => "Enter a heavy stance, equipping both your <style=cIsDamage>primary attacks</style> at once. Gain <style=cIsUtility>" + (5 * armor) + " decaying armor</style>, but lose <style=cIsHealth>-" + Mathf.Round((1 - (1 / (1 + speedReduc))) * 100) + "% movement speed</style>.";

        public override void Init()
        {
            armor = ConfigOption(20f, "Armor Buff", "Vanilla is 100");
            count = ConfigOption(5, "How many armor stacks to get", "Vanilla is 1 BUT it will decay in a second with vanilla settings, as the decay is 1 stack of armor per second.");
            speedReduc = ConfigOption(0.5f, "Slow Debuff", "Decimal. Vanilla is 0.5");
            base.Init();
        }

        public override void Hooks()
        {
            Changes();

            On.EntityStates.Toolbot.ToolbotDualWieldBase.OnEnter += ToolbotDualWieldBase_OnEnter;
            On.EntityStates.Toolbot.ToolbotDualWield.FixedUpdate += ToolbotDualWield_FixedUpdate;
            On.EntityStates.Toolbot.ToolbotDualWieldBase.OnExit += ToolbotDualWieldBase_OnExit;

            RecalculateStatsAPI.GetStatCoefficients += RecalculateStatsAPI_GetStatCoefficients;
        }

        private void Changes()
        {
            armorBuff = ScriptableObject.CreateInstance<BuffDef>();

            armorBuff.isDebuff = false;
            armorBuff.canStack = true;
            armorBuff.isHidden = false;
            armorBuff.name = "Power Mode Decaying Armor";
            armorBuff.buffColor = new Color32(214, 201, 58, 255);
            armorBuff.iconSprite = Addressables.LoadAssetAsync<BuffDef>("RoR2/Base/Common/bdArmorBoost.asset").WaitForCompletion().iconSprite;

            ContentAddition.AddBuffDef(armorBuff);

            slowBuff = ScriptableObject.CreateInstance<BuffDef>();

            slowBuff.isDebuff = true;
            slowBuff.isCooldown = false;
            slowBuff.isHidden = false;
            slowBuff.buffColor = new Color32(234, 104, 107, 255);
            slowBuff.name = "Power Mode Slow";
            slowBuff.canStack = false;
            slowBuff.iconSprite = Addressables.LoadAssetAsync<BuffDef>("RoR2/Base/SprintOutOfCombat/bdWhipBoost.asset").WaitForCompletion().iconSprite;

            ContentAddition.AddBuffDef(slowBuff);
        }

        private static float countdown = 1f;

        private void ToolbotDualWieldBase_OnExit(On.EntityStates.Toolbot.ToolbotDualWieldBase.orig_OnExit orig, EntityStates.Toolbot.ToolbotDualWieldBase self)
        {
            if (self.characterBody.GetBuffCount(armorBuff) > 0)
            {
                self.characterBody.SetBuffCount(armorBuff.buffIndex, 0);
            }
            orig(self);
        }

        private void ToolbotDualWield_FixedUpdate(On.EntityStates.Toolbot.ToolbotDualWield.orig_FixedUpdate orig, EntityStates.Toolbot.ToolbotDualWield self)
        {
            orig(self);
            countdown -= Time.fixedDeltaTime;
            if (countdown <= 0)
            {
                if (self.characterBody.GetBuffCount(armorBuff) > 0)
                {
                    self.characterBody.RemoveBuff(armorBuff);
                }
                countdown = 1f;
            }
        }

        private void RecalculateStatsAPI_GetStatCoefficients(CharacterBody sender, RecalculateStatsAPI.StatHookEventArgs args)
        {
            if (sender)
            {
                if (sender.HasBuff(slowBuff))
                {
                    args.moveSpeedReductionMultAdd += speedReduc;
                }
                args.armorAdd += armor * sender.GetBuffCount(armorBuff);
            }
        }

        private void ToolbotDualWieldBase_OnEnter(On.EntityStates.Toolbot.ToolbotDualWieldBase.orig_OnEnter orig, EntityStates.Toolbot.ToolbotDualWieldBase self)
        {
            self.applyBonusBuff = false;
            EntityStates.Toolbot.ToolbotDualWieldBase.penaltyBuff = slowBuff;
            for (int i = 0; i < count; i++)
            {
                self.characterBody.AddBuff(armorBuff);
            }
            orig(self);
        }
    }
}