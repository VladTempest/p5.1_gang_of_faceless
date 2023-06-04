using UnityEngine;

namespace Editor.Scripts.GlobalUtils
{
	public static class ConvenientLogger
	{
		public static void LogError(string tag, bool isLocalLogEnable, string message, Object context = null)
		{
			if (GlobalLogConstant.IsAllLogEnabled && isLocalLogEnable)
			{
				Debug.LogError($"<color=red>[{tag.ToUpper()}] : {message}</color>", context);
			}
		}
		
		public static void Log(string tag, bool isLocalLogEnable, string message, Object context = null)
		{
			if (GlobalLogConstant.IsAllLogEnabled && isLocalLogEnable)
			{
				Debug.Log($"<color=green>[{tag.ToUpper()}] : {message}</color>", context);
			}
		}

		public static void LogWarning(string tag, bool isLocalLogEnable, string message, Object context = null)
		{
			if (GlobalLogConstant.IsAllLogEnabled && isLocalLogEnable)
			{
				Debug.LogWarning($"<color=yellow>[{tag.ToUpper()}] : {message}</color>", context);
			}
		}
	}
}