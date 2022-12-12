using RoR2;
using RoR2.Projectile;
using R2API;
using Starstorm2.Cores;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Starstorm2.Components
{
    public class ChirrController : MonoBehaviour
    {
        private CharacterBody characterBody;
        private CharacterModel model;
        private ChildLocator childLocator;//gonna store this for when it's needed (if it's ever needed kek)
        

        private void Awake()
        {
            this.characterBody = this.gameObject.GetComponent<CharacterBody>();
            this.childLocator = this.gameObject.GetComponentInChildren<ChildLocator>();
            this.model = this.GetComponentInChildren<CharacterModel>();


            if (this.characterBody)
            {
                Transform modelTransform = this.characterBody.GetComponent<ModelLocator>().modelTransform;
                /*if (modelTransform)
                {
                    this.gunMat = modelTransform.GetComponent<ModelSkinController>().skins[this.characterBody.skinIndex].rendererInfos[1].defaultMaterial;
                }*/
            }

            //this.InitItemDisplays();
        }

        private void FixedUpdate()
        {
            /*this.currentEmission = Mathf.Lerp(this.currentEmission, this.characterBody.skillLocator.secondary.stock, 1.5f * Time.fixedDeltaTime);

            if (this.gunMat)
            {
                this.gunMat.SetFloat("_EmPower", Util.Remap(this.currentEmission, 0, this.characterBody.skillLocator.secondary.maxStock, 0, this.maxEmission));
                float colorValue = Util.Remap(this.currentEmission, 0, this.characterBody.skillLocator.secondary.maxStock, 0f, 1f);
                Color emColor = emColor = new Color(colorValue, colorValue, colorValue);
                this.gunMat.SetColor("_EmColor", emColor);
            }*/
        }

        private void InitItemDisplays()
        {
            // i really don't know why this is necessary but just deal with it for now
            Starstorm2.Cores.ItemDisplays.CyborgItemDisplays.RegisterModdedDisplays();

            ItemDisplayRuleSet newRuleset = Instantiate(Starstorm2.Cores.ItemDisplays.CyborgItemDisplays.itemDisplayRuleSet);
            this.model.itemDisplayRuleSet = newRuleset;
        }
    }
}