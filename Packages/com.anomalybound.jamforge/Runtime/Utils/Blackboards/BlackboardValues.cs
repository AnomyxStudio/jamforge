using System;
using UnityEngine;

namespace JamForge.Blackboards
{
    [Serializable]
    public class BlackboardInt : BlackboardValue<int>
    {
        public BlackboardInt() { }
        public BlackboardInt(int value) : base(value) { }
    }

    [Serializable]
    public class BlackboardFloat : BlackboardValue<float>
    {
        public BlackboardFloat() { }
        public BlackboardFloat(float value) : base(value) { }
    }

    [Serializable]
    public class BlackboardBool : BlackboardValue<bool>
    {
        public BlackboardBool() { }
        public BlackboardBool(bool value) : base(value) { }
    }

    [Serializable]
    public class BlackboardString : BlackboardValue<string>
    {
        public BlackboardString() { }
        public BlackboardString(string value) : base(value) { }

        public override void OnAfterDeserialize()
        {
            base.OnAfterDeserialize();
            Value ??= string.Empty;
        }
    }

    [Serializable]
    public class BlackboardEnum<T> : BlackboardValue<T> where T : Enum
    {
        public BlackboardEnum() { }
        public BlackboardEnum(T value) : base(value) { }
    }

    [Serializable]
    public class BlackboardVector2 : BlackboardValue<Vector2>
    {
        public BlackboardVector2() { }
        public BlackboardVector2(Vector2 value) : base(value) { }
    }

    [Serializable]
    public class BlackboardVector3 : BlackboardValue<Vector3>
    {
        public BlackboardVector3() { }
        public BlackboardVector3(Vector3 value) : base(value) { }
    }

    [Serializable]
    public class BlackboardQuaternion : BlackboardValue<Quaternion>
    {
        public BlackboardQuaternion() { }
        public BlackboardQuaternion(Quaternion value) : base(value) { }

        public override void OnAfterDeserialize()
        {
            base.OnAfterDeserialize();
            // Ensure quaternion is normalized after deserialization
            if (Value != Quaternion.identity) { Value = Quaternion.Normalize(Value); }
        }
    }
}
