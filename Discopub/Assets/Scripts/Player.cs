using UnityEngine;
using UnityEngine.Networking;
using TMPro;

namespace Assets.Scripts
{
    public class Player : CaptainsMessPlayer
    {
        TextMeshProUGUI textSentLabel;

        [ClientRpc]
        public void RpcNotifyMessage(string peerId, string message)
        {
            textSentLabel.text = $"Player {peerId} sent {message}";
        }

        protected void Start()
        {
            textSentLabel = GameObject.FindGameObjectWithTag("TextSentLabel").GetComponent<TextMeshProUGUI>();
        }
    }
}
