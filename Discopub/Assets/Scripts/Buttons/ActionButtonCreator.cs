using Assets.Scripts.Actions;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts.Buttons
{
    public class ActionButtonCreator : ButtonCreator
    {
        private readonly ButtonColorManager _buttonColorManager;

        public ActionButtonCreator(ButtonInstantiator buttonInstantiator, GameObject actionButtonPrefab) : base(buttonInstantiator, actionButtonPrefab)
        {
            _buttonColorManager = GameObject.FindObjectOfType<ButtonColorManager>();
        }

        protected override void SetUpButton(GameObject button, Action action, string playerPeerId)
        {
            var controller = button.GetComponentInChildren<ActionButtonController>();
            controller.SetUp(action.Name, action.Text, playerPeerId);

            ChangeButtonColor(button);
        }

        private void ChangeButtonColor(GameObject button)
        {
            var buttonComponent = button.GetComponent<Button>();
            buttonComponent.colors = _buttonColorManager.GetRandomColorBlock();
            
            if (_buttonColorManager.UseButtonColorForText)
            {
                var textComponent = button.GetComponentInChildren<Text>();
                textComponent.color = buttonComponent.colors.normalColor;
            }
        }
    }
}
