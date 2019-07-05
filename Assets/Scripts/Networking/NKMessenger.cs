using Nakama;
using System;
using System.Text;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Auroraland
{
    public class NKMessenger : MonoBehaviour
    {
        protected INClient _client;
        protected INSession _session;
        protected Queue<IEnumerator> _executionQueue;

        protected string localUserId;
        protected string masterClientUserId;

        public string ServerIP = "52.53.222.194";
        public uint ServerPort = 7350;
        public bool IsConnected { get; protected set; }

        // Session related events
        public event EventHandler<NKErrorArgs> SessionDisconnect;
        public event EventHandler<NKErrorArgs> SessionError;

        // Master Client related events
        public event EventHandler<NKSingleArg<bool>> BecomeMasterClient;

        // ControlData related events
        public event EventHandler<NKSingleArg<INControlData>> ControlDataUpdate;

        // Player related events
        public event EventHandler<NKSingleArg<INEntity>> PlayerJoin;
        public event EventHandler<NKSingleArg<INEntity>> PlayerLeave;
        // Actors related events
        public event EventHandler<NKSingleArg<INEntity>> ActorJoin;
        public event EventHandler<NKSingleArg<INEntity>> ActorLeave;
        public event EventHandler<NKSingleArg<INEntity>> ActorUpdate;

        // Objects related events
        public event EventHandler<NKSingleArg<INEntity>> ObjectAdd;
        public event EventHandler<NKSingleArg<INEntity>> ObjectDelete;
        public event EventHandler<NKSingleArg<INEntity>> ObjectUpdate;

        // Game related events
        public event EventHandler<NKSingleArg<INEntity>> GameLoad;
        public event EventHandler<NKSingleArg<INEntity>> GameUnload;

        // Platform related events
        public event EventHandler<NKSingleArg<INEvent>> PlatformEvent;
        public event EventHandler<NKSingleArg<INEvent>> PlatformAuthoritateEvent;

        public event EventHandler<NKSingleArg<INEvent>> GameEvent;
        public event EventHandler<NKSingleArg<INEvent>> GameAuthoritateEvent;

        // Getters & Setters
        public string GetLocalUserId()
        {
            return localUserId;
        }

        public string GetMasterClientUserId()
        {
            return masterClientUserId;
        }

        public bool IsMasterClient()
        {
            return localUserId == masterClientUserId;
        }

        // Client Event Handlers
        protected void OnTopicMessage(INTopicMessage tm)
        {
            if (tm.Type != TopicMessageType.MasterClient) return;

            masterClientUserId = tm.UserId;

            if (localUserId != tm.UserId) return;

            if (BecomeMasterClient != null) BecomeMasterClient(this, new NKSingleArg<bool>(true));
        }

        protected enum EntityType { Player, Actor, Object, Game, Unknown };

        protected EntityType GetEntityType(INEntity entity)
        {
            var type = EntityType.Unknown;
            switch (entity.AssetType)
            {
                case "avatar":
                    type = localUserId != entity.UserId ? EntityType.Actor : EntityType.Player;
                    break;
                case "prefab":
                    type = EntityType.Object;
                    break;
                case "static":
                    type = EntityType.Object;
                    break;
                case "game":
                    type = EntityType.Game;
                    break;
            }
            return type;
        }

        protected void OnSpacePresence(INSpacePresence sp)
        {
            Enqueue(() =>
            {
                foreach (var entity in sp.Adds)
                {
                    var type = GetEntityType(entity);
                    var arg = new NKSingleArg<INEntity>(entity);
                    switch (type)
                    {
                        case EntityType.Player:
                            if (PlayerJoin != null) PlayerJoin(this, arg);
                            break;
                        case EntityType.Actor:
                            if (ActorJoin != null) ActorJoin(this, arg);
                            break;
                        case EntityType.Object:
                            if (ObjectAdd != null) ObjectAdd(this, arg);
                            break;
                        case EntityType.Game:
                            if (GameLoad != null) GameLoad(this, arg);
                            break;
                    }
                }

                foreach (var entity in sp.Dels)
                {
                    var type = GetEntityType(entity);
                    var arg = new NKSingleArg<INEntity>(entity);
                    switch (type)
                    {
                        case EntityType.Player:
                            if (PlayerLeave != null) PlayerLeave(this, arg);
                            break;
                        case EntityType.Actor:
                            if (ActorLeave != null) ActorLeave(this, arg);
                            break;
                        case EntityType.Object:
                            if (ObjectDelete != null) ObjectDelete(this, arg);
                            break;
                        case EntityType.Game:
                            if (GameUnload != null) GameUnload(this, arg);
                            break;
                    }
                }

                foreach (var entity in sp.Changes)
                {
                    var type = GetEntityType(entity);
                    var arg = new NKSingleArg<INEntity>(entity);
                    switch (type)
                    {
                        case EntityType.Actor:
                            if (ActorUpdate != null) ActorUpdate(this, arg);
                            break;
                        case EntityType.Object:
                            if (ObjectUpdate != null) ObjectUpdate(this, arg);
                            break;
                        case EntityType.Game:
                            if (ObjectUpdate != null) ObjectUpdate(this, arg);
                            break;
                    }
                }
            });
        }


        protected void OnEvent(INEvent evt)
        {
            Enqueue(() =>
            {
                if (evt is INCustomEvent || evt is INReadyEvent)
                {
                    if (GameEvent != null)
                        GameEvent(this, new NKSingleArg<INEvent>(evt));
                }
                else
                {
                    if (PlatformEvent != null)
                        PlatformEvent(this, new NKSingleArg<INEvent>(evt));
                }
            });
        }

        protected void OnAuthoritateEvent(INEvent evt)
        {
            Enqueue(() =>
            {
                if (evt is INCustomEvent || evt is INReadyEvent)
                {
                    if (GameAuthoritateEvent != null)
                        GameAuthoritateEvent(this, new NKSingleArg<INEvent>(evt));
                }
                else
                {
                    if (PlatformAuthoritateEvent != null)
                        PlatformAuthoritateEvent(this, new NKSingleArg<INEvent>(evt));
                }
            });
        }

        protected void OnControlData(INControlData controlData)
        {
            Enqueue(() =>
            {
                if (ControlDataUpdate != null)
                    ControlDataUpdate(this, new NKSingleArg<INControlData>(controlData));
            });
        }


        protected void OnDisconnect(INDisconnectEvent evt)
        {
            DebugLogger.LogError("Disconnected from server");
            DebugLogger.LogErrorFormat("Reason '{0}'", evt.Reason);
            IsConnected = false;
            Enqueue(() =>
            {
                INError err = new NError(string.Format("Reason: {0}", evt.Reason));

                if (SessionDisconnect != null)
                    SessionDisconnect(this, new NKErrorArgs(err));
            });
        }

        protected void OnError(INError error)
        {
            DebugLogger.LogErrorFormat("server error! Code'{0}' : '{1}'", error.Code, error.Message);
            Enqueue(() =>
            {
                if (SessionError != null)
                {
                    SessionError(this, new NKErrorArgs(error));
                }
            });
        }

        public NKMessenger()
        {
            _client = new NClient.Builder("defaultkey")
                .Host(ServerIP)
                .Port(ServerPort)
                .SSL(false)
                .Build();

            _client.OnTopicMessage = this.OnTopicMessage;
            _client.OnSpacePresence = this.OnSpacePresence;
            _client.OnEvent = this.OnEvent;
            _client.OnAuthoritateEvent = this.OnAuthoritateEvent;
            _client.OnControlData = this.OnControlData;
            _client.OnDisconnect = this.OnDisconnect;
            _client.OnError = this.OnError;
            _executionQueue = new Queue<IEnumerator>(1024);
        }

        protected void RestoreSessionAndConnect()
        {
            // Lets check if we can restore a cached session.
            var sessionString = PlayerPrefs.GetString("nk.session");
            if (string.IsNullOrEmpty(sessionString))
            {
                return; // We have no session to restore.
            }

            var session = NSession.Restore(sessionString);
            if (session.HasExpired(DateTime.UtcNow))
            {
                return; // We can't restore an expired session.
            }

            HandleSession(session);
        }


        public void Logout()
        {
            _client.Logout();
            IsConnected = false;
        }

        public void LoginByEmail(string email, string password, Action<bool, INError> callback)
        {
            var message = NAuthenticateMessage.Email(email, password);
            _client.Login(message, s =>
            {
                HandleSession(s);
                Enqueue(() =>
                {
                    callback(true, null);
                });
            }, e =>
            {
                HandleError(e);
                Enqueue(() =>
                {
                    callback(false, e);
                });
            });
        }

        public void RegisterByEmail(string email, string password, Action<bool, INError> callback)
        {
            var message = NAuthenticateMessage.Email(email, password);
            _client.Register(message, s =>
            {
                HandleSession(s);
                Enqueue(() =>
                {
                    callback(true, null);
                });

            }, e =>
            {
                HandleError(e);
                Enqueue(() =>
                {
                    callback(false, e);
                });

            });
        }

        public void Send<T>(INCollatedMessage<T> message, Action<T> callback, Action<INError> errback)
        {
            _client.Send(message,
                msg =>
                {
                    Enqueue(() =>
                    {
                        callback(msg);
                    });
                }, err =>
                {
                    Enqueue(() =>
                    {
                        errback(err);
                    });
                });
        }

        public void Send(INUncollatedMessage message, bool reliable, Action<bool> callback, Action<INError> errback)
        {
            _client.Send(message,
                msg =>
                {
                    Enqueue(() =>
                    {
                        callback(msg);
                    });
                }, err =>
                {
                    Enqueue(() =>
                    {
                        errback(err);
                    });
                });
        }
        public void Send(INUncollatedMessage message, Action<bool> callback, Action<INError> errback)
        {
            _client.Send(message,
                msg =>
                {
                    Enqueue(() =>
                    {
                        callback(msg);
                    });
                }, err =>
                {
                    Enqueue(() =>
                    {
                        errback(err);
                    });
                });
        }

        [Serializable]
        public class Token
        {
            public string exp;
            public string han;
            public string uid;
        }

        protected void HandleSession(INSession session)
        {
            _session = session;

            _client.Connect(_session, (bool done) =>
            {
                // We enqueue callbacks which contain code which must be dispatched on
                // the Unity main thread.
                string jsonData = UnpackJwt(session.Token);
                Token t = JsonUtility.FromJson<Token>(jsonData);
                IsConnected = true;
                localUserId = t.uid;

                Enqueue(() =>
                    {
                        // Store session for quick reconnects.
                        PlayerPrefs.SetString("nk.session", session.Token);
                        PlayerPrefs.SetString("nk.uid", t.uid);
                    });
            });
        }

        protected void Update()
        {
            lock (_executionQueue)
            {
                for (int i = 0, len = _executionQueue.Count; i < len; i++)
                {
                    StartCoroutine(_executionQueue.Dequeue());
                }
            }
        }

        protected void OnApplicationQuit()
        {
            if (_session != null)
            {
                _client.Disconnect();
            }
        }

        protected void Enqueue(Action action)
        {
            lock (_executionQueue)
            {
                _executionQueue.Enqueue(WrapAction(action));
                if (_executionQueue.Count > 1024)
                {
                    _client.Disconnect();
                }
            }
        }

        protected IEnumerator WrapAction(Action action)
        {
            action();
            yield return null;
        }

        protected static void HandleError(INError err)
        {
            Debug.LogErrorFormat("Error: code '{0}' with '{1}'.", err.Code, err.Message);
            DebugLogger.LogError(err);
        }

        protected string UnpackJwt(string jwt)
        {
            // Hack decode JSON payload from JWT
            var payload = jwt.Split('.')[1];

            var padLength = Math.Ceiling(payload.Length / 4.0) * 4;
            payload = payload.PadRight(Convert.ToInt32(padLength), '=');

            return Encoding.UTF8.GetString(Convert.FromBase64String(payload));
        }
    }
}