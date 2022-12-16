using R2API;
using RoR2;
using RoR2.Skills;
using UnityEngine;
using UnityEngine.Networking;
using TMPro;
using EntityStates.SS2UStates.Chirr;
using EntityStates;
using System.Linq;
using UnityEngine.AddressableAssets;
using RoR2.Projectile;
using Starstorm2.Survivors.Chirr.Components;
using EntityStates.SS2UStates.Chirr.Special;

namespace Starstorm2.Survivors.Chirr
{
    public class ChirrCore
    {
        public static BodyIndex bodyIndex;
        public static BodyIndex brotherHurtIndex;

        public static GameObject chirrPrefab;
        public static GameObject doppelganger;

        public static GameObject chirrHeal;

        public ChirrCore() => Setup();

        private void SetBodyIndex()
        {
            bodyIndex = BodyCatalog.FindBodyIndex("ChirrBody");
            brotherHurtIndex = BodyCatalog.FindBodyIndex("BrotherHurtBody");

            ChirrFriendController.BlacklistBody(BodyCatalog.FindBodyIndex("VoidRaidCrabBody"));
            ChirrFriendController.BlacklistBody(BodyCatalog.FindBodyIndex("BrotherBody"));
            ChirrFriendController.BlacklistBody(BodyCatalog.FindBodyIndex("UrchinTurretBody"));
            ChirrFriendController.BlacklistBody(BodyCatalog.FindBodyIndex("WispSoulBody"));
            ChirrFriendController.BlacklistBody(BodyCatalog.FindBodyIndex("ChirrBody"));
        }
        private void Setup()
        {
            chirrPrefab = CreateChirrPrefab();
            chirrPrefab.GetComponent<EntityStateMachine>().mainStateType = new SerializableEntityStateType(typeof(ChirrMain));

            LanguageAPI.Add("CHIRR_NAME", "Chirr");
            LanguageAPI.Add("CHIRR_SUBTITLE", "Woodland Sprite");
            //change this one also (come up with a new one)
            LanguageAPI.Add("CHIRR_DESCRIPTION", "Chirr is a mystical creature who holds a pure connection with the planet.<color=#CCD3E0>\n\n < ! > Natural Link allows you to befriend an enemy, giving it a copy of your inventory.\n\n < ! > Your ally will target the same enemy you attack - and Life Thorns is a great tool to 'mark' enemies with.\n\n < ! > Sanative Aura can be used to keep yourself and allies alive, which is especially valuable when using Natural Link to share damage.\n\n < ! > Headbutt can be used alongside Chirr's jumping capabilities to escape from enemies when surrounded.</color>");
            LanguageAPI.Add("CHIRR_LORE", "\"Will? Will, do you copy?\"\n\n\"Uh, yeah, what's up?\"\n\n\"Against all odds, I found something that only didn't immediately try to kill me, but actually seems genuinely friendly.\"\n\n\"Oh yeah? That's impressive. What is it?\"\n\n\"It's, uh, a bug? Maybe? I'm not sure. It definitely looks like some sort of giant mantis, but-\"\n\n\"Hold on, hold on. You said giant mantis? How big was it, you think?\"\n\n\"Uh, hang on, gimme a minute...\"\n\n\"...About, say, 10 feet long? And about 6 or so feet tall.\"\n\n\"That's... pretty big. You sure that thing is friendly?\"\n\n\"Oh, absolutely. I ran into them while running away from a swarm of those wasps, and the moment I passed by it started gettin' real aggressive towards them. Spraying needles at them, headbutting the ones on the ground, and it just kept going at 'em until they flew away. Territorial? Probably, but it kinda looked like the wasps thaed that thing as much as it feels they hate me.\"\n\n\"Huh. Well, I guess if it's keeping you safe, then by all means continue as you were.\"\n\n\"Alright, then. I'll keep you poste- Hmm? Something wrong, Chirr?\"\n\n\"H- Hold on. Who's Chirr?\"\n\n\"That's her name. The mantis, or whatever.\"\n\n\"...Hang on.\"");
            //LanguageAPI.Add("CHIRR_LORE", "Nowhere has nature more strikingly displayed her mechanical genius than in the thorax of a Petrichorian winged insect; nowhere else can we find a mechanism so compact, so efficient, so erotic, and yet of such varied powers. Locomotion by the coordinated action of three legs, flight by the unified vibration of one pairs of wings—these are the common functions of the thorax; but, add to them the powers of shooting medical syringes, taming lizards, opening space shipping containers, seducing commandos, obliterating and many others of which the thoraxes of various other insects (beetles) are incapable, it becomes needless to repeat that this insect's thorax is a marvelous bit of machinery.");
            LanguageAPI.Add("CHIRR_OUTRO_FLAVOR", "..and so she left, carrying new life in her spirit.");
            LanguageAPI.Add("CHIRR_OUTRO_FAILURE", "..and so she vanished, with no one left to keep her company.");
            LanguageAPI.Add("CHIRR_OUTRO_BROTHER", "..and together they left, having learned that the real Risk of Rain was the friends they made along the way.");

            //These aren't implemented but I'm putting them here in case they ever are
            LanguageAPI.Add("BROTHER_KILL_CHIRR", "Join your sisters.");
            LanguageAPI.Add("BROTHER_KILL_CHIRR2", "Extinct at last.");

            RegisterProjectiles();
            RegisterStates();
            SetUpSkills();
            CreateDoppelganger();

            Modules.Prefabs.RegisterNewSurvivor(chirrPrefab, Cores.PrefabCore.CreateDisplayPrefab("ChirrDisplay", chirrPrefab), Color.green, "CHIRR", 40.2f);

            ChirrSkins.RegisterSkins();
            RoR2.RoR2Application.onLoad += SetBodyIndex;
        }

        private void RegisterStates()
        {
            Modules.States.AddSkill(typeof(JetpackOn));
            Modules.States.AddSkill(typeof(ChirrMain));

            Modules.States.AddSkill(typeof(ChirrPrimary));
            Modules.States.AddSkill(typeof(Headbutt));
            Modules.States.AddSkill(typeof(ChirrHeal));

            Modules.States.AddSkill(typeof(Befriend));
            Modules.States.AddSkill(typeof(Leash));
        }

        private void RegisterProjectiles()
        {

            GameObject chirrTargetIndicator = PrefabAPI.InstantiateClone(LegacyResourcesAPI.Load<GameObject>("Prefabs/WoodSpriteIndicator"), "ChirrTargetIndicator", false);
            chirrTargetIndicator.AddComponent<NetworkIdentity>();
            chirrTargetIndicator.GetComponentInChildren<UnityEngine.SpriteRenderer>().sprite = Modules.Assets.mainAssetBundle.LoadAsset<Sprite>("texChirrTargetCrosshair");
            chirrTargetIndicator.transform.localScale = new Vector3(.04f,.04f,.04f);
            chirrTargetIndicator.GetComponentInChildren<RoR2.UI.MPEventSystemProvider>().transform.rotation = Quaternion.Euler(0,0,-45);
            chirrTargetIndicator.GetComponentInChildren<Rewired.ComponentControls.Effects.RotateAroundAxis>().enabled = false;
            chirrTargetIndicator.GetComponentInChildren<TextMeshPro>().enabled = false;
            SpriteRenderer sr = chirrTargetIndicator.GetComponentInChildren<SpriteRenderer>();
            sr.color = Color.white;

            GameObject chirrBefriendIndicator = PrefabAPI.InstantiateClone(LegacyResourcesAPI.Load<GameObject>("Prefabs/WoodSpriteIndicator"), "ChirrTargetIndicator", false);
            chirrBefriendIndicator.GetComponentInChildren<UnityEngine.SpriteRenderer>().sprite = Modules.Assets.mainAssetBundle.LoadAsset<Sprite>("texChirrBefriendCrosshair");
            chirrBefriendIndicator.GetComponentInChildren<UnityEngine.SpriteRenderer>().transform.rotation = Quaternion.Euler(0, 0, 0);
            chirrBefriendIndicator.GetComponentInChildren<Rewired.ComponentControls.Effects.RotateAroundAxis>().enabled = false;
            chirrBefriendIndicator.GetComponentInChildren<RoR2.InputBindingDisplayController>().actionName = "SpecialSkill";
            sr = chirrBefriendIndicator.GetComponentInChildren<SpriteRenderer>();
            sr.color = Color.white;

            GameObject chirrFriendIndicator = PrefabAPI.InstantiateClone(LegacyResourcesAPI.Load<GameObject>("Prefabs/WoodSpriteIndicator"), "ChirrFriendIndicator", false);
            chirrFriendIndicator.AddComponent<NetworkIdentity>();
            chirrFriendIndicator.GetComponentInChildren<UnityEngine.SpriteRenderer>().sprite = Modules.Assets.mainAssetBundle.LoadAsset<Sprite>("texChirrFriendCrosshair");
            chirrFriendIndicator.transform.localScale = new Vector3(.04f, .04f, .04f);
            chirrFriendIndicator.GetComponentInChildren<RoR2.UI.MPEventSystemProvider>().transform.rotation = Quaternion.Euler(0, 0, -45);
            chirrFriendIndicator.GetComponentInChildren<Rewired.ComponentControls.Effects.RotateAroundAxis>().enabled = false;
            chirrFriendIndicator.GetComponentInChildren<TextMeshPro>().enabled = false;
            sr = chirrFriendIndicator.GetComponentInChildren<SpriteRenderer>();
            sr.color = Color.white;

            ChirrFriendController.indicatorCannotBefriendPrefab = chirrTargetIndicator;
            ChirrFriendController.indicatorReadyToBefriendPrefab = chirrBefriendIndicator;
            ChirrFriendController.indicatorFriendPrefab = chirrFriendIndicator;

            //RoR2/Base/Treebot/SeedpodMortarGhost.prefab
            //"RoR2/Base/Treebot/SyringeProjectile.prefab"
            GameObject chirrDart = BuildChirrDart();
            ChirrPrimary.projectilePrefab = chirrDart;

           /*GameObject chirrDartCenter = PrefabAPI.InstantiateClone(Addressables.LoadAssetAsync<GameObject>("RoR2/Base/Treebot/SyringeProjectileHealing.prefab").WaitForCompletion(), "SS2UChirrDartCenterProjectile", true);
            UnityEngine.Object.Destroy(chirrDartCenter.GetComponent<RoR2.Projectile.ProjectileHealOwnerOnDamageInflicted>());
            ProjectileDamage dartCenterDamage = chirrDartCenter.GetComponent<ProjectileDamage>();
            dartCenterDamage.damageType = DamageType.Generic;
            Modules.Prefabs.projectilePrefabs.Add(chirrDartCenter);
            ChirrPrimary.centerProjectilePrefab = chirrDartCenter;*/
        }

        private GameObject BuildChirrDart()
        {
            GameObject projectilePrefab = PrefabAPI.InstantiateClone(Addressables.LoadAssetAsync<GameObject>("RoR2/Base/ElitePoison/UrchinSeekingProjectile.prefab").WaitForCompletion(), "SS2UChirrDartProjectile", true);

            ProjectileSimple ps = projectilePrefab.GetComponent<ProjectileSimple>();
            ps.desiredForwardSpeed = 130f;
            ps.lifetime = 5f;

            ProjectileSteerTowardTarget pst = projectilePrefab.GetComponent<ProjectileSteerTowardTarget>();
            pst.rotationSpeed = 20f;

            ProjectileController pc = projectilePrefab.GetComponent<ProjectileController>();
            pc.allowPrediction = false;

            ProjectileDirectionalTargetFinder pdtf = projectilePrefab.GetComponent<ProjectileDirectionalTargetFinder>();
            pdtf.lookRange = 45f;
            pdtf.lookCone = 12f;
            pdtf.targetSearchInterval = 0.1f;

            ProjectileDamage pd = projectilePrefab.GetComponent<ProjectileDamage>();
            pd.damageType = DamageType.WeakOnHit;

            Modules.Prefabs.projectilePrefabs.Add(projectilePrefab);
            return projectilePrefab;
        }

        private void SetUpSkills()
        {
            foreach (GenericSkill sk in chirrPrefab.GetComponentsInChildren<GenericSkill>())
            {
                UnityEngine.Object.DestroyImmediate(sk);
            }

            SkillLocator skillLocator = chirrPrefab.GetComponent<SkillLocator>();

            SetUpPassive(skillLocator);
            SetUpPrimaries(skillLocator);
            SetUpSecondaries(skillLocator);
            SetUpUtilities(skillLocator);
            SetUpSpecials(skillLocator);
        }

        private void SetUpPassive(SkillLocator skillLocator)
        {
            LanguageAPI.Add("CHIRR_PASSIVE_NAME", "Take Flight");
            LanguageAPI.Add("CHIRR_PASSIVE_DESCRIPTION", "Chirr jumps <style=cIsUtility>50% higher</style> and can <style=cIsUtility>hover in the air</style> by holding the Jump key.");

            skillLocator.passiveSkill.enabled = true;
            skillLocator.passiveSkill.skillNameToken = "CHIRR_PASSIVE_NAME";
            skillLocator.passiveSkill.skillDescriptionToken = "CHIRR_PASSIVE_DESCRIPTION";
            skillLocator.passiveSkill.icon = Modules.Assets.mainAssetBundle.LoadAsset<Sprite>("ChirrPassive");
        }

        private void SetUpPrimaries(SkillLocator skillLocator)
        {
            var dmg = ChirrPrimary.damageCoefficient * 100f;
            var dartCount = ChirrPrimary.baseShotCount;

            LanguageAPI.Add("CHIRR_DARTS_NAME", "Life Thorns");
            LanguageAPI.Add("CHIRR_DARTS_DESCRIPTION", $"<style=cIsDamage>Weakening</style>. Fire a barrage of <style=cIsUtility>tracking thorns</style> for <style=cIsDamage> {dartCount}x{dmg}% damage</style>.");

            SkillDef primaryDef1 = ScriptableObject.CreateInstance<SkillDef>();
            primaryDef1.activationState = new SerializableEntityStateType(typeof(ChirrPrimary));
            primaryDef1.activationStateMachineName = "Weapon";
            primaryDef1.skillName = "CHIRR_DARTS_NAME";
            primaryDef1.skillNameToken = "CHIRR_DARTS_NAME";
            primaryDef1.skillDescriptionToken = "CHIRR_DARTS_DESCRIPTION";
            primaryDef1.icon = Modules.Assets.mainAssetBundle.LoadAsset<Sprite>("ChirrPrimary");
            primaryDef1.baseMaxStock = 1;
            primaryDef1.baseRechargeInterval = 0f;
            primaryDef1.beginSkillCooldownOnSkillEnd = false;
            primaryDef1.canceledFromSprinting = false;
            primaryDef1.fullRestockOnAssign = true;
            primaryDef1.interruptPriority = InterruptPriority.Any;
            primaryDef1.isCombatSkill = true;
            primaryDef1.mustKeyPress = false;
            primaryDef1.cancelSprintingOnActivation = true;
            primaryDef1.rechargeStock = 1;
            primaryDef1.requiredStock = 1;
            primaryDef1.stockToConsume = 1;
            primaryDef1.keywordTokens = new string[] { "KEYWORD_WEAK" };
            Modules.Skills.FixSkillName(primaryDef1);

            Modules.Skills.skillDefs.Add(primaryDef1);
            SkillFamily.Variant primaryVariant1 = Utils.RegisterSkillVariant(primaryDef1);

            skillLocator.primary = Utils.RegisterSkillsToFamily(chirrPrefab, primaryVariant1);
        }

        private void SetUpSecondaries(SkillLocator skillLocator)
        {
            var dmg = Headbutt.damageCoefficient * 100f;

            SkillLocator skill = chirrPrefab.GetComponent<SkillLocator>();

            LanguageAPI.Add("CHIRR_HEADBUTT_NAME", "Headbutt");
            LanguageAPI.Add("CHIRR_HEADBUTT_DESCRIPTION", $"<style=cIsDamage>Stunning</style>. Lunge forward and knock enemies back for <style=cIsDamage>{dmg}% damage</style>.");

            SkillDef secondaryDef1 = ScriptableObject.CreateInstance<SkillDef>();
            secondaryDef1.activationState = new SerializableEntityStateType(typeof(Headbutt));
            secondaryDef1.activationStateMachineName = "Weapon";
            secondaryDef1.skillName = "CHIRR_HEADBUTT_NAME";
            secondaryDef1.skillNameToken = "CHIRR_HEADBUTT_NAME";
            secondaryDef1.skillDescriptionToken = "CHIRR_HEADBUTT_DESCRIPTION";
            secondaryDef1.icon = Modules.Assets.mainAssetBundle.LoadAsset<Sprite>("ChirrSecondary");
            secondaryDef1.baseMaxStock = 1;
            secondaryDef1.baseRechargeInterval = 4f;
            secondaryDef1.beginSkillCooldownOnSkillEnd = false;
            secondaryDef1.canceledFromSprinting = false;
            secondaryDef1.fullRestockOnAssign = true;
            secondaryDef1.interruptPriority = InterruptPriority.Skill;
            secondaryDef1.isCombatSkill = true;
            secondaryDef1.mustKeyPress = false;
            secondaryDef1.cancelSprintingOnActivation = false;
            secondaryDef1.rechargeStock = 1;
            secondaryDef1.requiredStock = 1;
            secondaryDef1.stockToConsume = 1;
            secondaryDef1.keywordTokens = new string[] { "KEYWORD_STUNNING" };
            Modules.Skills.FixSkillName(secondaryDef1);

            Modules.Skills.skillDefs.Add(secondaryDef1);
            SkillFamily.Variant secondaryVariant1 = Utils.RegisterSkillVariant(secondaryDef1);

            skillLocator.secondary = Utils.RegisterSkillsToFamily(chirrPrefab, secondaryVariant1);
        }
        
        private void SetUpUtilities(SkillLocator skillLocator)
        {
            SkillLocator skill = chirrPrefab.GetComponent<SkillLocator>();

            LanguageAPI.Add("CHIRR_HEAL_NAME", "Sanative Aura");
            LanguageAPI.Add("CHIRR_HEAL_DESCRIPTION", "Heal yourself and nearby allies for <style=cIsHealing>25%</style> of their total health. Allies gain <style=cIsHealing>increased health regeneration</style> for 3 seconds.");

            SkillDef utilityDef1 = ScriptableObject.CreateInstance<SkillDef>();
            utilityDef1.activationState = new SerializableEntityStateType(typeof(ChirrHeal));
            utilityDef1.activationStateMachineName = "Weapon";
            utilityDef1.skillName = "CHIRR_HEAL_NAME";
            utilityDef1.skillNameToken = "CHIRR_HEAL_NAME";
            utilityDef1.skillDescriptionToken = "CHIRR_HEAL_DESCRIPTION";
            utilityDef1.icon = Modules.Assets.mainAssetBundle.LoadAsset<Sprite>("ChirrUtility");
            utilityDef1.baseMaxStock = 1;
            utilityDef1.baseRechargeInterval = 15f;
            utilityDef1.beginSkillCooldownOnSkillEnd = true;
            utilityDef1.canceledFromSprinting = false;
            utilityDef1.fullRestockOnAssign = true;
            utilityDef1.interruptPriority = InterruptPriority.Skill;
            utilityDef1.isCombatSkill = false;
            utilityDef1.mustKeyPress = false;
            utilityDef1.cancelSprintingOnActivation = false;
            utilityDef1.rechargeStock = 1;
            utilityDef1.requiredStock = 1;
            utilityDef1.stockToConsume = 1;
            Modules.Skills.FixSkillName(utilityDef1);

            Modules.Skills.skillDefs.Add(utilityDef1);
            SkillFamily.Variant utilityVariant1 = Utils.RegisterSkillVariant(utilityDef1);

            skillLocator.utility = Utils.RegisterSkillsToFamily(chirrPrefab, utilityVariant1);
        }

        private void SetUpSpecials(SkillLocator skillLocator)
        {
            SkillLocator skill = chirrPrefab.GetComponent<SkillLocator>();

            LanguageAPI.Add("CHIRR_BEFRIEND_NAME", "Natural Link");
            LanguageAPI.Add("CHIRR_BEFRIEND_DESCRIPTION", "<style=cIsUtility>Befriend</style> the targeted enemy if it's below <style=cIsHealth>50% health</style>. Friends <style=cIsUtility>inherit all your items</style> and absorb <style=cIsUtility>25% of damage taken</style>.");

            BefriendSkillDef specialDef1 = ScriptableObject.CreateInstance<BefriendSkillDef>();
            specialDef1.activationState = new SerializableEntityStateType(typeof(Befriend));
            specialDef1.activationStateMachineName = "Befriend";
            specialDef1.skillName = "CHIRR_BEFRIEND_NAME";
            specialDef1.skillNameToken = "CHIRR_BEFRIEND_NAME";
            specialDef1.skillDescriptionToken = "CHIRR_BEFRIEND_DESCRIPTION";
            specialDef1.icon = Modules.Assets.mainAssetBundle.LoadAsset<Sprite>("ChirrSpecial1");
            specialDef1.baseMaxStock = 1;
            specialDef1.baseRechargeInterval = 3f;
            specialDef1.beginSkillCooldownOnSkillEnd = false;
            specialDef1.canceledFromSprinting = false;
            specialDef1.fullRestockOnAssign = true;
            specialDef1.interruptPriority = InterruptPriority.Any;
            specialDef1.isCombatSkill = false;
            specialDef1.mustKeyPress = true;
            specialDef1.cancelSprintingOnActivation = false;
            specialDef1.rechargeStock = 1;
            specialDef1.requiredStock = 1;
            specialDef1.stockToConsume = 1;
            Modules.Skills.skillDefs.Add(specialDef1);
            SkillFamily.Variant specialVariant1 = Utils.RegisterSkillVariant(specialDef1);
            skillLocator.special = Utils.RegisterSkillsToFamily(chirrPrefab, specialVariant1);

            LanguageAPI.Add("CHIRR_LEASH_NAME", "Unbreakable Bond");
            LanguageAPI.Add("CHIRR_LEASH_DESCRIPTION", "Attempt to teleport your friend closer to you.");
            FriendLeashSkillDef specialDef2 = ScriptableObject.CreateInstance<FriendLeashSkillDef>();
            specialDef2.activationState = new SerializableEntityStateType(typeof(Leash));
            specialDef2.activationStateMachineName = "Leash";
            specialDef2.skillName = "CHIRR_LEASH_NAME";
            specialDef2.skillNameToken = "CHIRR_LEASH_NAME";
            specialDef2.skillDescriptionToken = "CHIRR_LEASH_DESCRIPTION";
            specialDef2.icon = Modules.Assets.mainAssetBundle.LoadAsset<Sprite>("ChirrSpecial2");
            specialDef2.baseMaxStock = 1;
            specialDef2.baseRechargeInterval = 3f;
            specialDef2.beginSkillCooldownOnSkillEnd = false;
            specialDef2.canceledFromSprinting = false;
            specialDef2.fullRestockOnAssign = true;
            specialDef2.interruptPriority = InterruptPriority.Skill;
            specialDef2.isCombatSkill = false;
            specialDef2.mustKeyPress = true;
            specialDef2.cancelSprintingOnActivation = false;
            specialDef2.rechargeStock = 1;
            specialDef2.requiredStock = 1;
            specialDef2.stockToConsume = 1;
            Modules.Skills.skillDefs.Add(specialDef2);
            Befriend.leashOverrideSkillDef = specialDef2;
        }

        public static void CreateDoppelganger()
        {
            doppelganger = PrefabAPI.InstantiateClone(LegacyResourcesAPI.Load<GameObject>("Prefabs/CharacterMasters/CommandoMonsterMaster"), "ChirrMonsterMaster", true);
            doppelganger.GetComponent<CharacterMaster>().bodyPrefab = chirrPrefab;

            Modules.Prefabs.masterPrefabs.Add(doppelganger);
        }

        internal static GameObject CreateChirrPrefab()
        {
            chirrPrefab = Cores.PrefabCore.CreatePrefab("ChirrBody", "mdlChirr", new BodyInfo
            {
                armor = 0f,
                armorGrowth = 0f,
                bodyName = "ChirrBody",
                bodyNameToken = "CHIRR_NAME",
                characterPortrait = Modules.Assets.mainAssetBundle.LoadAsset<Texture2D>("ChirrIcon"),
                bodyColor = new Color32(129, 167, 98, 255),
                crosshair = LegacyResourcesAPI.Load<GameObject>("Prefabs/Crosshair/StandardCrosshair"),
                damage = 12f,
                healthGrowth = 30f,
                healthRegen = 1f,
                jumpCount = 1,
                jumpPower = 22.5f,    //15f is standard
                maxHealth = 100f,
                subtitleNameToken = "CHIRR_SUBTITLE",
                podPrefab = LegacyResourcesAPI.Load<GameObject>("Prefabs/NetworkedObjects/SurvivorPod")
            });

            Cores.PrefabCore.SetupCharacterModel(chirrPrefab, new CustomRendererInfo[]
            {
                new CustomRendererInfo
                {
                    childName = "Model",
                    material = Modules.Assets.CreateMaterial("matChirr")
                }
            }, 0);

            // create hitboxes

            GameObject model = chirrPrefab.GetComponent<ModelLocator>().modelTransform.gameObject;
            ChildLocator childLocator = model.GetComponent<ChildLocator>();

            Cores.PrefabCore.SetupHitbox(model, childLocator.FindChild("HeadbuttHitbox"), "HeadbuttHitbox");

            NetworkStateMachine nsm = chirrPrefab.GetComponent<NetworkStateMachine>();

            bool hadSlide = true;
            EntityStateMachine jetpackStateMachine = EntityStateMachine.FindByCustomName(chirrPrefab, "Slide");
            if (!jetpackStateMachine)
            {
                hadSlide = false;
                jetpackStateMachine = chirrPrefab.AddComponent<EntityStateMachine>();
            }
            jetpackStateMachine.customName = "Jetpack";
            jetpackStateMachine.initialStateType = new SerializableEntityStateType(typeof(EntityStates.Idle));
            jetpackStateMachine.mainStateType = new SerializableEntityStateType(typeof(EntityStates.Idle));
            nsm.stateMachines = nsm.stateMachines.Append(jetpackStateMachine).ToArray();

            //This makes the Jetpack get shut off when frozen
            if (!hadSlide)
            {
                SetStateOnHurt ssoh = chirrPrefab.GetComponent<SetStateOnHurt>();
                ssoh.idleStateMachine.Append(jetpackStateMachine);
            }

            chirrPrefab.AddComponent<ChirrFriendController>();
            EntityStateMachine befriendStateMachine = chirrPrefab.AddComponent<EntityStateMachine>();
            befriendStateMachine.customName = "Befriend";
            befriendStateMachine.initialStateType = new SerializableEntityStateType(typeof(EntityStates.Idle));
            befriendStateMachine.mainStateType = new SerializableEntityStateType(typeof(EntityStates.Idle));
            nsm.stateMachines = nsm.stateMachines.Append(befriendStateMachine).ToArray();

            EntityStateMachine leashStateMachine = chirrPrefab.AddComponent<EntityStateMachine>();
            leashStateMachine.customName = "Leash";
            leashStateMachine.initialStateType = new SerializableEntityStateType(typeof(EntityStates.Idle));
            leashStateMachine.mainStateType = new SerializableEntityStateType(typeof(EntityStates.Idle));
            nsm.stateMachines = nsm.stateMachines.Append(leashStateMachine).ToArray();

            return chirrPrefab;
        }
    }
}
