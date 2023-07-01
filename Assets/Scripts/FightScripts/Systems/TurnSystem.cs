using System;
using System.Collections;
using System.Collections.Generic;
using Editor.Scripts.GlobalUtils;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class TurnSystem : MonoBehaviour
{
   public static TurnSystem Instance { get; private set; }

   public event EventHandler OnTurnChanged;
   public int CurrentTurnNumber => _turnNumber;

   public bool IsPlayerTurn => _isPlayerTurn;
   private int _turnNumber = 1;
   private bool _isPlayerTurn = true;


   private void Awake()
   {
      if (Instance != null)
      {
         ConvenientLogger.Log(name, GlobalLogConstant.IsSingltonsLogEnabled, $"There are many singletonss");
         Destroy(gameObject);
         return;
      }
      Instance = this;
   }

   public void NextTurn()
   {
      _turnNumber++;
      _isPlayerTurn = !_isPlayerTurn;
      
      OnTurnChanged?.Invoke(this, EventArgs.Empty);
   }
}
