using Nakama;
using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Auroraland
{
    public class SpaceManager : MonoBehaviour
    {
        [Header("Spawn Root")]
        public Transform EnvRoot;
        public Transform GameRoot;
        public Transform InteractiveRoot;
        public Transform ActorsRoot;
        public Transform CurrentAvatarRoot;
        public Transform PlayerRoot;

        [Header("Hand Controllers")]
        public HandController RightHandController;

        [Header("Prefabs")]
        public GameObject NameTagPrefab;
        public GameObject ActorPrefab;
        public GameObject DeleteButtonPrefab;

        public VoiceXmlEditor VoiceXmlEditor;

        [Header("Settings")]
        public float EntityUpdateRate = 10f; // rates to update entity (unit in times/second)

        public string CurrentSpaceId { get; set; }
        public bool HasBlackJack { get; set; }
        public bool HasLeftSpace { get; private set; }
        public GameObject CurrentAvatar;
        private bool hasInitialized;
        private bool isDeleteModeOn;

        private Dictionary<string, GameObject> entityDict = new Dictionary<string, GameObject>();
        private Dictionary<string, GameObject> groupAssets = new Dictionary<string, GameObject>();
        private List<string> avatarEntityIds = new List<string>();

        //for checking the instantiate objects amount is equal to list got from server
        private readonly object counterLocker = new object();
        private int actorsCount;
        private int envObjCount;
        private int interactiveCount;
        private int gameCount;
        private bool isEditMode;

        // singleton pattern
        private static SpaceManager _instance;
        public static SpaceManager Instance { get { return _instance; } }

        public event EventHandler PlayerSpawned;
        public event EventHandler PlayerDeleted;

        void Awake()
        {
            // singleton pattern
            if (_instance != null && _instance != this)
            {
                Destroy(this.gameObject);
            }
            else
            {
                _instance = this;
            }
        }

        private void OnEnable()
        {
            // space related events handler
            NKController.Instance.JoinSpaceSuccess += OnJoinSpaceSuccess;
            NKController.Instance.LeaveSpaceSuccess += OnLeaveSpaceSuccess;

            // object related events handler
            NKController.Instance.ObjectAdd += OnObjectAdd;
            NKController.Instance.ObjectDelete += OnObjectDelete;
            NKController.Instance.ObjectUpdate += OnObjectUpdate;

            // player reltated event hander
            NKController.Instance.PlayerJoin += OnPlayerJoin;
            NKController.Instance.PlayerLeave += OnPlayerLeave;
            // actors related event handler
            NKController.Instance.ActorJoin += OnActorJoin;
            NKController.Instance.ActorLeave += OnActorLeave;
            NKController.Instance.ActorUpdate += OnActorUpdate;

            // game related event handler
            NKController.Instance.GameLoad += OnGameLoad;
            NKController.Instance.GameUnload += OnGameUnload;
        }

        private void OnDisable()
        {
            // space related events handler
            NKController.Instance.JoinSpaceSuccess -= OnJoinSpaceSuccess;
            NKController.Instance.LeaveSpaceSuccess -= OnLeaveSpaceSuccess;

            // object related events handler
            NKController.Instance.ObjectAdd -= OnObjectAdd;
            NKController.Instance.ObjectDelete -= OnObjectDelete;
            NKController.Instance.ObjectUpdate -= OnObjectUpdate;

            // player reltated event hander
            NKController.Instance.PlayerJoin -= OnPlayerJoin;
            NKController.Instance.PlayerLeave -= OnPlayerLeave;

            // actors related event handler
            NKController.Instance.ActorJoin -= OnActorJoin;
            NKController.Instance.ActorLeave -= OnActorLeave;
            NKController.Instance.ActorUpdate -= OnActorUpdate;

            // game related event handler
            NKController.Instance.GameLoad -= OnGameLoad;
            NKController.Instance.GameUnload -= OnGameUnload;
        }

        public void EnterSpace()
        {
            if (PlayerPrefs.HasKey("spaceId"))
            {
                CurrentSpaceId = PlayerPrefs.GetString("spaceId");
            }
            if (PlayerPrefs.HasKey("isEditMode"))
            {
                isEditMode = (PlayerPrefs.GetInt("isEditMode") == 1);
            }

            HasLeftSpace = false;

            NKGameManager.Instance.SpaceID = CurrentSpaceId;
            NKGameManager.Instance.UserID = NKController.Instance.GetLocalUserId();

            // Join current space
            NKController.Instance.JoinSpace(CurrentSpaceId);

            //real start of the scene
            StartCoroutine(SyncOutEntity());
        }

        // Sync Out Coroutine
        IEnumerator SyncOutEntity()
        {
            yield return new WaitUntil(() => entityDict != null && entityDict.Count > 0);

            while (true)
            {
                foreach (var entry in entityDict)
                {
                    if (!entry.Value.activeSelf)
                    {
                        continue;
                    }

                    var targetObject = entry.Value;
                    var netComponent = targetObject.GetComponent<NetObject>();
                    var entity = netComponent.GetNEntity();

                    if (netComponent.enabled && netComponent.AllowSyncOut && netComponent.IsManagedLocally())
                    {
                        NKController.Instance.UpdateEntity(entity);
                    }
                }
                yield return new WaitForSeconds(1f / EntityUpdateRate);
            }
        }


        // Space Event Handlers
        private void OnJoinSpaceSuccess(object sender, NKListArgs<INEntity> entityArgs)
        {
            ResetCounter();
            hasInitialized = false;
            StartCoroutine(LoadEntities(entityArgs.values));

        }
        private void OnLeaveSpaceSuccess(object sender, NKSingleArg<bool> arg)
        {
            HasLeftSpace = true;
            entityDict.Clear();
            groupAssets.Clear();
            avatarEntityIds.Clear();

            foreach (Transform child in InteractiveRoot)
            {
                Destroy(child.gameObject);
            }
            foreach (Transform child in GameRoot)
            {
                Destroy(child.gameObject);
            }
            foreach (Transform child in EnvRoot)
            {
                Destroy(child.gameObject);
            }
            foreach (Transform child in ActorsRoot)  //TODO check actors
            {
                Destroy(child.gameObject);
            }

            PlayerRoot.GetComponent<NetPlayer>().enabled = false;
            CurrentAvatarRoot.GetComponent<AvatarController>().enabled = false;
            Destroy(CurrentAvatar);

            if (PlayerDeleted != null)
            {
                PlayerDeleted(this, EventArgs.Empty);
            }
        }

        // Object Event Handlers
        private void OnObjectAdd(object sender, NKSingleArg<INEntity> entityArg)
        {
            var entity = entityArg.value;
            StartCoroutine(Spawn(entity));
        }

        private void OnObjectDelete(object sender, NKSingleArg<INEntity> entityArg)
        {
            var entity = entityArg.value;
            var target = entityDict[entity.Id];
            DecreaseCounterByType(entity);
            Destroy(target);
            Untrack(entity.Id);
        }

        private void OnObjectUpdate(object sender, NKSingleArg<INEntity> entityArg)
        {
            var entity = entityArg.value;

            if (entity.UserId == NKController.Instance.GetLocalUserId() || string.IsNullOrEmpty(entity.Id) || !HasEntity(entity.Id))
            {
                return;
            }

            var gameObj = entityDict[entity.Id];
            var netObj = gameObj.GetComponent<NetObject>();

            if (netObj.IsOwnedLocally() && netObj.GetNEntity().UserId != entity.UserId)
            {
                RightHandController.GetComponent<VRTK.VRTK_InteractGrab>().ForceRelease();
            }

            gameObj.GetComponent<NetObject>().SyncIn(entity);
        }

        // Player Event Handler
        private void OnPlayerJoin(object sender, NKSingleArg<INEntity> entityArg)
        {
            var entity = entityArg.value;
            StartCoroutine(Spawn(entity));
        }

        private void OnPlayerLeave(object sender, NKSingleArg<INEntity> entityArg)
        {
        }

        // Actor Event Handlers
        private void OnActorJoin(object sender, NKSingleArg<INEntity> entityArg)
        {
            var entity = entityArg.value;
            StartCoroutine(Spawn(entity));
        }

        private void OnActorLeave(object sender, NKSingleArg<INEntity> entityArg)
        {
            var entity = entityArg.value;
            var target = entityDict[entity.Id];

            if (entity.UserId != NKController.Instance.GetLocalUserId())
            {
                DecreaseCounterByType(entity);
            }

            Destroy(target);
            Untrack(entity.Id);
        }

        // TODO: probably should be different than object update
        private void OnActorUpdate(object sender, NKSingleArg<INEntity> entityArg)
        {
            var entity = entityArg.value;
            if (entity.UserId == NKController.Instance.GetLocalUserId() &&
                (!string.IsNullOrEmpty(entity.UserId) || !NKController.Instance.IsMasterClient()) || (string.IsNullOrEmpty(entity.Id) || !HasEntity(entity.Id)))
            {
                return;
            }

            var gameObj = entityDict[entity.Id];
            gameObj.GetComponent<NetObject>().SyncIn(entity);
        }

        // Game Event Handlers
        private void OnGameLoad(object sender, NKSingleArg<INEntity> entityArg)
        {
            var entity = entityArg.value;
            StartCoroutine(Spawn(entity));
            LoadDynamicVoiceCommandXml(isEditMode, true, true, true);
        }

        private void OnGameUnload(object sender, NKSingleArg<INEntity> entityArg)
        {
            var entity = entityArg.value;
            if (entity.UserId != NKController.Instance.GetLocalUserId())
            {
                var target = entityDict[entity.Id];
                DecreaseCounterByType(entity);
                if (entity.IsSegment) // destroy segment component
                {
                    long firstIndex = 0;
                    if (entity.SegmentId == firstIndex)
                    {
                        Destroy(target);
                        //TODO: check if this game is blackjack
                        LoadDynamicVoiceCommandXml(isEditMode, true, false, true);
                    }
                    else
                    {
                        Destroy(target);
                    }
                }
                else
                {
                    Destroy(target);
                }
                Untrack(entity.Id);
            }
        }

        void OnApplicationQuit()
        {
            NKController.Instance.LeaveSpace();
            PlayerPrefs.DeleteKey("spaceId");
        }

        private void Track(string entityId, GameObject obj)
        {
            entityDict.Add(entityId, obj);

            var netObj = obj.GetComponent<NetObject>();
            if (netObj is NetPlayer || netObj is NetActor)
            {
                avatarEntityIds.Add(entityId);
            }
        }

        private void Untrack(string entityId)
        {
            GameObject obj = entityDict[entityId];

            // remove entityId from avatarEntityIds
            var netObj = obj.GetComponent<NetObject>();
            if (netObj is NetPlayer || netObj is NetActor)
            {
                avatarEntityIds.Remove(entityId);
            }
            entityDict.Remove(entityId);
        }

        // for spatial sound
        public Dictionary<string, INEntity> GetAvatarEntityDict()
        {
            Dictionary<string, INEntity> dict = new Dictionary<string, INEntity>();
            foreach (var entityId in avatarEntityIds)
            {
                GameObject obj = entityDict[entityId];
                var netObj = obj.GetComponent<NetObject>();

                if (netObj)
                {
                    var entity = netObj.GetNEntity();

                    if (!dict.ContainsKey(entity.UserId))
                    {
                        dict.Add(entity.UserId, entity);
                    }
                }
            }
            return dict;
        }

        public GameObject GetObjectByEntityId(string entityId)
        {
            if (!entityDict.ContainsKey(entityId))
            {
                return null;
            }

            return entityDict[entityId];
        }

        private bool HasEntity(string entityId)
        {
            return entityDict.ContainsKey(entityId);
        }

        private void CreateNameTag(string displayName, Transform root, Vector3 position, bool isLocal)
        {
            GameObject nametag = Instantiate(NameTagPrefab, root);
            nametag.GetComponent<NameTag>().SetNameTag(displayName, position, isLocal);
        }

        IEnumerator LoadEntities(IList<INEntity> entities)
        {
            ResetCounter();

            foreach (var entity in entities)
            {
                if (entity.AssetType == "game")
                {
                    //TODO: check if blackjack asset exist
                    HasBlackJack = true;
                }
                StartCoroutine(Spawn(entity));
            }

            LoadDynamicVoiceCommandXml(isEditMode, true, HasBlackJack, true);

            yield return new WaitUntil(() => HasSpawnedAllAssets());

            foreach (Transform child in EnvRoot)
            {
                child.GetComponent<NetObject>().enabled = true;
                child.gameObject.SetActive(true);
            }

            foreach (NetObject child in InteractiveRoot.GetComponentsInChildren<NetObject>(true))
            {
                child.enabled = true;
                child.gameObject.SetActive(true);
            }

            foreach (NetObject child in GameRoot.GetComponentsInChildren<NetObject>(true))
            {
                child.enabled = true;
                child.gameObject.SetActive(true);
            }

            foreach (Transform child in ActorsRoot)
            {
                child.gameObject.SetActive(true);
                child.GetComponent<NetActor>().enabled = true;
                child.GetComponent<User>().enabled = true;
                child.GetComponentInChildren<IKController>().enabled = false;
                child.GetComponentInChildren<IKSetter>().enabled = true;
            }

            CurrentAvatar.SetActive(true);
            CurrentAvatar.GetComponent<IKController>().enabled = true;
            CurrentAvatar.GetComponent<IKSetter>().enabled = false;
            CurrentAvatarRoot.GetComponent<AvatarController>().enabled = true;
            PlayerRoot.GetComponent<NetPlayer>().enabled = true;
            PlayerRoot.GetComponent<User>().enabled = true;

            GameObject movieScreenGo = GameObject.Find("Movie_Screen");
            GameObject domeGo = GameObject.Find("Dome_Theater");
            GameObject imaxGo = GameObject.Find("Screen_03");
            GameObject therapyGo = GameObject.Find("Movie");
            GameObject t360movieGo = GameObject.Find("Room_Holder");

            if (movieScreenGo != null)
            {
                movieScreenGo.AddComponent<StandardTheaterVoiceControl>();
            }
            else if (domeGo != null)
            {
                domeGo.AddComponent<DomeTheaterVoiceControl>();
            }
            else if (imaxGo != null)
            {
                imaxGo.AddComponent<IMAXTheaterVoiceControl>();
            }
            else if (therapyGo != null)
            {
                therapyGo.AddComponent<TherapyVoiceControl>();
            }
            else if (t360movieGo != null) {
                t360movieGo.AddComponent<T360TheaterVoiceControl>();
            }

            hasInitialized = true;
        }

        private void ResetCounter()
        {
            //reset counts
            lock (counterLocker)
            {
                actorsCount = 0;
                envObjCount = 0;
                interactiveCount = 0;
                gameCount = 0;

            }
        }

        private void IncreaseCounterByType(INEntity entity)
        {
            lock (counterLocker)
            {
                switch (entity.AssetType)
                {
                    case "avatar":
                        if (entity.UserId != NKController.Instance.GetLocalUserId())
                        {
                            actorsCount++;
                        }
                        break;
                    case "prefab": interactiveCount++; break;
                    case "static": envObjCount++; break;
                    case "game": gameCount++; break;
                }
            }
        }

        private void DecreaseCounterByType(INEntity entity)
        {
            lock (counterLocker)
            {
                switch (entity.AssetType)
                {
                    case "avatar":
                        if (entity.UserId != NKController.Instance.GetLocalUserId())
                        {
                            actorsCount--;
                        }
                        break;
                    case "prefab": interactiveCount--; break;
                    case "static": envObjCount--; break;
                    case "game": gameCount--; break;
                }
            }
        }

        /**
        * The assets will be not be displayed until all of the assets in the list has been downloaded.  
		*/
        public IEnumerator Spawn(INEntity entity)
        {
            IncreaseCounterByType(entity);

            GameObject assetPrefab = AssetBundleDownloader.Instance.GetPrefab(entity.AssetUrl, entity.Version);
            if (assetPrefab == null)
            {
                AssetBundleDownloader.Instance.AddRequest(entity.AssetUrl, entity.Version);
                yield return new WaitUntil(() => AssetBundleDownloader.Instance.GetPrefab(entity.AssetUrl, entity.Version) != null);
                assetPrefab = AssetBundleDownloader.Instance.GetPrefab(entity.AssetUrl, entity.Version);
            }

            GameObject spawnedObj = null;
            switch (entity.AssetType)
            {
                case "prefab":
                case "game":
                case "static":
                    spawnedObj = SpawnObject(entity, assetPrefab);
                    break;
                case "avatar":
                    spawnedObj = entity.UserId == NKController.Instance.GetLocalUserId() ? SpawnPlayer(entity, assetPrefab) : SpawnActor(entity, assetPrefab);
                    break;
            }

            if (spawnedObj != null)
            {
                Track(entity.Id, spawnedObj);
            }
        }

        private GameObject SpawnObject(INEntity entity, GameObject asset)
        {
            Transform root = InteractiveRoot;
            switch (entity.AssetType)
            {
                case "prefab": root = InteractiveRoot; break;
                case "static": root = EnvRoot; break;
                case "game": root = GameRoot; break;
            }

            GameObject model = null;
            GameObject child = null;

            if (!string.IsNullOrEmpty(entity.GroupId))
            {  //this is a group asset

                if (!groupAssets.ContainsKey(entity.GroupId)) // this group has not be spawned yet
                {
                    model = Instantiate(asset, GameRoot);
                    groupAssets.Add(entity.GroupId, model);
                }
                else
                {
                    model = groupAssets[entity.GroupId];
                }
                //set the entity on asset that matches segment id
                List<Networking.SegmentId> segments = model.transform.GetComponentsInChildren<Networking.SegmentId>().ToList();
                child = segments.First(segment => segment.Id == entity.SegmentId).gameObject;
                NetObject childNetObject = child.AddComponent<NetObject>();
                if (childNetObject == null)
                {
                    childNetObject = child.AddComponent<NetObject>();
                }
                childNetObject.AllowSyncIn = true;
                childNetObject.AllowSyncOut = true;
                childNetObject.SetEntity(entity);
                childNetObject.enabled = hasInitialized;
                childNetObject.gameObject.SetActive(hasInitialized);
                if (child.GetComponent<MeshMerge>() == null)
                {
                    child.AddComponent<MeshMerge>();
                }
                child.GetComponent<MeshMerge>().Generate();
                if (entity.AssetType == "prefab")
                {   //change highlight color for showing mode for prefab
                    child.GetComponent<InteractableAsset>().EnableDeleteMode(isDeleteModeOn);
                }
            }
            else
            {
                model = Instantiate(asset, root);

                NetObject netObject = model.GetComponent<NetObject>();

                if (netObject == null)
                {
                    netObject = model.AddComponent<NetObject>();
                    netObject.AllowSyncIn = true;
                    netObject.AllowSyncOut = true;
                }

                netObject.SetEntity(entity);
                netObject.enabled = hasInitialized;
                model.SetActive(hasInitialized);

                // attach mesh merge for prefab, not game 
                if (entity.AssetType == "prefab")
                {
                    if (model.GetComponent<MeshMerge>() == null)
                    {
                        model.AddComponent<MeshMerge>();
                    }
                    model.GetComponent<MeshMerge>().Generate();
                    model.GetComponent<InteractableAsset>().EnableDeleteMode(isDeleteModeOn);
                }
            }

            // attach delete button for game
            if (entity.AssetType != "game" || !entity.SegmentId.Equals(0))
            {
                return !string.IsNullOrEmpty(entity.GroupId) ? child : model;
            }

            GameObject button = Instantiate(DeleteButtonPrefab);
            button.transform.SetParent((entity.IsSegment) ? child.transform : model.transform);
            button.transform.SetAsFirstSibling();
            button.GetComponent<AssetDeleteButton>().Target = (entity.IsSegment) ? child : model;
            button.SetActive(isDeleteModeOn);
            SendDownloadGameComplete(entity, Audience.MasterClient);

            return !string.IsNullOrEmpty(entity.GroupId) ? child : model;
        }

        private GameObject SpawnPlayer(INEntity entity, GameObject asset)
        {
            var root = PlayerRoot;
            CurrentAvatarRoot.GetComponent<AvatarSkinController>().SetAvatarModel(asset);
            CurrentAvatar = CurrentAvatarRoot.GetComponent<AvatarSkinController>().CurrentAvatar;
            root.GetComponent<NetPlayer>().SetModel(CurrentAvatar);
            root.GetComponent<NetPlayer>().SetEntity(entity);
            RightHandController.SetIKController(CurrentAvatar.GetComponent<IKController>());

            // CreateNameTag(entity.DisplayName, CurrentAvatarRoot, new Vector3(0, 2f, 0), true); //add name tag TODO set the position based on model's height
            CurrentAvatar.SetActive(hasInitialized);
            if (hasInitialized)
            {
                root.GetComponent<NetPlayer>().enabled = true;
                root.GetComponent<User>().enabled = true;
                CurrentAvatar.GetComponent<IKController>().enabled = true;
                CurrentAvatar.GetComponent<IKSetter>().enabled = false;
                CurrentAvatarRoot.GetComponent<AvatarController>().enabled = true;

            }

            if (PlayerSpawned != null)
            {
                PlayerSpawned(this, EventArgs.Empty);
            }

            return root.gameObject;
        }


        private GameObject SpawnActor(INEntity entity, GameObject asset)
        {
            GameObject actor = Instantiate(ActorPrefab, ActorsRoot); // instantiate actor root
            Transform root = actor.transform;
            GameObject model = Instantiate(asset, actor.transform);
            root.GetComponent<NetActor>().SetModel(model);
            root.GetComponent<NetActor>().SetEntity(entity);
            root.GetComponent<ActorController>().ChangeAvatar(model);
            CreateNameTag(entity.DisplayName, root.transform, new Vector3(0, 2f, 0), false); //add name tag TODO set the position based on model's height
            root.gameObject.SetActive(hasInitialized);

            if (!hasInitialized)
            {
                return actor;
            }

            root.GetComponent<NetActor>().enabled = true;
            root.GetComponent<User>().enabled = true;
            model.GetComponent<IKController>().enabled = false;
            model.GetComponent<IKSetter>().enabled = true;

            return actor;
        }

        /// <summary>
        /// Used to return object spawn rotation from FindOptimalSpawnPostion
        /// TODO: BETTER TO RETURN AN OBJECT WITH POSITION AND ROTATION
        /// </summary>
        private Quaternion objectSpawnRotation;

        // EventHandler when spawn asset button is clicked
        public void OnSpawnAssetButtonClicked(INAsset asset)
        {
            objectSpawnRotation = CurrentAvatar.transform.rotation;
            //Vector3 pos = FindOptimalSpawnPosition2(asset);
            //if (pos == Vector3.positiveInfinity) {
            //    Debug.Log("ERROR: COULD NOT SPAWN " + asset.Name);
            //    return;
            //}
            StartCoroutine(SendEntityCreate(asset, FindOptimalSpawnPosition(asset),
                (string.Equals(asset.Name, "Chess Set")) ?
                Quaternion.identity :   // HACK: USED TO SOLVE CHESS PIECES MISALIGNED WITH CHESSBOARD WHEN ROTATED
                objectSpawnRotation) // other objects will spawn rotated the same as the player
            );
        }

        /// <summary>
        /// Searches the scene for an obstacle-free position for the asset to spawn
        /// </summary>
        /// <param name="asset">The asset to be spawned</param>
        /// <returns>The optimal position to spawn</returns>
        /// TODO: CLEAN UP THIS ROUTINE, EXPERIMENTAL BUT WORKING
        //private Vector3 FindOptimalSpawnPosition2(INAsset asset)
        //{
        //    float objectOffset = 0.0125f;   // distance between bounding box checks later
        //    Quaternion rotation = CurrentAvatar.transform.rotation;
        //    GameObject assetPrefab = AssetBundleDownloader.Instance.GetPrefab(asset.Url, asset.Version);
        //    Bounds bounds = GetBoundingBox(assetPrefab, rotation);
        //    float yBoundsOffset = -bounds.center.y; // to solve discrepancy between "pivot" and "center" transform positions
        //    Vector3 halfExtents = bounds.extents / 2.0f;
        //    Vector3 spawnPosition = Vector3.zero;

        //    int startSides = 3;
        //    int sides = startSides;
        //    int maxSides = 5;
        //    int yTrial = 0;
        //    int maxY = 4;

        //    // Draws various sized n-gons around player based on object sizes to find optimal position, rotation
        //    while (yTrial <= maxY) // Y trials
        //    {
        //        ++yTrial;
        //        for (int side = sides; side <= maxSides; side++) // poly trials
        //        {
        //            float degrees = 360.0f / side;
        //            float radius = (bounds.size.x / Mathf.Sin(Mathf.PI / side)) / 2.0f;

        //            Vector3 direction = CurrentAvatar.transform.forward;
        //            rotation = CurrentAvatar.transform.rotation;
        //            for (int i = 0; i < side; i++) // side trials
        //            {
        //                spawnPosition = CurrentAvatar.transform.position + direction * radius;
        //                spawnPosition.y = GetTerrainHeightAt(spawnPosition);
        //                spawnPosition.y += ((objectOffset + bounds.extents.y) * yTrial);

        //                if (debugSpawning)
        //                { // add bounds to lists to draw wire cubes later
        //                    boundPositions.Add(spawnPosition);
        //                    boundScales.Add(bounds.size);
        //                }

        //                LayerMask layerMask = ~LayerMask.GetMask("UI");
        //                Collider[] colliders = Physics.OverlapBox(spawnPosition, halfExtents, rotation, layerMask, QueryTriggerInteraction.Ignore);
        //                if (colliders.Length > 0)
        //                { // check if bounding box overlaps any colliders
        //                    Debug.Log("COLLISION: " + colliders[0].name);
        //                    direction = Quaternion.AngleAxis(degrees, Vector3.up) * direction;
        //                    rotation = Quaternion.LookRotation(direction);
        //                }
        //                else
        //                {
        //                    objectSpawnRotation = rotation;
        //                    goto POSITION_FOUND;
        //                }
        //            }
        //        }
        //    }

        //    return Vector3.positiveInfinity;

        //    POSITION_FOUND:

        //    spawnPosition.y += yBoundsOffset;
        //    return spawnPosition;
        //}

        /// <summary>
        /// Searches the scene for an obstacle-free position for the asset to spawn
        /// </summary>
        /// <param name="asset">The asset to be spawned</param>
        /// <returns>The optimal position to spawn</returns>
        private Vector3 FindOptimalSpawnPosition(INAsset asset)
        {
            float objectOffset = 0.0125f;   // distance between bounding box checks later
            float minSpawnDistance = 2.0f;     // distance in front of player to spawn object
            Vector3 spawnPosition = CurrentAvatar.transform.position + CurrentAvatar.transform.forward * minSpawnDistance;
            spawnPosition.y = GetTerrainHeightAt(spawnPosition);

            GameObject assetPrefab = AssetBundleDownloader.Instance.GetPrefab(asset.Url, asset.Version);
            Bounds bounds = GetBoundingBox(assetPrefab, CurrentAvatar.transform.rotation);
            if (bounds.extents.z > minSpawnDistance) // object will clip player, so move it further away
            {
                spawnPosition += CurrentAvatar.transform.forward * (bounds.extents.z - minSpawnDistance + objectOffset);
            }

            float yBoundsOffset = -bounds.center.y; // to solve discrepancy between "pivot" and "center" transform positions
            Vector3 halfExtents = bounds.extents / 2.0f;

            // start testing bounds for collisions at spawnPosition in front of player
            bounds.center = RaiseYBoundsBy(objectOffset + bounds.extents.y, spawnPosition);

        COLLISION_CHECK: // Check if bounds will intersect other colliders, stacking bounds to find free space

            if (debugSpawning)
            { // add bounds to lists to draw wire cubes later
                boundPositions.Add(bounds.center);
                boundScales.Add(bounds.size);
            }

            LayerMask layerMask = ~LayerMask.GetMask("UI");
            Collider[] colliders = Physics.OverlapBox(bounds.center, halfExtents, Quaternion.identity, layerMask, QueryTriggerInteraction.Ignore);
            if (colliders.Length > 0)
            { // check if bounding box overlaps any colliders
                Debug.Log("COLLISION: " + colliders[0].name);
                // stack bounds on top of each other until we find a clear space to spawn
                float colliderCenterY = colliders[0].bounds.center.y;
                float colliderExtentsY = colliders[0].bounds.extents.y;
                // find next Y position to try testing bounds at
                float goalY = colliderCenterY + colliderExtentsY + bounds.extents.y + objectOffset;
                bounds.center = RaiseYBoundsBy(goalY - bounds.center.y, bounds.center);
                goto COLLISION_CHECK;
            }

            spawnPosition = RaiseYBoundsBy(yBoundsOffset, bounds.center);
            return spawnPosition;
        }

        /// <summary>
        /// Returns the new center position for Bounds with Y position incremented by value
        /// </summary>
        /// <param name="value">The value to increment Y position by</param>
        /// <param name="boundsCenter">The original center bounds position</param>
        /// <returns></returns>
        private Vector3 RaiseYBoundsBy(float value, Vector3 boundsCenter)
        {
            boundsCenter.y += value;
            return boundsCenter;
        }

        /// <summary>
        /// Returns the bounding box of an asset GameObject based on MeshRenderers
        /// </summary>
        /// <param name="assetPrefab">The GameObject of an asset prefab</param>
        /// <returns>The Bounds of assetPrefab</returns>
        private Bounds GetBoundingBox(GameObject assetPrefab, Quaternion rotation)
        {
            GameObject assetObject = Instantiate(assetPrefab, Vector3.zero, rotation);
            Bounds bounds = new Bounds();
            MeshRenderer[] meshRenderers = assetObject.GetComponentsInChildren<MeshRenderer>();
            foreach (var meshRenderer in meshRenderers)
            {
                bounds.Encapsulate(meshRenderer.bounds); // grow bounds to include all meshes
            }
            Destroy(assetObject);
            return bounds;
        }

        /// <summary>
        /// Returns the world height of the terrain at a given position
        /// </summary>
        /// <param name="position">Position in world space</param>
        /// <returns>Height (y-axis) of terrain if one exists, else position.y</returns>
        private float GetTerrainHeightAt(Vector3 position)
        {
            List<RaycastHit> hits = new List<RaycastHit>();
            hits.AddRange(Physics.RaycastAll(position, Vector3.down));
            hits.AddRange(Physics.RaycastAll(position, Vector3.up));
            foreach (var hit in hits)
            {
                if (hit.collider.GetType() != typeof(TerrainCollider)) continue;
                Terrain terrain = hit.collider.gameObject.GetComponent<Terrain>();
                return terrain.SampleHeight(position);
            }
            return position.y;
        }

        /// <summary>
        /// Debugging spawning by drawing wire cubes for bounding boxes
        /// </summary>
        private readonly bool debugSpawning = true;
        private List<Vector3> boundPositions = new List<Vector3>();
        private List<Vector3> boundScales = new List<Vector3>();
        private void OnDrawGizmos()
        {
            if (debugSpawning)
            {
                Gizmos.color = Color.red;
                for (int i = 0; i < boundPositions.Count; i++)
                {
                    Gizmos.DrawWireCube(boundPositions[i], boundScales[i]);
                }
            }
        }

        private IEnumerator SendEntityCreate(INAsset asset, Vector3 position, Quaternion rotation)
        {
            EntityMeta meta = JsonConvert.DeserializeObject<EntityMeta>(asset.Metadata);
            EntityMeta entityMeta = new EntityMeta();
            entityMeta.Static = meta.Static;
            string metaJson = JsonConvert.SerializeObject(entityMeta);

            //TODO game should be able to have multiple sets in space
            NKController.Instance.CreateEntity(CurrentSpaceId, position, rotation.eulerAngles, Vector3.one, asset.Id, false, metaJson);
            yield return null;
        }
        // End of EventHandler for spawn asset button

        public void ToggleDeleteMode()
        {
            isDeleteModeOn = !isDeleteModeOn;
            SetDeleteMode(isDeleteModeOn);
        }

        public void SetDeleteMode(bool isOn)
        {
            isDeleteModeOn = isOn;
            foreach (InteractableAsset asset in InteractiveRoot.GetComponentsInChildren<InteractableAsset>(true))
            {
                asset.EnableDeleteMode(isDeleteModeOn);
            }

            foreach (AssetDeleteButton deleteBtn in GameRoot.GetComponentsInChildren<AssetDeleteButton>(true))
            {
                deleteBtn.gameObject.SetActive(isDeleteModeOn);
            }
        }

        /*send ready event message to master client to notify that this player has finished downloading this asset*/
        public void SendDownloadGameComplete(INEntity entity, Audience audience, string excludeId = "")
        {
            var e = new NReadyEvent { EntityId = entity.Id, UserId = NKController.Instance.GetLocalUserId(), Metadata = "{}" };

            var msg = new NEventSendMessage.Builder(entity.Id)
                .Audience(audience)
                .ReadyEvent(e)
                .Exclude(excludeId)
                .Build();

            NKController.Instance.Send(msg, (bool success) =>
                {
                }, (INError error) =>
                {
                });

        }

        private void LoadDynamicVoiceCommandXml(bool hasAssetEditor = false, bool hasTheater = false, bool hasBlackjack = false, bool hasTherapy = false, bool hasUserDefined = false)
        {
            VoiceXmlEditor.CreateXML(hasAssetEditor, hasTheater, hasBlackjack, hasTherapy, hasUserDefined);
        }

        public bool HasSpawnedAllAssets()
        {
            //TODO too strict
            bool value = EnvRoot.childCount == envObjCount
                         && InteractiveRoot.GetComponentsInChildren<NetObject>(true).Length == interactiveCount
                         && GameRoot.GetComponentsInChildren<NetObject>(true).Length == gameCount
                         && ActorsRoot.childCount == actorsCount && CurrentAvatar != null;
            return value;
        }
    }
}
