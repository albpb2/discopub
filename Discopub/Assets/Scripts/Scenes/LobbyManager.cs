using System.Collections;
using UnityEngine;

namespace Assets.Scripts.Scenes
{
    public class LobbyManager : MonoBehaviour
    {
        [SerializeField]
        private CaptainsMess _captainsMess;

        public void Awake()
        {
            StartCoroutine(AutoConnect());
        }

        public IEnumerator AutoConnect()
        {
            const int secondsToWaitToAutoconnect = 1; // To allow for the captainsMess object to be active
            yield return new WaitForSeconds(secondsToWaitToAutoconnect);
            _captainsMess.AutoConnect();
        }
    }
}
