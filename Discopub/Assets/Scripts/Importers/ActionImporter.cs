﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using UnityEngine;
using Action = Assets.Scripts.Actions.Action;

namespace Assets.Scripts.Importers
{
    public static class ActionImporter
    {
        private const int ExpectedLineParts = 6;
        private const char LineSeparator = ';';
        private const char ValuesSeparator = ',';

        private const int NameLinePartIndex = 0;
        private const int ControlTypeLinePartIndex = 1;
        private const int TextLinePartIndex = 2;
        private const int ValuesLinePartIndex = 3;
        private const int ValuesTextsLinePartIndex = 4;
        private const int ActionPointsLinePartIndex = 5;

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

            var lineParts = line.Replace("\r", "").Split(LineSeparator);

            if (lineParts.Length != ExpectedLineParts)
            {
                throw new Exception($"Actions line {lineIndex} has {lineParts.Length}, expected {ExpectedLineParts}.");
            }

            var action = new Action
            {
                Name = lineParts[NameLinePartIndex],
                ControlType = lineParts[ControlTypeLinePartIndex],
                Text = lineParts[TextLinePartIndex],
                Values = lineParts[ValuesLinePartIndex].Split(ValuesSeparator),
                ValuesTexts = lineParts[ValuesTextsLinePartIndex].Split(ValuesSeparator),
                ActionPoints = int.Parse(lineParts[ActionPointsLinePartIndex])
            };

            return action;
        }
    }
}
