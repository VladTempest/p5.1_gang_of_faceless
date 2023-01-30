using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

namespace Editor.Scripts.FightScripts.UI.CharacterListUI
{
    public class UnitListUI : MonoBehaviour
    { 
        [SerializeField] private UnitInfoUI unitInfoPrefab;
        private UnitManager _unitManger;

        private void Start()
        {
            _unitManger = UnitManager.Instance;
            
            List<global::Unit> playerCharacters = _unitManger.FriendlyUnitList;
            ClearInstantiatedCharacters();
            SpawnCharacterInfoForPlayerCharacters(playerCharacters);
        }

        private void ClearInstantiatedCharacters()
        {
            foreach (Transform child in transform)
            {
                Destroy(child.gameObject);
            }
        }

        private void SpawnCharacterInfoForPlayerCharacters(List<global::Unit> playerCharacters)
        {
            foreach (global::Unit unit in playerCharacters)
            {
                UnitInfoUI unitInfo = Instantiate(unitInfoPrefab, transform);

                if (ConstantsProvider.Instance.classesParametersSO.ClassesParametersDictionary.TryGetValue(
                        unit.UnitType, out var classesParameters))
                {
                    unitInfo.SetUpUnitInfo(classesParameters, unit);
                }
                
            }
        }
    }
}