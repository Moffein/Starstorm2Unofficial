using JetBrains.Annotations;
using RoR2;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

namespace Starstorm2Unofficial.Survivors.Cyborg
{
    //Allows Defense Matrix to block enemy hitscan without blocking friendly hitscan/projectiles
    public static class DefenseMatrixManager
    {
        private static List<DefenseMatrixInfo> activeDefenseMatrices = new List<DefenseMatrixInfo>();

        private static bool initialized = false;
        public static void Initialize()
        {
            if (initialized) return;
            initialized = true;
            RoR2.Stage.onStageStartGlobal += Hooks.ClearDefenseMatrixListOnStageStart;
            On.RoR2.BulletAttack.Fire += Hooks.CheckHostileDefenseMatrices;
            On.EntityStates.GolemMonster.FireLaser.OnEnter += Hooks.DefenseMatrixBlockGolemLaserFire;
            On.EntityStates.GolemMonster.ChargeLaser.Update += Hooks.ChargeLaser_Update;
            On.EntityStates.TitanMonster.ChargeMegaLaser.FixedUpdate += Hooks.ChargeMegaLaser_FixedUpdate;
            On.EntityStates.TitanMonster.FireMegaLaser.FixedUpdate += Hooks.FireMegaLaser_FixedUpdate;
            On.EntityStates.EngiTurret.EngiTurretWeapon.FireBeam.GetBeamEndPoint += Hooks.FireBeam_GetBeamEndPoint;
        }

        /*public static void DeInitialize()
        {
            if (!initialized) return;
            initialized = false;
            RoR2.Stage.onStageStartGlobal -= Hooks.ClearDefenseMatrixListOnStageStart;
            On.RoR2.BulletAttack.Fire -= Hooks.CheckHostileDefenseMatrices;
            On.EntityStates.GolemMonster.FireLaser.OnEnter -= Hooks.DefenseMatrixBlockGolemLaserFire;
            On.EntityStates.GolemMonster.ChargeLaser.Update -= Hooks.ChargeLaser_Update;
            On.EntityStates.TitanMonster.FireMegaLaser.FixedUpdate -= Hooks.FireMegaLaser_FixedUpdate;
            On.EntityStates.EngiTurret.EngiTurretWeapon.FireBeam.GetBeamEndPoint -= Hooks.FireBeam_GetBeamEndPoint;
        }*/

        public static void ClearList()
        {
            activeDefenseMatrices.Clear();
        }

        public static DefenseMatrixInfo AddMatrix(DefenseMatrixInfo defenseMatrixInfo)
        {
            if (defenseMatrixInfo != null && defenseMatrixInfo.colliders != null && defenseMatrixInfo.colliders.Length > 0)
            {
                activeDefenseMatrices.Add(defenseMatrixInfo);
                return defenseMatrixInfo;
            }
            return null;
        }

        public static DefenseMatrixInfo AddMatrix(Collider[] colliders, TeamIndex teamIndex)
        {
            return AddMatrix(new DefenseMatrixInfo(colliders, teamIndex));
        }

        public static void RemoveMatrix(DefenseMatrixInfo defenseMatrixInfo)
        {
            activeDefenseMatrices.Remove(defenseMatrixInfo);
        }

        public static void EnableMatrices(TeamIndex attackerTeam)
        {
            foreach (DefenseMatrixInfo dmi in activeDefenseMatrices)
            {
                if (dmi.teamIndex != attackerTeam)
                {
                    dmi.EnableColliders();
                }
            }
        }
        public static void DisableMatrices(TeamIndex attackerTeam)
        {
            foreach (DefenseMatrixInfo dmi in activeDefenseMatrices)
            {
                if (dmi.teamIndex != attackerTeam)
                {
                    dmi.DisableColliders();
                }
            }
        }

        public class DefenseMatrixInfo
        {
            public Collider[] colliders;
            public TeamIndex teamIndex;

            public DefenseMatrixInfo(Collider[] colliders, TeamIndex teamIndex)
            {
                this.colliders = colliders;
                this.teamIndex = teamIndex;
            }

            public void EnableColliders()
            {
                for (int i = 0; i < colliders.Length; i++)
                {
                    colliders[i].enabled = true;
                }
            }

            public void DisableColliders()
            {
                for (int i = 0; i < colliders.Length; i++)
                {
                    colliders[i].enabled = false;
                }
            }
        }

        private static class Hooks
        {
            public static Vector3 FireBeam_GetBeamEndPoint(On.EntityStates.EngiTurret.EngiTurretWeapon.FireBeam.orig_GetBeamEndPoint orig, EntityStates.EngiTurret.EngiTurretWeapon.FireBeam self)
            {
                TeamIndex teamIndex = self.GetTeam();
                DefenseMatrixManager.EnableMatrices(teamIndex);
                Vector3 endpoint = orig(self);
                DefenseMatrixManager.DisableMatrices(teamIndex);
                return endpoint;
            }

            public static void ChargeMegaLaser_FixedUpdate(On.EntityStates.TitanMonster.ChargeMegaLaser.orig_FixedUpdate orig, EntityStates.TitanMonster.ChargeMegaLaser self)
            {
                TeamIndex teamIndex = self.GetTeam();
                DefenseMatrixManager.EnableMatrices(teamIndex);
                orig(self);
                DefenseMatrixManager.DisableMatrices(teamIndex);
            }

            public static void FireMegaLaser_FixedUpdate(On.EntityStates.TitanMonster.FireMegaLaser.orig_FixedUpdate orig, EntityStates.TitanMonster.FireMegaLaser self)
            {
                TeamIndex teamIndex = self.GetTeam();
                DefenseMatrixManager.EnableMatrices(teamIndex);
                orig(self);
                DefenseMatrixManager.DisableMatrices(teamIndex);
            }

            public static void ChargeLaser_Update(On.EntityStates.GolemMonster.ChargeLaser.orig_Update orig, EntityStates.GolemMonster.ChargeLaser self)
            {
                TeamIndex teamIndex = self.GetTeam();
                DefenseMatrixManager.EnableMatrices(teamIndex);
                orig(self);
                DefenseMatrixManager.DisableMatrices(teamIndex);
            }

            public static void DefenseMatrixBlockGolemLaserFire(On.EntityStates.GolemMonster.FireLaser.orig_OnEnter orig, EntityStates.GolemMonster.FireLaser self)
            {
                TeamIndex teamIndex = self.GetTeam();
                if (self.isAuthority)
                {
                    DefenseMatrixManager.EnableMatrices(teamIndex);
                }
                orig(self);
                if (self.isAuthority)
                {
                    DefenseMatrixManager.DisableMatrices(teamIndex);
                }
            }

            public static void CheckHostileDefenseMatrices(On.RoR2.BulletAttack.orig_Fire orig, BulletAttack self)
            {
                TeamIndex teamIndex = TeamIndex.None;
                if (self.owner)
                {
                    TeamComponent tc = self.owner.GetComponent<TeamComponent>();
                    if (tc) teamIndex = tc.teamIndex;
                }
                DefenseMatrixManager.EnableMatrices(teamIndex);
                orig(self);
                DefenseMatrixManager.DisableMatrices(teamIndex);
            }

            public static void ClearDefenseMatrixListOnStageStart(Stage obj)
            {
                DefenseMatrixManager.ClearList();
            }
        }
    }
}
