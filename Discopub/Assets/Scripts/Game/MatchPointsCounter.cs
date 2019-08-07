using System;
using UnityEngine;
using UnityEngine.Networking;

namespace Assets.Scripts.Game
{
    public class MatchPointsCounter : NetworkBehaviour
    {
        [SerializeField]
        private TMPro.TMP_Text _matchPointsText;

        private int _currentPoints;
        private int _maxPoints;
        
        public void ResetPoints()
        {
            _currentPoints = 0;
            PrintPoints();
        }

        public void SetMaxPoints(int maxPoints)
        {
            _maxPoints = maxPoints;
            PrintPoints();
        }

        public void SetPoints(int currentPoints)
        {
            _currentPoints = Math.Min(currentPoints, _maxPoints);
            PrintPoints();
        }

        public void IncreasePoints(int pointsToAdd)
        {
            _currentPoints += pointsToAdd;
            _currentPoints = Math.Min(_currentPoints, _maxPoints);
            PrintPoints();
        }

        private void PrintPoints()
        {
            _matchPointsText.text = $"{_currentPoints}/{_maxPoints}";
        }
    }
}
