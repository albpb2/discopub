using UnityEngine;
using UnityEngine.Networking;
using TMPro;
using System.Linq;

namespace Assets.Scripts.Player
{
    public class Player : CaptainsMessPlayer
    {
        private TextMeshProUGUI _textSentLabel;
        private TMP_InputField _inputField;
        private CaptainsMess _captainMess;

        [ClientRpc]
        public void RpcNotifyMessage(string senderPeerId, string message)
        {
            Debug.Log($"Player {senderPeerId} sent {message}");
            _textSentLabel.text = $"Player {senderPeerId} sent {message}";
        }
        
        public void SendMessage()
        {
            CmdSendMessage(_inputField.text, peerId);
        }

        [Command]
        public void CmdSendMessage(string text, string senderPeerId)
        {
            Debug.Log($"Sending message {text}");
            foreach (var player in _captainMess.Players().Cast<Player>())
            {
                if (player.peerId != senderPeerId)
                {
                    Debug.Log($"Receiver peerId = {player.peerId}");
                    player.RpcNotifyMessage(senderPeerId, text);
                }
            }
        }

        public override void OnStartLocalPlayer()
        {
            base.OnStartLocalPlayer();

            var playerCommandSender = FindObjectOfType<PlayerCommandSender>();
            playerCommandSender.SetPlayer(this);
        }

        protected void Start()
        {
            _textSentLabel = GameObject.FindGameObjectWithTag(Labels.UiElements.TextSentLabel).GetComponent<TextMeshProUGUI>();
            _inputField = GameObject.FindGameObjectWithTag(Labels.UiElements.Textbox).GetComponent<TMP_InputField>();
            _captainMess = GameObject.FindObjectOfType<CaptainsMess>();
        }
    }
}
