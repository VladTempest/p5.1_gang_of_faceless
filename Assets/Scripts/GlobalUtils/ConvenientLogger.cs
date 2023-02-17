using UnityEngine;

namespace Editor.Scripts.GlobalUtils
{
	public static class ConvenientLogger
	{
		public static void LogError(string tag, bool isLocalLogEnable, string message)
		{
			if (GlobalLogConstant.IsAllLogEnabled && isLocalLogEnable)
			{
				Debug.LogError($"<color=red>[{tag.ToUpper()}] : {message}</color>");
			}
		}
		
		public static void Log(string tag, bool isLocalLogEnable, string message)
		{
			if (GlobalLogConstant.IsAllLogEnabled && isLocalLogEnable)
			{
				Debug.Log($"<color=green>[{tag.ToUpper()}] : {message}</color>");
			}
		}
	}
}