using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine.UIElements;

namespace JamForge.Blackboards
{
    [CustomPropertyDrawer(typeof(BlackboardValue), true)]
    public class BlackboardValueDrawer : PropertyDrawer
    {
        public override VisualElement CreatePropertyGUI(SerializedProperty property)
        {
            var container = new VisualElement();

            var valueProperty = property.FindPropertyRelative("value");
            if (valueProperty == null) { return container; }

            var field = new PropertyField(valueProperty) { label = "" };

            container.Add(field);
            return container;
        }
    }
}
