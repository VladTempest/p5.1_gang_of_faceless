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
                ConvenientLogger.LogError(nameof(JSONFileDataHandler), GlobalLogConstant.IsSaveLoadLogEnabled, $"File {_dataFilePath} does not exist");
                return new Dictionary<Type, object>();
            }

            try
            {
                var jsonString = File.ReadAllText(_dataFilePath);
                JsonReader reader = new JsonTextReader(new StringReader(jsonString));
                var serializer = new JsonSerializer();
                var result = serializer.Deserialize<Dictionary<Type, JToken>>(reader)
                    .ToDictionary(k => k.Key, v => (object)v.Value.ToObject<object>());

                return result;
            }
            catch (Exception e)
            {
                ConvenientLogger.LogError(nameof(JSONFileDataHandler), GlobalLogConstant.IsSaveLoadLogEnabled, $"Error loading data from file {_dataFilePath} {e}");
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
                Dictionary<Type, object> stateDictionary = (Dictionary<Type, object>) state;
                Dictionary<Type, object> serializableDictionary = new Dictionary<Type, object>(stateDictionary);
                var jsonString = JsonConvert.SerializeObject(serializableDictionary, Formatting.Indented);
                File.WriteAllText(_dataFilePath, jsonString);
            }
            catch (Exception e)
            {
                ConvenientLogger.LogError(nameof(JSONFileDataHandler), GlobalLogConstant.IsSaveLoadLogEnabled, $"Error saving data to file {_dataFilePath} {e}");
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