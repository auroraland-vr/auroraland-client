using Nakama;
using Newtonsoft.Json;
using System.Collections.Generic;
using UnityEngine;
using VRTK;

namespace Auroraland
{

    public class NetObject : MonoBehaviour
    {
        public bool AllowSyncIn;
        public bool AllowSyncOut;
        // public float SyncDuration;
        private List<Renderer> rendererList = new List<Renderer>();

        protected string entityType;

        protected INEntity nEntity;
        protected Vector3 originPosition;
        protected Vector3 originRotation;

        protected Vector3 targetPosition;
        protected Vector3 targetRotation;

        protected float timer = 0f;

        protected float DISTANCE_DIFF_THRESHOLD = 0.001f;
        protected float ANGLE_DIFF_THRESHOLD = 1;

        protected bool needReconciliation = false;
        private Color assignedColor = new Color(0, 0, 0, 0);

        void Awake()
        {
            Renderer renderer = GetComponent<Renderer>();
            if (renderer != null)
            {
                rendererList.Add(renderer);
            }

            var descendants = gameObject.GetComponentsInChildren<Renderer>();
            if (descendants != null)
            {
                rendererList.AddRange(descendants);
            }
        }

        public string GetUserId()
        {
            return nEntity.UserId;
        }

        private void ChangeOwnership(string userId, string authorityUserId)
        {
            nEntity.UserId = userId;
            nEntity.AuthorityUserId = authorityUserId;
            UpdateVRTKInteraction();
            UpdateKinematic();
        }

        public bool IsOwnedLocally()
        {
            return (nEntity.UserId == NKController.Instance.GetLocalUserId());
        }

        public bool IsOwnedByOthers()
        {
            return !string.IsNullOrEmpty(nEntity.UserId) && !IsOwnedLocally();
        }

        public void TakeOwnership()
        {
            ChangeOwnership(NKController.Instance.GetLocalUserId(), "");
        }

        public void ReleaseOwnership()
        {
            ChangeOwnership("", NKController.Instance.GetLocalUserId());
        }

        private void TakeAuthority()
        {
            if (nEntity.UserId == "")
            {
                nEntity.AuthorityUserId = NKController.Instance.GetLocalUserId();
            }
        }

        private void UpdateVRTKInteraction()
        {
            if (nEntity.UserId == "" || IsManagedLocally()) // objects owned by no one or is managed by local
            {
                var interactableObj = this.GetComponent<VRTK_InteractableObject>();
                if (interactableObj != null)
                {
                    interactableObj.enabled = true;
                }
            }
            else
            { // objects owned by other players
                var interactableObj = this.GetComponent<VRTK_InteractableObject>();
                if (interactableObj != null)
                {
                    interactableObj.enabled = false;
                }
            }
        }

        private void UpdateKinematic()
        {
            var rigidbody = this.gameObject.GetComponent<Rigidbody>();
            if (rigidbody == null)
            {
                return;
            }

            rigidbody.isKinematic = nEntity.UserId != "";
        }

        public bool IsManagedLocally()
        {
            var localUserId = NKController.Instance.GetLocalUserId();
            return nEntity.AuthorityUserId == localUserId || nEntity.UserId == localUserId;
        }

        public bool HasAuthorityToModify()
        {
            var localUserId = NKController.Instance.GetLocalUserId();
            return nEntity.AuthorityUserId == localUserId || NKController.Instance.IsMasterClient();
        }

        public void SetEntity(INEntity entity)
        {
            nEntity = entity;
            entityType = entity.AssetType;

            ChangeOwnership(entity.UserId, entity.AuthorityUserId);
            if (entity.Position != null)
            {
                transform.position = NakamaTypeConverter.INVector3ToVector3(entity.Position);
            }

            if (entity.Rotation != null)
            {
                transform.eulerAngles = NakamaTypeConverter.INVector3ToVector3(entity.Rotation);
            }

            if (entity.Scale != null)
            {
                transform.localScale = NakamaTypeConverter.INVector3ToVector3(entity.Scale);
            }

            switch (entityType)
            {
                case "prefab":
                    {
                        if (GetComponent<Rigidbody>())
                        {
                            GetComponent<Rigidbody>().isKinematic = (!string.IsNullOrEmpty(entity.UserId)) ? true : false;
                        }
                        break;
                    }
                case "game":
                    {
                        if (GetComponent<Rigidbody>())
                        {
                            GetComponent<Rigidbody>().isKinematic = true;
                        }

                        break;
                    }
            }

            SetMaterialColor(entity.Metadata);
        }

        public virtual void SetControlData(INControlData controlData)
        {
        }

        public virtual NControlData GetControlData()
        {
            return default(NControlData);
        }

        public virtual INEntity GetNEntity()
        {
            NVector3 pos = NakamaTypeConverter.Vector3ToNVector3(transform.position);
            NVector3 rot = NakamaTypeConverter.Vector3ToNVector3(transform.eulerAngles);
            NVector3 scale = NakamaTypeConverter.Vector3ToNVector3(transform.localScale);

            nEntity.Position = pos;
            nEntity.Rotation = rot;
            nEntity.Scale = scale;
            return nEntity;
        }

        /// <summary>
        /// Called when an entity is manipulated/affected by another player.
        /// </summary>
        /// <param name="entity">A space entity (object spawned in a space, a player, etc.)</param>
        public virtual void SyncIn(INEntity entity)
        {
            if (AllowSyncIn)
            {
                if (entity.UserId != nEntity.UserId || entity.AuthorityUserId != nEntity.AuthorityUserId)
                {
                    ChangeOwnership(entity.UserId, entity.AuthorityUserId);
                }
                UpdateTransform(entity);
            }
        }

        private void UpdateTransform(INEntity entity)
        {
            if (entity.Position != null)
            {
                originPosition = transform.position;
                targetPosition = NakamaTypeConverter.INVector3ToVector3(entity.Position);
                needReconciliation = true;
            }
            if (entity.Rotation != null)
            {
                originRotation = transform.eulerAngles;
                targetRotation = NakamaTypeConverter.INVector3ToVector3(entity.Rotation);
                needReconciliation = true;
            }
            if (entity.Position != null || entity.Rotation != null)
            {
                timer = 0;
            }

            if (entity.Scale != null && NakamaTypeConverter.INVector3ToVector3(entity.Scale) != Vector3.zero)
            {
                transform.localScale = NakamaTypeConverter.INVector3ToVector3(entity.Scale);
            }
        }

        private void OnCollisionEnter(Collision collision)
        {
            if (nEntity == null)
            {
                return;
            }

            var collideObject = collision.gameObject;
            var collideNetObj = collideObject.GetComponent<NetObject>();
            if (collideNetObj != null)
            {
                collideNetObj.TakeAuthority();
            }
        }

        void Update()
        {
            if (!needReconciliation)
            {
                return;
            }

            timer += 1000 * Time.deltaTime; // accumulate the ms
            var percentage = timer / 100.0f;
            var distanceDiff = Vector3.Distance(transform.position, targetPosition);
            float xRotationDiff = Mathf.DeltaAngle(transform.eulerAngles.x, targetRotation.x);
            float yRotationDiff = Mathf.DeltaAngle(transform.eulerAngles.y, targetRotation.y);
            float zRotationDiff = Mathf.DeltaAngle(transform.eulerAngles.z, targetRotation.z);

            if (distanceDiff > DISTANCE_DIFF_THRESHOLD)
            {
                transform.position = Vector3.Lerp(originPosition, targetPosition, percentage);
            }
            if ((xRotationDiff != 0 && Mathf.Abs(xRotationDiff) > ANGLE_DIFF_THRESHOLD) || (yRotationDiff != 0 && Mathf.Abs(yRotationDiff) > ANGLE_DIFF_THRESHOLD) || (zRotationDiff != 0 && Mathf.Abs(zRotationDiff) > ANGLE_DIFF_THRESHOLD))
            {
                transform.eulerAngles = Vector3.Lerp(originRotation, targetRotation, percentage);
            }
            if (percentage >= 1.0)
            {
                needReconciliation = false;
            }
        }

        public virtual void SetMaterialColor(string json)
        {
            var dt = Newtonsoft.Json.Linq.JObject.Parse(json);
            EntityMeta data = JsonConvert.DeserializeObject<EntityMeta>(json);
            if (dt["Color"] == null || data.Color.a == 0)
            {
                return;
            }

            assignedColor = new Color(data.Color.r, data.Color.g, data.Color.b, data.Color.a);
            foreach (Renderer renderer in rendererList)
            {
                renderer.material.color = assignedColor;
            }
            GetComponent<VRTK.Highlighters.VRTK_BaseHighlighter>().Initialise(GetComponent<VRTK.VRTK_InteractableObject>().touchHighlightColor);
        }
    }
}
