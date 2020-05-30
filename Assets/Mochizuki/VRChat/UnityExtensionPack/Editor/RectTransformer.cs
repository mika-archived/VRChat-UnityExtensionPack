/*-------------------------------------------------------------------------------------------
 * Copyright (c) Fuyuno Mikazuki / Natsuneko. All rights reserved.
 * Licensed under the MIT License. See LICENSE in the project root for license information.
 *------------------------------------------------------------------------------------------*/

using System.Collections.Generic;
using System.Linq;

using UnityEditor;

using UnityEngine;

namespace Mochizuki.VRChat.UnityExtensionPack
{
    public class RectTransformer : EditorWindow
    {
        private Vector3 _positionOffset;
        private Vector3 _rotationOffset; // euler
        private List<RectTransform> _transforms;

        [MenuItem("Mochizuki/VRC Utilities/RectTransformer")]
        public static void ShowEditorWindow()
        {
            var window = GetWindow<RectTransformer>();
            window.titleContent = new GUIContent("Rect Transformer");

            window.Show();
        }

        private void OnGUI()
        {
            EditorGUILayout.Space();

            EditorGUILayout.BeginVertical(GUI.skin.box);
            EditorGUILayout.LabelField("Description", new GUIStyle { fontStyle = FontStyle.Bold });
            EditorGUILayout.LabelField("Changes multiple RectTransform values at once with the same operating criteria. This change is disruptive. Please check it carefully before operation.", new GUIStyle { wordWrap = true });
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

                        _transforms = DragAndDrop.objectReferences?.Where(w => w is GameObject).Select(w => ((GameObject) w).GetComponent<RectTransform>()).Where(w => w != null).ToList();

                        DragAndDrop.activeControlID = 0;
                        Event.current.Use();
                        break;
                }

            if (_transforms == null || _transforms.Count == 0)
                return;

            EditorGUILayout.Space();
            EditorGUILayout.LabelField("Target RectTransforms are");

            EditorGUI.BeginDisabledGroup(true);
            foreach (var transform in _transforms) EditorGUILayout.ObjectField("RectTransform", transform, typeof(RectTransform), false);
            EditorGUI.EndDisabledGroup();

            EditorGUILayout.Space();
            EditorGUILayout.LabelField("Transform RectTransforms Configurations");

            _positionOffset = EditorGUILayout.Vector3Field("Position Offset from Current", _positionOffset);
            _rotationOffset = EditorGUILayout.Vector3Field("Rotation Offset from Current", _rotationOffset);

            if (GUILayout.Button("Apply Transform Modifications (Breaking Changes)"))
            {
                foreach (var transform in _transforms)
                {
                    transform.localPosition += _positionOffset;
                    transform.localRotation = transform.localRotation * Quaternion.Euler(_rotationOffset);
                }

                _positionOffset = Vector3.zero;
                _transforms.Clear();
            }
        }
    }
}