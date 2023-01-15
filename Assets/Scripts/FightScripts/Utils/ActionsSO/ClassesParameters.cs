using System;
using UnityEngine.UI;
using Sprite = UnityEngine.Sprite;

namespace Editor.Scripts.Utils
{
    [Serializable]
    public struct ClassesParameters
    {
        public Sprite ClassLogo;
        
        public int HP;
        public int ActionPoints;
    }
}