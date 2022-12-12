using KinematicCharacterController;
using R2API;
using RoR2;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Networking;

//TODO: ethereal tele assets
//TODO: make hud reflect ethereal difficulty change

namespace Starstorm2.Cores
{
    class EtherealCore
    {
        public static EtherealCore instance { get; private set; }
        public static Color32 ethColor = new Color32(18, 93, 74, 255);

        public int etherealsCompleted { get; private set; }
        public bool teleIsEthereal;
        private float storedScalingValue;
        private LanguageAPI.LanguageOverlay ethDiffToken;

        public GameObject uMithBody;
        public SpawnCard uMithSC;

        public EtherealCore()
        {
            instance = this;

            LanguageAPI.Add("SHRINE_ETHEREAL_NAME", "Ethereal Terminal");
            LanguageAPI.Add("SHRINE_ETHEREAL_CONTEXT", "Activate Ethereal Terminal...?\n(This will make the run significantly harder!)");
            LanguageAPI.Add("SHRINE_ETHEREAL_USE_MESSAGE", "{0} has activated the Ethereal Terminal...");
            LanguageAPI.Add("SHRINE_ETHEREAL_USE_MESSAGE_2P", "You have activated the Ethereal Terminal...");
            LanguageAPI.Add("ETHEREAL_DIFFICULTY_WARNING", "<style=cDeath>The planet redoubles its efforts against you...</style>");

            //SetUpUltraMithrix();

            Run.onRunStartGlobal += Run_onRunStartGlobal;
            Run.onRunDestroyGlobal += Run_onRunDestroyGlobal;
            On.RoR2.SceneDirector.Start += SceneDirector_Start;
            TeleporterInteraction.onTeleporterBeginChargingGlobal += TeleporterInteraction_onTeleporterBeginChargingGlobal;
            TeleporterInteraction.onTeleporterFinishGlobal += TeleporterInteraction_onTeleporterFinishGlobal;
            On.EntityStates.Missions.BrotherEncounter.PreEncounter.OnEnter += PreEncounter_OnEnter;
            On.EntityStates.Missions.BrotherEncounter.BrotherEncounterPhaseBaseState.OnEnter += BrotherEncounterPhaseBaseState_OnEnter;
        }

        private void SetUpUltraMithrix()
        {
            LanguageAPI.Add("ULTRAMITH_NAME", "Ultra Mithrix");
            LanguageAPI.Add("ULTRAMITH_SUBTITLE", "The Big Man");

            uMithBody = PrefabAPI.InstantiateClone(Resources.Load<GameObject>("Prefabs/CharacterBodies/BrotherBody"), "UltraMithrixBody", true);
            CharacterBody body = uMithBody.GetComponent<CharacterBody>();
            body.baseNameToken = "ULTRAMITH_NAME";
            body.subtitleNameToken = "ULTRAMITH_SUBTITLE";

            //scale him up
            float scale = 1.5f;
            body.GetComponent<ModelLocator>().modelBaseTransform.localScale *= scale;
            foreach (KinematicCharacterMotor motor in body.GetComponentsInChildren<KinematicCharacterMotor>())
            {
                if (motor)
                    motor.SetCapsuleDimensions(motor.Capsule.radius * scale, motor.Capsule.height * scale, scale);
            }

            //adjust stats
            body.baseMoveSpeed *= 1.2f;
            body.baseAttackSpeed *= 0.9f;
            body.baseMaxHealth *= 4;
            body.levelMaxHealth *= 4;
            body.baseDamage *= 1.5f;
            body.levelDamage *= 1.5f;

            //TODO: define big man in prefabcore, migrate to contentpack
            //BodyCatalog.getAdditionalEntries += delegate (List<GameObject> list)
            //{
            //    list.Add(uMithBody);
            //};

            GameObject uMithMaster = PrefabAPI.InstantiateClone(Resources.Load<GameObject>("Prefabs/CharacterMasters/BrotherMaster"), "UltraMithrixMaster", true);
            uMithMaster.GetComponent<CharacterMaster>().bodyPrefab = uMithBody;

            //MasterCatalog.getAdditionalEntries += delegate (List<GameObject> list)
            //{
            //    list.Add(uMithMaster);
            //};

            var MithSC = Resources.Load<CharacterSpawnCard>("SpawnCards/CharacterSpawnCards/cscBrother");
            uMithSC = UnityEngine.Object.Instantiate(MithSC);
            uMithSC.prefab = uMithMaster;
        }

        private void PreEncounter_OnEnter(On.EntityStates.Missions.BrotherEncounter.PreEncounter.orig_OnEnter orig, EntityStates.Missions.BrotherEncounter.PreEncounter self)
        {
            orig(self);

            if (etherealsCompleted > 0)
            {
                var mithEncounter = self.GetComponent<ChildLocator>().FindChild("Phase1").GetComponent<ScriptedCombatEncounter>();
                for (int i = 0; i < mithEncounter.spawns.Count(); i++)
                {
                    mithEncounter.spawns[i].spawnCard = uMithSC;
                }
            }
        }

        private void BrotherEncounterPhaseBaseState_OnEnter(On.EntityStates.Missions.BrotherEncounter.BrotherEncounterPhaseBaseState.orig_OnEnter orig, EntityStates.Missions.BrotherEncounter.BrotherEncounterPhaseBaseState self)
        {
            orig(self);

            if (etherealsCompleted > 0)
            {
                var directorObj = GameObject.Find("Director");
                if (directorObj)
                {
                    var director = directorObj.GetComponent<CombatDirector>();
                    director.enabled = true;
                    //TODO: add everything to director
                    /*
                    var allcards = Resources.FindObjectsOfTypeAll<CharacterSpawnCard>();
                    foreach (var card in allcards)
                    {
                        DirectorCard card2 = new DirectorCard();
                        card2.spawnCard = card;
                        director.monsterCardsSelection.AddChoice(card2, 1);
                    }
                    */
                }
            }
        }

        private void Run_onRunStartGlobal(Run run)
        {
            //StringBuilder.Append(Language.GetString(string contexttoken))
            //StringBuilder.Append(" +1")
            var diffToken = DifficultyCatalog.GetDifficultyDef(run.selectedDifficulty).nameToken;
            var builder = new StringBuilder();
            builder.Append(Language.GetString(diffToken));
            builder.Append(" +1");
            //Debug.Log(builder.Take());
            ethDiffToken = LanguageAPI.AddOverlay(diffToken, builder.Take());
            storedScalingValue = DifficultyCatalog.GetDifficultyDef(run.selectedDifficulty).scalingValue;
            Debug.Log(DifficultyCatalog.GetDifficultyDef(run.selectedDifficulty).scalingValue);
            etherealsCompleted = 0;
            teleIsEthereal = false;
        }

        private void Run_onRunDestroyGlobal(Run run)
        {
            if (ethDiffToken != null)
                ethDiffToken.Remove();
            DifficultyCatalog.GetDifficultyDef(run.selectedDifficulty).scalingValue = storedScalingValue;
            Debug.Log(DifficultyCatalog.GetDifficultyDef(run.selectedDifficulty).scalingValue);
        }

        private void SceneDirector_Start(On.RoR2.SceneDirector.orig_Start orig, SceneDirector self)
        {
            orig(self);
            if (self.teleporterInstance)
            {
                GameObject newt = GameObject.Find("NewtStatue");
                if (newt)
                {
                    var ethBehavior = newt.AddComponent<ShrineEtherealBehavior>();
                    var pi = newt.GetComponent<PurchaseInteraction>();
                    if (pi)
                    {
                        pi.onPurchase.AddListener(ethBehavior.ActivateEtherealTeleporter);
                        pi.displayNameToken = "SHRINE_ETHEREAL_NAME";
                        pi.contextToken = "SHRINE_ETHEREAL_CONTEXT";
                    }
                }
            }

            if (teleIsEthereal)
            {
                self.StartCoroutine(Utils.BroadcastChat("ETHEREAL_DIFFICULTY_WARNING"));
                teleIsEthereal = false;
                etherealsCompleted++;
                //update difficulty
                var run = Run.instance;
                if (NetworkServer.active && run)
                {
                    //one-time difficulty adjustments
                    DifficultyDef curDiff = DifficultyCatalog.GetDifficultyDef(run.selectedDifficulty);
                    switch (curDiff.scalingValue)
                    {
                        case 1:
                            foreach (CharacterMaster cm in run.userMasters.Values)
                            {
                                if (cm.inventory)
                                    cm.inventory.RemoveItem(RoR2Content.Items.DrizzlePlayerHelper.itemIndex);
                            }
                            break;
                        case 2:
                            foreach (CharacterMaster cm in run.userMasters.Values)
                            {
                                if (cm.inventory)
                                    cm.inventory.GiveItem(RoR2Content.Items.MonsoonPlayerHelper.itemIndex);
                            }
                            break;
                        case 3:
                            if (Modules.Config.EnableTyphoon.Value && Modules.Config.TyphoonIncreaseSpawnCap.Value)
                                TeamCatalog.GetTeamDef(TeamIndex.Monster).softCharacterLimit *= 2;
                            break;
                    }
                    //drizzle -> rainstorm, rainstorm -> monsoon
                    if (curDiff.scalingValue < 3)
                        curDiff.scalingValue += 1;
                    //monsoon -> typhoon, typhoon +25%
                    else
                    {
                        curDiff.scalingValue += 0.5f;
                    }

                    string diffToken = curDiff.nameToken;
                    //curDiff.nameToken += " +1";
                    Debug.Log(DifficultyCatalog.GetDifficultyDef(run.selectedDifficulty).scalingValue);
                }
                //update difficulty hud display
            }
        }

        private void TeleporterInteraction_onTeleporterBeginChargingGlobal(TeleporterInteraction tele)
        {
            if (teleIsEthereal)
            {
                if (NetworkServer.active)
                {
                    //tele.holdoutZoneController.currentRadius *= 2;
                    TeleporterInteraction.instance.holdoutZoneController.calcRadius += HoldoutZoneController_calcRadius;
                    TeleporterInteraction.instance.holdoutZoneController.calcChargeRate += HoldoutZoneController_calcChargeRate;

                    if (tele.bossDirector)
                    {
                        //effectively triggers a shrine of the mountain - not exactly how ethereal teles worked
                        //tele.bossDirector.monsterCredit += (float)(int)(600f * Mathf.Pow(Run.instance.compensatedDifficultyCoefficient, 0.5f));
                    }
                    if (tele.bonusDirector)
                    {
                        tele.bonusDirector.monsterCredit += (float)(int)(600f * Mathf.Pow(Run.instance.compensatedDifficultyCoefficient, 0.5f));
                    }
                    else
                    {
                        Debug.LogWarning("*** Teleporter does not have a bonus director!");
                    }
                }
            }
        }

        private void HoldoutZoneController_calcRadius(ref float radius)
        {
            if (teleIsEthereal)
            {
                radius *= 2;
            }
        }

        private void HoldoutZoneController_calcChargeRate(ref float rate)
        {
            if (teleIsEthereal)
            {
                rate /= 1.25f;
            }
        }

        private void TeleporterInteraction_onTeleporterFinishGlobal(TeleporterInteraction obj)
        {
        }
    }

    public class ShrineEtherealBehavior : NetworkBehaviour
    {
        private PurchaseInteraction purchaseInteraction;
        //private bool waitingForRefresh;

        private void Start()
        {
            this.purchaseInteraction = base.GetComponent<PurchaseInteraction>();
        }

        [Server]
        public void ActivateEtherealTeleporter(Interactor interactor)
        {
            if (!NetworkServer.active)
                return;

            if (TeleporterInteraction.instance && VoidCore.instance != null)
            {
                EtherealCore.instance.teleIsEthereal = true;
            }

            CharacterBody body = interactor.GetComponent<CharacterBody>();
            Chat.SendBroadcastChat(new Chat.SubjectFormatChatMessage
            {
                subjectAsCharacterBody = body,
                baseToken = "SHRINE_ETHEREAL_USE_MESSAGE",
            });
            EffectManager.SpawnEffect(Resources.Load<GameObject>("Prefabs/Effects/ShrineUseEffect"), new EffectData
            {
                origin = base.transform.position,
                rotation = Quaternion.identity,
                scale = 1,
                color = EtherealCore.ethColor
            }, true);
        }

        public void FixedUpdate()
        {
        }
    }
}
