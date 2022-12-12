using RoR2;
using UnityEngine;

namespace Starstorm2.Components
{
    public class NemesisBossComponent : MonoBehaviour
    {
        private void Start()
        {
            CharacterBody body = this.GetComponent<CharacterBody>();

            if (body)
            {
                if (body.teamComponent && body.teamComponent.teamIndex == TeamIndex.Monster)
                {
                    //body.baseMaxHealth = 2800f;
                    //body.levelMaxHealth = 840f;
                    //body.baseRegen = 0f;
                    //body.levelRegen = 0f;
                    //body.baseDamage = 6f;
                    //body.levelDamage = body.baseDamage * 0.2f;

                    //body.bodyFlags = CharacterBody.BodyFlags.IgnoreFallDamage;

                    ModelLocator modelLocator = this.GetComponent<ModelLocator>();
                    if (modelLocator)
                    {
                        Transform modelTransform = modelLocator.modelBaseTransform;
                        if (modelTransform)
                        {
                            modelTransform.localScale *= 1.5f;
                        }

                        HurtBoxGroup hurtbox = modelLocator.modelTransform.GetComponent<HurtBoxGroup>();
                        if (hurtbox)
                        {
                            if (hurtbox.hurtBoxes[0])
                            {
                                hurtbox.hurtBoxes[0].transform.localScale = new Vector3(1.7f, 1.4f, 1);
                            }
                        }
                    }
                }
            }

            Destroy(this);
        }
    }
}