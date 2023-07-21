using UnityEngine;
using UnityEngine.AddressableAssets;

namespace HIFUMultTweaks.Misc
{
    internal class HurtBox : MiscBase
    {
        public static float SizeMultiplier;
        public override string Name => "Misc : Hurt Box";

        public override void Init()
        {
            SizeMultiplier = ConfigOption(0.8f, "Size Multiplier", "Vanilla is 1");
            base.Init();
        }

        public override void Hooks()
        {
            Changes();
        }

        private void Changes()
        {
            var mult = Addressables.LoadAssetAsync<GameObject>("RoR2/Base/Toolbot/ToolbotBody.prefab").WaitForCompletion();
            var mainHurtBox = mult.transform.GetChild(0).GetChild(0).GetChild(0).GetChild(0).Find("tempHurtBox").GetComponent<CapsuleCollider>();
            mainHurtBox.radius = 3.74f * SizeMultiplier;
            mainHurtBox.height = 12.04f * SizeMultiplier;
        }
    }
}