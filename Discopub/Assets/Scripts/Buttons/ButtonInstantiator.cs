using UnityEngine;

namespace Assets.Scripts.Buttons
{
    public class ButtonInstantiator : MonoBehaviour
    {
        public GameObject InstantiateButton(GameObject prefab, Transform parentTransform)
        {
            return Instantiate(prefab, parentTransform);
        }
    }
}
