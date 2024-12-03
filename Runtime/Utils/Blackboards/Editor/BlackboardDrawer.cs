using System;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace JamForge.Blackboards
{
    [CustomPropertyDrawer(typeof(Blackboard))]
    public class BlackboardDrawer : PropertyDrawer
    {
        private VisualElement _root;
        private bool _isFolded;
        private int _lastRefreshTick;

        private const int MinRefreshTicks = 30;

        public override VisualElement CreatePropertyGUI(SerializedProperty property)
        {
            _root = new VisualElement();
            _root.TrackPropertyValue(property, sp => RefreshUI(sp));
            RefreshUI(property, true);
            return _root;
        }

        private void RefreshUI(SerializedProperty property, bool forceRefresh = false)
        {
            if (!forceRefresh)
            {
                if (Time.frameCount - _lastRefreshTick < MinRefreshTicks) { return; }
            }

            _lastRefreshTick = Time.frameCount;
            _root.Clear();

            // Style the container
            _root.style.backgroundColor = new Color(0.2f, 0.2f, 0.2f, 0.3f);
            _root.style.borderBottomWidth = _root.style.borderTopWidth =
                _root.style.borderLeftWidth = _root.style.borderRightWidth = 1;
            _root.style.borderBottomColor = _root.style.borderTopColor =
                _root.style.borderLeftColor = _root.style.borderRightColor = new Color(0.1f, 0.1f, 0.1f, 0.3f);
            _root.style.marginTop = _root.style.marginBottom = 2;
            _root.style.paddingTop = _root.style.paddingBottom =
                _root.style.paddingLeft = _root.style.paddingRight = 4;

            // Create foldout
            var foldout = new Foldout { value = !_isFolded, text = ObjectNames.NicifyVariableName(property.name), style = { marginLeft = 12 } };
            foldout.RegisterValueChangedCallback(evt => _isFolded = !evt.newValue);
            _root.Add(foldout);

            // Content container
            var content = new VisualElement { style = { marginLeft = -12, marginTop = 4 } };
            foldout.Add(content);

            // Header
            var header = new VisualElement { style = { flexDirection = FlexDirection.Row, justifyContent = Justify.SpaceBetween, marginBottom = 4 } };

            // Display Total Entries: X
            var totalEntries = new Label($"Total Entries: {property.FindPropertyRelative("keys")?.arraySize}")
            {
                style = { unityFontStyleAndWeight = FontStyle.Bold }
            };
            header.Add(totalEntries);

            // Add new entry name field and button container
            var addContainer = new VisualElement { style = { flexDirection = FlexDirection.Row } };

            // New entry name field
            var newEntryName = new TextField { value = "NewEntry", style = { width = 120, marginRight = 4 } };
            addContainer.Add(newEntryName);

            var addButton = new Button(() => ShowAddMenu(property, newEntryName.value)) { text = "+", style = { width = 20 } };
            addContainer.Add(addButton);

            header.Add(addContainer);

            content.Add(header);

            // Entries
            var keysProperty = property.FindPropertyRelative("keys");
            var valuesProperty = property.FindPropertyRelative("values");

            if (keysProperty != null && valuesProperty != null)
            {
                var entriesContainer = new VisualElement { style = { marginLeft = 0 } };
                content.Add(entriesContainer);

                for (var i = 0; i < keysProperty.arraySize; i++)
                {
                    var index = i;
                    var entryContainer = new VisualElement { style = { flexDirection = FlexDirection.Row, marginBottom = 2 } };

                    var keyProperty = keysProperty.GetArrayElementAtIndex(index);
                    var valueProperty = valuesProperty.GetArrayElementAtIndex(index);

                    // Key label (readonly)
                    var keyLabel = new Label(keyProperty.stringValue)
                    {
                        style =
                        {
                            width = 120,
                            marginRight = 4,
                            backgroundColor = new Color(0.2f, 0.2f, 0.2f, 0.3f),
                            paddingLeft = 3,
                            paddingRight = 3,
                            paddingTop = 1,
                            paddingBottom = 1,
                            unityFontStyleAndWeight = FontStyle.Bold
                        }
                    };
                    entryContainer.Add(keyLabel);

                    // Value field
                    var valueField = new PropertyField(valueProperty) { style = { flexGrow = 1 } };
                    valueField.Bind(valueProperty.serializedObject);

                    // Track value changes using SerializedObject
                    valueField.TrackPropertyValue(valueProperty, changedProperty =>
                    {
                        changedProperty.serializedObject.ApplyModifiedProperties();
                        EditorUtility.SetDirty(changedProperty.serializedObject.targetObject);
                    });

                    entryContainer.Add(valueField);

                    // Remove button
                    var removeButton = new Button(() => RemoveEntry(property, index)) { text = "-", style = { marginLeft = 8, width = 20 } };
                    entryContainer.Add(removeButton);

                    entriesContainer.Add(entryContainer);
                }
            }
        }

        private void ShowAddMenu(SerializedProperty property, string baseKeyName)
        {
            var menu = new GenericMenu();

            // Add menu items for each BlackboardValue type
            // TODO: Use reflection to get all types later
            AddMenuItem<BlackboardInt>(menu, "Int", property, baseKeyName);
            AddMenuItem<BlackboardFloat>(menu, "Float", property, baseKeyName);
            AddMenuItem<BlackboardBool>(menu, "Bool", property, baseKeyName);
            AddMenuItem<BlackboardString>(menu, "String", property, baseKeyName);
            AddMenuItem<BlackboardVector2>(menu, "Vector2", property, baseKeyName);
            AddMenuItem<BlackboardVector3>(menu, "Vector3", property, baseKeyName);
            AddMenuItem<BlackboardQuaternion>(menu, "Quaternion", property, baseKeyName);

            menu.ShowAsContext();
        }

        private void AddMenuItem<T>(GenericMenu menu, string name, SerializedProperty property, string baseKeyName) where T : BlackboardValue, new() =>
            menu.AddItem(new GUIContent($"Add {name}"), false, () =>
            {
                var keysProperty = property.FindPropertyRelative("keys");
                var valuesProperty = property.FindPropertyRelative("values");

                // Generate a unique key name
                var keyName = baseKeyName;
                var counter = 1;

                while (IsKeyNameTaken(keysProperty, keyName))
                {
                    keyName = $"{baseKeyName}{counter}";
                    counter++;
                }

                keysProperty.InsertArrayElementAtIndex(keysProperty.arraySize);
                valuesProperty.InsertArrayElementAtIndex(valuesProperty.arraySize);

                var newKey = keysProperty.GetArrayElementAtIndex(keysProperty.arraySize - 1);
                var newValue = valuesProperty.GetArrayElementAtIndex(valuesProperty.arraySize - 1);

                newKey.stringValue = keyName;
                newValue.managedReferenceValue = new T();

                property.serializedObject.ApplyModifiedProperties();
                property.serializedObject.Update();
                RefreshUI(property, true);
            });

        private bool IsKeyNameTaken(SerializedProperty keysProperty, string keyName)
        {
            for (var i = 0; i < keysProperty.arraySize; i++)
            {
                var existingKey = keysProperty.GetArrayElementAtIndex(i).stringValue;
                if (string.Equals(keyName, existingKey, StringComparison.Ordinal) ||
                    keyName.GetHashCode() == existingKey.GetHashCode()) { return true; }
            }
            return false;
        }

        private void RemoveEntry(SerializedProperty property, int index)
        {
            var keysProperty = property.FindPropertyRelative("keys");
            var valuesProperty = property.FindPropertyRelative("values");

            keysProperty.DeleteArrayElementAtIndex(index);
            valuesProperty.DeleteArrayElementAtIndex(index);

            property.serializedObject.ApplyModifiedProperties();
            property.serializedObject.Update();
            RefreshUI(property, true);
        }
    }
}
