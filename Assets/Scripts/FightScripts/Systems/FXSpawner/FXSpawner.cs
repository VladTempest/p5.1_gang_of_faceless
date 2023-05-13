using System;
using Editor.Scripts.Utils;
using UnityEngine;

namespace Editor.Scripts
{
    public class FXSpawner : MonoBehaviour
    {
        [SerializeField] private SerializableDictionary<FXenum,Transform> _fxDictionary;
        
        public static FXSpawner Instance { get; private set; }

        private void Awake()
        {
            if (Instance != null)
            {
                Debug.LogError("There are many singletonss");
                Destroy(gameObject);
                return;
            }

            Instance = this;
        }

        private void Start()
        {
            HealthSystem.OnAnyUnitDamaged += HealthSystem_OnAnyUnitDamaged;   
        }

        private void HealthSystem_OnAnyUnitDamaged(object sender, EventArgs e)
        {
            var transformOfSender = ((HealthSystem) sender).transform;
            Instantiate(_fxDictionary[FXenum.BloodSplash], transformOfSender.position + new Vector3(0, GameGlobalConstants.UNIT_SHOULDER_HEIGHT-0.25f, 0), Quaternion.identity);
        }
        
        public void InstantiateFog(Vector3 position)
        {
            Instantiate(_fxDictionary[FXenum.BackStabFog], position, Quaternion.identity);
        }
        
    }
}