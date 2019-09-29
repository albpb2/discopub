using Assets.Scripts.Game;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts.Buttons
{
    public class MultiValueSliderController : MonoBehaviour
    {
        [SerializeField]
        private TMP_Text _text;
        [SerializeField]
        private List<TMP_Text> _valueTexts;
        [SerializeField]
        private Slider _slider;

        private string _actionName;
        private Player.Player _player;
        private MultiValueControlsManager _multiValueControlsManager;
        private string[] _values;

        public void SetUp(string actionName, string actionText, string playerPeerId, string[] values, string[] valueTexts)
        {
            _actionName = actionName;

            var captainsMessNetworkManager = CaptainsMessNetworkManager.singleton as CaptainsMessNetworkManager;
            _player = captainsMessNetworkManager.LobbyPlayers().Single(p => p.peerId == playerPeerId) as Player.Player;
            _multiValueControlsManager = FindObjectOfType<MultiValueControlsManager>();

            _text.text = actionText.ToUpper();

            _values = values;

            for (var i = 0; i < _valueTexts.Count; i++)
            {
                _valueTexts[i].text = valueTexts[i];
            }
        }

        public void SetValue()
        {
            var value = _values[(int)_slider.value];
            _player.CmdSubmitAction(_actionName, value);
            _multiValueControlsManager.CmdSetButtonValue(_actionName, value);
        }
    }
}
