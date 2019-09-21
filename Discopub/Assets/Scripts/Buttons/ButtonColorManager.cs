using Assets.Scripts.Extensions;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts.Buttons
{
    public class ButtonColorManager : MonoBehaviour
    {
        [SerializeField]
        private List<ColorBlock> _actionButtonColorBlocks;
        [SerializeField]
        private bool _useButtonColorForText;

        public ColorBlock GetRandomColorBlock() => _actionButtonColorBlocks.SelectRandomValue();
        public bool UseButtonColorForText => _useButtonColorForText;
    }
}
