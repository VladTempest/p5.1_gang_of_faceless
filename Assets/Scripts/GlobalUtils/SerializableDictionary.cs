using System;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class SerializableDictionary<TKey, TValue> : Dictionary<TKey, TValue>, ISerializationCallbackReceiver
{
	[SerializeField] private List<TKey> keys = new List<TKey>();
	[SerializeField] private List<TValue> values = new List<TValue>();
    
	// OnBeforeSerialize is called immediately before Unity serializes your object
	public void OnBeforeSerialize()
	{
		keys.Clear();
		values.Clear();

		foreach (KeyValuePair<TKey, TValue> pair in this)
		{
			keys.Add(pair.Key);
			values.Add(pair.Value);
		}
	}

	// OnAfterDeserialize is called immediately after Unity deserializes your object
	public void OnAfterDeserialize()
	{
		this.Clear();

		if (keys.Count != values.Count)
			throw new System.Exception("there are " + keys.Count + " keys and " + values.Count + " values after deserialization. Make sure that both key and value types are serializable.");

		for (int i = 0; i < keys.Count; i++)
			this.Add(keys[i], values[i]);
	}
}