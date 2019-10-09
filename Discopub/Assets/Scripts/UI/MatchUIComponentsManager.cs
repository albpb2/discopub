using UnityEngine;
using UnityEngine.Networking;

namespace Assets.Scripts.UI
{
    public class MatchUIComponentsManager : NetworkBehaviour
    {
        [SerializeField] 
        private GameObject _timer;
        [SerializeField] 
        private GameObject _pointsCounter;
        [SerializeField] 
        private GameObject _goalText;
        [SerializeField] 
        private GameObject _progressBar;

        [ClientRpc]
        public void RpcEnableMatchUIComponents()
        {
            _timer.SetActive(true);
            _pointsCounter.SetActive(true);
            _goalText.SetActive(true);
            _progressBar.SetActive(true);
        }

        [ClientRpc]
        public void RpcDisableMatchUIComponents()
        {
            _timer.SetActive(false);
            _pointsCounter.SetActive(false);
            _goalText.SetActive(false);
            _progressBar.SetActive(false);
        }
    }
}