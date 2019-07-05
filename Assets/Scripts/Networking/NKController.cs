using System;
using Nakama;
using System.Collections.Generic;
using UnityEngine;

namespace Auroraland
{
    public class NKController : NKMessenger
    {

        private INSelf localUserSelf;
        private string selectedAvatarId;
        private string currentSpaceId;
        private string currentAvatarEntityId;
        private int userSeq;
        private long spaceSeq;
        private string userSpaceId;

        [Header("Settings")]
        public float SyncRate = 10f; // rate for send out entity sync messages (unit in times/seconds)

        private Dictionary<string, INEntity> localEntities = new Dictionary<string, INEntity>();
        private Dictionary<string, INEntity> latestLocalEntities = new Dictionary<string, INEntity>();

        private int PAGE_LIMIT = 100;

        // singleton pattern
        private static NKController _instance;
        public static NKController Instance { get { return _instance; } }
        private void Awake()
        {
            // singleton pattern
            if (_instance != null && _instance != this)
            {
                Destroy(this.gameObject);
            }
            else
            {
                _instance = this;
                DontDestroyOnLoad(this.gameObject);
            }
            StartCoroutine(entityDeltaSync());
        }

        void Start()
        {
            localUserId = "";
            currentSpaceId = "";
            currentAvatarEntityId = "";
            this.ErrorHappened += this.GlobalErrorHandler;
            this.PlayerJoin += this.OnPlayerJoin;
            this.PlayerLeave += this.OnPlayerLeave;
        }

        // Getters & Setters
        public int GetUserSeq()
        {
            return userSeq;
        }

        public long GetSpaceSeq()
        {
            return spaceSeq;
        }

        public string GetSpaceId() {
            return userSpaceId;
        }

        public INSelf GetSelf()
        {
            return localUserSelf;
        }

        // Global Error Handler
        public event EventHandler<NKErrorArgs> ErrorHappened;
        public void GlobalErrorHandler(object sender, NKErrorArgs errArgs)
        {
            DebugLogger.LogErrorFormat("Error: code '{0}' with '{1}'.", errArgs.code, errArgs.message);
        }

        // Load Space List
        public event EventHandler<NKListArgs<INSpace>> LoadSpaceListSuccess;
        public event EventHandler<NKErrorArgs> LoadSpaceListFailure;

        public void LoadSpaceList(bool loadMySpaceOnly = false)
        {
            var builder = new NSpacesListMessage.Builder();
            if (loadMySpaceOnly && localUserId != "")
            {
                builder.FilterByCreatorId(localUserId);
            }
            builder.PageLimit(PAGE_LIMIT);
            var message = builder.Build();

            this.Send(message, (INResultSet<INSpace> spaces) =>
            {
                DebugLogger.Log("Successfully listed spaces");
                if (LoadSpaceListSuccess != null)
                    LoadSpaceListSuccess(this, new NKListArgs<INSpace>(spaces));
            }, (INError err) =>
            {
                ErrorHappened(this, new NKErrorArgs(err));
                if (LoadSpaceListFailure != null) LoadSpaceListFailure(this, new NKErrorArgs(err));
            });
        }

        //Load Avatar List
        public event EventHandler<NKListArgs<INAvatar>> LoadAvatarListSuccess;
        public event EventHandler<NKErrorArgs> LoadAvatarListFailure;

        public void LoadAvatarList()
        {
            var message = NAvatarsListMessage.Default();
            this.Send(message, (INResultSet<INAvatar> avatars) =>
            {
                if(LoadAvatarListSuccess != null)LoadAvatarListSuccess(this, new NKListArgs<INAvatar>(avatars));
            }, (INError err) =>
            {
                ErrorHappened(this, new NKErrorArgs(err));
                if (LoadAvatarListFailure != null) LoadAvatarListFailure(this, new NKErrorArgs(err));
            });
        }

        // Choose Avatar
        public event EventHandler<NKSingleArg<bool>> ChooseAvatarSuccess;
        public event EventHandler<NKErrorArgs> ChooseAvatarFailure;

        public void ChooseAvatar(string avatarId)
        {
            var message = NAvatarChooseMessage.Default(avatarId);
            this.Send(message, (bool success) =>
            {
                selectedAvatarId = avatarId;
                if (ChooseAvatarSuccess != null)
                {
                    ChooseAvatarSuccess(this, new NKSingleArg<bool>(true));
                }
            }, (INError err) =>
            {
                var errorArgs = new NKErrorArgs(err);
                ErrorHappened(this, errorArgs);
                if(ChooseAvatarFailure != null) ChooseAvatarFailure(this, errorArgs);
            });
        }

        //Load Asset List
        public event EventHandler<NKListArgs<INAsset>> LoadAssetListSuccess;
        public event EventHandler<NKErrorArgs> LoadAssetListFailure;


        public void LoadAssetList(string typeFilter = "", string categoryFilter = "")
        {
            var builder = new NAssetsListMessage.Builder().PageLimit(PAGE_LIMIT);
            if (typeFilter != "")
            {
                builder = builder.FilterByType(typeFilter);
            }
            if (categoryFilter != "")
            {
                builder = builder.FilterByCategory(categoryFilter);
            }
            if (categoryFilter=="" && typeFilter == "")
            {
                builder = builder.FilterByType("prefab");
            }
            
            var message = builder.Build();
            this.Send(message, (INResultSet<INAsset> assets) =>
            {
                Debug.Log("Successfully listed assets");
                if(LoadAssetListSuccess != null)LoadAssetListSuccess(this, new NKListArgs<INAsset>(assets));
            }, (INError err) =>
            {
                var errorArgs = new NKErrorArgs(err);
                ErrorHappened(this, errorArgs);
                if(LoadAssetListFailure != null) LoadAssetListFailure(this, errorArgs);
            });
        }

        // Load Self Info
        public event EventHandler<NKSingleArg<INSelf>> LoadSelfInfoSuccess;
        public event EventHandler<NKErrorArgs> LoadSelfInfoFailure;

        public void LoadSelfInfo()
        {
            var message = NSelfFetchMessage.Default();
            this.Send(message, (INSelf self) =>
            {
                this.localUserSelf = self;

                if (LoadSelfInfoSuccess != null)
                    LoadSelfInfoSuccess(this, new NKSingleArg<INSelf>(self));
            }, (INError err) =>
            {
                var errorArgs = new NKErrorArgs(err);
                ErrorHappened(this, errorArgs);
                if (LoadSelfInfoFailure != null) LoadSelfInfoFailure(this, errorArgs);
            });
        }


        public void UpdateSelfInfo(string displayName, string profileUrl, float height, string heightUnit)
        {
            string meta = "{\"height\":" + height + ", \"heightUnit\":\"" + heightUnit + "\"}";
            var message = new NSelfUpdateMessage.Builder().
                Fullname(displayName).
                AvatarUrl(profileUrl).
                Metadata(meta).Build();

            this.Send(message, (bool completed) =>
            {
                Debug.Log("Successfully update user information");
                LoadSelfInfo();

            }, (INError err) =>
            {
                var errorArgs = new NKErrorArgs(err);
                ErrorHappened(this, errorArgs);
            });
        }

        // Load User Info
        public event EventHandler<NKSingleArg<INUser>> LoadUserInfoSuccess;
        public event EventHandler<NKErrorArgs> LoadUserInfoFailure;

        public void LoadUserInfo(string userId)
        {
            var message = NUsersFetchMessage.ById(new string[] { userId });
            Send(message, 
                (INResultSet<INUser> results) =>
            {
                if (LoadUserInfoSuccess != null) LoadUserInfoSuccess(this, new NKSingleArg<INUser>(results.Results[0]));
            }, 
                (INError err) =>
            {
                var errorArgs = new NKErrorArgs(err);
                ErrorHappened(this, errorArgs);
                if(LoadUserInfoFailure != null) LoadUserInfoFailure(this, errorArgs);
            });
        }


        // Space Join
        public event EventHandler<NKListArgs<INEntity>> JoinSpaceSuccess;
        public event EventHandler<NKErrorArgs> JoinSpaceFailure;

        public void JoinSpace(string spaceId)
        {
            var message = Nakama.NSpaceJoinMessage.Default(spaceId);
            this.Send(message, (INResultSet<INSpaceJoinAck> result) =>
            {
                DebugLogger.Log("Successfully joined a space");
                var entities = result.Results[0].Entities;
                currentSpaceId = spaceId;
                userSeq = result.Results[0].UserSeq;
                spaceSeq = result.Results[0].SpaceSeq;
                userSpaceId = result.Results[0].SpaceId;
                if(JoinSpaceSuccess != null)JoinSpaceSuccess(this, new NKListArgs<INEntity>(entities));
            }, (Nakama.INError err) =>
            {
                var errorArgs = new NKErrorArgs(err);
                ErrorHappened(this, errorArgs);
                if(JoinSpaceFailure != null)JoinSpaceFailure(this, errorArgs);
            });
        }

        // Space Leave
        public event EventHandler<NKSingleArg<bool>> LeaveSpaceSuccess;
        public event EventHandler<NKErrorArgs> LeaveSpaceFailure;

        public void LeaveSpace()
        {
            var message = Nakama.NSpaceLeaveMessage.Default(currentSpaceId);
            this.Send(message, (bool success) =>
            {
                DebugLogger.Log("Successfully left a space");
                currentSpaceId = null;
                if(LeaveSpaceSuccess != null)LeaveSpaceSuccess(this, new NKSingleArg<bool>(true));
                userSeq = 0;
                spaceSeq = 0;

                // Clear entities list
                lock (localEntities)
                {
                    localEntities.Clear();
                }
                lock (latestLocalEntities)
                {
                    latestLocalEntities.Clear();
                }
            }, (Nakama.INError err) =>
            {
                var errorArgs = new NKErrorArgs(err);
                ErrorHappened(this, errorArgs);
                if(LeaveSpaceFailure != null)LeaveSpaceFailure(this, errorArgs);
            });
        }

        // Create Entity
        // event will be triggered by SpacePresence
        public void CreateEntity(string spaceId, Vector3 position, Vector3 rotation, Vector3 scale, string assetId, bool userOwned, string metaData)
        {
            var message = new Nakama.NEntityCreateMessage.Builder().
                SpaceId(spaceId).
                Position(NakamaTypeConverter.Vector3ToNVector3(position)).
                Rotation(NakamaTypeConverter.Vector3ToNVector3(rotation)).
                Scale(NakamaTypeConverter.Vector3ToNVector3(scale)).
                AssetId(assetId).
                Owned(userOwned).
                Metadata(metaData).
                Build();

            this.Send(message, (Nakama.INResultSet<Nakama.INEntity> entities) =>
            {
                // ignore
            }, (Nakama.INError err) =>
            {
                var errorArgs = new NKErrorArgs(err);
                ErrorHappened(this, errorArgs);
            });
        }

        // Delete Entity
        // event will be triggered by SpacePresence
        public void DeleteEntity(INEntity entity)
        {
            var message = new Nakama.NEntityDeleteMessage.Builder(entity.SpaceId).
                EntityId(entity.Id).
                Build();

            this.Send(message, (bool success) =>
            {
                // ignore
            }, (Nakama.INError err) =>
            {
                var errorArgs = new NKErrorArgs(err);
                ErrorHappened(this, errorArgs);
            });

        }

        // Space Create
        public event EventHandler<NKSingleArg<INSpace>> CreateSpaceSuccess;
        public event EventHandler<NKErrorArgs> CreateSpaceFailure;

        public void CreateSpace(string spaceName, string description, string assetId)
        {
            var message = new NSpaceCreateMessage.Builder(spaceName)
                .Description(description)
                .AssetId(assetId)
                .Metadata("{}").Build();
            Debug.Log(message.ToString());
            this.Send(message, (INResultSet<INSpace> spaces) =>
             {
                 if(CreateSpaceSuccess != null)CreateSpaceSuccess(this, new NKSingleArg<INSpace>(spaces.Results[0]));
             }, (INError err) =>
             {
                 var errorArgs = new NKErrorArgs(err);
                 ErrorHappened(this, errorArgs);
                 if(CreateSpaceFailure != null)CreateSpaceFailure(this, errorArgs);
             });
        }

        // Space Update
        public event EventHandler<NKSingleArg<INSpace>> UpdateSpaceSuccess;
        public event EventHandler<NKErrorArgs> UpdateSpaceFailure;

        public void UpdateSpace(INSpace space)
        {
            var message = new NSpaceUpdateMessage.Builder(space.Id)
                .DisplayName(space.DisplayName)
                .Description(space.Description)
                .ThumbnailUrl(space.ThumbnailUrl).Build();

            this.Send(message, (bool success) =>
            {
                if(UpdateSpaceSuccess != null)UpdateSpaceSuccess(this, new NKSingleArg<INSpace>(space));
            }, (INError err) =>
            {
                var errorArgs = new NKErrorArgs(err);
                ErrorHappened(this, errorArgs);
                if(UpdateSpaceFailure != null)UpdateSpaceFailure(this, errorArgs);
            });
        }


        // Space Create
        public event EventHandler<NKSingleArg<string>> DeleteSpaceSuccess;
        public event EventHandler<NKErrorArgs> DeleteSpaceFailure;

        public void DeleteSpace(string spaceId)
        {
            var message = NSpaceDeleteMessage.Default(spaceId);
            this.Send(message, (bool success) => {
                if(DeleteSpaceSuccess != null)DeleteSpaceSuccess(this, new NKSingleArg<string>(spaceId));
            }, (INError err) => {
                var errorArgs = new NKErrorArgs(err);
                ErrorHappened(this, errorArgs);
                if(DeleteSpaceFailure != null)DeleteSpaceFailure(this, errorArgs);
            });
        }

        // Syncing related methods
        private System.Collections.IEnumerator entityDeltaSync()
        {
            while (true)
            {
                lock (latestLocalEntities)
                {
                    lock (localEntities)
                    {
                        // Debug.Log("local entities count:" + localEntities.Count + "/ latest Local entities count:" + latestLocalEntities.Count);
                        foreach (var item in latestLocalEntities)
                        {
                            INEntityDelta delta = new NEntityDelta();
                            INEntity previousEntity = null;
                            INEntity currentEntity = null;
                            // Debug.LogFormat("latest:{0}\nlocal:{1}", item.Value, localEntities[item.Key]);
                           if (localEntities.ContainsKey(item.Key))
                            {
                                previousEntity = localEntities[item.Key];
                                currentEntity = item.Value;
                                // calc delta
                                delta = localEntities[item.Key].GetDelta(item.Value);
                                // update
                                localEntities[item.Key] = item.Value;
                                // Debug.LogFormat("Update localEntities: {0}", item.Key, item.Value);
                                // Debug.LogFormat("Update localEntities Delta: {0}", delta.ToString());
                            }
                            else
                            {
                                previousEntity = null;
                                delta = new NEntity().GetDelta(item.Value);

                                // insert
                                // Debug.LogFormat("Add to localEntities: {0}", item.Key);
                                localEntities.Add(item.Key, item.Value);
                            }
                            if (delta.UpdatedFields.Length > 0)
                            {
                                // Debug.LogFormat("Send out delta: {0}", delta);
                                NEntitySyncMessage message = new NEntitySyncMessage.Builder(item.Value.SpaceId).EntityDelta(delta).Build();
                                this.Send(message, (bool success) => { }, (INError error) => { });
                            }
                        }
                        latestLocalEntities.Clear();
                    }
                }
                yield return new WaitForSeconds(1f / SyncRate);
            }
        }

        private void OnPlayerLeave(object sender, NKSingleArg<INEntity> e)
        {
            var entity = e.value;
            if (!localEntities.ContainsKey(entity.Id))
            {
                lock (localEntities)
                {
                    localEntities.Remove(entity.Id);
                }
            }
        }

        private void OnPlayerJoin(object sender, NKSingleArg<INEntity> e)
        {
            var entity = e.value;
            if (!localEntities.ContainsKey(entity.Id))
            {
                lock (localEntities)
                {
                    localEntities.Add(entity.Id, entity.Clone());
                }
            }
        }

        public void UpdateEntity(INEntity entity)
        {
            if (!string.IsNullOrEmpty(entity.Id))
            {
                lock (latestLocalEntities)
                {
                    if (latestLocalEntities.ContainsKey(entity.Id))
                    {
                        if (!latestLocalEntities[entity.Id].Equals(entity))
                        {
                            latestLocalEntities[entity.Id] = entity.Clone();
                        }
                        Debug.LogFormat ("Updated latestLocalEntities: {0}", entity.Id);
                    }
                    else
                    {
                        latestLocalEntities.Add(entity.Id, entity.Clone());
                        Debug.LogFormat ("Add to latestLocalEntities: {0}", entity.Id);
                    }
                }
            }
        }
    }
}