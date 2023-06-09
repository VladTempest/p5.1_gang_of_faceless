using System.ComponentModel;
using UnityEngine;

[CreateAssetMenu(fileName = "Data", menuName = "ScriptableObjects/ResourceScriptableObject", order = 1)]
public class ResourceScriptableObject : ScriptableObject
{
	public int Value;
}