using System;
using System.Collections.Generic;
using UnityEngine.Serialization;

namespace Editor.Scripts.HubLocation
{
	[System.Serializable]
	public class BinarySerializableDictionary<TKey, TValue>
	{
		[FormerlySerializedAs("keys")] public List<TKey> Keys=new List<TKey>();
		[FormerlySerializedAs("values")] public List<TValue> Values=new List<TValue>();
    
		public Dictionary<TKey, TValue> ToDictionary()
		{
			Dictionary<TKey, TValue> dictionary = new Dictionary<TKey, TValue>();
			for (var i = 0; i!= Math.Min(Keys.Count, Values.Count); i++)
				dictionary.Add(Keys[i], Values[i]);
            
			return dictionary;
		}
		
		public BinarySerializableDictionary(Dictionary<TKey, TValue> dictionary)
		{
			if (dictionary == null)
			{
				throw new ArgumentNullException(nameof(dictionary));
			}

			foreach (var pair in dictionary)
			{
				Keys.Add(pair.Key);
				Values.Add(pair.Value);
			}
		}
	}
}