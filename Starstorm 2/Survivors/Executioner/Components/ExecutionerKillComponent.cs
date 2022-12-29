using RoR2;
using RoR2.Orbs;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

namespace Starstorm2.Survivors.Executioner.Components
{
    public class ExecutionerKillComponent : MonoBehaviour
    {
        public void FixedUpdate()
        {
            del.Clear();
            foreach (ExecutionerTimer b in hitList)
            {
                b.length -= Time.fixedDeltaTime;
                if (b.length <= 0f)
                {
                    del.Add(b);
                }
            }
            foreach (ExecutionerTimer b in del)
            {
                hitList.Remove(b);
            }
            del.Clear();
        }

        public void TriggerEffects(CharacterBody killerBody, DamageType damageType)
        {
            List<CharacterBody> resetList = new List<CharacterBody>();
            bool repeat;
            foreach (ExecutionerTimer b in hitList)
            {
                repeat = false;
                foreach (CharacterBody s in resetList)
                {
                    if (s == b.body)
                    {
                        repeat = true;
                        break;
                    }
                }
                if (!repeat)
                {
                    resetList.Add(b.body);
                    if (NetworkServer.active)
                    {
                        int orbCount = 1;
                        CharacterBody body = this.GetComponent<CharacterBody>();
                        if (body) orbCount = Executioner.ExecutionerCore.GetIonCountFromBody(body);
                        if (body.HasBuff(Cores.BuffCore.fearDebuff)) orbCount *= 2;

                        for (int i = 0; i < orbCount; i++)
                        {
                            Modules.Orbs.ExecutionerIonOrb ionOrb = new Modules.Orbs.ExecutionerIonOrb();
                            ionOrb.origin = this.transform.position;
                            ionOrb.target = Util.FindBodyMainHurtBox(b.body);
                            OrbManager.instance.AddOrb(ionOrb);
                        }

                        if (orbCount >= 50 && orbCount < 80)
                        {
                            Modules.Orbs.ExecutionerIonTempSuperOrb tempSuperIonOrb = new Modules.Orbs.ExecutionerIonTempSuperOrb();
                            tempSuperIonOrb.origin = this.transform.position;
                            tempSuperIonOrb.target = Util.FindBodyMainHurtBox(b.body);
                            OrbManager.instance.AddOrb(tempSuperIonOrb);
                        }

                        if (orbCount >= 80)
                        {
                            Modules.Orbs.ExecutionerIonSuperOrb superIonOrb = new Modules.Orbs.ExecutionerIonSuperOrb();
                            superIonOrb.origin = this.transform.position;
                            superIonOrb.target = Util.FindBodyMainHurtBox(b.body);
                            OrbManager.instance.AddOrb(superIonOrb);
                        }
                    }
                }
            }
            hitList.Clear();
        }

        public void AddTimer(CharacterBody b, float timer)
        {
            if (b.skillLocator)
            {
                ExecutionerTimer bt = new ExecutionerTimer(b, timer);
                hitList.Add(bt);
            }
        }

        public bool HasReset(CharacterBody body)
        {
            foreach (ExecutionerTimer bt in hitList)
            {
                if (bt.body == body)
                {
                    return true;
                }
            }
            return false;
        }

        public static float graceDuration = 3f;
        private List<ExecutionerTimer> hitList = new List<ExecutionerTimer>();
        List<ExecutionerTimer> del = new List<ExecutionerTimer>();
        public class ExecutionerTimer
        {
            public float length;
            public CharacterBody body;
            public ExecutionerTimer(CharacterBody b, float l)
            {
                body = b;
                length = l;
            }
        }
    }
}