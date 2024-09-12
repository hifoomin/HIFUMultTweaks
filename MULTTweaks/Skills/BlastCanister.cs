using RoR2;
using RoR2.Projectile;
using RoR2.Skills;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace HIFUMultTweaks
{
    public class BlastCanister : TweakBase<BlastCanister>
    {
        public static bool Ignite;
        public override string Name => "Secondary : Blast Canister";

        public override string SkillToken => "secondary";

        public override string DescText => "<style=cIsDamage>Stunning</style>. " + (Ignite ? "<style=cIsDamage>Ignite</style>." : "") + " Launch a canister for <style=cIsDamage>220% damage</style>. Drops <style=cIsDamage>stun</style> bomblets for <style=cIsDamage>5x44% damage</style>.";

        public override void Init()
        {
            Ignite = ConfigOption(true, "Ignite?", "Vanilla is false");
            base.Init();
        }

        public override void Hooks()
        {
            Changes();
        }

        private void Changes()
        {
            if (Ignite)
            {
                var canister = Addressables.LoadAssetAsync<GameObject>("RoR2/Base/Toolbot/CryoCanisterProjectile.prefab").WaitForCompletion();
                var projectileDamage = canister.GetComponent<ProjectileDamage>();
                projectileDamage.damageType = DamageType.Stun1s | DamageType.IgniteOnHit;

                var skilldef = Addressables.LoadAssetAsync<SkillDef>("RoR2/Base/Toolbot/ToolbotBodyStunDrone.asset").WaitForCompletion();
                skilldef.keywordTokens = new string[] { "KEYWORD_STUNNING", "KEYWORD_IGNITE" };
            }
        }
    }
}