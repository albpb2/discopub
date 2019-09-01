﻿using System;
using UnityEngine;
using UnityEngine.Networking;

namespace Assets.Scripts.Game
{
    public class MatchPointsCounter : NetworkBehaviour
    {
        private const int MaxPoints = 500;

        [SerializeField]
        private TMPro.TMP_Text _matchPointsText;

        private int _currentPoints;
        private int _maxPoints;
        
        public void ResetCurrentPoints()
        {
            _currentPoints = 0;
            RpcPrintPoints(_currentPoints, _maxPoints);
        }

        public void ResetCounter()
        {
            SetMaxPoints(MaxPoints);
            SetPoints(0);
        }

        public void SetMaxPoints(int maxPoints)
        {
            _maxPoints = maxPoints;
            RpcPrintPoints(_currentPoints, _maxPoints);
        }

        public void SetPoints(int currentPoints)
        {
            _currentPoints = Math.Min(currentPoints, _maxPoints);

            const int minimumPoints = 0;
            _currentPoints = Math.Max(_currentPoints, minimumPoints);

            RpcPrintPoints(_currentPoints, _maxPoints);
        }

        public void IncreasePoints(int pointsToAdd)
        {
            SetPoints(_currentPoints + pointsToAdd);
        }

        public void DecreasePoints(int pointsToAdd)
        {
            SetPoints(_currentPoints - pointsToAdd);
        }

        [ClientRpc]
        private void RpcPrintPoints(int currentPoints, int maxPoints)
        {
            _matchPointsText.text = $"{currentPoints}/{maxPoints}";
        }
    }
}
