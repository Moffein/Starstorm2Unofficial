using UnityEngine;
using R2API;
using RoR2;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine.AddressableAssets;
using static UnityEngine.GridBrushBase;

namespace Starstorm2Unofficial.Cores
{
    public class ItemDisplayCore
    {
        public static Dictionary<string, string> itemDisplayPaths = new Dictionary<string, string>()
        {
            { "DisplayEliteHorn", "RoR2/Base/EliteFire/DisplayEliteHorn.prefab" },

            { "DisplayEliteStealthCrown", "RoR2/Base/EliteHaunted/DisplayEliteStealthCrown.prefab" },

            { "DisplayEliteIceCrown", "RoR2/Base/EliteIce/DisplayEliteIceCrown.prefab" },

            { "DisplayEliteRhinoHorn", "RoR2/Base/EliteLightning/DisplayEliteRhinoHorn.prefab" },

            { "DisplayEliteLunar, Fire", "RoR2/Base/EliteLunar/DisplayEliteLunar, Fire.prefab" },
            { "DisplayEliteLunar,Eye", "RoR2/Base/EliteLunar/DisplayEliteLunar,Eye.prefab" },

            { "DisplayEliteUrchinCrown", "RoR2/Base/ElitePoison/DisplayEliteUrchinCrown.prefab" },

            { "DisplayBFG", "RoR2/Base/BFG/DisplayBFG.prefab" },

            { "DisplayGravCube", "RoR2/Base/Blackhole/DisplayGravCube.prefab" },
            { "DisplayGravCubeFollower", "RoR2/Base/Blackhole/DisplayGravCubeFollower.prefab" },

            { "DisplayPotion", "RoR2/Base/BurnNearby/DisplayPotion.prefab" },

            { "DisplayWaterPack", "RoR2/Base/Cleanse/DisplayWaterPack.prefab" },

            { "DisplayMissileRack", "RoR2/Base/CommandMissile/DisplayMissileRack.prefab" },

            { "DisplayEffigy", "RoR2/Base/CrippleWard/DisplayEffigy.prefab" },

            { "DisplayNeuralImplant", "RoR2/Base/CritOnUse/DisplayNeuralImplant.prefab" },

            { "DisplayDeathProjectile", "RoR2/Base/DeathProjectile/DisplayDeathProjectile.prefab" },
            //RoR2/Base/DeathProjectile/mdlDeathProjectileDisplay.fbx

            { "DisplayRadio", "RoR2/Base/DroneBackup/DisplayRadio.prefab" },

            { "DisplayEgg", "RoR2/Base/FireBallDash/DisplayEgg.prefab" },

            { "DisplayFruit", "RoR2/Base/Fruit/DisplayFruit.prefab" },

            { "DisplayElephantFigure", "RoR2/Base/GainArmor/DisplayElephantFigure.prefab" },

            { "DisplayVase", "RoR2/Base/Gateway/DisplayVase.prefab" },

            { "DisplayGoldGat", "RoR2/Base/GoldGat/DisplayGoldGat.prefab" },

            { "DisplayBugWings", "RoR2/Base/Jetpack/DisplayBugWings.prefab" },

            { "DisplayLifestealOnHit", "RoR2/Base/LifestealOnHit/DisplayLifestealOnHit.prefab" },

            { "DisplayLightningArmLeft", "RoR2/Base/Lightning/DisplayLightningArmLeft.prefab" },
            { "DisplayLightningArmLeftVoidSurvivor", "RoR2/Base/Lightning/DisplayLightningArmLeftVoidSurvivor.prefab" },
            { "DisplayLightningArmRight,Bandit2", "RoR2/Base/Lightning/DisplayLightningArmRight,Bandit2.prefab" },
            { "DisplayLightningArmRight,Croco", "RoR2/Base/Lightning/DisplayLightningArmRight,Croco.prefab" },
            { "DisplayLightningArmRight", "RoR2/Base/Lightning/DisplayLightningArmRight.prefab" },

            { "DisplayMeteor", "RoR2/Base/Meteor/DisplayMeteor.prefab" },
            { "DisplayMeteorFollower", "RoR2/Base/Meteor/DisplayMeteorFollower.prefab" },

            { "DisplayBatteryArray", "RoR2/Base/QuestVolatileBattery/DisplayBatteryArray.prefab" },

            { "DisplayRecycler", "RoR2/Base/Recycle/DisplayRecycler.prefab" },

            { "DisplaySawmerang", "RoR2/Base/Saw/DisplaySawmerang.prefab" },
            { "DisplaySawmerangFollower", "RoR2/Base/Saw/DisplaySawmerangFollower.prefab" },

            { "DisplayScanner", "RoR2/Base/Scanner/DisplayScanner.prefab" },

            { "DisplayTeamWarCry", "RoR2/Base/TeamWarCry/DisplayTeamWarCry.prefab" },

            { "DisplayTonic", "RoR2/Base/Tonic/DisplayTonic.prefab" },

            { "DisplayAlienHead", "RoR2/Base/AlienHead/DisplayAlienHead.prefab" },

            { "DisplayRepulsionArmorPlate", "RoR2/Base/ArmorPlate/DisplayRepulsionArmorPlate.prefab" },

            { "DisplayWarhammer", "RoR2/Base/ArmorReductionOnHit/DisplayWarhammer.prefab" },

            { "DisplayWolfPelt", "RoR2/Base/AttackSpeedOnCrit/DisplayWolfPelt.prefab" },

            { "DisplayFossil", "RoR2/Base/AutoCastEquipment/DisplayFossil.prefab" },

            { "DisplayBandolier", "RoR2/Base/Bandolier/DisplayBandolier.prefab" },

            { "DisplayBrooch", "RoR2/Base/BarrierOnKill/DisplayBrooch.prefab" },

            { "DisplayAegis", "RoR2/Base/BarrierOnOverHeal/DisplayAegis.prefab" },

            { "DisplayBear", "RoR2/Base/Bear/DisplayBear.prefab" },
            { "DisplayBearSit", "RoR2/Base/Bear/DisplayBearSit.prefab" },

            { "DisplayBeetleGland", "RoR2/Base/BeetleGland/DisplayBeetleGland.prefab" },

            { "DisplayBehemoth", "RoR2/Base/Behemoth/DisplayBehemoth.prefab" },

            { "DisplayTriTip", "RoR2/Base/BleedOnHit/DisplayTriTip.prefab" },

            { "DisplayBleedOnHitAndExplode", "RoR2/Base/BleedOnHitAndExplode/DisplayBleedOnHitAndExplode.prefab" },

            { "DisplayTome", "RoR2/Base/BonusGoldPackOnKill/DisplayTome.prefab" },

            { "DisplayAPRound", "RoR2/Base/BossDamageBonus/DisplayAPRound.prefab" },

            { "DisplayHook", "RoR2/Base/BounceNearby/DisplayHook.prefab" },
            { "DisplayHookHead", "RoR2/Base/BounceNearby/DisplayHookHead.prefab" },

            { "DisplayUkulele", "RoR2/Base/ChainLightning/DisplayUkulele.prefab" },

            { "DisplayClover", "RoR2/Base/Clover/DisplayClover.prefab" },

            { "DisplayGlasses", "RoR2/Base/CritGlasses/DisplayGlasses.prefab" },

            { "DisplayCrowbar", "RoR2/Base/Crowbar/DisplayCrowbar.prefab" },

            { "DisplayDagger", "RoR2/Base/Dagger/DisplayDagger.prefab" },

            { "DisplayDeathMark", "RoR2/Base/DeathMark/DisplayDeathMark.prefab" },

            { "DisplayFireRing", "RoR2/Base/ElementalRings/DisplayFireRing.prefab" },
            { "DisplayIceRing", "RoR2/Base/ElementalRings/DisplayIceRing.prefab" },

            { "DisplayWarHorn", "RoR2/Base/EnergizedOnEquipmentUse/DisplayWarHorn.prefab" },

            { "DisplayBattery", "RoR2/Base/EquipmentMagazine/DisplayBattery.prefab" },

            { "DisplayGuillotine", "RoR2/Base/ExecuteLowHealthElite/DisplayGuillotine.prefab" },

            { "DisplayWilloWisp", "RoR2/Base/ExplodeOnDeath/DisplayWilloWisp.prefab" },

            { "DisplayHippo", "RoR2/Base/ExtraLife/DisplayHippo.prefab" },

            { "DisplayGravBoots", "RoR2/Base/FallBoots/DisplayGravBoots.prefab" },

            { "DisplayFeather", "RoR2/Base/Feather/DisplayFeather.prefab" },

            { "DisplayFireballsOnHit", "RoR2/Base/FireballsOnHit/DisplayFireballsOnHit.prefab" },

            { "DisplayFirework", "RoR2/Base/Firework/DisplayFirework.prefab" },

            { "DisplaySteakCurved", "RoR2/Base/FlatHealth/DisplaySteakCurved.prefab" },
            { "DisplaySteakFlat", "RoR2/Base/FlatHealth/DisplaySteakFlat.prefab" },

            { "DisplayFocusedConvergence", "RoR2/Base/FocusConvergence/DisplayFocusedConvergence.prefab" },
            { "DisplayFocusedConvergenceFollower", "RoR2/Base/FocusConvergence/DisplayFocusedConvergenceFollower.prefab" },

            { "DisplayMask", "RoR2/Base/GhostOnKill/DisplayMask.prefab" },

            { "DisplayBoneCrown", "RoR2/Base/GoldOnHit/DisplayBoneCrown.prefab" },

            { "DisplaySkullcrown", "RoR2/Base/HeadHunter/DisplaySkullcrown.prefab" },

            { "DisplayScythe", "RoR2/Base/HealOnCrit/DisplayScythe.prefab" },

            { "DisplaySnail", "RoR2/Base/HealWhileSafe/DisplaySnail.prefab" },

            { "DisplayHoof", "RoR2/Base/Hoof/DisplayHoof.prefab" },

            { "DisplayFrostRelic", "RoR2/Base/Icicle/DisplayFrostRelic.prefab" },
            { "DisplayFrostRelicFollower", "RoR2/Base/Icicle/DisplayFrostRelicFollower.prefab" },
            { "DisplayIcicle", "RoR2/Base/Icicle/DisplayIcicle.prefab" },

            { "DisplayGasoline", "RoR2/Base/IgniteOnKill/DisplayGasoline.prefab" },

            { "DisplayAntler", "RoR2/Base/IncreaseHealing/DisplayAntler.prefab" },

            { "DisplayInfusion", "RoR2/Base/Infusion/DisplayInfusion.prefab" },

            { "DisplayWaxBird", "RoR2/Base/JumpBoost/DisplayWaxBird.prefab" },

            { "DisplayBrainstalk", "RoR2/Base/KillEliteFrenzy/DisplayBrainstalk.prefab" },

            { "DisplayKnurl", "RoR2/Base/Knurl/DisplayKnurl.prefab" },

            { "DisplayLaserTurbine", "RoR2/Base/LaserTurbine/DisplayLaserTurbine.prefab" },

            { "DisplayChargedPerforator", "RoR2/Base/LightningStrikeOnHit/DisplayChargedPerforator.prefab" },

            { "DisplayLunarDagger", "RoR2/Base/LunarDagger/DisplayLunarDagger.prefab" },

            { "DisplayBirdEye", "RoR2/Base/LunarSkillReplacements/DisplayBirdEye.prefab" },
            { "DisplayBirdClaw", "RoR2/Base/LunarSkillReplacements/DisplayBirdClaw.prefab" },
            { "DisplayBirdHeart", "RoR2/Base/LunarSkillReplacements/DisplayBirdHeart.prefab" },
            { "DisplayBirdFoot", "RoR2/Base/LunarSkillReplacements/DisplayBirdFoot.prefab" },

            { "DisplayBeads", "RoR2/Base/LunarTrinket/DisplayBeads.prefab" },

            { "DisplayMedkit", "RoR2/Base/Medkit/DisplayMedkit.prefab" },

            { "DisplayMissileLauncher", "RoR2/Base/Missile/DisplayMissileLauncher.prefab" },

            { "DisplayMonstersOnShrineUse", "RoR2/Base/MonstersOnShrineUse/DisplayMonstersOnShrineUse.prefab" },

            { "DisplayMushroom", "RoR2/Base/Mushroom/DisplayMushroom.prefab" },

            { "DisplayDiamond", "RoR2/Base/NearbyDamageBonus/DisplayDiamond.prefab" },

            { "DisplayDevilHorns", "RoR2/Base/NovaOnHeal/DisplayDevilHorns.prefab" },

            { "DisplayJellyGuts", "RoR2/Base/NovaOnLowHealth/DisplayJellyGuts.prefab" },

            { "DisplayParentEgg", "RoR2/Base/ParentEgg/DisplayParentEgg.prefab" },

            { "DisplayPearl", "RoR2/Base/Pearl/DisplayPearl.prefab" },

            { "DisplayShieldGenerator", "RoR2/Base/PersonalShield/DisplayShieldGenerator.prefab" },

            { "DisplayStealthkit", "RoR2/Base/Phasing/DisplayStealthkit.prefab" },

            { "DisplayInterstellarDeskPlant", "RoR2/Base/Plant/DisplayInterstellarDeskPlant.prefab" },

            { "DisplayRandomDamageZone", "RoR2/Base/RandomDamageZone/DisplayRandomDamageZone.prefab" },

            { "DisplayCorpseflower", "RoR2/Base/RepeatHeal/DisplayCorpseflower.prefab" },

            { "DisplayEmpathyChip", "RoR2/Base/RoboBallBuddy/DisplayEmpathyChip.prefab" },

            { "DisplayDoubleMag", "RoR2/Base/SecondarySkillMagazine/DisplayDoubleMag.prefab" },

            { "DisplaySeed", "RoR2/Base/Seed/DisplaySeed.prefab" },

            { "DisplayShieldBug", "RoR2/Base/ShieldOnly/DisplayShieldBug.prefab" },

            { "DisplayShinyPearl", "RoR2/Base/ShinyPearl/DisplayShinyPearl.prefab" },

            { "DisplayTeslaCoil", "RoR2/Base/ShockNearby/DisplayTeslaCoil.prefab" },

            { "DisplaySiphonOnLowHealth", "RoR2/Base/SiphonOnLowHealth/DisplaySiphonOnLowHealth.prefab" },

            { "DisplayBauble", "RoR2/Base/SlowOnHit/DisplayBauble.prefab" },

            { "DisplayBuckler", "RoR2/Base/SprintArmor/DisplayBuckler.prefab" },

            { "DisplaySoda", "RoR2/Base/SprintBonus/DisplaySoda.prefab" },

            { "DisplayWhip", "RoR2/Base/SprintOutOfCombat/DisplayWhip.prefab" },

            { "DisplayBrokenMask", "RoR2/Base/SprintWisp/DisplayBrokenMask.prefab" },

            { "DisplaySquidTurret", "RoR2/Base/Squid/DisplaySquidTurret.prefab" },

            { "DisplayStickyBomb", "RoR2/Base/StickyBomb/DisplayStickyBomb.prefab" },

            { "DisplayStunGrenade", "RoR2/Base/StunChanceOnHit/DisplayStunGrenade.prefab" },

            { "DisplaySyringe", "RoR2/Base/Syringe/DisplaySyringe.prefab" },
            { "DisplaySyringeCluster", "RoR2/Base/Syringe/DisplaySyringeCluster.prefab" },

            { "DisplayTalisman", "RoR2/Base/Talisman/DisplayTalisman.prefab" },
            { "DisplayTalismanFollower", "RoR2/Base/Talisman/DisplayTalismanFollower.prefab" },

            { "DisplayRazorwireCoiled", "RoR2/Base/Thorns/DisplayRazorwireCoiled.prefab" },
            { "DisplayRazorwireLeft", "RoR2/Base/Thorns/DisplayRazorwireLeft.prefab" },
            { "DisplayRazorwireLeftVoidSurvivor", "RoR2/Base/Thorns/DisplayRazorwireLeftVoidSurvivor.prefab" },
            { "DisplayRazorwireRight", "RoR2/Base/Thorns/DisplayRazorwireRight.prefab" },

            { "DisplayGoldHeart", "RoR2/Base/TitanGoldDuringTP/DisplayGoldHeart.prefab" },
            //Hopoo shit
            { "DisplayToothMeshLarge", "RoR2/Base/Tooth/DisplayToothMeshLarge.prefab" },
            { "DisplayToothMeshSmall1", "RoR2/Base/Tooth/DisplayToothMeshSmall1.prefab" },
            { "DisplayToothMeshSmall2", "RoR2/Base/Tooth/DisplayToothMeshSmall2.prefab" },
            { "DisplayToothNecklaceDecal", "RoR2/Base/Tooth/DisplayToothNecklaceDecal.prefab" },

            //RoR2/Base/Tooth/mdlToothNecklaceDisplay.fbx

            { "DisplayGlowFlower", "RoR2/Base/TPHealingNova/DisplayGlowFlower.prefab" },

            { "DisplayKey", "RoR2/Base/TreasureCache/DisplayKey.prefab" },

            { "DisplayAfterburner", "RoR2/Base/UtilitySkillMagazine/DisplayAfterburner.prefab" },
            { "DisplayAfterburnerShoulderRing", "RoR2/Base/UtilitySkillMagazine/DisplayAfterburnerShoulderRing.prefab" },

            { "DisplayPauldron", "RoR2/Base/WarCryOnMultiKill/DisplayPauldron.prefab" },

            { "DisplayWarbanner", "RoR2/Base/WardOnLevel/DisplayWarbanner.prefab" },

            { "SkillDisplay", "RoR2/Base/UI/SkillDisplay.prefab" },

            { "RegeneratingScrapExplosionDisplay", "RoR2/DLC1/RegeneratingScrap/RegeneratingScrapExplosionDisplay.prefab" },


            { "RailgunnerDisplay", "RoR2/DLC1/Railgunner/RailgunnerDisplay.prefab" },

            { "VoidSurvivorDisplay", "RoR2/DLC1/VoidSurvivor/VoidSurvivorDisplay.prefab" },

            { "DisplayEliteMendingAntlers", "RoR2/DLC1/EliteEarth/DisplayEliteMendingAntlers.prefab" },

            { "DisplayEliteRabbitEars", "RoR2/DLC1/DisplayEliteRabbitEars.prefab" },

            { "DisplayAffixVoid", "RoR2/DLC1/EliteVoid/DisplayAffixVoid.prefab" },

            { "DisplayBlunderbuss", "RoR2/DLC1/BossHunter/DisplayBlunderbuss.prefab" },
            { "DisplayBlunderbussFollower", "RoR2/DLC1/BossHunter/DisplayBlunderbussFollower.prefab" },
            { "DisplayTricornGhost", "RoR2/DLC1/BossHunter/DisplayTricornGhost.prefab" },
            { "DisplayTricornUsed", "RoR2/DLC1/BossHunter/DisplayTricornUsed.prefab" },

            { "DisplayGummyClone", "RoR2/DLC1/GummyClone/DisplayGummyClone.prefab" },

            { "DisplayIrradiatingLaser", "RoR2/DLC1/IrradiatingLaser/DisplayIrradiatingLaser.prefab" },

            { "DisplayLunarPortalOnUse", "RoR2/DLC1/LunarPortalOnUse/DisplayLunarPortalOnUse.prefab" },
            { "DisplayLunarPortalOnUseFollower", "RoR2/DLC1/LunarPortalOnUse/DisplayLunarPortalOnUseFollower.prefab" },

            { "DisplayMolotov", "RoR2/DLC1/Molotov/DisplayMolotov.prefab" },

            { "DisplayExecutiveCard", "RoR2/DLC1/MultiShopCard/DisplayExecutiveCard.prefab" },

            { "DisplayVendingMachine", "RoR2/DLC1/VendingMachine/DisplayVendingMachine.prefab" },
            { "DisplayVendingMachine2", "RoR2/DLC1/VendingMachine/DisplayVendingMachine2.prefab" },

            { "DisplayCoffee", "RoR2/DLC1/AttackSpeedAndMoveSpeed/DisplayCoffee.prefab" },

            { "DisplayBearVoid", "RoR2/DLC1/BearVoid/DisplayBearVoid.prefab" },
            { "DisplayBearVoidSit", "RoR2/DLC1/BearVoid/DisplayBearVoidSit.prefab" },

            { "DisplayTriTipVoid", "RoR2/DLC1/BleedOnHitVoid/DisplayTriTipVoid.prefab" },

            { "DisplayUkuleleVoid", "RoR2/DLC1/ChainLightningVoid/DisplayUkuleleVoid.prefab" },

            { "DisplayCloverVoid", "RoR2/DLC1/CloverVoid/DisplayCloverVoid.prefab" },

            { "DisplayLaserSight", "RoR2/DLC1/CritDamage/DisplayLaserSight.prefab" },

            { "DisplayGlassesVoid", "RoR2/DLC1/CritGlassesVoid/DisplayGlassesVoid.prefab" },

            { "DisplayDroneWeaponLauncher", "RoR2/DLC1/DroneWeapons/DisplayDroneWeaponLauncher.prefab" },
            { "DisplayDroneWeaponMinigun", "RoR2/DLC1/DroneWeapons/DisplayDroneWeaponMinigun.prefab" },
            { "DisplayDroneWeaponRobotArm", "RoR2/DLC1/DroneWeapons/DisplayDroneWeaponRobotArm.prefab" },

            { "DisplayVoidRing", "RoR2/DLC1/ElementalRingVoid/DisplayVoidRing.prefab" },

            { "DisplayFuelCellVoid", "RoR2/DLC1/EquipmentMagazineVoid/DisplayFuelCellVoid.prefab" },

            { "DisplayWillowWispVoid", "RoR2/DLC1/ExplodeOnDeathVoid/DisplayWillowWispVoid.prefab" },

            { "DisplayHippoVoid", "RoR2/DLC1/ExtraLifeVoid/DisplayHippoVoid.prefab" },

            { "DisplayDelicateWatch", "RoR2/DLC1/FragileDamageBonus/DisplayDelicateWatch.prefab" },

            { "DisplayShippingRequestForm", "RoR2/DLC1/FreeChest/DisplayShippingRequestForm.prefab" },

            { "DisplayRollOfPennies", "RoR2/DLC1/GoldOnHurt/DisplayRollOfPennies.prefab" },

            { "DisplayLunarShoulderNature", "RoR2/DLC1/HalfAttackSpeedHalfCooldowns/DisplayLunarShoulderNature.prefab" },

            { "DisplayLunarShoulderStone", "RoR2/DLC1/HalfSpeedDoubleHealth/DisplayLunarShoulderStone.prefab" },

            { "DisplayHealingPotion", "RoR2/DLC1/HealingPotion/DisplayHealingPotion.prefab" },

            { "DisplayRainCoatBelt", "RoR2/DLC1/ImmuneToDebuff/DisplayRainCoatBelt.prefab" },
            //Not sure if its used
            //RoR2/DLC1/ImmuneToDebuff/mdlRaincoatDisplayBelt.fbx
            //RoR2/DLC1/ImmuneToDebuff/mdlRaincoatDisplayFolded.fbx

            { "DisplaySunHead", "RoR2/DLC1/LunarSun/DisplaySunHead.prefab" },
            { "DisplaySunHeadNeck", "RoR2/DLC1/LunarSun/DisplaySunHeadNeck.prefab" },

            //Unused
            { "DisplayLunarWings", "RoR2/DLC1/LunarWings/DisplayLunarWings.prefab" },

            { "DisplayDefenseNucleus", "RoR2/DLC1/MinorConstructOnKill/DisplayDefenseNucleus.prefab" },
            { "DisplayDefenseNucleusFollower", "RoR2/DLC1/MinorConstructOnKill/DisplayDefenseNucleusFollower.prefab" },

            { "DisplayMissileLauncherVoid", "RoR2/DLC1/MissileVoid/DisplayMissileLauncherVoid.prefab" },

            { "DisplayICBM", "RoR2/DLC1/MoreMissile/DisplayICBM.prefab" },

            { "DisplayGrappleHook", "RoR2/DLC1/MoveSpeedOnKill/DisplayGrappleHook.prefab" },

            { "DisplayMushroomVoid", "RoR2/DLC1/MushroomVoid/DisplayMushroomVoid.prefab" },

            { "DisplayOddlyShapedOpal", "RoR2/DLC1/OutOfCombatArmor/DisplayOddlyShapedOpal.prefab" },

            { "DisplayScorpion", "RoR2/DLC1/PermanentDebuffOnHit/DisplayScorpion.prefab" },

            { "DisplayShuriken", "RoR2/DLC1/PrimarySkillShuriken/DisplayShuriken.prefab" },

            { "DisplayBottledChaos", "RoR2/DLC1/RandomEquipmentTrigger/DisplayBottledChaos.prefab" },

            { "DisplayDomino", "RoR2/DLC1/RandomlyLunar/DisplayDomino.prefab" },
            { "DisplayDominoFollower", "RoR2/DLC1/RandomlyLunar/DisplayDominoFollower.prefab" },

            { "DisplayRegeneratingScrap", "RoR2/DLC1/RegeneratingScrap/DisplayRegeneratingScrap.prefab" },

            { "DisplayScrapVoid", "RoR2/DLC1/ScrapVoid/DisplayScrapVoid.prefab" },
            { "DisplayScrapVoidGreen", "RoR2/DLC1/ScrapVoid/DisplayScrapVoidGreen.prefab" },
            { "DisplayScrapVoidRed", "RoR2/DLC1/ScrapVoid/DisplayScrapVoidRed.prefab" },
            { "DisplayScrapVoidWhite", "RoR2/DLC1/ScrapVoid/DisplayScrapVoidWhite.prefab" },

            { "DisplayBaubleVoid", "RoR2/DLC1/SlowOnHitVoid/DisplayBaubleVoid.prefab" },

            { "DisplayGasTank", "RoR2/DLC1/StrengthenBurn/DisplayGasTank.prefab" },

            { "DisplayKeyVoid", "RoR2/DLC1/TreasureCacheVoid/DisplayKeyVoid.prefab" },

            { "DisplayMegaCrabItem", "RoR2/DLC1/DisplayMegaCrabItem.prefab" },
            
            //idfk if this is actually an item display
            { "PickupCaptainDefenseMatrix", "RoR2/Base/CaptainDefenseMatrix/PickupCaptainDefenseMatrix.prefab" },
            { "PickupArtifactKey", "RoR2/Base/ArtifactKey/PickupArtifactKey.prefab" },
            { "PickupStarSeed", "RoR2/Base/LunarBadLuck/PickupStarSeed.prefab" }
        };
        public static Dictionary<string, GameObject> itemDisplayPrefabs = new Dictionary<string, GameObject>(System.StringComparer.OrdinalIgnoreCase);

        public static GameObject capacitorPrefab;

        public static bool scepterInstalled = false;

        public ItemDisplayCore()
        {
            if (BepInEx.Bootstrap.Chainloader.PluginInfos.ContainsKey("com.DestroyedClone.AncientScepter")) scepterInstalled = true;

            PopulateDisplays();
        }

        public static ItemDisplayRuleSet.KeyAssetRuleGroup CreateGenericDisplayRule(Object keyAsset, string prefabName, string childName, Vector3 position, Vector3 rotation, Vector3 scale)
        {
            return CreateGenericDisplayRule(keyAsset, LoadDisplay(prefabName), childName, position, rotation, scale);
        }

        public static ItemDisplayRuleSet.KeyAssetRuleGroup CreateGenericDisplayRule(Object keyAsset, GameObject itemPrefab, string childName, Vector3 position, Vector3 rotation, Vector3 scale)
        {
            ItemDisplayRuleSet.KeyAssetRuleGroup displayRule = new ItemDisplayRuleSet.KeyAssetRuleGroup
            {
                keyAsset = keyAsset,
                displayRuleGroup = new DisplayRuleGroup
                {
                    rules = new ItemDisplayRule[]
                    {
                        new ItemDisplayRule
                        {
                            ruleType = ItemDisplayRuleType.ParentedPrefab,
                            childName = childName,
                            followerPrefab = itemPrefab,
                            limbMask = LimbFlags.None,
                            localPos = position,
                            localAngles = rotation,
                            localScale = scale
                        }
                    }
                }
            };

            return displayRule;
        }

        public static ItemDisplayRuleSet.KeyAssetRuleGroup CreateMirroredDisplayRule(Object keyAsset, string prefabName, string childName, Vector3 position, Vector3 rotation, Vector3 scale)
        {
            ItemDisplayRuleSet.KeyAssetRuleGroup displayRule = new ItemDisplayRuleSet.KeyAssetRuleGroup
            {
                keyAsset = keyAsset,
                displayRuleGroup = new DisplayRuleGroup
                {
                    rules = new ItemDisplayRule[]
                    {
                        new ItemDisplayRule
                        {
                            ruleType = ItemDisplayRuleType.ParentedPrefab,
                            childName = childName,
                            followerPrefab = LoadDisplay(prefabName),
                            limbMask = LimbFlags.None,
                            localPos = position,
                            localAngles = rotation,
                            localScale = scale
                        },
                        new ItemDisplayRule
                        {
                            ruleType = ItemDisplayRuleType.ParentedPrefab,
                            childName = childName,
                            followerPrefab = LoadDisplay(prefabName),
                            limbMask = LimbFlags.None,
                            localPos = new Vector3(-1f * position.x, position.y, position.z),
                            localAngles = rotation,
                            localScale = new Vector3(scale.x, scale.y, -1f * scale.z)
                        }
                    }
                }
            };

            return displayRule;
        }

        public static ItemDisplayRuleSet.KeyAssetRuleGroup CreateZMirroredDisplayRule(Object keyAsset, string prefabName, string childName, Vector3 position, Vector3 rotation, Vector3 scale)
        {
            ItemDisplayRuleSet.KeyAssetRuleGroup displayRule = new ItemDisplayRuleSet.KeyAssetRuleGroup
            {
                keyAsset = keyAsset,
                displayRuleGroup = new DisplayRuleGroup
                {
                    rules = new ItemDisplayRule[]
                    {
                        new ItemDisplayRule
                        {
                            ruleType = ItemDisplayRuleType.ParentedPrefab,
                            childName = childName,
                            followerPrefab = LoadDisplay(prefabName),
                            limbMask = LimbFlags.None,
                            localPos = position,
                            localAngles = rotation,
                            localScale = scale
                        },
                        new ItemDisplayRule
                        {
                            ruleType = ItemDisplayRuleType.ParentedPrefab,
                            childName = childName,
                            followerPrefab = LoadDisplay(prefabName),
                            limbMask = LimbFlags.None,
                            localPos = new Vector3(-1f * position.x, position.y, position.z),
                            localAngles = rotation,
                            localScale = new Vector3(-1f * scale.x, scale.y, scale.z)
                            //look i'm sure there was a better solution than creating another display rule but i'm very high and can't figure out how to do this otherwise
                            // nigga this is literally making shit so easy a monkey could do it, cry about it
                        }
                    }
                }
            };

            return displayRule;
        }

        public static ItemDisplayRuleSet.KeyAssetRuleGroup CreateFollowerDisplayRule(Object keyAsset, GameObject displayPrefab, Vector3 position, Vector3 rotation, Vector3 scale)
        {
            ItemDisplayRuleSet.KeyAssetRuleGroup displayRule = new ItemDisplayRuleSet.KeyAssetRuleGroup
            {
                keyAsset = keyAsset,
                displayRuleGroup = new DisplayRuleGroup
                {
                    rules = new ItemDisplayRule[]
                    {
                        new ItemDisplayRule
                        {
                            ruleType = ItemDisplayRuleType.ParentedPrefab,
                            childName = "Base",
                            followerPrefab = displayPrefab,
                            limbMask = LimbFlags.None,
                            localPos = position,
                            localAngles = rotation,
                            localScale = scale
                        }
                    }
                }
            };

            return displayRule;
        }

        public static ItemDisplayRuleSet.KeyAssetRuleGroup CreateFollowerDisplayRule(Object keyAsset, string prefabName, Vector3 position, Vector3 rotation, Vector3 scale)
        {
            return CreateFollowerDisplayRule(keyAsset, LoadDisplay(prefabName), position, rotation, scale);
        }

        public static ItemDisplayRule[] CreateDisplayRule(string childName, GameObject displayPrefab, Vector3 position, Vector3 rotation, Vector3 scale)
        {
            return new ItemDisplayRule[]
            {
                new ItemDisplayRule
                {
                    childName = childName,
                    followerPrefab = displayPrefab,
                    limbMask = LimbFlags.None,
                    localPos = position,
                    localAngles = rotation,
                    localScale = scale,
                    ruleType = ItemDisplayRuleType.ParentedPrefab
                }
            };
        }


        public static GameObject LoadDisplay(string name)
        {
            string key = name.ToLower();
            if (itemDisplayPrefabs.ContainsKey(key))
            {
                if (itemDisplayPrefabs[key]) return itemDisplayPrefabs[key];
            }
            Debug.LogWarning("could not find item display prefab for " + name);
            return null;
        }

        /// <summary>
        /// Method for loading a gameobject via addressables
        /// </summary>
        /// <param name="path">The path of the addressableRoR2/DLC1/MushroomVoid/DisplayMushroomVoid.prefab</param>
        /// <returns></returns>
        public static GameObject LoadAddressable(string path)
        {
            return Addressables.LoadAssetAsync<GameObject>(path).WaitForCompletion();
        }

        private static void PopulateDisplays()
        {
            //PopulateFromBody("Commando");
            //PopulateFromBody("Croco");
            //PopulateFromBody("Mage");
            foreach (var item in itemDisplayPaths)
            {
                var loadedObject = LoadAddressable(item.Value);
                if (!loadedObject)
                {
                    Debug.LogError($"[SS2U]ItemDisplayCore.LoadAddressable:: Issue loading path for \"{item.Key}\" with path \"{item.Value}\".");
                    continue;
                }
                itemDisplayPrefabs.Add(item.Key, loadedObject);
            }
        }
        private static void PopulateFromBody(string bodyName)
        {
            ItemDisplayRuleSet itemDisplayRuleSet = LegacyResourcesAPI.Load<GameObject>("Prefabs/CharacterBodies/" + bodyName + "Body").GetComponent<ModelLocator>().modelTransform.GetComponent<CharacterModel>().itemDisplayRuleSet;

            ItemDisplayRuleSet.KeyAssetRuleGroup[] item = itemDisplayRuleSet.keyAssetRuleGroups;

            for (int i = 0; i < item.Length; i++)
            {
                ItemDisplayRule[] rules = item[i].displayRuleGroup.rules;

                for (int j = 0; j < rules.Length; j++)
                {
                    GameObject followerPrefab = rules[j].followerPrefab;
                    if (followerPrefab)
                    {
                        string name = followerPrefab.name;
                        string key = name?.ToLower();
                        if (!itemDisplayPrefabs.ContainsKey(key))
                        {
                            itemDisplayPrefabs[key] = followerPrefab;
                        }
                    }
                }
            }
        }

        [MethodImpl(MethodImplOptions.NoInlining | MethodImplOptions.NoOptimization)]
        public static ItemDef LoadScepterObject()
        {
            return AncientScepter.AncientScepterItem.instance.ItemDef;
        }
    }
}