using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BepInEx.Configuration;
using RoR2;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.Rendering.PostProcessing;
using R2API;
using R2API.Networking.Interfaces;
using R2API.Networking;
using UnityEngine.AddressableAssets;
//TODO: event selection weight

namespace Starstorm2Unofficial.Cores
{
    public class EventsCore
    {

        public static EventsCore instance;
        public static float stormDuration = 90;
        //public static float stormDuration = 10;
        public static float stormWarningTimer;
        public EventDirector director;

        private PostProcessVolume stormPP;
        private GameObject rainObjPrefab = Modules.Assets.mainAssetBundle.LoadAsset<GameObject>("StormFX");
        private GameObject rainObjPrefabOptimized = Modules.Assets.mainAssetBundle.LoadAsset<GameObject>("StormFXOptimized");
        private GameObject stormFXOBjprefab;
        public GameObject stormFXObj;
        public Run currentRun;

        public EventsCore()
        {
            instance = this;

            NetworkingAPI.RegisterMessageType<SyncStorms>();

            SetUpVisualsPrefab();

            RoR2.Run.onRunStartGlobal += Run_Start;
            RoR2.Run.onRunDestroyGlobal += Run_OnDestroy;
            On.RoR2.SceneCatalog.OnActiveSceneChanged += SceneCatalog_OnActiveSceneChanged;
        }

        private void SetUpVisualsPrefab() {

            if (Modules.Config.disableStormVisuals.Value) {

                stormFXOBjprefab = new GameObject("stormFXsimple");
            } else {

                if (Modules.Config.enableOptimizedStormVisuals.Value) {

                    stormFXOBjprefab = rainObjPrefabOptimized;
                } else {

                    stormFXOBjprefab = rainObjPrefab;
                }
            }
        }

        private void Run_Start(Run run)
        {
            currentRun = run;

            //warning timer is needed by server to set up storm card but clients also need it to decide how long visuals take to fade in
            stormWarningTimer = 15f;
            DifficultyIndex diff = run.selectedDifficulty;
            if (diff == DifficultyIndex.Easy) stormWarningTimer = 30f;
            else if (diff == DifficultyIndex.Normal) stormWarningTimer = 25f;
            else if (diff == DifficultyIndex.Hard || (diff >= DifficultyIndex.Eclipse1 && diff <= DifficultyIndex.Eclipse8)) stormWarningTimer = 20f;
            else if (Modules.Config.TyphoonIncreaseSpawnCap.Value && diff == TyphoonCore.diffIdxTyphoon) stormWarningTimer = 15f;
            //for other modded difficulties try to guess how long the warning should be based on scaling
            else
            {
                float factor = (DifficultyCatalog.GetDifficultyDef(diff).scalingValue * 0.8f);
                if (factor == 0) factor = 1;
                stormWarningTimer = 30 / factor;

            }

            stormFXObj = SetUpVisuals();
            stormFXObj.transform.parent = run.transform;
            //ugh
            var curve = stormFXObj.GetComponent<PostProcessDuration>();
            curve.maxDuration += stormWarningTimer;
            //curve.ppWeightCurve.AddKey(stormWarningTimer / (stormDuration + stormWarningTimer), 1);

            director = run.gameObject.AddComponent<EventDirector>();

            var spawncard = ScriptableObject.CreateInstance<SpawnCard>();
            spawncard.directorCreditCost = Modules.Config.stormCreditCost.Value;
            spawncard.prefab = null;

            var events = new List<EventCard>();
            events.Add(new EventCard()
            {
                card = spawncard,
                duration = stormDuration,
                preActivationDuration = stormWarningTimer,
                OnSelected = WarnStorm,
                OnActivated = StartStorm,
                OnDeactivated = StopStorm
            });
            director.eventCards = events;
        }

        private void Run_OnDestroy(Run self)
        {
            if (director)
            {
                director.StopAllEvents();
            }
        }

        private void SceneCatalog_OnActiveSceneChanged(On.RoR2.SceneCatalog.orig_OnActiveSceneChanged orig, UnityEngine.SceneManagement.Scene oldScene, UnityEngine.SceneManagement.Scene newScene) {
            
            orig(oldScene, newScene);

            if (stormFXObj) {
                if (Modules.Config.disableStormVisuals.Value)
                    return;

                int particleIndex;
                switch (newScene.name)
                {
                    case "frozenwall":
                    case "itfrozenwall":
                        particleIndex = 1;
                        break;
                    case "goolake":
                    case "itgoolake":
                    case "drybasin":
                        particleIndex = 2;
                        break;
                    case "skymeadow":
                    case "itskymeadow":
                    case "slumberingsatellite":
                        particleIndex = 3;
                        break;
                    case "dampcavesimple":
                    case "itdampcavesimple":
                        particleIndex = 4;
                        break;
                    default:
                        particleIndex = 0;
                        break;
                }

                for (int i = 0; i < stormFXObj.transform.childCount; i++) {
                    stormFXObj.transform.GetChild(i).gameObject.SetActive(i == particleIndex);
                }
            }
        }

        private GameObject SetUpVisuals()
        {
            var fog = ScriptableObject.CreateInstance<RampFog>();
            fog.enabled.value = true;
            fog.enabled.overrideState = true;
            //fog.fogIntensity.value = 0.7f;
            //fog.fogIntensity.overrideState = true;
            fog.fogZero.value = -0.01f;
            fog.fogZero.overrideState = true;
            fog.fogOne.value = 0.169f;
            fog.fogOne.overrideState = true;
            fog.fogColorStart.value = new Color32(39, 42, 45, 16);
            fog.fogColorStart.overrideState = true;
            fog.fogColorMid.value = new Color32(58, 64, 72, 202);
            fog.fogColorMid.overrideState = true;
            fog.fogColorEnd.value = new Color32(75, 87, 100, 255);
            fog.fogColorEnd.overrideState = true;
            fog.skyboxStrength.value = 0;
            fog.skyboxStrength.overrideState = true;

            GameObject stormFXObj = stormFXOBjprefab.InstantiateClone("stormFX", false);
            stormFXObj.AddComponent<Components.StormSoundComponent>();

            stormFXObj.SetActive(false);
            stormFXObj.layer = 20;
                                                                                    
            PostProcessProfile prof = ScriptableObject.CreateInstance<PostProcessProfile>();
            prof.settings.Add(fog);

            stormPP = stormFXObj.AddComponent<PostProcessVolume>();
            stormPP.profile = prof;
            stormPP.sharedProfile = prof;
            stormPP.enabled = true;
            stormPP.isGlobal = true;
            stormPP.priority = 1000;

            var stormCurve = stormFXObj.AddComponent<PostProcessDuration>();
            stormCurve.maxDuration = stormDuration;
            stormCurve.ppVolume = stormPP;
            stormCurve.ppWeightCurve = new AnimationCurve();
            stormCurve.ppWeightCurve.AddKey(0, 0);
            stormCurve.ppWeightCurve.AddKey(0.05f, 1);
            stormCurve.ppWeightCurve.AddKey(0.95f, 1);
            stormCurve.ppWeightCurve.AddKey(1, 0);
            stormCurve.ppWeightCurve.preWrapMode = WrapMode.ClampForever;
            stormCurve.ppWeightCurve.postWrapMode = WrapMode.ClampForever;

            var weatherParticles = stormFXObj.AddComponent<WeatherParticles>();
            weatherParticles.resetPositionToZero = true;
            weatherParticles.lockRotation = false;

            return stormFXObj;
        }

        private void WarnStorm() {
            //activate fear
            Chat.SendBroadcastChat(new Chat.SimpleChatMessage() { baseToken = "<style=cWorldEvent><sprite name=\"CloudRight\">     A storm is approaching...</style>" });
            //activate pp
            showStormFX(true);
        }

        private void StartStorm()
        {

            if (Modules.Config.disableStormVisuals.Value) {
                Chat.SendBroadcastChat(new Chat.SimpleChatMessage() { baseToken = "(Storm started.)" });
            }
            //activate stat buffs
            RecalculateStatsAPI.GetStatCoefficients += StormStats;
            On.RoR2.DeathRewards.OnKilledServer += DeathRewards_OnKilledServer;
            foreach (TeamComponent tc in new List<TeamComponent>(TeamComponent.GetTeamMembers(TeamIndex.Monster)))
            {
                if (tc)
                    tc.GetComponent<CharacterBody>().statsDirty = true;
            }
            /*
             * bool wasAliveAtStormStart[players.count];
             * foreach (player in players) {
             *   if (player.isalive)
             *   wasAliveAtStormStart[player.index] = true;
             * }
             */
        }

        private void StopStorm() {
            if (Modules.Config.disableStormVisuals.Value) {
                Chat.SendBroadcastChat(new Chat.SimpleChatMessage() { baseToken = "(Storm ended.)" });
            }
            showStormFX(false);
            RecalculateStatsAPI.GetStatCoefficients -= StormStats;
            On.RoR2.DeathRewards.OnKilledServer -= DeathRewards_OnKilledServer;
            foreach (TeamComponent tc in new List<TeamComponent>(TeamComponent.GetTeamMembers(TeamIndex.Monster))) {
                if (tc)
                    tc.GetComponent<CharacterBody>().statsDirty = true;
            }
            /* 
             * foreach (player in players) {
             *   if (player.isalive && wasAliveAtStormStart[player.index])
             *     player.awardMuleAchievement();
             * }
             */
        }

        private void StormStats(CharacterBody sender, RecalculateStatsAPI.StatHookEventArgs args)
        {
            if (sender.teamComponent && sender.teamComponent.teamIndex != TeamIndex.Player)
            {
                args.armorAdd += 50f;
                //args.critAdd += 10f;
                args.attackSpeedMultAdd += 0.5f;
                args.damageMultAdd += 0.5f;
                args.moveSpeedMultAdd += 0.5f;
            }
        }

        private void showStormFX(bool shouldshow) {
            stormFXObj.SetActive(shouldshow);

            NetworkIdentity runIdentity = currentRun.GetComponent<NetworkIdentity>();
            if (runIdentity) {
                LogCore.LogInfo("currentRun networkidentity found. sending network message");
                new SyncStorms(runIdentity.netId, shouldshow).Send(NetworkDestination.Clients); ;
            } else {
                LogCore.LogInfo("currentRun networkidentity not found");
            }
        }

        private void DeathRewards_OnKilledServer(On.RoR2.DeathRewards.orig_OnKilledServer orig, DeathRewards self, DamageReport damageReport)
        {
            if (self)
                self.expReward = (uint)(self.expReward * 1.5f);
            orig(self, damageReport);
        }
    }

    public class EventCard
    {
        public SpawnCard card;
        public int cost
        {
            get
            {
                return card.directorCreditCost;
            }
        }
        //whether the event is currently active
        public bool isActive { get; private set; }
        //how long to wait after the event is selected to activate it
        public float preActivationDuration;
        //how long the event should be active for
        public float duration;
        //timestamp when the event is activated
        public float timeActivated;
        //whether the card should be considered for spawning (this is called when the director tries to spawn the card)
        public Func<bool> isAvailable = () => true;
        //function to call when the card is selected (event announced to player but not yet active)
        public Action OnSelected;
        //function to call when the card becomes active
        public Action OnActivated;
        //function to call when the event is deactivated, either by running for its duration or through force-deactivation
        public Action OnDeactivated;

        public void Select()
        {
            this.OnSelected();
        }

        public void Activate()
        {
            this.isActive = true;
            this.timeActivated = Time.time;
            this.OnActivated();
        }

        public void Deactivate()
        {
            this.isActive = false;
            this.OnDeactivated();
        }
    }

    public class EventDirector : MonoBehaviour
    {
        public float eventCredit;
        //should there ever be multiple instances of this
        public static readonly List<EventDirector> instancesList = new List<EventDirector>();

        //public DirectorCardCategorySelection eventCards;
        //private WeightedSelection<EventCard> eventSelection;
        public List<EventCard> eventCards;
        //private List<EventCard> activeEvents;
        private EventCard selectedEvent;
        private float eventSelectionTime;
        private Xoroshiro128Plus rng;
        private float interval;

        public void Awake()
        {
            if (NetworkServer.active)
            {
                //FIXME: not synced to run seed (do it like voidcore does it)
                this.rng = new Xoroshiro128Plus((ulong)Run.instance.stageRng.nextUint);
            }
        }

        private void OnEnable()
        {
            EventDirector.instancesList.Add(this);
        }

        private void OnDisable()
        {
            EventDirector.instancesList.Remove(this);
            //transfer credits to another instance if available
        }

        private void FixedUpdate()
        {
            if (NetworkServer.active && Run.instance && !StarstormPlugin.kingArenaActive)
            {
                if (!Run.instance.isRunStopwatchPaused)
                {
                    interval -= Time.fixedDeltaTime;
                    if (interval <= 0)
                    {
                        interval = rng.RangeFloat(1, 15);
                        float diff = DifficultyCatalog.GetDifficultyDef(Run.instance.selectedDifficulty).scalingValue;
                        float add = rng.RangeFloat(0, 1) * diff;
                        eventCredit += add;
                        //Debug.Log(eventCredit + " (+" + add + ")");

                        AttemptSpawnEvent();
                    }
                }

                //run active events
                foreach (EventCard card in eventCards)
                {
                    if (card.isActive)
                    {
                        if (Time.time - card.timeActivated > card.duration)
                        {
                            card.Deactivate();
                        }
                        else
                        {
                            //event things here
                        }
                    }

                }
            }
        }

        private void AttemptSpawnEvent()
        {
            if (selectedEvent is null)
            {
                EventCard card = eventCards[rng.RangeInt(0, eventCards.Count)];
                if (eventCredit >= card.cost && !card.isActive && card.isAvailable())
                {
                    eventCredit -= card.cost;
                    PrepareEvent(card);
                }
            }
            else if (Time.time - eventSelectionTime > selectedEvent.preActivationDuration)
            {
                selectedEvent.Activate();
                selectedEvent = null;
            }
        }

        private void PrepareEvent(EventCard card)
        {
            selectedEvent = card;
            selectedEvent.Select();
            eventSelectionTime = Time.time;
        }

        public int StopAllEvents()
        {
            int stoppedEventCount = 0;
            foreach (EventCard e in eventCards)
            {
                if (e.isActive)
                {
                    e.Deactivate();
                    stoppedEventCount++;
                }
            }
            return stoppedEventCount;
        }

        //FIXME: event console command broken again
        /*
        [ConCommand(commandName = "ss_force_event", flags = ConVarFlags.ExecuteOnServer, helpText = "Forces a gamewide event to begin.")]
        private static void ForceEvent(ConCommandArgs args)
        {
            var dir = EventsCore.instance.director;
            if (dir)
            {
                int? evArg = args.TryGetArgInt(0);
                if (evArg.HasValue)
                {
                    int evIdx = (int)evArg;
                    int evCt = dir.eventCards.Count() - 1;
                    if (evArg < 0 || evArg > evCt)
                    {
                        Debug.Log($"Argument out of range (must be 0..{evArg}).");
                        return;
                    }
                    if (!(dir.eventCards[evIdx].isActive || dir.selectedEvent == dir.eventCards[evIdx]))
                    {
                        dir.PrepareEvent(dir.eventCards[evIdx]);
                    }
                    else
                    {
                        Debug.Log($"Event {evIdx} is already active or queued.");
                    }
                }
                else
                {
                    Debug.Log("Command requires one int argument (event card index).");
                }
            }
            else
            {
                Debug.Log("Event director is unavailable! Cannot start any events.");
            }

        }

        [ConCommand(commandName = "ss_stop_events", flags = ConVarFlags.ExecuteOnServer, helpText = "Forces all active events to stop.")]
        private static void StopEvents(ConCommandArgs args)
        {
            var dir = EventsCore.instance.director;
            if (dir)
            {
                int ct = dir.StopAllEvents();
                Debug.Log($"Stopped {ct} events.");
            }
            else
            {
                Debug.Log("Event director is unavailable! Cannot start any events.");
            }
        }
        */
            }

    public class SyncStorms : INetMessage {

        private NetworkInstanceId bodyID;
        private bool stormActive;

        public SyncStorms() { }
        public SyncStorms(NetworkInstanceId netBodyID, bool netStormActive) {
            bodyID = netBodyID;
            stormActive = netStormActive;
        }

        public void OnReceived() {

            if (NetworkServer.active) {
                LogCore.LogWarning("SyncStorms: Host ran this. Skip");
                return;
            }

            GameObject stormFxObject = Util.FindNetworkObject(bodyID);
            if (!stormFxObject) {
                LogCore.LogWarning("SyncStorms: stormFxObject is null.");
                return;
            }

            LogCore.LogInfo("SyncStorms: running " + stormActive);

            stormFxObject.transform.GetChild(0).gameObject.SetActive(stormActive);
        }

        public void Serialize(NetworkWriter writer) {
            writer.Write(bodyID);
            writer.Write(stormActive);
        }
        public void Deserialize(NetworkReader reader) {
            bodyID = reader.ReadNetworkId();
            stormActive = reader.ReadBoolean();

        }
    }
}
