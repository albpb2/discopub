using UnityEngine;
using UnityEngine.Networking;

namespace Assets.Scripts.Network
{
    /// <summary>
    /// This component will disable its object and its children if the player is not
    /// the server.
    /// </summary>
    public class ServerObjectsManager : NetworkBehaviour
    {
        protected void Awake()
        {
            if (!isServer)
            {
                gameObject.SetActive(false);
            }
        }
    }
}
