using UnityEngine;
using UnityEngine.Networking;

namespace Assets.Scripts.UI
{
    public class EndOfRoundPanel : NetworkBehaviour
    {
        [SerializeField]
        private GameObject _panel;
        [SerializeField]
        private EndOfRoundPanelAnimator _endOfRoundPanelAnimator;

        [ClientRpc]
        public void RpcShowPanel()
        {
            _panel.SetActive(true);
            _endOfRoundPanelAnimator.AnimateEndOfRoundPanelEnter();
        }

        [ClientRpc]
        public void RpcHidePanel()
        {
            _endOfRoundPanelAnimator.AnimateEndOfRoundPanelExit();
        }
    }
}
