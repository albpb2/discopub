using Assets.Scripts.Game;
using Assets.Scripts.UI;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

namespace Assets.Scripts.Buttons
{
    public class DrinkButtonsPanelCreator : NetworkBehaviour
    {
        private GameObject _drinkButtonsPanel;

        [SerializeField]
        private GameObject _drinkButtonsPanelParent;
        [SerializeField]
        private GameObject _drinkButtonPrefab;
        [SerializeField]
        private GameObject _emptyLayoutPrefab;
        [SerializeField]
        private ButtonInstantiator _buttonInstantiator;

        private DrinkButtonCreator _drinkButtonCreator;

        public void Awake()
        {
            _drinkButtonCreator = new DrinkButtonCreator(_buttonInstantiator, _drinkButtonPrefab);
        }

        [TargetRpc]
        public void TargetCreateDrinkButtonsPanel(NetworkConnection connection, string parsedActions, string playerPeerId)
        {
            Debug.Log($"Received rpc to create drinks panel");
            var actions = JsonConvert.DeserializeObject<List<Action>>(parsedActions);
            _drinkButtonsPanel = InstantiateLayout(_drinkButtonsPanelParent);
            FillLayout(_drinkButtonsPanel, actions.ToList(), playerPeerId);
            _drinkButtonsPanel.SetActive(false);
        }

        [TargetRpc]
        public void TargetEnableDrinkButtonsPannel(NetworkConnection connection)
        {
            Debug.Log($"Received rpc to enable drinks panel");
            _drinkButtonsPanel.SetActive(true);
        }

        [ClientRpc]
        public void RpcDestroyPanel()
        {
            Debug.Log($"Received rpc to disable drinks panel");
            Destroy(_drinkButtonsPanel.gameObject);
        }

        private void FillLayout(GameObject layoutGameObject, List<Action> actions, string playerPeerId)
        {
            foreach(var action in actions)
            {
                _drinkButtonCreator.CreateButton(action, _drinkButtonsPanel, playerPeerId);
            }
        }

        private GameObject InstantiateLayout( GameObject parentObject)
        {
            var layoutGameObject = Instantiate(_emptyLayoutPrefab, parentObject.transform);

            var layout = layoutGameObject.AddComponent<HorizontalLayoutGroup>();

            layout.childControlHeight = true;
            layout.childControlWidth = true;
            layout.childForceExpandHeight = true;
            layout.childForceExpandWidth = true;

            return layoutGameObject;
        }
    }
}
