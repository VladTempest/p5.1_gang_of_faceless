using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Editor.Scripts.GlobalUtils;
using UnityEngine;

namespace SaveSystem
{
	public class DataPersistenceManager : MonoBehaviour
	{
		[Header("File Storage Config")]
		[SerializeField]
		[Tooltip("The name of file where the data will be stored")]
		private string fileName = "gameData.game";
		
		private GameData _gameData;
		public static DataPersistenceManager Instance { get; set; }
		
		private FileDataHandler _fileDataHandler; 
		
		private List<IDataPersistence> _dataPersistenceList;
		
		private void Awake()
		{
			if (Instance != null)
			{
				ConvenientLogger.Log(name, GlobalLogConstant.IsSingltonsLogEnabled, $"There are many singletonss");
				Destroy(gameObject);
				return;
			}
			Instance = this;
			DontDestroyOnLoad(Instance);
		}

		private void Start()
		{
			_fileDataHandler = new FileDataHandler(Application.persistentDataPath, fileName);
			LoadGame();
		}
        

		public void NewGame()
		{
			this._gameData = new GameData();
		}
		
		public void RegisterDataPersistence(IDataPersistence dataPersistence)
		{
			if (_dataPersistenceList == null)
			{
				_dataPersistenceList = new List<IDataPersistence>();
			}
			_dataPersistenceList.Add(dataPersistence);
		}

		void SaveGame()
		{
			//ToDo - pass the data to other scripts so they can update it
			foreach (var dataPersistence in _dataPersistenceList)
			{
				dataPersistence.SaveData(ref _gameData);
			}
			
			ConvenientLogger.Log(nameof(DataPersistenceManager), GlobalLogConstant.IsSaveLoadLogEnabled, $"Save game data {_gameData.goldCount}");
			
			_fileDataHandler.Save(_gameData);
		}

		void LoadGame()
		{
			_gameData = _fileDataHandler.Load();
			// Load any saved data from a file using tha data handler
			// if the file doesn't exist, create a new game
			if (_gameData == null)
			{
				ConvenientLogger.Log(nameof(DataPersistenceManager), GlobalLogConstant.IsSaveLoadLogEnabled, $"Load game data");
				NewGame();
			}

			foreach (var dataPersistence in _dataPersistenceList)
			{
				dataPersistence.LoadData(_gameData);
			}
			//ToDo - push the loaded data to all other scripts that need it
			
			ConvenientLogger.Log(nameof(DataPersistenceManager), GlobalLogConstant.IsSaveLoadLogEnabled, $"Load game data {_gameData.goldCount}");
		}

		private void OnApplicationQuit()
		{
			SaveGame();
		}
	}
}