using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Auroraland
{
    public class FreezeAvatarHandler : MonoBehaviour
    {

        public AvatarController AvatarController;

        private void OnEnable()
        {
            MainMenu.OnDisableMainMenu += UnfrozeAvatarMovement;
            MainMenu.OnEnableMainMenu += FreezeAvatarMovement;
        }
        private void OnDisable()
        {
            MainMenu.OnDisableMainMenu -= UnfrozeAvatarMovement;
            MainMenu.OnEnableMainMenu -= FreezeAvatarMovement;
        }

        private void FreezeAvatarMovement()
        {
            AvatarController.AllowMovement = false;
        }

        private void UnfrozeAvatarMovement()
        {
            AvatarController.AllowMovement = true;
        }
    }
}
