using System;
using UnityEngine;

namespace SoundSystemScripts
{
    public class OSTStarter : MonoBehaviour
    {
        [SerializeField]
        private TypeOfOSTByItsNature _typeOfOstByItsNature;

        private void Start()
        {
            SoundtrackPlayerWrapper.PlayOST(_typeOfOstByItsNature);
        }
    }
}