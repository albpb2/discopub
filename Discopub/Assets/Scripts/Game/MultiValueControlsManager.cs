using Assets.Scripts.Buttons;
using Assets.Scripts.Extensions;
using System.Collections.Generic;
using System.Linq;
using UnityEngine.Networking;

namespace Assets.Scripts.Game
{
    public class MultiValueControlsManager : NetworkBehaviour
    {
        private const string DefaultOnOffControllerValue = OnOffButtonController.OffValue;

        private Dictionary<string, string> _controls;
        private Dictionary<string, List<string>> _controlValues;

        public void Awake()
        {
            ResetControls();
        }

        public void AddOnOffControl(string actionName)
        {
            _controls[actionName] = DefaultOnOffControllerValue;
            _controlValues[actionName] = new List<string>
            {
                OnOffButtonController.OnValue,
                OnOffButtonController.OffValue
            };
        }

        public string GetControlValue(string actionName)
        {
            if (_controls.TryGetValue(actionName, out var value))
            {
                return value;
            }

            return OnOffButtonController.OffValue;
        }

        public string GetRandomControlValue(string actionName)
        {
            var currentValue = GetControlValue(actionName);
            return _controlValues[actionName].Where(v => v != currentValue).ToList().SelectRandomValue();
        }

        public void ResetControls()
        {
            _controls = new Dictionary<string, string>();
            _controlValues = new Dictionary<string, List<string>>();
        }

        public void SetUp(List<Action> actions)
        {
            ResetControls();

            foreach(var action in actions)
            {
                if (action.ControlType == ActionControlType.OnOffButton)
                {
                    AddOnOffControl(action.Name);
                }
            }
        }

        [Command]
        public void CmdSetButtonValue(string controlName, string value)
        {
            _controls[controlName] = value;
        }
    }
}
