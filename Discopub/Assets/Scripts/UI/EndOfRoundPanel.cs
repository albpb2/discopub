using UnityEngine;
using UnityEngine.Networking;

namespace Assets.Scripts.UI
{
    public class EndOfRoundPanel : NetworkBehaviour
    {
        [SerializeField]
        private GameObject _panel;

        [ClientRpc]
        public void RpcShowPanel()
        {
            _panel.SetActive(true);
        }

        [ClientRpc]
        public void RpcHidePanel()
        {
            _panel.SetActive(false);
        }
    }
}
