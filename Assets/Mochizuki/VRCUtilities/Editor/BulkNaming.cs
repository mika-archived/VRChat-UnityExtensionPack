/*-------------------------------------------------------------------------------------------
 * Copyright (c) Fuyuno Mikazuki / Natsuneko. All rights reserved.
 * Licensed under the MIT License. See LICENSE in the project root for license information.
 *------------------------------------------------------------------------------------------*/

using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

using UnityEditor;

using UnityEngine;

namespace Mochizuki.VRCUtilities
{
    public class BulkNaming : EditorWindow
    {
        private bool _enableCountUp;
        private List<GameObject> _gameObjects;
        private string _prefix;
        private string _replaceFrom;
        private string _replaceTo;
        private string _suffix;

        [MenuItem("Mochizuki/VRC Utilities/BulkNaming")]
        public static void ShowWindow()
        {
            var window = GetWindow<BulkNaming>();
            window.titleContent = new GUIContent("Bulk Naming");

            window.Show();
        }

        private void OnGUI()
        {
            EditorGUILayout.Space();

            EditorGUILayout.BeginVertical(GUI.skin.box);
            EditorGUILayout.LabelField("Description", new GUIStyle { fontStyle = FontStyle.Bold });
            EditorGUILayout.LabelField("Applies the same naming convention to multiple GameObjects. This change is disruptive. Please check it carefully before operation.", new GUIStyle { wordWrap = true });
            EditorGUILayout.EndVertical();

            EditorGUILayout.Space();
            EditorGUILayout.LabelField("Please drag and drop GameObjects to below box");

            var area = GUILayoutUtility.GetRect(0.0f, 50.0f, GUILayout.ExpandWidth(true));
            GUI.Box(area, "Drag and Drop GameObjects");

            if (area.Contains(Event.current.mousePosition))
                switch (Event.current.type)
                {
                    case EventType.DragUpdated:
                        DragAndDrop.visualMode = DragAndDropVisualMode.Copy;
                        Event.current.Use();
                        break;

                    case EventType.DragPerform:
                        DragAndDrop.AcceptDrag();

                        _gameObjects = DragAndDrop.objectReferences?.Where(w => w is GameObject).Cast<GameObject>().ToList();

                        DragAndDrop.activeControlID = 0;
                        Event.current.Use();
                        break;
                }

            if (_gameObjects == null || _gameObjects.Count == 0)
                return;

            EditorGUILayout.Space();
            EditorGUILayout.LabelField("Target RectTransforms are");

            EditorGUI.BeginDisabledGroup(true);
            foreach (var transform in _gameObjects) EditorGUILayout.ObjectField("GameObject", transform, typeof(RectTransform), false);
            EditorGUI.EndDisabledGroup();

            EditorGUILayout.Space();
            EditorGUILayout.LabelField("Naming Convention");

            _prefix = EditorGUILayout.TextField("Prefix", _prefix);
            _suffix = EditorGUILayout.TextField("Suffix", _suffix);

            EditorGUILayout.LabelField("Regex Replacement");
            EditorGUI.indentLevel += 1;

            _replaceFrom = EditorGUILayout.TextField("From", _replaceFrom);
            _replaceTo = EditorGUILayout.TextField("To", _replaceTo);

            EditorGUI.indentLevel -= 1;

            _enableCountUp = EditorGUILayout.Toggle("Increment", _enableCountUp);

            if (GUILayout.Button("Apply Naming Convention (Breaking Changes)"))
            {
                foreach (var o in _gameObjects.Select((w, i) => new { Index = i, Value = w }))
                {
                    var sb = new StringBuilder();

                    if (!string.IsNullOrWhiteSpace(_prefix))
                        sb.Append(_prefix);

                    if (!string.IsNullOrWhiteSpace(_replaceFrom))
                    {
                        var regex = new Regex(_replaceFrom);
                        sb.Append(regex.Replace(o.Value.name, _replaceTo));
                    }
                    else
                    {
                        sb.Append(o.Value.name);
                    }

                    if (!string.IsNullOrWhiteSpace(_suffix))
                        sb.Append(_suffix);

                    if (_enableCountUp)
                        sb.Append($"_({o.Index + 1})");

                    o.Value.name = sb.ToString();
                }

                _prefix = "";
                _suffix = "";
                _replaceFrom = "";
                _replaceTo = "";
                _enableCountUp = false;
                _gameObjects.Clear();
            }
        }
    }
}