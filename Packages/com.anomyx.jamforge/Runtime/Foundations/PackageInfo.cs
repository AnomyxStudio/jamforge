using System;
using UnityEngine;

namespace JamForge
{
    [Serializable]
    public class PackageInfo
    {
        [SerializeField] private string version;

        public string Version => version;
    }
}
