using RoR2;
using RoR2.Orbs;
using RoR2.Projectile;
using System;
using UnityEngine;
using UnityEngine.Networking;

namespace Starstorm2Unofficial.Cores.States.Nucleator
{
    // Token: 0x02000603 RID: 1539
    [RequireComponent(typeof(ProjectileController))]
    [RequireComponent(typeof(ProjectileDamage))]
    public class NucleatorProjectile : MonoBehaviour
    {
        private ProjectileController projectileController;
        private ProjectileImpactExplosion projectileImpactExplosion;
        private ProjectileDamage projectileDamage;
        
        private float duration;
        private float fixedAge;

        private Rigidbody rigidbody;
        private float frequency = 14.0f;  // Speed of sine movement
        private float magnitude = 0.25f;   // Size of sine movement
        private Vector3 axis;
        private Vector3 pos;

        public TeamIndex teamIndex;
        public float baseDuration = 2f;
        public float acceleration = 50f;
        public GameObject rotateObject;
        public float removalTime = 1f;
        public float baseRadius = 0f;
        public float baseDamage = 0f;
        public float charge;

        private void Awake()
        {
            this.projectileController = base.GetComponent<ProjectileController>();
            this.projectileDamage = base.GetComponent<ProjectileDamage>();
            this.projectileImpactExplosion = base.GetComponent<ProjectileImpactExplosion>();
            this.rigidbody = base.GetComponent<Rigidbody>();
            this.rigidbody.useGravity = false; 
            this.duration = this.baseDuration;
            this.fixedAge = 0f;
            this.baseDamage = projectileDamage.damage;
            this.projectileImpactExplosion.lifetime = this.baseDuration;
            this.acceleration = (1 - this.charge) * (50f - 20f) + 20f;           

            pos = transform.position;
            axis = transform.up * -1;  // May or may not be the axis you want
        }
        
        private void FixedUpdate()
        {
            this.fixedAge += Time.deltaTime;
            var lifetimeCoef = this.fixedAge / this.baseDuration;

            if (this.fixedAge < this.baseDuration)
            {                
                this.projectileImpactExplosion.blastRadius = baseRadius + lifetimeCoef * (baseRadius * 1.25f - 1) + 1;
                this.projectileDamage.damage = baseDamage + lifetimeCoef * (baseDamage * 1.25f - 1) + 1;

                pos += transform.forward * Time.deltaTime * acceleration;
                transform.position = pos + axis * Mathf.Sin(Time.time * frequency) * magnitude;
            }

            
        }

        
    }
}