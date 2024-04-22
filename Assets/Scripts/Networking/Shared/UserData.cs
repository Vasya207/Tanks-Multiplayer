using System;
using UnityEngine.Serialization;

namespace Networking.Shared
{
    [Serializable]
    public class UserData
    {
        public string userName;
        public string userAuthId;
    }
}
