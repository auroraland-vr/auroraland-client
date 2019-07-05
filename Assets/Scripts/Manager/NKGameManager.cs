using UnityEngine;
using System;
using Nakama;

namespace Auroraland
{
    public class NKGameManager : MonoBehaviour
    {
        static NKGameManager m_Instance;
        public static NKGameManager Instance { get { return m_Instance; } }

        public event EventHandler BecomeMasterClient;
        public event EventHandler<NKPlayerEvent> PlayerJoins;
        public event EventHandler<NKPlayerEvent> PlayerLeaves;
        public event EventHandler<NKCustomEvent> EventReceived;
        public event EventHandler<NKCustomEvent> AuthoritateEventReceived;
        public event EventHandler<NKPlayerReadyEvent> PlayerReadyEventReceived;
        public event EventHandler<NKPlayerReadyEvent> AuthoritatePlayerReadyEventReceived;

        public GameObject LeftController; // attach custom left controller script to this game object
        public GameObject RightController; // attach custom right controller script to this game object
        public string UserID;
        public string SpaceID;

        void Awake()
        {
            if (m_Instance == null)
                m_Instance = this;
            else
            {
                Debug.Log("Cannot have more than one NKGameManager object in scene. The involved objects are " + m_Instance.name + " and " + name);
                Destroy(this);
            }
            if (LeftController == null)
            {
                LeftController = new GameObject();
                LeftController.name = "CustomLeftController";
                LeftController.transform.parent = transform;
            }
            if (RightController == null)
            {
                RightController = new GameObject();
                RightController.name = "CustomRightController";
                RightController.transform.parent = transform;
            }
        }

        private void Start()
        {
            NKController.Instance.BecomeMasterClient += OnBecomeMasterClient;
            NKController.Instance.GameEvent += OnGameEvent;
            NKController.Instance.GameAuthoritateEvent += OnGameAuthoritateEvent;
            NKController.Instance.PlayerJoin += OnPlayerJoin;
            NKController.Instance.ActorJoin += OnPlayerJoin;
            NKController.Instance.ActorLeave += OnPlayerLeave;
        }

        public void SendToMasterClient(string message)
        {
            if (string.IsNullOrEmpty(SpaceID))
                return;

            var e = new NCustomEvent
            {
                UserId = UserID,
                Metadata = "{" + message + "}"
            };

            var msg = new NEventSendMessage.Builder(SpaceID)
                .Audience(Audience.MasterClient)
                .CustomEvent(e)
                .Build();

            Debug.Log("Trying to send " + e.Metadata + " to the master client.");

            NKController.Instance.Send(msg, (bool success) =>
            {
                Debug.Log("Sending successful.");
            }, (INError error) =>
            {
                Debug.Log("Error Code: " + error.Code + ", Message: " + error.Message);
            });
        }
        public void SendTo(string message, string userId)
        {
            if (string.IsNullOrEmpty(SpaceID))
                return;

            var e = new NCustomEvent
            {
                UserId = UserID,
                Metadata = "{" + message + "}"
            };

            var msg = new NEventSendMessage.Builder(SpaceID)
                .Audience(Audience.MasterClient)
                .CustomEvent(e)
                .TargetUser(userId)
                .Build();

            Debug.Log("[Master Client] Trying to send " + e.Metadata + " to " + userId + ".");

            NKController.Instance.Send(msg, (bool success) =>
            {
                Debug.Log("Sending successful.");
            }, (INError error) =>
            {
                Debug.Log("Error Code: " + error.Code + ", Message: " + error.Message);
            });
        }
        public void BroadcastExcluding(string message, string excludeId)
        {
            if (string.IsNullOrEmpty(SpaceID))
                return;

            if (NKController.Instance.GetLocalUserId() == NKController.Instance.GetMasterClientUserId())
            {
                var e = new NCustomEvent
                {
                    UserId = UserID,
                    Metadata = "{" + message + "}"
                };

                var msg = new NEventSendMessage.Builder(SpaceID)
                    .Audience(Audience.Others)
                    .CustomEvent(e)
                    .Exclude(excludeId)
                    .Build();

                Debug.Log("[Master Client] Trying to broadcast " + e.Metadata + ", excluding " + excludeId + ".");

                NKController.Instance.Send(msg, (bool success) =>
                {
                    Debug.Log("Sending successful.");
                }, (INError error) =>
                {
                    Debug.Log("Error Code: " + error.Code + ", Message: " + error.Message);
                });
            }
            else
            {
                Debug.Log("Only master clients can broadcast.");
            }
        }

        public void OnBecomeMasterClient(object sender, NKSingleArg<bool> isMasterClient)
        {
            EventHandler safeguard = BecomeMasterClient;
            EventArgs e = new EventArgs();

            if (safeguard != null)
                safeguard(this, e);
        }

        public void OnPlayerJoin(object sender, NKSingleArg<INEntity> e)
        {
            var entity = e.value;
            var userId = entity.UserId;
            NKPlayerEvent payload = new NKPlayerEvent(userId);

            EventHandler<NKPlayerEvent> safeguard = PlayerJoins;

            if (safeguard != null)
                safeguard(this, payload);
        }

        public void OnPlayerLeave(object sender, NKSingleArg<INEntity> e)
        {
            var entity = e.value;
            var userId = entity.UserId;
            NKPlayerEvent payload = new NKPlayerEvent(userId);

            EventHandler<NKPlayerEvent> safeguard = PlayerLeaves;

            if (safeguard != null)
                safeguard(this, payload);
        }

        public void OnGameEvent(object sender, NKSingleArg<INEvent> e)
        {
            var evt = e.value;
            if (evt is INCustomEvent)
            {
                NKCustomEvent payload = new NKCustomEvent(evt as INCustomEvent);

                EventHandler<NKCustomEvent> safeguard = EventReceived;

                if (safeguard != null)
                    safeguard(this, payload);
            }
            else if (evt is INReadyEvent)
            {
                NKPlayerReadyEvent payload = new NKPlayerReadyEvent(evt as INReadyEvent);

                EventHandler<NKPlayerReadyEvent> safeguard = PlayerReadyEventReceived;

                if (safeguard != null)
                    safeguard(this, payload);
            }
        }

        public void OnGameAuthoritateEvent(object sender, NKSingleArg<INEvent> e)
        {
            var evt = e.value;
            if (evt is INCustomEvent)
            {
                NKCustomEvent payload = new NKCustomEvent(evt as INCustomEvent);

                EventHandler<NKCustomEvent> safeguard = AuthoritateEventReceived;

                if (safeguard != null)
                    safeguard(this, payload);
            }
            else if (evt is INReadyEvent)
            {
                NKPlayerReadyEvent payload = new NKPlayerReadyEvent(evt as INReadyEvent);

                EventHandler<NKPlayerReadyEvent> safeguard = AuthoritatePlayerReadyEventReceived;

                if (safeguard != null)
                    safeguard(this, payload);
            }
        }

        public sealed class NKPlayerEvent : EventArgs
        {
            public readonly string UserId;

            public NKPlayerEvent(string userId)
            {
                UserId = userId;
            }
        }

        public sealed class NKCustomEvent : EventArgs
        {
            public readonly INCustomEvent Payload;

            public NKCustomEvent(INCustomEvent payload)
            {
                Payload = payload;
            }
        }

        public sealed class NKPlayerReadyEvent : EventArgs
        {
            public readonly INReadyEvent Payload;

            public NKPlayerReadyEvent(INReadyEvent payload)
            {
                Payload = payload;
            }
        }
    }
}