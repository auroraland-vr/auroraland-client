using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Auroraland
{
	public class User : MonoBehaviour
    {
		private Nakama.INUser user;

		public void SetUser(Nakama.INUser user)
        {
			this.user = user;
		}

		public string GetUserId()
        {
			return user.Id;
		}

		public string GetUserDisplayName()
        {
			return user.Fullname;
		}
	}
}
