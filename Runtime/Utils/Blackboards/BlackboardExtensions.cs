using System;
using UnityEngine;

namespace JamForge.Blackboards
{
    public static class BlackboardExtensions
    {
        public static void SetInt(this Blackboard blackboard, string key, int value) => blackboard.SetValue(key, value);

        public static void SetFloat(this Blackboard blackboard, string key, float value) => blackboard.SetValue(key, value);

        public static void SetBool(this Blackboard blackboard, string key, bool value) => blackboard.SetValue(key, value);

        public static void SetString(this Blackboard blackboard, string key, string value) => blackboard.SetValue(key, value);

        public static void SetVector2(this Blackboard blackboard, string key, Vector2 value) => blackboard.SetValue(key, value);

        public static void SetVector3(this Blackboard blackboard, string key, Vector3 value) => blackboard.SetValue(key, value);

        public static void SetQuaternion(this Blackboard blackboard, string key, Quaternion value) => blackboard.SetValue(key, value);

        public static void SetGameObject(this Blackboard blackboard, string key, GameObject value) => blackboard.SetValue(key, value);

        public static void SetComponent<T>(this Blackboard blackboard, string key, T value) where T : Component => blackboard.SetValue(key, value);

        public static void SetEnum<T>(this Blackboard blackboard, string key, T value) where T : Enum => blackboard.SetValue(key, value);

        public static int GetInt(this Blackboard blackboard, string key) => blackboard.GetValue<int>(key);

        public static float GetFloat(this Blackboard blackboard, string key) => blackboard.GetValue<float>(key);

        public static bool GetBool(this Blackboard blackboard, string key) => blackboard.GetValue<bool>(key);

        public static string GetString(this Blackboard blackboard, string key) => blackboard.GetValue<string>(key);

        public static Vector2 GetVector2(this Blackboard blackboard, string key) => blackboard.GetValue<Vector2>(key);

        public static Vector3 GetVector3(this Blackboard blackboard, string key) => blackboard.GetValue<Vector3>(key);

        public static Quaternion GetQuaternion(this Blackboard blackboard, string key) => blackboard.GetValue<Quaternion>(key);

        public static GameObject GetGameObject(this Blackboard blackboard, string key) => blackboard.GetValue<GameObject>(key);

        public static T GetComponent<T>(this Blackboard blackboard, string key) where T : Component => blackboard.GetValue<T>(key);

        public static T GetEnum<T>(this Blackboard blackboard, string key) where T : Enum => blackboard.GetValue<T>(key);

        public static bool TryGetInt(this Blackboard blackboard, string key, out int value) => blackboard.TryGetValue(key, out value);

        public static bool TryGetFloat(this Blackboard blackboard, string key, out float value) => blackboard.TryGetValue(key, out value);

        public static bool TryGetBool(this Blackboard blackboard, string key, out bool value) => blackboard.TryGetValue(key, out value);

        public static bool TryGetString(this Blackboard blackboard, string key, out string value) => blackboard.TryGetValue(key, out value);

        public static bool TryGetVector2(this Blackboard blackboard, string key, out Vector2 value) => blackboard.TryGetValue(key, out value);

        public static bool TryGetVector3(this Blackboard blackboard, string key, out Vector3 value) => blackboard.TryGetValue(key, out value);

        public static bool TryGetQuaternion(this Blackboard blackboard, string key, out Quaternion value) => blackboard.TryGetValue(key, out value);

        public static bool TryGetGameObject(this Blackboard blackboard, string key, out GameObject value) => blackboard.TryGetValue(key, out value);

        public static bool TryGetComponent<T>(this Blackboard blackboard, string key, out T value) where T : Component =>
            blackboard.TryGetValue(key, out value);

        public static bool TryGetEnum<T>(this Blackboard blackboard, string key, out T value) where T : Enum => blackboard.TryGetValue(key, out value);
    }
}
