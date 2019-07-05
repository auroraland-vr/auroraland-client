using System;
using System.IO;
using UnityEngine;

namespace Auroraland.Networking.Utilities
{
    /// <summary>
    /// This class is responsible for traversing the hierarchy and storing children transforms in a CSV-format.
    /// </summary>
    public sealed class ChildTransformChecker : MonoBehaviour
    {
        private void Start()
        {
            string csv = "";

            for (int i = 0; i < transform.childCount; i++)
            {
                Transform child = transform.GetChild(i);
                SegmentId id = child.GetComponent<SegmentId>();

                csv += string.Format("{0},{1},{2},{3},{4},{5},{6}", id.Id, child.position.x, child.position.y, child.position.z, child.eulerAngles.x, child.eulerAngles.y, child.eulerAngles.z);

                if (i + 1 != transform.childCount)
                    csv += Environment.NewLine;
            }

            File.WriteAllText(gameObject.name + ".txt", csv);
        }
    }
}