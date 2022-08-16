using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class TurnSystem : MonoBehaviour
{
   public static TurnSystem Instance { get; private set; }

   public event EventHandler OnTurnChanged;
   private int _turnNumber = 1;
   private bool _isPlayerTurn = true;

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

   public void NextTurn()
   {
      _turnNumber++;
      _isPlayerTurn = !_isPlayerTurn;
      
      OnTurnChanged?.Invoke(this, EventArgs.Empty);
   }

   public int GetCurrentTurnNumber()
   {
      return _turnNumber;
   }

   public bool IsPlayerTurn()
   {
      return _isPlayerTurn;
   }
}
