using BepInEx.Configuration;

namespace Starstorm2Unofficial
{
    public static class StaticValues
    {
        public static float forkDamageValue;

        public static float coffeeAttackSpeedValue;
        public static float coffeeMoveSpeedValue;

        public static float maliceRangeValue;
        public static float maliceRangeStackValue;
        public static float maliceDmgReductionValue;
        public static float maliceProcCoefValue;

        public static float trematodeDamage;
        public static float trematodeDuration;
        public static float trematodeCritical;

        public static float diaryTime;

        public static float coinChance;
        public static float coinDuration;
        public static float coinDamage;
        public static float coinMoneyGained;

        public static float massFactor;
        public static float massHealthGain;

        public static float testerGold;
        public static float testerHealing;

        public static float gadgetDamage;
        public static float gadgetCrit;

        public static float droidLife;
        public static float droidDamage;
        public static float droidSpeed;

        public static float soulChance;

        public static float bootsBase;
        public static float bootsStack;
        public static float bootsRadius;
        public static float bootsProc;
        public static JetBootsEffectQuality timbsQuality;

        public static float canBaseChance;
        public static float canStackChance;
        public static float canDuration;
        public static float canDamage;

        public static float dungusBase;
        public static float dungusStack;
        public static float dungusTime;

        public static float choccyThreshold;
        public static float choccyBaseTime;
        public static float choccyStackTime;

        public static float hottestSusRadius;
        public static float hottestSusHit;
        public static float hottestSusDuration;
        public static float hottestSusDamage;

        public static float sekiroArmor;
        public static float sekiroArmorStack;
        public static float sekiroCrit;
        public static float sekiroCritStack;

        internal static void InitValues()
        {
            sekiroArmor = 15f;
            sekiroArmorStack = 10f;
            sekiroCrit = 25f;
            sekiroCritStack = 20f;

            hottestSusHit = 1.5f;
            hottestSusRadius = 30f;
            hottestSusDuration = 6f;
            hottestSusDamage = 1f;

            choccyThreshold = 0.2f;
            choccyBaseTime = 5f;
            choccyStackTime = 10f;

            dungusBase = 0.015f;
            dungusStack = 0.005f;
            dungusTime = 1f;

            canBaseChance = 8.5f;
            canStackChance = 5f;
            canDuration = 3.5f;
            canDamage = 1f;

            bootsBase = 1.5f;
            bootsStack = 1f;
            bootsRadius = 7.5f;
            bootsProc = 0f;
            timbsQuality = JetBootsEffectQuality.Default;

            forkDamageValue = 0.07f;

            coffeeAttackSpeedValue = 0.075f;
            coffeeMoveSpeedValue = 0.07f;

            maliceRangeValue = 9f;
            maliceRangeStackValue = 1f;
            maliceDmgReductionValue = 0.55f;
            maliceProcCoefValue = 0f;

            trematodeDamage = 1f;
            trematodeDuration = 3f;
            trematodeCritical = 0.4f;

            diaryTime = 2f;

            coinChance = 6f;
            coinDuration = 4f;
            coinDamage = 1f;
            coinMoneyGained = 1f;

            massFactor = 8f;
            massHealthGain = 1f;

            testerGold = 5f;
            testerHealing = 15f;

            gadgetDamage = 0.5f;
            gadgetCrit = 10f;

            droidLife = 15f;
            droidDamage = 1f;
            droidSpeed = 2f;

            soulChance = 3f;
        }

        // helper for ez item stat config
        internal static float ItemStatConfigValue(string itemName, string configName, string desc, float defaultValue)
        {
            ConfigEntry<float> config = StarstormPlugin.instance.Config.Bind<float>("Starstorm 2 :: Items :: " + itemName, configName, defaultValue, new ConfigDescription(desc));
            return config.Value;
        }

        internal static float ItemStatStupidConfigValue(string itemName, string configName, string desc, int defaultValue)
        {
            ConfigEntry<float> config = StarstormPlugin.instance.Config.Bind<float>("Starstorm 2 :: Items :: " + itemName, configName, defaultValue, new ConfigDescription(desc));
            return config.Value;
        }

        public enum JetBootsEffectQuality
        {
            None,
            Light,
            Default
        };
    }
}