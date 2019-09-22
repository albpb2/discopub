using Assets.Scripts.Actions;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts.Buttons
{
    public class MultiValueButtonCreator : ButtonCreator
    {
        private GameObject _multiValueChildButtonPrefab;

        public MultiValueButtonCreator(
            ButtonInstantiator buttonInstantiator, 
            GameObject multiValueButtonPrefab, 
            GameObject multiValueChildButtonPrefab) : base(buttonInstantiator, multiValueButtonPrefab)
        {
            _multiValueChildButtonPrefab = multiValueChildButtonPrefab;
        }

        protected override void SetUpButton(GameObject button, Action action, string playerPeerId)
        {
            var controller = button.GetComponent<MultiValueButtonController>();
            controller.SetUp(action.Name, action.Text, playerPeerId);

            var parentLayout = button.GetComponentInParent<HorizontalOrVerticalLayoutGroup>();
            var multiValueButtonsPanel = button.GetComponentsInChildren<LayoutElement>()[2].gameObject;
            HorizontalOrVerticalLayoutGroup finalChildLayout;

            if (parentLayout is VerticalLayoutGroup)
            {
                finalChildLayout = multiValueButtonsPanel.AddComponent<HorizontalLayoutGroup>();
            }
            else
            {
                finalChildLayout = multiValueButtonsPanel.AddComponent<VerticalLayoutGroup>();
            }

            var buttons = multiValueButtonsPanel.GetComponentsInChildren<Button>();
            foreach(var b in buttons)
            {
                GameObject.Destroy(b.gameObject);
            }

            for (var i = 0; i < action.Values.Length; i++)
            {
                CreateChildButton(action.Values[i], action.ValuesTexts[i], multiValueButtonsPanel, controller);
            }
        }

        private void CreateChildButton(string value, string valueText, GameObject multiValueButtonsPanel, MultiValueButtonController controller)
        {
            var childButton = _buttonInstantiator.InstantiateButton(_multiValueChildButtonPrefab, multiValueButtonsPanel.transform);
            var buttonComponent = childButton.GetComponent<Button>();
            buttonComponent.GetComponentInChildren<Text>().text = valueText;
            buttonComponent.onClick.AddListener(() => { controller.SetValue(value); });
        }
    }
}
