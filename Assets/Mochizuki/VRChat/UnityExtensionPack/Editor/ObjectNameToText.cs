/*-------------------------------------------------------------------------------------------
 * Copyright (c) Fuyuno Mikazuki / Natsuneko. All rights reserved.
 * Licensed under the MIT License. See LICENSE in the project root for license information.
 *------------------------------------------------------------------------------------------*/

using System.Collections.Generic;
using System.Linq;

using UnityEditor;

using UnityEngine;
using UnityEngine.UI;

namespace Mochizuki.VRChat.UnityExtensionPack
{
    public class ObjectNameToText : EditorWindow
    {
        private List<GameObject> _gameObjects;

        [MenuItem("Mochizuki/VRC Utilities/ObjectNameToText")]
        public static void ShowEditorWindow()
        {
            var window = GetWindow<ObjectNameToText>();
            window.titleContent = new GUIContent("ObjectName To Text");

            window.Show();
        }

        private void OnGUI()
        {
            EditorGUILayout.Space();

            EditorGUILayout.BeginVertical(GUI.skin.box);
            EditorGUILayout.LabelField("Description", new GUIStyle { fontStyle = FontStyle.Bold });
            EditorGUILayout.LabelField("Matches the value of the first Text Component contained in the descendant object to the name of the Game Object. This change is disruptive. Please check it carefully before operation.", new GUIStyle { wordWrap = true });
            EditorGUILayout.EndVertical();

            EditorGUILayout.Space();
            EditorGUILayout.LabelField("Please drag and drop RectTransforms to below box");

            var area = GUILayoutUtility.GetRect(0.0f, 50.0f, GUILayout.ExpandWidth(true));
            GUI.Box(area, "Drag and Drop RectTransforms");

            if (area.Contains(Event.current.mousePosition))
                switch (Event.current.type)
                {
                    case EventType.DragUpdated:
                        DragAndDrop.visualMode = DragAndDropVisualMode.Copy;
                        Event.current.Use();
                        break;

                    case EventType.DragPerform:
                        DragAndDrop.AcceptDrag();

                        _gameObjects = DragAndDrop.objectReferences?.Where(w => w is GameObject).Cast<GameObject>().Where(w => w.GetComponentInChildren<Text>() != null).ToList();

                        DragAndDrop.activeControlID = 0;
                        Event.current.Use();
                        break;
                }

            if (_gameObjects == null || _gameObjects.Count == 0)
                return;

            EditorGUILayout.Space();
            EditorGUILayout.LabelField("Target GameObjects are");

            EditorGUI.BeginDisabledGroup(true);
            foreach (var gameObject in _gameObjects) EditorGUILayout.ObjectField("GameObject", gameObject, typeof(RectTransform), false);
            EditorGUI.EndDisabledGroup();

            if (GUILayout.Button("Apply (Breaking Changes)"))
            {
                foreach (var gameObject in _gameObjects)
                {
                    var objectName = gameObject.name;
                    var first = gameObject.GetComponentInChildren<Text>();
                    first.text = objectName;
                }

                _gameObjects.Clear();
            }
        }
    }
}