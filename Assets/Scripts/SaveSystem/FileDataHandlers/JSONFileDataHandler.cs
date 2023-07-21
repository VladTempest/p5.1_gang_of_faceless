using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Editor.Scripts.GlobalUtils;
using Editor.Scripts.HubLocation;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using SaveSystem.FileDataHandlers;
using UnityEngine;

namespace SaveSystem
{
    public class JSONFileDataHandler : IFileDataHandler
    {
        //Didn't find a way to serialize Dictionary<Type, object> with TypeNameHandling.All
        private string _dataDirectoryPath;
        private string _dataFileName;
        
        private string _dataFilePath => Path.Combine(_dataDirectoryPath, _dataFileName + ".json");

        public JSONFileDataHandler(string dataDirectoryPath, string dataFileName)
        {
            _dataDirectoryPath = dataDirectoryPath;
            _dataFileName = dataFileName;
        }

        public Dictionary<Type, object> Load()
        {
            if (!File.Exists(_dataFilePath))
            {
                Debug.LogError($"File {_dataFilePath} does not exist");
                return new Dictionary<Type, object>();
            }

            JsonSerializer serializer = new JsonSerializer
            {
                TypeNameHandling = TypeNameHandling.All // Add this setting
            };

            try
            {
                using (StreamReader sr = new StreamReader(_dataFilePath))
                using (JsonTextReader reader = new JsonTextReader(sr))
                {
                    return serializer.Deserialize<Dictionary<Type, object>>(reader);
                }
            }
            catch (Exception e)
            {
                Debug.LogError($"Error loading data from file {_dataFilePath} {e}");
                throw;
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
                JsonSerializer serializer = new JsonSerializer
                {
                    Formatting = Formatting.Indented,
                    TypeNameHandling = TypeNameHandling.All // Add this setting
                };

                using (StreamWriter sw = new StreamWriter(_dataFilePath))
                using (JsonTextWriter writer = new JsonTextWriter(sw))
                {
                    serializer.Serialize(writer, state);
                }
            }
            catch (Exception e)
            {
                Debug.LogError($"Error saving data to file {_dataFilePath} {e}");
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