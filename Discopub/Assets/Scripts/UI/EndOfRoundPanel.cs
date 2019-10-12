using UnityEngine;
using UnityEngine.Networking;

namespace Assets.Scripts.UI
{
    public class EndOfRoundPanel : NetworkBehaviour
    {
        [SerializeField]
        private GameObject _panel;
        [SerializeField]
        private Animator _animator;

        [ClientRpc]
        public void RpcShowPanel()
        {
            _panel.SetActive(true);
        }

        [ClientRpc]
        public void RpcHidePanel()
        {
            _animator.SetBool("NextRoundStarting", true);
        }

        public void DisableEndOfRoundPanel()
        {
            _panel.SetActive(false);
        }
    }
}
