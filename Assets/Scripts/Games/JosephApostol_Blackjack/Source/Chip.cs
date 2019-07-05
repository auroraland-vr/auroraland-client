using UnityEngine;

namespace ProjectRenaissance
{
    public class Chip : MonoBehaviour
    {
        [Header("Configuration")]
        public int Value;
        public Chip GreaterChip;
        public Chip LesserChip;

        [Header("State")]
        public bool IsLocked;
        public bool IsMidair = true;
        public Gambler Owner;

        Rigidbody _rigidbody;
        Vector3 _velocityLimit;

        void Awake()
        {
            _rigidbody = GetComponent<Rigidbody>();
            _velocityLimit = new Vector3(0, -6f, 0);
        }
        void FixedUpdate()
        {
            if (_rigidbody.velocity.y < _velocityLimit.y)
                _rigidbody.velocity = _velocityLimit;
        }
        void OnCollisionEnter(Collision collision)
        {
            if (collision.collider.name == "Table")
            {
                IsMidair = false;
                _rigidbody.isKinematic = true;
            }
            else if (collision.collider.gameObject.layer.Equals(gameObject.layer))
            {
                Chip chip = collision.collider.GetComponent<Chip>();

                if (chip != null && !chip.IsMidair)
                {
                    _rigidbody.isKinematic = true;
                    IsMidair = false;
                }
            }
        }
        private void OnCollisionStay(Collision collision)
        {
            if (collision.collider.name == "Table")
            {
                IsMidair = false;
                _rigidbody.isKinematic = true;
            }
            else if (collision.collider.gameObject.layer.Equals(gameObject.layer))
            {
                Chip chip = collision.collider.GetComponent<Chip>();

                if (chip != null && !chip.IsMidair)
                {
                    _rigidbody.isKinematic = true;
                    IsMidair = false;
                }
            }
        }

        public void Breakdown(bool isLocal)
        {
            int chipsToSpawn = Value / LesserChip.Value;

            for (int i = 0; i < chipsToSpawn; i++)
            {
                Chip instance = Instantiate(LesserChip.gameObject).GetComponent<Chip>();
                Transform spawnpoint = Owner.transform.Find("Chip Spawnpoints/" + LesserChip.name);
                instance.transform.position = spawnpoint.position + new Vector3(0, 0.5f + 0.2f * i);
                instance.transform.SetParent(Owner.transform);
                instance.Owner = Owner;
                instance.IsMidair = true;
                Owner.AddChip(instance);
                Owner.RemoveChip(this);
            }

            Destroy(gameObject);
        }
    }
}