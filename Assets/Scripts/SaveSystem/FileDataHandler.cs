using System;
using System.IO;
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

		public GameData Load()
		{
			string dataFilePath = Path.Combine(_dataDirectoryPath, _dataFileName);
			GameData loadedData = null;

			if (File.Exists(dataFilePath))
			{
				try
				{
					string dataAsJson = "";
					using (FileStream fs = new FileStream(dataFilePath, FileMode.Open))
					{
						using (StreamReader reader = new StreamReader(fs))
						{
							dataAsJson = reader.ReadToEnd();
						}
					}
					
					loadedData = JsonUtility.FromJson<GameData>(dataAsJson);
				}
				catch (Exception e)
				{
					Console.WriteLine(e);
					throw;
				}
			}

			return loadedData;
		}
		
		public void Save(GameData data)
		{
			string dataFilePath = Path.Combine(_dataDirectoryPath, _dataFileName);
			try
			{
				//create the directory if it doesn't exist
				Directory.CreateDirectory(_dataDirectoryPath);
				
				//serialize the data into a json string
				string dataAsJson = JsonUtility.ToJson(data, true);
				
				//write the json string to the file
				using (FileStream fs = new FileStream(dataFilePath, FileMode.Create))
				{
					using (StreamWriter writer = new StreamWriter(fs))
					{
						writer.Write(dataAsJson);
					}
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