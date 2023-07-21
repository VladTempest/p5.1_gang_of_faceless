using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using Editor.Scripts.GlobalUtils;
using SaveSystem.FileDataHandlers;
using UnityEngine;

namespace SaveSystem
{
	public class BinaryFileDataHandler : IFileDataHandler
	{
		private string _dataDirectoryPath;
		private string _dataFileName;
		private string _dataFilePath => Path.Combine(_dataDirectoryPath, _dataFileName + ".bn");

		public BinaryFileDataHandler(string dataDirectoryPath, string dataFileName)
		{
			_dataDirectoryPath = dataDirectoryPath;
			_dataFileName = dataFileName;
		}

		public Dictionary<Type, object> Load()
		{
            
			if (!File.Exists(_dataFilePath))
            {
                ConvenientLogger.LogError(nameof(BinaryFileDataHandler), GlobalLogConstant.IsSaveLoadLogEnabled, $"File {_dataFilePath} does not exist");
                return new Dictionary<Type, object>();
            }
			
			using (FileStream stream = File.Open(_dataFilePath, FileMode.Open))
			{
				try
				{
					var formatter = new BinaryFormatter();
					return (Dictionary<Type, object>)formatter.Deserialize(stream);
				}
				catch (Exception e)
				{
					ConvenientLogger.LogError(nameof(BinaryFileDataHandler), GlobalLogConstant.IsSaveLoadLogEnabled, $"Error loading data from file {_dataFilePath} {e}");
					throw;
				}
			}

		}
		
		public void Save(object state)
		{
			if (!File.Exists(_dataFilePath))
			{
				File.Create(_dataFilePath).Dispose();
			}
			try
			{
				using (var stream = File.Open(_dataFilePath,FileMode.Open))
				{
					var formatter = new BinaryFormatter();
					formatter.Serialize(stream, state);
				}
				
			}
			catch (Exception e)
			{
				ConvenientLogger.LogError(nameof(BinaryFileDataHandler), GlobalLogConstant.IsSaveLoadLogEnabled, $"Error saving data to file {_dataFilePath} {e}");
				throw;
			}
		}

		public void DeleteSaveFile()
		{
			if (File.Exists(_dataFilePath))
			{
				File.Delete(_dataFilePath);
				ConvenientLogger.Log(nameof(DataPersistenceManager), GlobalLogConstant.IsSaveLoadLogEnabled, $"Delete save file at path {_dataFilePath}");
			}
			else
			{
				ConvenientLogger.Log(nameof(DataPersistenceManager), GlobalLogConstant.IsSaveLoadLogEnabled, $"Save file not found at path {_dataFilePath}");
			}
		}
	}
}