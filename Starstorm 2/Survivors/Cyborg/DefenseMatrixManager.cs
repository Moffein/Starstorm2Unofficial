using RoR2;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

namespace Starstorm2.Survivors.Cyborg
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

        public static void AddMatrix(Collider collider, TeamIndex teamIndex)
        {
            activeDefenseMatrices.Add(new DefenseMatrixInfo(collider, teamIndex));
        }

        public static void RemoveMatrix(Collider collider, TeamIndex teamIndex)
        {
            List<DefenseMatrixInfo> toRemove = new List<DefenseMatrixInfo>();
            foreach (DefenseMatrixInfo dmi in activeDefenseMatrices)
            {
                if (dmi.collider == collider && dmi.teamIndex == teamIndex)
                {
                    toRemove.Add(dmi);
                }
            }

            foreach (DefenseMatrixInfo dmi in toRemove)
            {
                activeDefenseMatrices.Remove(dmi);
            }
        }

        public static void EnableMatrices(TeamIndex attackerTeam)
        {
            foreach (DefenseMatrixInfo dmi in activeDefenseMatrices)
            {
                if (dmi.teamIndex != attackerTeam)
                {
                    dmi.collider.enabled = true;
                }
            }
        }
        public static void DisableMatrices(TeamIndex attackerTeam)
        {
            foreach (DefenseMatrixInfo dmi in activeDefenseMatrices)
            {
                if (dmi.teamIndex != attackerTeam)
                {
                    dmi.collider.enabled = false;
                }
            }
        }

        public class DefenseMatrixInfo
        {
            public Collider collider;
            public TeamIndex teamIndex;

            public DefenseMatrixInfo(Collider collider, TeamIndex teamIndex)
            {
                this.collider = collider;
                this.teamIndex = teamIndex;
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
