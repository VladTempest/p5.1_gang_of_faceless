using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Editor.Scripts.GlobalUtils;
using Editor.Scripts.SceneLoopScripts;
using SaveSystem.FileDataHandlers;
using Sirenix.OdinInspector;
using UnityEngine;

namespace SaveSystem
{
	public class DataPersistenceManager : MonoBehaviour
	{
		[Header("File Storage Config")]
		[SerializeField]
		[Tooltip("The name of file where the data will be stored")]
		private string fileName = "gameData.save";
        
		public static DataPersistenceManager Instance { get; set; }

		[SerializeField] private FileHandlerType _fileHandlerType = FileHandlerType.Binary;
		
		[SerializeField]
		private bool _isAutosaveActive = false;
		private IFileDataHandler _fileDataHandler; 
		
		private List<ISaveable> _dataPersistenceList;
		private Dictionary<Type, object> _savedData;
		
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
			ScenesController.Instance.OnLoadingSceneStarted += SceneController_OnLoadingSceneStarted;
			
			_fileDataHandler = GetFileDataHandler(_fileHandlerType);
			ConvenientLogger.Log(nameof(DataPersistenceManager), GlobalLogConstant.IsSaveLoadLogEnabled, $"Start DataPersistenceManager with path {Path.Combine(Application.persistentDataPath, fileName)}");
			_savedData = new Dictionary<Type, object>();
			LoadGame();
		}

		private void OnDestroy()
		{
			ScenesController.Instance.OnLoadingSceneStarted -= SceneController_OnLoadingSceneStarted;
		}

		private void SceneController_OnLoadingSceneStarted()
		{
			AutoSave();
		}

		private IFileDataHandler GetFileDataHandler(FileHandlerType fileHandlerType)
		{
			switch (fileHandlerType)
			{
				case FileHandlerType.Binary:
					return new BinaryFileDataHandler(Application.persistentDataPath, fileName);
				case FileHandlerType.JSON:
					return new JSONFileDataHandler(Application.persistentDataPath, fileName);
				default:
					throw new ArgumentOutOfRangeException(nameof(fileHandlerType), fileHandlerType, null);
			}
		}


		public void NewGame()
		{
			_savedData = new Dictionary<Type, object>();
		}

		public void RegisterDataPersistence(ISaveable saveable)
		{
			if (_dataPersistenceList == null)
			{
				_dataPersistenceList = new List<ISaveable>();
			}
			_dataPersistenceList.Add(saveable);
		}

		[Button("Save Game")]
		private void SaveGame()
		{
			foreach (var dataPersistence in _dataPersistenceList)
			{
				var saveData = dataPersistence.CaptureData();

				_savedData[saveData.Item1] = saveData.Item2;
			}
			
			ConvenientLogger.Log(nameof(DataPersistenceManager), GlobalLogConstant.IsSaveLoadLogEnabled, $"Save game data {_savedData.Count} to path {Application.persistentDataPath}");
			
			_fileDataHandler.Save(_savedData);
		}

		[Button("Load Game")]
		private void LoadGame()
		{
			_savedData = _fileDataHandler.Load();

			if (_savedData == null)
			{
				ConvenientLogger.Log(nameof(DataPersistenceManager), GlobalLogConstant.IsSaveLoadLogEnabled, $"Load game data at path {Application.persistentDataPath}");
				NewGame();
			}

			if (_dataPersistenceList == null)
			{
				_dataPersistenceList = new List<ISaveable>();
			}
			foreach (var saveable in _dataPersistenceList)
			{
				saveable.RestoreData();
			}
			
			ConvenientLogger.Log(nameof(DataPersistenceManager), GlobalLogConstant.IsSaveLoadLogEnabled, $"Load game data {_savedData.Count}");
		}

		[Button("Delete Save File")]
		private void DeleteSaveFile()
		{
			if (_fileDataHandler == null)
			{
				_fileDataHandler = GetFileDataHandler(_fileHandlerType);
			}
			_fileDataHandler.DeleteSaveFile();
		}

		private void OnApplicationQuit()
		{
			AutoSave();
		}

		public void AutoSave()
		{
			if (_isAutosaveActive)
			{
				ConvenientLogger.Log(nameof(DataPersistenceManager), GlobalLogConstant.IsSaveLoadLogEnabled,
					$"Autosaved on Application quit");
				SaveGame();
			}
		}


		public object GetState(Type type)
		{
			ConvenientLogger.Log(nameof(DataPersistenceManager), GlobalLogConstant.IsSaveLoadLogEnabled, $"GetState {type} from path {Application.persistentDataPath}");
			return _savedData.TryGetValue(type, out var state) ? state : null;
		}
	}
}