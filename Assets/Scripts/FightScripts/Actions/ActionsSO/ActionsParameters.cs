using System;
using Editor.Scripts.HubLocation;

namespace Editor.Scripts.Utils
{
    [Serializable]
    public struct ActionsParameters
    {
        public string Name;
        public float Damage;
        public int ActionPoints;
        public float CoolDown;
        public bool IsChargeable => Charges > 0;
        public int Charges;
        public ResourceTypes ConnectedResourceType;
        public int MinRange;
        public int MaxRange;
        public string Description;
        public bool IsTargeted;
    }
}