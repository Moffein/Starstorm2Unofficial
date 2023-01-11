using RoR2;
using UnityEngine;

namespace Starstorm2Unofficial.Survivors.Executioner.Components
{
    public class ExecutionerController : MonoBehaviour
    {
        private float maxEmission = 80f;
        private float currentEmission;
        private float defaultBodyEmission;
        private bool wasMaxCharge;
        private bool isSuperCharged;
        private Material gunMat;
        private Material bodyMat;
        private CharacterBody characterBody;
        private CharacterModel model;
        private ChildLocator childLocator;

        private ParticleSystem dashEffect;
        private ParticleSystem axeSpawnEffect;
        private ParticleSystem maxChargeEffect;
        private ParticleSystem superchargeEffect;
        private ParticleSystem superchargeEffectLoop;

        private void Awake()
        {
            this.characterBody = this.gameObject.GetComponent<CharacterBody>();
            this.childLocator = this.gameObject.GetComponentInChildren<ChildLocator>();
            this.model = this.GetComponentInChildren<CharacterModel>();
            this.currentEmission = 0f;
            this.wasMaxCharge = false;

            if (this.childLocator)
            {
                this.dashEffect = this.childLocator.FindChild("DashEffect").GetComponentInChildren<ParticleSystem>();
                this.axeSpawnEffect = this.childLocator.FindChild("AxeSpawnEffect").GetComponentInChildren<ParticleSystem>();
                this.maxChargeEffect = this.childLocator.FindChild("MaxChargeEffect").GetComponentInChildren<ParticleSystem>();
                this.superchargeEffect = this.childLocator.FindChild("SuperchargeEffect").GetComponentInChildren<ParticleSystem>();
                this.superchargeEffectLoop = this.childLocator.FindChild("SuperchargePassiveEffect").GetComponentInChildren<ParticleSystem>();
            }

            // disable dynamic bones on mastery skin
            Invoke("VigilanteCheck", 2f);
            Invoke("CheckInventory", 0.2f);
        }


        //Duplicated from ExecutionerMain. This makes special stock get set before he exits his pod.
        private void Start()
        {
            SkillLocator skillLocator = base.GetComponent<SkillLocator>();
            if (skillLocator)
            {
                GenericSkill ionGunSkill = skillLocator.secondary;
                if (ionGunSkill)
                {
                    ionGunSkill.stock = 0;
                    if (this.characterBody && this.characterBody.masterObject)
                    {
                        MasterIonStockComponent ionStocks = this.characterBody.masterObject.GetComponent<MasterIonStockComponent>();
                        if(ionStocks) ionGunSkill.stock = ionStocks.stocks;
                    }
                }
            }
        }

        public void CheckInventory()
        {
            if (this.characterBody && this.characterBody.master)
            {
                Inventory inventory = this.characterBody.master.inventory;
                if (inventory)
                {
                    //bool hasGunReplacement = inventory.GetItemCount(ItemIndex.LunarPrimaryReplacement) > 0;
                    bool hasGunReplacement = inventory.GetItemCount(RoR2Content.Items.LunarPrimaryReplacement.itemIndex) > 0;

                    if (hasGunReplacement)
                    {
                        this.ShowGun(false);
                    }
                    else
                    {
                        this.ShowGun(true);
                    }

                    //bool hasAxeReplacement = inventory.GetItemCount(ItemIndex.ArmorReductionOnHit) > 0;
                    bool hasAxeReplacement = inventory.GetItemCount(RoR2Content.Items.ArmorReductionOnHit.itemIndex) > 0;

                    if (hasAxeReplacement)
                    {
                        this.ShowAxe(false);
                    }
                    else
                    {
                        this.ShowAxe(true);
                    }
                }
            }

            if (this.model)
            {
                this.bodyMat = this.model.baseRendererInfos[0].defaultMaterial;
                this.gunMat = this.model.baseRendererInfos[1].defaultMaterial;
            }
        }

        private void ShowGun(bool hhhh)
        {
            if (this.model)
            {
                if (hhhh) this.model.baseRendererInfos[1].defaultMaterial = this.model.gameObject.GetComponent<ModelSkinController>().skins[this.characterBody.skinIndex].rendererInfos[1].defaultMaterial;
                else this.model.baseRendererInfos[1].defaultMaterial = null;
            }
        }

        private void ShowAxe(bool hhhh)
        {
            if (this.model)
            {
                if (hhhh) this.model.baseRendererInfos[2].defaultMaterial = this.model.gameObject.GetComponent<ModelSkinController>().skins[this.characterBody.skinIndex].rendererInfos[2].defaultMaterial;
                else this.model.baseRendererInfos[2].defaultMaterial = null;
            }
        }

        private void VigilanteCheck()
        {
            if (this.model)
            {
                if (this.characterBody.skinIndex == 1)
                {
                    foreach (DynamicBone i in this.model.GetComponents<DynamicBone>())
                    {
                        if (i) i.enabled = false;
                    }
                }
            }
        }

        private void FixedUpdate()
        {
            //This breaks emission between runs
            /*this.currentEmission = Mathf.Lerp(this.currentEmission, this.characterBody.skillLocator.secondary.stock, 1.5f * Time.fixedDeltaTime);

            if (this.gunMat)
            {
                this.gunMat.SetFloat("_EmPower", Util.Remap(this.currentEmission, 0, this.characterBody.skillLocator.secondary.maxStock, 0, this.maxEmission));
                float colorValue = Util.Remap(this.currentEmission, 0, this.characterBody.skillLocator.secondary.maxStock, 0f, 1f);
                Color emColor = emColor = new Color(colorValue, colorValue, colorValue);
                this.gunMat.SetColor("_EmColor", emColor);
            }

            if (this.bodyMat)
            {
                if (this.characterBody.HasBuff(Starstorm2.Cores.BuffCore.exeSuperchargedBuff))
                {
                    this.bodyMat.SetFloat("_EmPower", this.maxEmission);
                }
                else
                {
                    this.bodyMat.SetFloat("_EmPower", this.defaultBodyEmission);
                }
            }*/

            // max charge sound
            if (this.characterBody && this.characterBody.skillLocator)
            {
                if (this.characterBody.skillLocator.secondary.stock == this.characterBody.skillLocator.secondary.maxStock)
                {
                    if (!this.wasMaxCharge)
                    {
                        Util.PlaySound("SS2UExecutionerMaxCharge", this.gameObject);
                        this.PlayMaxChargeEffect();
                    }
                    this.wasMaxCharge = true;
                }
                else
                {
                    this.wasMaxCharge = false;
                }
            }

            // supercharge effect loop
            if (this.characterBody)
            {
                bool charged = this.characterBody.HasBuff(Starstorm2Unofficial.Cores.BuffCore.exeSuperchargedBuff);
                if (this.isSuperCharged && !charged && this.superchargeEffectLoop) this.superchargeEffectLoop.Stop();
                if (!this.isSuperCharged && charged && this.superchargeEffectLoop)
                {
                    this.superchargeEffectLoop.Play();
                    this.PlaySuperchargeEffect();
                }
                this.isSuperCharged = charged;
            }
        }

        public void PlayDashEffect()
        {
            if (this.dashEffect) this.dashEffect.Play();
        }

        public void PlayAxeSpawnEffect()
        {
            if (this.axeSpawnEffect) this.axeSpawnEffect.Play();
        }

        public void PlayMaxChargeEffect()
        {
            if (this.maxChargeEffect) this.maxChargeEffect.Play();
        }

        public void PlaySuperchargeEffect()
        {
            if (this.superchargeEffect) this.superchargeEffect.Play();
        }
    }
}