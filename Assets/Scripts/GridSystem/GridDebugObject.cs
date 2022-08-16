using System;
using TMPro;
using UnityEngine;

namespace GridSystems
{
    public class GridDebugObject : MonoBehaviour
    {
        private object _gridObject;
        [SerializeField] private TextMeshPro _textMeshPro;

        public virtual void SetGridObject(object gridObject)
        {   
            _gridObject = gridObject;
        }

        protected virtual void Update()
        {
            _textMeshPro.text = _gridObject.ToString();
        }
    }
}
