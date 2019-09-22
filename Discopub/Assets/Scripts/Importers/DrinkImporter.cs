using Assets.Scripts.Actions;
using Assets.Scripts.Buttons;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using UnityEngine;
using Action = Assets.Scripts.Actions.Action;

namespace Assets.Scripts.Importers
{
    public static class DrinkImporter
    {
        private const int ExpectedLineParts = 3;
        private const char LineSeparator = ';';

        private const int NameLinePartIndex = 0;
        private const int TextLinePartIndex = 1;
        private const int DrinkTypeLinePartIndex = 2;

        private const int DrinkActionPoints = 0;

        private static Dictionary<string, DrinkType> _drinkTypesDictionary = new Dictionary<string, DrinkType>
        {
            {"Drink", DrinkType.Drink },
            {"Beer", DrinkType.Beer }
        };

        public static List<Action> ImportDrinks(string filePath, bool hasHeaderLine)
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
                return lines.Select((l, i) => ParseLine(l, i)).Where(a => !string.IsNullOrEmpty(a.Name)).ToList();
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
                return new Action();
            }

            var lineParts = line.Split(LineSeparator);

            if (lineParts.Length != ExpectedLineParts)
            {
                throw new Exception($"Drinks line {lineIndex} has {lineParts.Length} parts, expected {ExpectedLineParts}.");
            }

            var action = new DrinkAction
            {
                Name = lineParts[NameLinePartIndex],
                ControlType = ActionControlType.DrinkButton,
                Text = lineParts[TextLinePartIndex],
                ActionPoints = DrinkActionPoints,
                DrinkType = _drinkTypesDictionary[lineParts[DrinkTypeLinePartIndex]]
            };

            return action;
        }
    }
}
