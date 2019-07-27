using UnityEngine;
using UnityEngine.Networking;
using TMPro;
using System.Linq;

namespace Assets.Scripts
{
    public class Player : CaptainsMessPlayer
    {
        private TextMeshProUGUI textSentLabel;
        private TMP_InputField inputField;
        private CaptainsMess captainMess;

        [ClientRpc]
        public void RpcNotifyMessage(string peerId, string message)
        {
            textSentLabel.text = $"Player {peerId} sent {message}";
        }

        [Command]
        public void CmdSendMessage()
        {
            foreach (var player in captainMess.Players().Cast<Player>())
            {
                if (player.peerId != peerId)
                {
                    player.RpcNotifyMessage(peerId, inputField.text);
                }
            }
        }

        protected void Start()
        {
            textSentLabel = GameObject.FindGameObjectWithTag(Labels.UiElements.TextSentLabel).GetComponent<TextMeshProUGUI>();
            inputField = GameObject.FindGameObjectWithTag(Labels.UiElements.Textbox).GetComponent<TMP_InputField>();
            captainMess = GameObject.FindObjectOfType<CaptainsMess>();
        }
    }
}
