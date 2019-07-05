using Nakama;
using System.Collections;
using UnityEngine;
using UnityEngine.Assertions;

namespace Auroraland
{
    public sealed class ControlDataManager : MonoBehaviour
    {
        string _previousHash = "";

        NetPlayer _netPlayer;

        [SerializeField]
        SpaceManager _spaceManager;

        /// <summary>
        /// How often, per second, should ControlDataManager send updates to the server?
        /// </summary>
        [Tooltip("How often, per second, should ControlDataManager send updates to the server?")]
        public float UpdateRate = 10f;

        IEnumerator _syncCoroutine;

        void OnEnable()
        {
            Assert.IsNotNull(_spaceManager);

            NKController.Instance.ControlDataUpdate += OnControlDataUpdate;
            _spaceManager.PlayerSpawned += SpaceManager_PlayerSpawned;
            _spaceManager.PlayerDeleted += SpaceManager_LeftSpace;
        }

        void OnDisable()
        {
            NKController.Instance.ControlDataUpdate -= OnControlDataUpdate;
            _spaceManager.PlayerSpawned -= SpaceManager_PlayerSpawned;
            _spaceManager.PlayerDeleted -= SpaceManager_LeftSpace;
        }

        void Send(NControlData controlData)
        {
            controlData.UserId = NKController.Instance.GetLocalUserId();
            var spaceId = _spaceManager.CurrentSpaceId;
            string dataHash = controlData.ToString();

            if (_previousHash != dataHash)
            {
                NControlDataSendMessage message = new NControlDataSendMessage.Builder(spaceId)
                .Audience(Audience.Others)
                .ControlData(controlData)
                .Build();

                NKController.Instance.Send(message, (bool success) =>
                {
                }, (INError error) =>
                {
                    Debug.Log("ERROR sending control data");
                });
                _previousHash = dataHash;
            }
        }

        IEnumerator SyncOutControlData()
        {
            while (true)
            {
                NControlData data = _netPlayer.GetControlData();
                Send(data);
                yield return new WaitForSeconds(1f / UpdateRate);
            }
        }

        // Control Data Event Handler
        void OnControlDataUpdate(object sender, NKSingleArg<INControlData> e)
        {
            var controlData = e.value;
            GameObject actor = SpaceManager.Instance.GetObjectByEntityId(e.value.EntityId);
            if (actor != null)
            {
                actor.GetComponent<NetActor>().SetControlData(controlData);
            }
        }

        void SpaceManager_PlayerSpawned(object sender, System.EventArgs e)
        {
            _netPlayer = _spaceManager.PlayerRoot.GetComponent<NetPlayer>();
            _syncCoroutine = SyncOutControlData();
            StartCoroutine(_syncCoroutine);
        }
        void SpaceManager_LeftSpace(object sender, System.EventArgs e)
        {
            StopCoroutine(_syncCoroutine);
        }
    }
}

