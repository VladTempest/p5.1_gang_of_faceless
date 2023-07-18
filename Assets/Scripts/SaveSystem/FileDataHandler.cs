using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using Editor.Scripts.GlobalUtils;
using UnityEngine;

namespace SaveSystem
{
	public class FileDataHandler
	{
		private string _dataDirectoryPath;
		private string _dataFileName;
		
		public FileDataHandler(string dataDirectoryPath, string dataFileName)
		{
			_dataDirectoryPath = dataDirectoryPath;
			_dataFileName = dataFileName;
		}

		public Dictionary<Type, object> Load()
		{
			string dataFilePath = Path.Combine(_dataDirectoryPath, _dataFileName);
			
			if (!File.Exists(dataFilePath))
            {
                ConvenientLogger.LogError(nameof(FileDataHandler), GlobalLogConstant.IsSaveLoadLogEnabled, $"File {dataFilePath} does not exist");
                return new Dictionary<Type, object>();
            }
			
			using (FileStream stream = File.Open(dataFilePath, FileMode.Open))
			{
				try
				{
					var formatter = new BinaryFormatter();
					return (Dictionary<Type, object>)formatter.Deserialize(stream);
				}
				catch (Exception e)
				{
					ConvenientLogger.LogError(nameof(FileDataHandler), GlobalLogConstant.IsSaveLoadLogEnabled, $"Error loading data from file {dataFilePath} {e}");
					throw;
				}
			}

		}
		
		public void Save(object state)
		{
			string dataFilePath = Path.Combine(_dataDirectoryPath, _dataFileName);
			if (!File.Exists(dataFilePath))
			{
				File.Create(dataFilePath).Dispose();
			}
			try
			{
				using (var stream = File.Open(Path.Combine(_dataDirectoryPath,_dataFileName),FileMode.Open))
				{
					var formatter = new BinaryFormatter();
					formatter.Serialize(stream, state);
				}
				
			}
			catch (Exception e)
			{
				ConvenientLogger.LogError(nameof(FileDataHandler), GlobalLogConstant.IsSaveLoadLogEnabled, $"Error saving data to file {dataFilePath} {e}");
				throw;
			}
		}
	}
}