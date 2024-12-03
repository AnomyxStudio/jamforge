using System.Collections.Generic;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEditorInternal;
using UnityEngine;

namespace JamForge
{
    [CustomEditor(typeof(GameProcedures), true)]
    public class GameProceduresEditor : Editor
    {
        private SerializedProperty _defaultProcedure;
        private SerializedProperty _procedures;

        private ReorderableList _procedureList;
        private GUIContent _addLabel;
        private GUIContent _removeLabel;
        private GUIContent _editLabel;

        private GameProcedures _gameProcedures;
        private bool _listExpanding = true;

        private string DefaultProcedure
        {
            get => _defaultProcedure.stringValue;
            set
            {
                _defaultProcedure.stringValue = value;
                serializedObject.ApplyModifiedProperties();
            }
        }

        private List<string> GetProcedures()
        {
            var proceduresArray = new List<string>();

            for (var i = 0; i < _procedures.arraySize; i++)
            {
                proceduresArray.Add(_procedures.GetArrayElementAtIndex(i).stringValue);
            }

            return proceduresArray;
        }

        private void SetProcedure(int index, string value)
        {
            _procedures.GetArrayElementAtIndex(index).stringValue = value;
            serializedObject.ApplyModifiedProperties();
        }

        private void AddProcedure(string procedure)
        {
            _procedures.arraySize++;
            _procedures.GetArrayElementAtIndex(_procedures.arraySize - 1).stringValue = procedure;
            serializedObject.ApplyModifiedProperties();
        }

        private void RemoveProcedure(int index)
        {
            _procedures.DeleteArrayElementAtIndex(index);
            serializedObject.ApplyModifiedProperties();
        }

        private void OnEnable()
        {
            _gameProcedures = target as GameProcedures;

            _defaultProcedure = serializedObject.FindProperty("defaultProcedure");
            _procedures = serializedObject.FindProperty("procedureTypes");

            _addLabel = new GUIContent
            {
                image = EditorGUIUtility.IconContent("d_Toolbar Plus More").image,
                tooltip = "Add a new procedure"
            };
            _removeLabel = new GUIContent
            {
                image = EditorGUIUtility.IconContent("d_Toolbar Minus").image,
                tooltip = "Remove select procedure"
            };
            _editLabel = new GUIContent
            {
                text = "Edit",
                tooltip = "Edit procedure script"
            };

            _procedureList = new ReorderableList(serializedObject, _procedures, true, true, false, false)
            {
                footerHeight = 0,
                drawHeaderCallback = OnAddDropdownCallback,
                drawElementCallback = OnDrawElementCallback,
                drawElementBackgroundCallback = OnDrawElementBackgroundCallback
            };
        }

        public override void OnInspectorGUI()
        {
            DrawEditorGUI();
            DrawRuntimeGUI();
        }

        private void DrawEditorGUI()
        {
            GUI.enabled = !EditorApplication.isPlaying;

            var scriptProperty = serializedObject.FindProperty("m_Script");
            using (new EditorGUI.DisabledScope(true))
            {
                EditorGUILayout.PropertyField(scriptProperty);
            }

            EditorGUILayout.Separator();
            using (new EditorGUILayout.HorizontalScope())
            {
                EditorGUILayout.PrefixLabel("Default Procedure");
                if (EditorGUILayout.DropdownButton(new GUIContent(_defaultProcedure.stringValue), FocusType.Passive))
                {
                    DrawDefaultMenu();
                }
            }
            EditorGUILayout.Separator();

            _procedureList.DoLayoutList();

            serializedObject.ApplyModifiedProperties();

            GUI.enabled = true;
        }

        private void DrawRuntimeGUI()
        {
            if (!Application.isPlaying) { return; }

            EditorGUILayout.Separator();
            EditorGUILayout.LabelField("Runtime Data:", EditorStyles.boldLabel);

            if (_gameProcedures.CurrentProcedure == null) { return; }

            using var verticalScope = new EditorGUILayout.VerticalScope(EditorStyles.helpBox);

            using (new EditorGUILayout.HorizontalScope())
            {
                EditorGUILayout.PrefixLabel("Current Procedure");
                EditorGUILayout.LabelField(_gameProcedures.CurrentProcedure.GetType().Name, EditorStyles.boldLabel);
            }

            EditorGUILayout.Separator();

            using var indentLevelScope = new EditorGUI.IndentLevelScope(1);

            _listExpanding = EditorGUILayout.Foldout(_listExpanding, $"Procedures [{_gameProcedures.Procedures.Count}]");
            if (!_listExpanding) { return; }

            using var indentLevelScope2 = new EditorGUI.IndentLevelScope(1);
            var originalColor = GUI.color;
            foreach (var procedure in _gameProcedures.Procedures)
            {
                using (new EditorGUILayout.HorizontalScope())
                {
                    var isActive = procedure.Value == _gameProcedures.CurrentProcedure;

                    GUI.color = isActive ? Color.green : originalColor;
                    EditorGUILayout.LabelField(procedure.Key);
                    GUI.color = originalColor;

                    GUI.enabled = !isActive;
                    GUILayout.FlexibleSpace();
                    if (GUILayout.Button("Switch", EditorStyles.miniButton))
                    {
                        _gameProcedures.SwitchProcedure(procedure.Key);
                    }
                    EditorGUILayout.Space(10);
                    GUI.enabled = true;
                }
            }
            EditorGUILayout.Separator();
        }

        private void OnDrawElementBackgroundCallback(Rect rect, int index, bool isActive, bool isFocused)
        {
            if (Event.current.type == EventType.Repaint)
            {
                GUIStyle gUIStyle = (index % 2 != 0) ? "CN EntryBackEven" : "Box";
                gUIStyle = (!isActive && !isFocused) ? gUIStyle : "RL Element";
                rect.x += 2;
                rect.width -= 6;
                gUIStyle.Draw(rect, false, isActive, isActive, isFocused);
            }
        }

        private void OnDrawElementCallback(Rect rect, int index, bool isActive, bool isFocused)
        {
            var defaultProcedure = DefaultProcedure;
            var procedures = GetProcedures();
            if (index >= 0 && index < procedures.Count)
            {
                var subRect = rect;
                subRect.Set(rect.x, rect.y + 2, rect.width, 16);

                // Set text to green if it's the default procedure
                if (procedures[index] == defaultProcedure)
                {
                    var originalColor = GUI.color;
                    GUI.color = Color.green;
                    GUI.Label(subRect, procedures[index], EditorStyles.boldLabel);
                    GUI.color = originalColor;
                }
                else
                {
                    GUI.Label(subRect, procedures[index], EditorStyles.boldLabel);
                }

                const int editButtonSize = 40;

                subRect.Set(rect.x + rect.width - editButtonSize - 5, rect.y + 3, editButtonSize, 18);
                if (GUI.Button(subRect, _editLabel, EditorStyles.miniButton))
                {
                    ScriptFinder.OpenScript(procedures[index]);
                }
            }

            serializedObject.ApplyModifiedProperties();
        }

        private void OnAddDropdownCallback(Rect rect)
        {
            var sub = rect;
            var procedures = GetProcedures();
            sub.Set(rect.x, rect.y, 200, rect.height);

            GUI.Label(sub, "Activated Procedures");
            if (!EditorApplication.isPlaying)
            {
                sub.Set(rect.x + rect.width - 40, rect.y - 2, 20, 20);
                if (GUI.Button(sub, _addLabel, "InvisibleButton"))
                {
                    var gm = new GenericMenu();
                    var types = TypeFinder.GetSubclassesOf(typeof(ProcedureBase));
                    for (var i = 0; i < types.Count; i++)
                    {
                        var j = i;

                        void AppItemFunction()
                        {
                            Undo.RecordObject(target, "Add Procedure");

                            AddProcedure(types[j].FullName);
                            if (string.IsNullOrEmpty(DefaultProcedure))
                            {
                                DefaultProcedure = _procedures.GetArrayElementAtIndex(0).stringValue;
                            }

                            MarkDirty();
                        }

                        if (procedures.Contains(types[j].FullName))
                        {
                            gm.AddDisabledItem(new GUIContent(types[j].FullName), true);
                        }
                        else
                        {
                            gm.AddItem(new GUIContent(types[j].FullName), false, AppItemFunction);
                        }
                    }
                    gm.ShowAsContext();
                }

                sub.Set(rect.x + rect.width - 20, rect.y - 2, 20, 20);
                GUI.enabled = _procedureList.index >= 0 && _procedureList.index < procedures.Count;
                if (GUI.Button(sub, _removeLabel, "InvisibleButton"))
                {
                    Undo.RecordObject(target, "Delete Procedure");

                    if (DefaultProcedure == _procedures.GetArrayElementAtIndex(_procedureList.index).stringValue)
                    {
                        DefaultProcedure = null;
                    }

                    RemoveProcedure(_procedureList.index);

                    if (string.IsNullOrEmpty(DefaultProcedure) && _procedures.arraySize > 0)
                    {
                        DefaultProcedure = _procedures.GetArrayElementAtIndex(0).stringValue;
                    }

                    MarkDirty();
                }
                GUI.enabled = true;
            }
        }

        private void DrawDefaultMenu()
        {
            var defaultProcedure = DefaultProcedure;
            var procedures = GetProcedures();
            var gm = new GenericMenu();
            for (var i = 0; i < procedures.Count; i++)
            {
                var procedure = procedures[i];
                if (defaultProcedure == procedure)
                {
                    gm.AddDisabledItem(new GUIContent(defaultProcedure), true);
                }
                else
                {
                    void SetDefaultFunction()
                    {
                        Undo.RecordObject(target, "Set Default Procedure");
                        DefaultProcedure = procedure;
                        MarkDirty();
                    }

                    gm.AddItem(new GUIContent(procedure), false, SetDefaultFunction);
                }
            }
            gm.ShowAsContext();
        }

        private void MarkDirty(bool markTarget = false)
        {
            if (markTarget)
            {
                EditorUtility.SetDirty(target);
            }
            else
            {
                foreach (var t in targets)
                {
                    EditorUtility.SetDirty(t);
                }
            }

            if (EditorApplication.isPlaying) return;

            var component = target as Component;
            if (component != null)
            {
                EditorSceneManager.MarkSceneDirty(component.gameObject.scene);
            }
        }
    }
}
