using UnityEditor;
using UnityEngine;

namespace JamForge.Audio
{
    [CustomEditor(typeof(AudioDefine))]
    public class AudioDefineEditor : Editor
    {
        private SerializedProperty _clips;
        private SerializedProperty _loop;
        private SerializedProperty _overrideVolume;
        private SerializedProperty _overridePitch;
        private SerializedProperty _playbackMode;

        private SerializedProperty _crossFade;
        private SerializedProperty _volume;
        private SerializedProperty _pitchMin;
        private SerializedProperty _pitchMax;

        private void OnEnable()
        {
            _clips = serializedObject.FindProperty("clips");
            _loop = serializedObject.FindProperty("loop");
            _overrideVolume = serializedObject.FindProperty("overrideVolume");
            _overridePitch = serializedObject.FindProperty("overridePitch");
            _playbackMode = serializedObject.FindProperty("playbackMode");

            _crossFade = serializedObject.FindProperty("crossFade");
            _volume = serializedObject.FindProperty("volume");
            _pitchMin = serializedObject.FindProperty("pitchMin");
            _pitchMax = serializedObject.FindProperty("pitchMax");
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();
            using var check = new EditorGUI.ChangeCheckScope();
            EditorGUILayout.PropertyField(_clips, true);
            EditorGUILayout.PropertyField(_loop);
            EditorGUILayout.PropertyField(_crossFade);
            if (_crossFade.floatValue < 0) { _crossFade.floatValue = 0; }

            EditorGUILayout.PropertyField(_playbackMode);
            EditorGUILayout.PropertyField(_overrideVolume);
            if (_overrideVolume.boolValue)
            {
                using (new EditorGUI.IndentLevelScope(1))
                {
                    EditorGUILayout.PropertyField(_volume);
                }
            }

            EditorGUILayout.PropertyField(_overridePitch);
            if (_overridePitch.boolValue)
            {
                using (new EditorGUI.IndentLevelScope(1))
                {
                    EditorGUILayout.PropertyField(_pitchMin);
                    EditorGUILayout.PropertyField(_pitchMax);
                }
            }

            if (check.changed)
            {
                serializedObject.ApplyModifiedProperties();
            }
        }
    }
}
