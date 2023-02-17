using System;

namespace Editor.Scripts.Utils
{
    [Serializable]
    public struct ActionsParameters
    {
        public float Damage;
        public int ActionPoints;
        public float CoolDown;
        public int Charges;
        public int MinRange;
        public int MaxRange;
        public string Description;
        public bool IsTargeted;
    }
}