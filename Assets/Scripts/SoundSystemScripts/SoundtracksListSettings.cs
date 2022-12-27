using System.Collections.Generic;
using UnityEngine;

namespace SoundSystemScripts
{

    [CreateAssetMenu(fileName = "SoundtracksListSettings", menuName = "Settings/SoundtracksListSettings", order = 0)]
    public class SoundtracksListSettings : ScriptableObject
    {
        public List<Soundtrack> list;

    }
}
