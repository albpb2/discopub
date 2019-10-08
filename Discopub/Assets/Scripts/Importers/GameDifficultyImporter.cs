using Assets.Scripts.Game;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using UnityEngine;

namespace Assets.Scripts.Importers
{
    public static class GameDifficultyImporter
    {
        private const int ExpectedLineParts = 5;
        private const char LineSeparator = ';';

        private const int MinRoundLinePartIndex = 0;
        private const int MaxRoundLinePartIndex = 1;
        private const int ActionPointsLinePartIndex = 2;
        private const int ActionSecondsLinePartIndex = 3;
        private const int NumberOfDrinksLinePartIndex = 4;

        public static List<GameDifficulty> ImportGamedifficultyLevels(string filePath, bool hasHeaderLine)
        {
            var resource = Resources.Load<TextAsset>(filePath);
            string text = resource.text;
            string[] lines = Regex.Split(text, Environment.NewLine);
            
            if (hasHeaderLine)
            {
                lines = RemoveHeaderLine(lines);
            }
            
            try
            {
                var difficultyLevels = lines.Select((l, i) => ParseLine(l, i)).ToList();
                ValidateDifficultyLevels(difficultyLevels);
                return difficultyLevels;
            }
            catch (Exception e)
            {
                throw new Exception("Error loading game difficulty levels", e);
            }
        }

        private static string[] RemoveHeaderLine(string[] lines)
        {
            Debug.Log($"Skipping game difficulty levels header.");
            return lines.Skip(1).ToArray();
        }

        private static GameDifficulty ParseLine(string line, int lineIndex)
        {
            if (string.IsNullOrWhiteSpace(line))
            {
                throw new Exception($"Game difficulty levels line {lineIndex} is empty.");
            }

            var lineParts = line.Replace("\r", "").Split(LineSeparator);

            if (lineParts.Length != ExpectedLineParts)
            {
                throw new Exception($"Game difficulty levels line {lineIndex} has {lineParts.Length}, expected {ExpectedLineParts}.");
            }

            var minRound = string.IsNullOrWhiteSpace(lineParts[MinRoundLinePartIndex])
                ? (int?)null : int.Parse(lineParts[MinRoundLinePartIndex]);

            var maxRound = string.IsNullOrWhiteSpace(lineParts[MaxRoundLinePartIndex])
                ? (int?)null : int.Parse(lineParts[MaxRoundLinePartIndex]);

            var gameDifficulty = new GameDifficulty
            {
                MinRound = minRound,
                MaxRound = maxRound,
                ActionPoints = int.Parse(lineParts[ActionPointsLinePartIndex]),
                ActionSeconds = int.Parse(lineParts[ActionSecondsLinePartIndex]),
                NumberOfDrinks = int.Parse(lineParts[NumberOfDrinksLinePartIndex]),
            };

            return gameDifficulty;
        }

        private static void ValidateDifficultyLevels(List<GameDifficulty> difficultyLevels)
        {
            var lastRoundsDifficultyLevels = difficultyLevels.Where(d => !d.MaxRound.HasValue).ToList();
            
            if (lastRoundsDifficultyLevels.Count > 1)
            {
                throw new Exception("There can only be one line without max round set.");
            }

            if (!lastRoundsDifficultyLevels.Any())
            {
                throw new Exception("There must be a line without max round set.");
            }

            var maxRoundWithDifficultyDefined = Math.Max(
                lastRoundsDifficultyLevels.Single().MinRound.GetValueOrDefault(),
                difficultyLevels.Max(d => d.MaxRound.GetValueOrDefault()));

            for (var i = 1; i <= maxRoundWithDifficultyDefined; i++)
            {
                var levelsForThisRound = difficultyLevels.Where(d =>
                    (!d.MinRound.HasValue || d.MinRound <= i) && 
                    (!d.MaxRound.HasValue || d.MaxRound >= i)).ToList();
                
                if (levelsForThisRound.Count > 1)
                {
                    throw new Exception($"Difficulty level for round {i} is defined in multiple lines");
                }

                if (!levelsForThisRound.Any())
                {
                    throw new Exception($"Difficulty level for round {i} is not defined");
                }
            }
        }
    }
}
