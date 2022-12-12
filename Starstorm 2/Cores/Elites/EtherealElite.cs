using EntityStates;
using R2API;
using RoR2;
using RoR2.Projectile;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Networking;

//FIXME: ethereal elites not registering correctly

namespace Starstorm2.Cores.Elites
{
    class EtherealElite : SS2Elite<EtherealElite>
    {
        public override string eliteName => "Ethereal";
        public override int TierIndex => 2;
        public override SS2Equipment AffixEquip => new Equipment.AffixEthereal();
        public override Color32 EliteColor => new Color32(18, 93, 74, 255);
        //public override string affixIconSprite => null;
        //FIXME: same stats as t2 = these will break t2 elite spawns
        public override CombatDirector.EliteTierDef customTier => new CombatDirector.EliteTierDef()
        {
            costMultiplier = CombatDirector.baseEliteCostMultiplier * 6,
            eliteTypes = Array.Empty<EliteDef>(),
            damageBoostCoefficient = CombatDirector.baseEliteDamageBoostCoefficient * 3,
            healthBoostCoefficient = CombatDirector.baseEliteHealthBoostCoefficient * 4.5f,
            isAvailable = (rules) =>
            {
                if (EtherealCore.instance != null && (EtherealCore.instance.etherealsCompleted > 0 || EtherealCore.instance.teleIsEthereal))
                    return true;
                return false;
            }
        };

        static internal GameObject ethExplosion;

        public override void RegisterHooks()
        {
            base.RegisterHooks();
            On.RoR2.CharacterModel.UpdateMaterials += SetUpEliteMaterialAlternate;
            On.RoR2.CharacterBody.OnInventoryChanged += CharacterBody_OnInventoryChanged;
        }

        public override void RegisterAdditional()
        {
            ethExplosion = Resources.Load<GameObject>("Prefabs/Projectiles/CaptainAirstrikeProjectile1").InstantiateClone("EthExplosion", true);
            var ethGhost = ethExplosion.GetComponent<ProjectileController>()?.ghostPrefab.InstantiateClone("EthExplosionGhost", true);
            ethGhost.AddComponent<NetworkIdentity>();
            ethExplosion.GetComponent<ProjectileController>().ghostPrefab = ethGhost;
            var expander = ethGhost.transform.Find("Expander");
            var ethSphere = expander.Find("Sphere, Inner Expanding").gameObject;
            var mats = Resources.FindObjectsOfTypeAll<Material>();
            var newmat = mats.Where(m => m.name == "matGlowFlowerAreaIndicator").FirstOrDefault();
            if (newmat)
                ethSphere.GetComponent<MeshRenderer>().material = newmat;
            expander.Find("AreaIndicatorCenter").gameObject.SetActive(false);
            UnityEngine.Object.Destroy(ethGhost.transform.Find("AirstrikeOrientation").gameObject);
        }

        private void CharacterBody_OnInventoryChanged(On.RoR2.CharacterBody.orig_OnInventoryChanged orig, CharacterBody self)
        {
            orig(self);
            self.AddItemBehavior<AffixEtherealBehavior>(self.HasBuff(eliteBuffDef) ? 1 : 0);
        }

        public class AffixEtherealBehavior : CharacterBody.ItemBehavior
        {
            //private const float explosionDamageCoefficient = 4.0f;
            //private const float explosionRadius = 20;
            private const float cooldownTimestamp = 8;
            //private const float explosionTimestamp = 13;
            //private GameObject ethWard;
            private float explosionTimer;
            //private bool explosionActive;

            public void Awake()
            {
                /*
                if (!NetworkServer.active)
                    return;
                if (!ethWard)
                {
                    ethWard = UnityEngine.Object.Instantiate(ethExplosion);
                    //ethWard.GetComponent<BuffWard>().Networkradius = explosionRadius + body.radius;
                    ethWard.GetComponent<TeamFilter>().teamIndex = body.teamComponent.teamIndex;
                    ethWard.GetComponent<NetworkedBodyAttachment>().AttachToGameObjectAndSpawn(body.gameObject);
                    ethWard.SetActive(false);
                }
                */
            }

            public void FixedUpdate()
            {
                if (!NetworkServer.active)
                    return;

                /*
                bool enabled = stack > 0;
                if (ethWard != enabled)
                {
                    if (enabled)
                    {
                        //ethWard = UnityEngine.Object.Instantiate(ethExplosion);
                        //ethWard.GetComponent<BuffWard>().Networkradius = 25f + body.radius;
                        //ethWard.GetComponent<TeamFilter>().teamIndex = body.teamComponent.teamIndex;
                        //ethWard.GetComponent<NetworkedBodyAttachment>().AttachToGameObjectAndSpawn(body.gameObject);
                        //Util.PlaySound("VoidEliteSpawn", voidWard);
                    }
                */
                explosionTimer += Time.fixedDeltaTime;
                if (explosionTimer > cooldownTimestamp)
                {
                    //start new explosion
                    var esm = gameObject.GetComponent<EntityStateMachine>();
                    if (esm && body.healthComponent.alive)
                    {
                        gameObject.GetComponent<EntityStateMachine>().SetNextState(new FireEtherealExplosion());
                    }
                    //ethWard = UnityEngine.Object.Instantiate(ethExplosion);
                    //explosionActive = true;
                    //ethWard.SetActive(true);
                    explosionTimer = 0;
                }
            }

            private void OnDisable()
            {
                //if (ethWard) UnityEngine.Object.Destroy(ethWard);
            }
        }

        public class FireEtherealExplosion : AimThrowableBase
        {
            private const float explosionDamageCoefficient = 4.0f;
            private const float explosionForce = 500;
            private const float explosionRadius = 15;

            public override void OnEnter()
            {
                this.projectilePrefab = ethExplosion;
                base.OnEnter();
            }

            public override void OnExit()
            {
                //play sound
                base.OnExit();
            }

            public override void ModifyProjectile(ref FireProjectileInfo fireProjectileInfo)
            {
                base.ModifyProjectile(ref fireProjectileInfo);
                var r = explosionRadius * Mathf.Max(characterBody.radius, 1);
                Func<float> RandOffset = () => { return UnityEngine.Random.Range(-r / 2, r / 2); };
                projectilePrefab.GetComponent<ProjectileImpactExplosion>().blastRadius = r;
                projectilePrefab.GetComponent<ProjectileController>().ghostPrefab.transform.Find("Expander").localScale = Vector3.one * r;
                fireProjectileInfo.damage = characterBody.damage * explosionDamageCoefficient;
                fireProjectileInfo.position = characterBody.corePosition + new Vector3(RandOffset(), RandOffset(), RandOffset());
                fireProjectileInfo.force = explosionForce;
                fireProjectileInfo.target = characterBody.gameObject;
                //var attachment = projectilePrefab.AddOrGetComponent<NetworkedBodyAttachment>();
                //attachment.attachedBody = characterBody;
                //attachment.attached = true;
            }

            public override bool KeyIsDown()
            {
                return false;
            }
        }
    }

}
