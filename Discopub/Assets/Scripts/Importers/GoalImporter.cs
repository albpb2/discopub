using Assets.Scripts.Game;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using UnityEngine;

namespace Assets.Scripts.Importers
{
    public class GoalImporter
    {
        private const int MinimumExpectedLineParts = 4; // Name, text and at least one action (name and value)
        private const char LineSeparator = ';';

        private const int NameLinePartIndex = 0;
        private const int TextLinePartIndex = 1;
        private const int FirstGoalActionLinePartIndex = 2;

        public static List<Goal> ImportGoals(string filePath, bool hasHeaderLine, int maxRequiredActions)
        {
            var resource = Resources.Load<TextAsset>(filePath);
            string text = resource.text;
            string[] lines = Regex.Split(text, Environment.NewLine);

            if (hasHeaderLine)
            {
                lines = RemoveHeaderLine(lines);
            }

            var maxLineParts = ((maxRequiredActions - 1) * 2) + MinimumExpectedLineParts;

            try
            {
                return lines.Select((l, i) => ParseLine(l, i, maxLineParts)).ToList();
            }
            catch (Exception e)
            {
                throw new Exception("Error loading goals", e);
            }
        }

        private static string[] RemoveHeaderLine(string[] lines)
        {
            Debug.Log($"Skipping goals header.");
            return lines.Skip(1).ToArray();
        }

        private static Goal ParseLine(string line, int lineIndex, int maxLineParts)
        {
            if (string.IsNullOrWhiteSpace(line))
            {
                throw new Exception($"Goals line {lineIndex} is empty.");
            }

            var lineParts = line.Split(LineSeparator);

            if (lineParts.Length < MinimumExpectedLineParts || lineParts.Length > maxLineParts)
            {
                throw new Exception($"Goals line {lineIndex} has {lineParts.Length}.");
            }

            var requiredActions = ParseRequiredActions(lineParts).ToList();
            if (!requiredActions.Any())
            {
                throw new Exception($"No actions found for goal in line {lineIndex}.");
            }

            var goal = new Goal
            {
                Name = lineParts[NameLinePartIndex],
                Text = lineParts[TextLinePartIndex],
                RequiredActions = requiredActions
            };

            return goal;
        }

        private static IEnumerable<GoalAction> ParseRequiredActions(string[] lineParts)
        {
            var requiredActions = new List<GoalAction>();
            for (var i = FirstGoalActionLinePartIndex; i < lineParts.Length; i += 2)
            {
                if (!string.IsNullOrWhiteSpace(lineParts[i]))
                {
                    yield return new GoalAction
                    {
                        Name = lineParts[i],
                        Value = lineParts[i + 1],
                    };
                }
            }
        }
    }
}
