using UnityEngine;
using TMPro;

namespace Assets.Scripts.Player
{
    public class Player : CaptainsMessPlayer
    {
        private TextMeshProUGUI _textSentLabel;
        private TMP_InputField _inputField;
        private CaptainsMess _captainMess;

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
