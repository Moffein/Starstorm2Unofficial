using RoR2;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Starstorm2.Cores.Elites
{
    class DazingElite : SS2Elite<DazingElite>
    {
        public override string eliteName => "Dazing";
        public override SS2Equipment AffixEquip => new Equipment.AffixDazing();
        public override Color32 EliteColor => new Color32(230, 235, 232, 255);

        public override void RegisterHooks()
        {
            base.RegisterHooks();
            On.RoR2.CharacterBody.OnTakeDamageServer += CharacterBody_OnTakeDamageServer;
        }

        public override void RegisterAdditional()
        {
            SetUpPostProcess();
        }

        private void CharacterBody_OnTakeDamageServer(On.RoR2.CharacterBody.orig_OnTakeDamageServer orig, CharacterBody self, DamageReport damageReport)
        {
            if (damageReport?.attackerBody && damageReport.attackerBody.HasBuff(AffixEquip.PassiveBuffDef))
            {
                Debug.Log("dazing elite hit");
            }

            orig(self, damageReport);
        }

        private void SetUpPostProcess()
        {

        }
    }
}
