using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.UI;

namespace Auroraland.Temporary
{
    public sealed class VideoReference : MonoBehaviour
    {
        public GameObject VideoPlane;

        private void Start()
        {
            Assert.IsNotNull(VideoPlane);
        }

        public void TurnOn()
        {
            VideoPlane.SetActive(true);
        }

        public void TurnOff()
        {
            VideoPlane.SetActive(false);
        }
    }
}