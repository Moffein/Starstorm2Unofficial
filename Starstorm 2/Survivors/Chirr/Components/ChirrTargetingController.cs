using RoR2;
using System.Linq;
using UnityEngine;
using UnityEngine.Networking;

namespace Starstorm2.Survivors.Chirr.Components
{
    public class ChirrTargetingController : NetworkBehaviour
    {
        public static GameObject indicatorCannotBefriendPrefab;
        public static GameObject indicatorReadyToBefriendPrefab;
        public static GameObject indicatorFriendPrefab;

        private Indicator indicatorCannotBefriend;
        private Indicator indicatorReadyToBefriend;
        private Indicator indicatorFriend;

        private HurtBox trackingTarget;
        private float trackerUpdateStopwatch;
        private readonly BullseyeSearch search = new BullseyeSearch();

        private InputBankTest inputBank;
        private CharacterBody characterBody;
        private TeamComponent teamComponent;

        public float trackerUpdateFrequency = 4f;
        public float maxTrackingDistance = 90f;
        public float maxTrackingAngle = 60f;

        [SyncVar]
        private bool _canBefriendTarget = false;

        [SyncVar]
        private bool _hasFriend = false;

        [SyncVar]
        private uint _trackingTargetMasterNetID;

        private CharacterBody targetBody;
        private CharacterMaster targetMaster;

        private void Awake()
        {
            characterBody = base.GetComponent<CharacterBody>();
            teamComponent = base.GetComponent<TeamComponent>();
            inputBank = base.GetComponent<InputBankTest>();

            this.indicatorCannotBefriend = new Indicator(base.gameObject, indicatorCannotBefriendPrefab);
            this.indicatorReadyToBefriend = new Indicator(base.gameObject, indicatorReadyToBefriendPrefab);
            this.indicatorFriend = new Indicator(base.gameObject, indicatorFriendPrefab);

            trackerUpdateStopwatch = 0f;
        }

        private void FixedUpdate()
        {
            if (NetworkServer.active)
            {
                //Server updates what you're supposed to be targeting
                FixedUpdateServer();
            }

            //Client tries to figure out what the server wants it to target.
            ResolveTargetOnClient();
        }

        private void ResolveTargetOnClient()
        {
            bool hasValidTarget = false;

            if (_trackingTargetMasterNetID != NetworkInstanceId.Invalid.Value)
            {
                GameObject networkMasterObject = Util.FindNetworkObject(new NetworkInstanceId(_trackingTargetMasterNetID));
                if (networkMasterObject)
                {
                    targetMaster = networkMasterObject.GetComponent<CharacterMaster>();
                    if (targetMaster)
                    {
                        targetBody = targetMaster.GetBody();
                        if (targetBody && targetBody.mainHurtBox)
                        {
                            hasValidTarget = true;
                        }
                    }
                }
            }

            if (hasValidTarget)
            {
                this.indicatorCannotBefriend.targetTransform = targetBody.mainHurtBox.transform;
                this.indicatorReadyToBefriend.targetTransform = targetBody.mainHurtBox.transform;
                this.indicatorFriend.targetTransform = targetBody.mainHurtBox.transform;

                if (this._hasFriend)
                {
                    this.indicatorFriend.active = true;
                    this.indicatorCannotBefriend.active = false;
                    this.indicatorReadyToBefriend.active = false;
                }
                else
                {
                    if (this._canBefriendTarget)
                    {
                        this.indicatorReadyToBefriend.active = true;
                        this.indicatorFriend.active = false;
                        this.indicatorCannotBefriend.active = false;
                    }
                    else
                    {
                        this.indicatorCannotBefriend.active = true;
                        this.indicatorFriend.active = false;
                        this.indicatorReadyToBefriend.active = false;
                    }
                }
            }
            else
            {
                targetMaster = null;
                targetBody = null;

                this.indicatorCannotBefriend.active = false;
                this.indicatorFriend.active = false;
                this.indicatorReadyToBefriend.active = false;
            }
        }

        [Server]
        private void FixedUpdateServer()
        {
            CheckTargetAliveServer();
            if (!_hasFriend)
            {
                trackerUpdateStopwatch += Time.fixedDeltaTime;
                if (trackerUpdateStopwatch >= 1f / trackerUpdateFrequency)
                {
                    trackerUpdateStopwatch = 0f;
                    HurtBox newHurtbox = SearchForTarget(inputBank.GetAimRay());
                    if (newHurtbox != trackingTarget)
                    {
                        ChangeTrackingTargetServer(newHurtbox);
                    }
                }
            }
        }

        [Server]
        private void CheckTargetAliveServer()
        {
            if (!trackingTarget || !(trackingTarget.healthComponent && trackingTarget.healthComponent.alive))
            {
                trackingTarget = null;
                if (_hasFriend) _hasFriend = false;
                _trackingTargetMasterNetID = NetworkInstanceId.Invalid.Value;
            }
        }

        [Server]
        private void ChangeTrackingTargetServer(HurtBox newHurtbox)
        {
            if (newHurtbox && newHurtbox.healthComponent && newHurtbox.healthComponent.body && newHurtbox.healthComponent.body.master)
            {
                uint newID = newHurtbox.healthComponent.body.master.netId.Value;
                if (newID != NetworkInstanceId.Invalid.Value && newID != _trackingTargetMasterNetID)
                {
                    _trackingTargetMasterNetID = newID;
                    trackingTarget = newHurtbox;
                }
            }
        }

        private HurtBox SearchForTarget(Ray aimRay)
        {
            this.search.teamMaskFilter = TeamMask.GetEnemyTeams(teamComponent.teamIndex);
            this.search.filterByLoS = true;
            this.search.searchOrigin = aimRay.origin;
            this.search.searchDirection = aimRay.direction;
            this.search.sortMode = BullseyeSearch.SortMode.Angle;
            this.search.maxDistanceFilter = this.maxTrackingDistance;
            this.search.maxAngleFilter = this.maxTrackingAngle;
            this.search.RefreshCandidates();
            this.search.FilterOutGameObject(base.gameObject);
            return this.search.GetResults().FirstOrDefault<HurtBox>();
        }
    }
}
