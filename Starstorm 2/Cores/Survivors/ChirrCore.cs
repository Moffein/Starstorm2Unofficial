using R2API;
using RoR2;
using RoR2.Skills;
using UnityEngine;
using UnityEngine.Networking;
using EntityStates.Chirr;
using Starstorm2.Cores.States.Chirr;
using TMPro;

namespace Starstorm2.Cores
{
    public class ChirrCore
    {
        public static GameObject chirrPrefab;
        public static GameObject doppelganger;

        public static SkillDef specialDef1;
        public static SkillDef specialDef2;

        public static GameObject chirrDart;
        public static GameObject chirrHeal;
        public static GameObject chirrTargetIndicator;
        public static GameObject chirrBefriendIndicator;

        public ChirrCore() => Setup();

        private void Setup()
        {
            chirrPrefab = PrefabCore.CreateChirrPrefab();
            chirrPrefab.GetComponent<EntityStateMachine>().mainStateType = new EntityStates.SerializableEntityStateType(typeof(ChirrMain));

            LanguageAPI.Add("CHIRR_NAME", "Chirr");
            LanguageAPI.Add("CHIRR_SUBTITLE", "Supreme Bug of Comf");
            //change this one also (come up with a new one)
            LanguageAPI.Add("CHIRR_DESCRIPTION", "Chirr is a mystical creature who holds a pure connection with the planet.<color=#CCD3E0>\n\n < ! > Natural Link allows you to befriend an enemy, giving it a copy of your inventory.\n\n < ! > Your ally will target the same enemy you attack - and Life Thorns is a great tool to 'mark' enemies with.\n\n < ! > Sanative Aura can be used to keep yourself and allies alive, which is especially valuable when using Natural Link to share damage.\n\n < ! > Headbutt can be used alongside Chirr's jumping capabilities to escape from enemies when surrounded.</color>");
            //LanguageAPI.Add("CHIRR_LORE", "\"Will? Will, do you copy?\"\n\n\"Uh, yeah, what's up?\"\n\n\"Against all odds, I found something that only didn't immediately try to kill me, but actually seems genuinely friendly.\"\n\n\"Oh yeah? That's impressive. What is it?\"\n\n\"It's, uh, a bug? Maybe? I'm not sure. It definitely looks like some sort of giant mantis, but-\"\n\n\"Hold on, hold on. You said giant mantis? How big was it, you think?\"\n\n\"Uh, hang on, gimme a minute...\"\n\n\"...About, say, 10 feet long? And about 6 or so feet tall.\"\n\n\"That's... pretty big. You sure that thing is friendly?\"\n\n\"Oh, absolutely. I ran into them while running away from a swarm of those wasps, and the moment I passed by it started gettin' real aggressive towards them. Spraying needles at them, headbutting the ones on the ground, and it just kept going at 'em until they flew away. Territorial? Probably, but it kinda looked like the wasps thaed that thing as much as it feels they hate me.\"\n\n\"Huh. Well, I guess if it's keeping you safe, then by all means continue as you were.\"\n\n\"Alright, then. I'll keep you poste- Hmm? Something wrong, Chirr?\"\n\n\"H- Hold on. Who's Chirr?\"\n\n\"That's her name. The mantis, or whatever.\"\n\n\"...Hang on.\"");
            LanguageAPI.Add("CHIRR_LORE", "Nowhere has nature more strikingly displayed her mechanical genius than in the thorax of a Petrichorian winged insect; nowhere else can we find a mechanism so compact, so efficient, so erotic, and yet of such varied powers. Locomotion by the coordinated action of three legs, flight by the unified vibration of one pairs of wings—these are the common functions of the thorax; but, add to them the powers of shooting medical syringes, taming lizards, opening space shipping containers, seducing commandos, obliterating and many others of which the thoraxes of various other insects (beetles) are incapable, it becomes needless to repeat that this insect's thorax is a marvelous bit of machinery.");
            LanguageAPI.Add("CHIRR_OUTRO_FLAVOR", "..and so she left, carrying new life in her spirit.");
            //"CHIRR_OUTRO_FAILURE": "..and so she vanished, with no one left to keep her company.",
            //actual line but leave it as the one below for a bit (it's funnier)
            LanguageAPI.Add("CHIRR_OUTRO_FAILURE", "..and so she died.");

            //These aren't implemented but I'm putting them here in case they ever are
            LanguageAPI.Add("BROTHER_KILL_CHIRR", "Join your sisters.");
            LanguageAPI.Add("BROTHER_KILL_CHIRR2", "Extinct at last.");

            RegisterProjectiles();
            RegisterStates();
            SetUpSkills();
            RegisterHooks();
            CreateDoppelganger();

            Modules.Prefabs.RegisterNewSurvivor(chirrPrefab, PrefabCore.chirrDisplayPrefab, Color.green, "CHIRR");
        }

        private void RegisterStates()
        {
            Modules.States.AddSkill(typeof(ChirrMain));
            Modules.States.AddSkill(typeof(ChirrFireDarts));
            Modules.States.AddSkill(typeof(ChirrHeadbutt));
            Modules.States.AddSkill(typeof(ChirrHeal));
            Modules.States.AddSkill(typeof(ChirrBefriend));
            Modules.States.AddSkill(typeof(ChirrLeash));
        }

        private void RegisterHooks()
        {
            On.RoR2.HealthComponent.TakeDamage += (orig, self, damageInfo) =>
            {
                if (self.body.GetComponent<ChirrInfoComponent>())
                {
                    if (damageInfo.damage != 0 && self.body.GetComponent<ChirrInfoComponent>().friend && self.body.GetComponent<ChirrInfoComponent>().sharing)
                    {
                        damageInfo.damage *= .75f;
                        self.body.GetComponent<ChirrInfoComponent>().friend.healthComponent.TakeDamage(damageInfo);
                        damageInfo.damage /= 3f;
                        orig(self, damageInfo);
                    }
                    else
                        orig(self, damageInfo);
                }
                else
                    orig(self, damageInfo);
            };
        }

        private void RegisterProjectiles()
        {
            chirrDart = PrefabAPI.InstantiateClone(LegacyResourcesAPI.Load<GameObject>("prefabs/projectiles/SyringeProjectile"), "Prefabs/Projectiles/ChirrDart", true);

            //chirrHeal = PrefabAPI.InstantiateClone(LegacyResourcesAPI.Load<GameObject>("prefabs/effects/TPHealNovaEffect.prefab"), "Prefabs/Projectiles/ChirrHeal", true);
            /*bfgProjectile.GetComponent<ProjectileController>().procCoefficient = 1f;
            //bfgProjectile.GetComponent<ProjectileController>().catalogIndex = 53;
            bfgProjectile.GetComponent<ProjectileController>().ghostPrefab = LegacyResourcesAPI.Load<GameObject>("Prefabs/ProjectileGhosts/BeamSphereGhost");
            bfgProjectile.GetComponent<ProjectileDamage>().damage = 1f;
            bfgProjectile.GetComponent<ProjectileDamage>().damageType = DamageType.Generic;
            bfgProjectile.GetComponent<ProjectileDamage>().damageColorIndex = DamageColorIndex.Default;
            bfgProjectile.GetComponent<ProjectileSimple>().velocity = 13;
            bfgProjectile.GetComponent<ProjectileSimple>().lifetime = 4;
            bfgProjectile.AddComponent<ProjectileProximityBeamController>();
            bfgProjectile.GetComponent<ProjectileProximityBeamController>().attackRange = 8;
            bfgProjectile.GetComponent<ProjectileProximityBeamController>().listClearInterval = .2f;
            bfgProjectile.GetComponent<ProjectileProximityBeamController>().attackInterval = .2f;
            bfgProjectile.GetComponent<ProjectileProximityBeamController>().damageCoefficient = 0.8f;
            bfgProjectile.GetComponent<ProjectileProximityBeamController>().procCoefficient = .2f;
            bfgProjectile.GetComponent<ProjectileProximityBeamController>().inheritDamageType = true;
            bfgProjectile.AddComponent<RadialForce>();
            bfgProjectile.GetComponent<RadialForce>().radius = 18;
            bfgProjectile.GetComponent<RadialForce>().damping = 0.5f;
            bfgProjectile.GetComponent<RadialForce>().forceMagnitude = -1500;
            bfgProjectile.GetComponent<RadialForce>().forceCoefficientAtEdge = 0.5f;
            bfgProjectile.AddComponent<ProjectileImpactExplosion>();
            bfgProjectile.GetComponent<ProjectileImpactExplosion>().impactEffect = LegacyResourcesAPI.Load<GameObject>("Prefabs/Effects/BeamSphereExplosion");
            bfgProjectile.GetComponent<ProjectileImpactExplosion>().destroyOnEnemy = true;
            bfgProjectile.GetComponent<ProjectileImpactExplosion>().destroyOnWorld = true;
            bfgProjectile.GetComponent<ProjectileImpactExplosion>().timerAfterImpact = false;
            bfgProjectile.GetComponent<ProjectileImpactExplosion>().falloffModel = BlastAttack.FalloffModel.SweetSpot;
            bfgProjectile.GetComponent<ProjectileImpactExplosion>().lifetime = 3;
            bfgProjectile.GetComponent<ProjectileImpactExplosion>().lifetimeAfterImpact = 0;
            bfgProjectile.GetComponent<ProjectileImpactExplosion>().lifetimeRandomOffset = 0;
            bfgProjectile.GetComponent<ProjectileImpactExplosion>().blastRadius = 20;
            bfgProjectile.GetComponent<ProjectileImpactExplosion>().blastDamageCoefficient = 12f;
            bfgProjectile.GetComponent<ProjectileImpactExplosion>().blastProcCoefficient = 1;
            bfgProjectile.GetComponent<ProjectileImpactExplosion>().blastAttackerFiltering = AttackerFiltering.Default;
            bfgProjectile.GetComponent<ProjectileOverlapAttack>().enabled = false;

            //bfgProjectile.GetComponent<ProjectileProximityBeamController>().enabled = false;

            chirrPylon = PrefabAPI.InstantiateClone(LegacyResourcesAPI.Load<GameObject>("Prefabs/Projectiles/EngiGrenadeProjectile"), "Prefabs/Projectiles/ChirrTPPylon", true);

            GameObject ghost = AssetsCore.mainAssetBundle.LoadAsset<GameObject>("chirrTeleGhost2");
            ghost.AddComponent<ProjectileGhostController>();
            chirrPylon.GetComponent<ProjectileController>().ghostPrefab = ghost;
            chirrPylon.GetComponent<ProjectileSimple>().lifetime = 2147483646;
            chirrPylon.GetComponent<ProjectileImpactExplosion>().lifetime = 2147483646;
            chirrPylon.GetComponent<ProjectileImpactExplosion>().lifetimeAfterImpact = 2147483646;
            chirrPylon.GetComponent<ProjectileImpactExplosion>().destroyOnEnemy = false;
            //chirrPylon.GetComponent<ProjectileImpactExplosion>().falloffModel = BlastAttack.FalloffModel.SweetSpot;
            chirrPylon.GetComponent<Rigidbody>().drag = 2;
            chirrPylon.GetComponent<Rigidbody>().angularDrag = 2f;
            chirrPylon.AddComponent<AntiGravityForce>();
            chirrPylon.GetComponent<AntiGravityForce>().rb = chirrPylon.GetComponent<Rigidbody>();
            chirrPylon.GetComponent<AntiGravityForce>().antiGravityCoefficient = 1;

            // register it for networking
            if (bfgProjectile) PrefabAPI.RegisterNetworkPrefab(bfgProjectile);*/
            if (chirrDart) PrefabAPI.RegisterNetworkPrefab(chirrDart);

            chirrTargetIndicator = PrefabAPI.InstantiateClone(LegacyResourcesAPI.Load<GameObject>("Prefabs/WoodSpriteIndicator"), "ChirrTargetIndicator", true);
            chirrTargetIndicator.AddComponent<NetworkIdentity>();
            chirrTargetIndicator.GetComponentInChildren<UnityEngine.SpriteRenderer>().sprite = Modules.Assets.mainAssetBundle.LoadAsset<Sprite>("texChirrTargetCrosshair");
            chirrTargetIndicator.transform.localScale = new Vector3(.04f,.04f,.04f);
            chirrTargetIndicator.GetComponentInChildren<RoR2.UI.MPEventSystemProvider>().transform.rotation = Quaternion.Euler(0,0,-45);
            chirrTargetIndicator.GetComponentInChildren<Rewired.ComponentControls.Effects.RotateAroundAxis>().enabled = false;
            chirrTargetIndicator.GetComponentInChildren<TextMeshPro>().enabled = false;

            chirrBefriendIndicator = PrefabAPI.InstantiateClone(LegacyResourcesAPI.Load<GameObject>("Prefabs/WoodSpriteIndicator"), "ChirrTargetIndicator", true);
            chirrBefriendIndicator.GetComponentInChildren<UnityEngine.SpriteRenderer>().sprite = Modules.Assets.mainAssetBundle.LoadAsset<Sprite>("texChirrBefriendCrosshair");
            chirrBefriendIndicator.GetComponentInChildren<UnityEngine.SpriteRenderer>().transform.rotation = Quaternion.Euler(0, 0, 0);
            chirrBefriendIndicator.GetComponentInChildren<Rewired.ComponentControls.Effects.RotateAroundAxis>().enabled = false;
            chirrBefriendIndicator.GetComponentInChildren<RoR2.InputBindingDisplayController>().actionName = "SpecialSkill";



            // add it to the projectile catalog or it won't work in multiplayer
            Modules.Prefabs.projectilePrefabs.Add(chirrDart);
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

        //FIXME: doesn't show ingame
        private void SetUpPassive(SkillLocator skillLocator)
        {
            LanguageAPI.Add("CHIRR_PASSIVE_NAME", "Wing Jump");
            LanguageAPI.Add("CHIRR_PASSIVE_DESCRIPTION", "Chirr can jump <style=cIsUtility>50% higher</style> than other survivors. Hold the jump button in midair to <style=cIsUtility>slow your descent</style>.");

            var passive = skillLocator.passiveSkill;
            passive.enabled = true;
            passive.skillNameToken = "CHIRR_PASSIVE_NAME";
            passive.skillDescriptionToken = "CHIRR_PASSIVE_DESCRIPTION";
            passive.icon = Modules.Assets.mainAssetBundle.LoadAsset<Sprite>("ChirrPassive");
        }

        private void SetUpPrimaries(SkillLocator skillLocator)
        {
            var dmg = ChirrFireDarts.damageCoefficient * 100f;

            LanguageAPI.Add("CHIRR_DARTS_NAME", "Life Thorns");
            LanguageAPI.Add("CHIRR_DARTS_DESCRIPTION", $"Fire a barrage of thorns for <style=cIsDamage> 3x{dmg}% damage</style>. <style=cIsDamage>Marking</style>.");

            SkillDef primaryDef1 = ScriptableObject.CreateInstance<SkillDef>();
            primaryDef1.activationState = new EntityStates.SerializableEntityStateType(typeof(ChirrFireDarts));
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
            primaryDef1.interruptPriority = EntityStates.InterruptPriority.Any;
            primaryDef1.isCombatSkill = true;
            primaryDef1.mustKeyPress = false;
            primaryDef1.cancelSprintingOnActivation = true;
            primaryDef1.rechargeStock = 1;
            primaryDef1.requiredStock = 1;
            primaryDef1.stockToConsume = 1;
            primaryDef1.keywordTokens = new string[] { "KEYWORD_MARKING" };

            Utils.RegisterSkillDef(primaryDef1, typeof(ChirrFireDarts));
            SkillFamily.Variant primaryVariant1 = Utils.RegisterSkillVariant(primaryDef1);

            skillLocator.primary = Utils.RegisterSkillsToFamily(chirrPrefab, primaryVariant1);
        }

        private void SetUpSecondaries(SkillLocator skillLocator)
        {
            var dmg = ChirrHeadbutt.damageCoefficient * 100f;

            SkillLocator skill = chirrPrefab.GetComponent<SkillLocator>();

            LanguageAPI.Add("CHIRR_HEADBUTT_NAME", "Headbutt");
            //LanguageAPI.Add("CYBORG_SECONDARY_AIMBOT_DESCRIPTION", $"Quickly fire three seeking shots at contenders in front for <style=cIsDamage>3x{dmg}% damage</style>. <style=cKeywordName>Stunning</style><style=cSub>.");
            LanguageAPI.Add("CHIRR_HEADBUTT_DESCRIPTION", $"Headbutt enemies in front of you for <style=cIsDamage>{dmg}% damage</style>. <style=cIsDamage>Stunning</style>.");

            SkillDef secondaryDef1 = ScriptableObject.CreateInstance<SkillDef>();
            secondaryDef1.activationState = new EntityStates.SerializableEntityStateType(typeof(ChirrHeadbutt));
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
            secondaryDef1.interruptPriority = EntityStates.InterruptPriority.Skill;
            secondaryDef1.isCombatSkill = true;
            secondaryDef1.mustKeyPress = false;
            secondaryDef1.cancelSprintingOnActivation = true;
            secondaryDef1.rechargeStock = 1;
            secondaryDef1.requiredStock = 1;
            secondaryDef1.stockToConsume = 1;
            secondaryDef1.keywordTokens = new string[] { "KEYWORD_STUNNING" };

            Utils.RegisterSkillDef(secondaryDef1, typeof(ChirrHeadbutt));
            SkillFamily.Variant secondaryVariant1 = Utils.RegisterSkillVariant(secondaryDef1);

            skillLocator.secondary = Utils.RegisterSkillsToFamily(chirrPrefab, secondaryVariant1);
        }
        
        private void SetUpUtilities(SkillLocator skillLocator)
        {

            

            //var dur = ExecutionerDash.debuffDuration;

            SkillLocator skill = chirrPrefab.GetComponent<SkillLocator>();

            LanguageAPI.Add("CHIRR_HEAL_NAME", "Sanative Aura");
            LanguageAPI.Add("CHIRR_HEAL_DESCRIPTION", "Heal yourself and nearby allies for <style=cIsHealing>15%</style> of their total health and <style=cIsHealing>20%</style> over time.");

            SkillDef utilityDef1 = ScriptableObject.CreateInstance<SkillDef>();
            utilityDef1.activationState = new EntityStates.SerializableEntityStateType(typeof(ChirrHeal));
            utilityDef1.activationStateMachineName = "Weapon";
            utilityDef1.skillName = "CHIRR_HEAL_NAME";
            utilityDef1.skillNameToken = "CHIRR_HEAL_NAME";
            utilityDef1.skillDescriptionToken = "CHIRR_HEAL_DESCRIPTION";
            utilityDef1.icon = Modules.Assets.mainAssetBundle.LoadAsset<Sprite>("ChirrUtility");
            utilityDef1.baseMaxStock = 1;
            utilityDef1.baseRechargeInterval = 14f;
            utilityDef1.beginSkillCooldownOnSkillEnd = false;
            utilityDef1.canceledFromSprinting = false;
            utilityDef1.fullRestockOnAssign = true;
            utilityDef1.interruptPriority = EntityStates.InterruptPriority.Skill;
            utilityDef1.isCombatSkill = true;
            utilityDef1.mustKeyPress = false;
            utilityDef1.cancelSprintingOnActivation = false;
            utilityDef1.rechargeStock = 1;
            utilityDef1.requiredStock = 1;
            utilityDef1.stockToConsume = 1;

            Utils.RegisterSkillDef(utilityDef1, typeof(ChirrHeal));
            SkillFamily.Variant utilityVariant1 = Utils.RegisterSkillVariant(utilityDef1);

            skillLocator.utility = Utils.RegisterSkillsToFamily(chirrPrefab, utilityVariant1);
        }

        private void SetUpSpecials(SkillLocator skillLocator)
        {
            SkillLocator skill = chirrPrefab.GetComponent<SkillLocator>();

            LanguageAPI.Add("CHIRR_BEFRIEND_NAME", "Natural Link");
            LanguageAPI.Add("CHIRR_BEFRIEND_DESCRIPTION", "Befriend an enemy under 50% health. Befriended enemies inherit items.");
            specialDef1 = ScriptableObject.CreateInstance<SkillDef>();
            specialDef1.activationState = new EntityStates.SerializableEntityStateType(typeof(ChirrBefriend));
            specialDef1.activationStateMachineName = "Weapon";
            specialDef1.skillName = "CHIRR_BEFRIEND_NAME";
            specialDef1.skillNameToken = "CHIRR_BEFRIEND_NAME";
            specialDef1.skillDescriptionToken = "CHIRR_BEFRIEND_DESCRIPTION";
            specialDef1.icon = Modules.Assets.mainAssetBundle.LoadAsset<Sprite>("ChirrSpecial1");
            specialDef1.baseMaxStock = 1;
            specialDef1.baseRechargeInterval = 3f;
            specialDef1.beginSkillCooldownOnSkillEnd = false;
            specialDef1.canceledFromSprinting = false;
            specialDef1.fullRestockOnAssign = true;
            specialDef1.interruptPriority = EntityStates.InterruptPriority.Skill;
            specialDef1.isCombatSkill = false;
            specialDef1.mustKeyPress = false;
            specialDef1.cancelSprintingOnActivation = false;
            specialDef1.rechargeStock = 1;
            specialDef1.requiredStock = 1;
            specialDef1.stockToConsume = 1;

            Utils.RegisterSkillDef(specialDef1, typeof(ChirrBefriend));
            SkillFamily.Variant specialVariant1 = Utils.RegisterSkillVariant(specialDef1);

            SkillLocator skill2 = chirrPrefab.GetComponent<SkillLocator>();

            LanguageAPI.Add("CHIRR_LEASH_NAME", "Natural Sync");
            LanguageAPI.Add("CHIRR_LEASH_DESCRIPTION", "Tap to being your ally to you. Hold to <style=cIsDamage>share damage taken</style> with your friend.");
            specialDef2 = ScriptableObject.CreateInstance<SkillDef>();
            specialDef2.activationState = new EntityStates.SerializableEntityStateType(typeof(ChirrLeash));
            specialDef2.activationStateMachineName = "Weapon";
            specialDef2.skillName = "CHIRR_LEASH_NAME";
            specialDef2.skillNameToken = "CHIRR_LEASH_NAME";
            specialDef2.skillDescriptionToken = "CHIRR_LEASH_DESCRIPTION";
            specialDef2.icon = Modules.Assets.mainAssetBundle.LoadAsset<Sprite>("ChirrSpecial2");
            specialDef2.baseMaxStock = 1;
            specialDef2.baseRechargeInterval = 3f;
            specialDef2.beginSkillCooldownOnSkillEnd = false;
            specialDef2.canceledFromSprinting = false;
            specialDef2.fullRestockOnAssign = true;
            specialDef2.interruptPriority = EntityStates.InterruptPriority.Skill;
            specialDef2.isCombatSkill = false;
            specialDef2.mustKeyPress = false;
            specialDef2.cancelSprintingOnActivation = false;
            specialDef2.rechargeStock = 1;
            specialDef2.requiredStock = 1;
            specialDef2.stockToConsume = 1;

            Utils.RegisterSkillDef(specialDef2, typeof(ChirrLeash));
            SkillFamily.Variant specialVariant2 = Utils.RegisterSkillVariant(specialDef2);

            skillLocator.special = Utils.RegisterSkillsToFamily(chirrPrefab, specialVariant1);
        }

        public static void CreateDoppelganger()
        {
            doppelganger = PrefabAPI.InstantiateClone(LegacyResourcesAPI.Load<GameObject>("Prefabs/CharacterMasters/CommandoMonsterMaster"), "ChirrMonsterMaster", true);
            doppelganger.GetComponent<CharacterMaster>().bodyPrefab = chirrPrefab;

            Modules.Prefabs.masterPrefabs.Add(doppelganger);
        }
    }
    public class ChirrInfoComponent : NetworkBehaviour
    {
        public CharacterBody friend = new CharacterBody();
        public bool friendState = false;
        public bool sharing = false;
        public Inventory baseInventory;
        public CharacterBody mouseTarget;
        public CharacterBody pingTarget;
        public HurtBox futureFriend;

        /*void Update()
        {
            if (friend)
            {
                BaseAI friendAI = friend.master.GetComponent<BaseAI>();

                if (pingTarget)
                    friendAI.currentEnemy.gameObject = pingTarget.gameObject;
                else if (mouseTarget)
                    friendAI.currentEnemy.gameObject = mouseTarget.gameObject;
                friendAI.UpdateTargets();
                if (mouseTarget)
                    Chat.AddMessage(mouseTarget.ToString());
            }
        }

        /*[ClientRpc]
         * I could need some RPC later, so keeping it handy
        public void RpcAddIonCharge()
        {
            SkillLocator skillLoc = this.gameObject.GetComponent<SkillLocator>();
            GenericSkill ionGunSkill = skillLoc?.secondary;
            if (this.hasAuthority && ionGunSkill && ionGunSkill.stock < ionGunSkill.maxStock)
                ionGunSkill.AddOneStock();
        }*/
    }

}
