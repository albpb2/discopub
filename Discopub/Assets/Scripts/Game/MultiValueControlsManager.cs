using Assets.Scripts.Buttons;
using Assets.Scripts.Extensions;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Assets.Scripts.Game
{
    public class MultiValueControlsManager : MonoBehaviour
    {
        private const string DefaultOnOffControllerValue = OnOffButtonController.OffValue;

        private Dictionary<string, string> _onOffControls;
        private Dictionary<string, List<string>> _onOffControlValues;

        public void Awake()
        {
            ResetControls();
        }

        public void AddOnOffControl(string actionName)
        {
            _onOffControls[actionName] = DefaultOnOffControllerValue;
            _onOffControlValues[actionName] = new List<string>
            {
                OnOffButtonController.OnValue,
                OnOffButtonController.OffValue
            };
        }
        
        public void ChangeOnOffControlValue(string actionName, string value)
        {
            if (_onOffControls.ContainsKey(actionName))
            {
                _onOffControls[actionName] = value;
            }
        }

        public string GetOnOffControlValue(string actionName)
        {
            if (_onOffControls.TryGetValue(actionName, out var value))
            {
                return value;
            }

            return OnOfButtonController.OffValue;
        }

        public string GetDifferentOnOffControlValue(string actionName)
        {
            var currentValue = GetOnOffControlValue(actionName);
            return _onOffControlValues[actionName].Where(v => v != currentValue).ToList().SelectRandomValue();
        }

        public void ResetControls()
        {
            _onOffControls = new Dictionary<string, string>();
            _onOffControlValues = new Dictionary<string, List<string>>();
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
    }
}
