using UnityEngine;
using Nakama;

namespace Auroraland
{
    public sealed class EventManager : MonoBehaviour
	{
        private void Start()
        {
            NKController.Instance.PlatformEvent += OnPlatformEvent;
            NKController.Instance.PlatformAuthoritateEvent += OnPlatformAuthoritateEvent;
        }

        private void OnPlatformEvent(object sender, NKSingleArg<INEvent> e)
        {
            var evt = e.value;
            if (evt is INMoveEvent)
            {
                OnMoveEvent((INMoveEvent) evt);
            }
            else if (evt is INCollideEvent)
            {
                OnCollideEvent((INCollideEvent)evt);
            }
        }

        private void OnPlatformAuthoritateEvent(object sender, NKSingleArg<INEvent> e)
        {
            var evt = e.value;
            if (evt is INMoveEvent)
            {
                OnAuthoritateMoveEvent((INMoveEvent) evt);
            }
            else if (evt is INCollideEvent)
            {
                OnAuthoritateCollideEvent((INCollideEvent)evt);
            }
        }

        public void OnMoveEvent(INMoveEvent evt){
			GameObject obj = SpaceManager.Instance.GetObjectByEntityId (evt.EntitySnapshot.EntityId);
			obj.transform.position = NakamaTypeConverter.INVector3ToVector3 (evt.EntitySnapshot.Position);
			obj.transform.eulerAngles = NakamaTypeConverter.INVector3ToVector3 (evt.EntitySnapshot.Rotation);
            Vector3 force = NakamaTypeConverter.INVector3ToVector3 (evt.Force);
			obj.GetComponent<Rigidbody> ().isKinematic = false;
			obj.GetComponent<Rigidbody> ().AddForce (force, ForceMode.VelocityChange);
		}

		public void OnCollideEvent(INCollideEvent evt)
        {
			// TODO: complete this
		}

		public void OnAuthoritateMoveEvent(INMoveEvent evt){
			//TODO: add max distance...to metadata
			// TODO: calculate trajectory
			NEventSendMessage message = new NEventSendMessage.Builder(SpaceManager.Instance.CurrentSpaceId)
				.Audience(Audience.Others)
				.MoveEvent(evt)
				.Build();
			NKController.Instance.Send (message, (bool success) => {
				Debug.Log("success in authoritate event");
			}, (INError error) => {
				Debug.Log ("error in authoriate event");
			});

		}

		// Master Client authoritate collide event
		public void OnAuthoritateCollideEvent(INCollideEvent evt){
		}

		public void Send(INEvent evt){
			var spaceId = SpaceManager.Instance.CurrentSpaceId;
			NEventSendMessage.Builder messageBuilder = new NEventSendMessage.Builder (spaceId);

			if (evt is INMoveEvent) {
				messageBuilder.MoveEvent ((NMoveEvent)evt);
			}

			if (evt is INCollideEvent) {
				messageBuilder.CollideEvent ((NCollideEvent)evt);
			}

			if (evt is INCustomEvent) {
				messageBuilder.CustomEvent ((NCustomEvent)evt);
			}

			NKController.Instance.Send (messageBuilder.Build(), (bool success) => {
				// Debug.Log("Send out event successfully");
			}, (INError error) => {
				Debug.Log ("ERROR in sending event");
			});
		}

	}
}

