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
        public void RpcNotifyMessage(string peerId, string message)
        {
            Debug.Log($"Player {peerId} sent {message}");
            _textSentLabel.text = $"Player {peerId} sent {message}";
        }

        [Command]
        public void CmdSendMessage()
        {
            Debug.Log($"Sending message {_inputField.text}");
            foreach (var player in _captainMess.Players().Cast<Player>())
            {
                if (player.peerId != peerId)
                {
                    player.RpcNotifyMessage(peerId, _inputField.text);
                }
            }
        }

        public override void OnStartLocalPlayer()
        {
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
