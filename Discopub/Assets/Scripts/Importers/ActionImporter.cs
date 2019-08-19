using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using UnityEngine;
using Action = Assets.Scripts.Game.Action;

namespace Assets.Scripts.Importers
{
    public static class ActionImporter
    {
        private const int ExpectedLineParts = 4;
        private const char LineSeparator = ';';
        private const char ValuesSeparator = ',';

        private const int NameLinePartIndex = 0;
        private const int ControlTypeLinePartIndex = 1;
        private const int ValuesLinePartIndex = 2;
        private const int ActionPointsLinePartIndex = 3;

        public static List<Action> ImportActions(string filePath, bool hasHeaderLine)
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
                return lines.Select((l, i) => ParseLine(l, i)).ToList();
            }
            catch (Exception e)
            {
                throw new Exception("Error loading actions", e);
            }
        }

        private static string[] RemoveHeaderLine(string[] lines)
        {
            Debug.Log($"Skipping actions header.");
            return lines.Skip(1).ToArray();
        }

        private static Action ParseLine(string line, int lineIndex)
        {
            if (string.IsNullOrWhiteSpace(line))
            {
                throw new Exception($"Actions line {lineIndex} is empty.");
            }

            var lineParts = line.Split(LineSeparator);

            if (lineParts.Length != ExpectedLineParts)
            {
                throw new Exception($"Actions line {lineIndex} has {lineParts.Length}.");
            }

            var action = new Action
            {
                Name = lineParts[NameLinePartIndex],
                ControlType = lineParts[ControlTypeLinePartIndex],
                Values = lineParts[ValuesLinePartIndex].Split(ValuesSeparator),
                ActionPoints = int.Parse(lineParts[ActionPointsLinePartIndex])
            };

            return action;
        }
    }
}
