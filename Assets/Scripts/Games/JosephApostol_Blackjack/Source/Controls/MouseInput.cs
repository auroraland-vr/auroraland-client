using System.Collections.Generic;
using UnityEngine;

namespace ProjectRenaissance.Controls
{
    public sealed class MouseInput : MonoBehaviour
    {
        public Vector3 MagnetLift = new Vector3(0, 0.2f);
        // HACK Replace when this functionality needs to be generalized to different types of attractable objects with different heights
        public Vector3 Gap = new Vector3(0, 0.05f);

        [SerializeField]
        public Gambler LocalGambler { get; private set; }

        List<Rigidbody> _liftedObjects = new List<Rigidbody>();

        int _chipLayer;

        void Awake()
        {
            _chipLayer = LayerMask.NameToLayer("Chip");
        }

        void Update()
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit, 100f, 1 | 1 << _chipLayer))
            {
                Debug.Log(hit.collider);

                if (hit.collider.gameObject.layer == _chipLayer)
                {
                    Chip chip = hit.collider.GetComponent<Chip>();

                    if (Input.GetMouseButtonDown(0) && !chip.IsLocked && chip.Owner == LocalGambler)
                    {
                        chip.IsMidair = true;
                        Lift(chip.GetComponent<Rigidbody>());
                    }
                    else if (Input.GetMouseButtonDown(2))
                        Breakdown(chip);
                }
                MoveAll(hit.point);
            }

            if (Input.GetMouseButtonDown(1))
                DropAll();
        }

        void Lift(Rigidbody rigidbody)
        {
            Renderer renderer = rigidbody.GetComponent<Renderer>();
            AdjustMagnetizedObjectsUp(renderer.bounds.size.y);

            rigidbody.isKinematic = true;
            rigidbody.position += MagnetLift;
            _liftedObjects.Add(rigidbody);
        }
        void Breakdown(Chip chip)
        {
            if (!chip.IsLocked && chip.LesserChip != null)
                chip.Breakdown(true);
        }
        void DropAll()
        {
            for (int i = 0; i < _liftedObjects.Count; i++)
            {
                Rigidbody rigidbody = _liftedObjects[i];
                rigidbody.isKinematic = false;
            }

            _liftedObjects.Clear();
        }
        void MoveAll(Vector3 hitPosition)
        {
            for (int i = 0; i < _liftedObjects.Count; i++)
                _liftedObjects[i].position = hitPosition + MagnetLift + Gap * i;
        }

        void AdjustMagnetizedObjectsUp(float height)
        {
            for (int i = 0; i < _liftedObjects.Count; i++)
            {
                Rigidbody rigidbody = _liftedObjects[i];
                rigidbody.position += Gap;//new Vector3(0, height);
            }
        }
        void AdjustMagnetizedObjectsDown(float height)
        {
            for (int i = 0; i < _liftedObjects.Count; i++)
            {
                Rigidbody rigidbody = _liftedObjects[i];
                Renderer renderer = rigidbody.GetComponent<Renderer>();
                rigidbody.position -= Gap;//new Vector3(0, height);
            }
        }
    }
}