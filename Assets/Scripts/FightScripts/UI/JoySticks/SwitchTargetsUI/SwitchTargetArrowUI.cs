using System;
using UnityEngine;

namespace FightScripts.UI.JoySticks.SwitchTargetsUI
{
	public class SwitchTargetArrowUI : MonoBehaviour
	{
		private void Start()
		{
			UnitActionSystem.Instance.OnSelectedActionChanged += UnitAction_OnSelectedActionChanged;
			gameObject.SetActive(UnitActionSystem.Instance.GetSelectedAction().IsTargeted);
		}
		
		private void OnDestroy()
		{
			UnitActionSystem.Instance.OnSelectedActionChanged -= UnitAction_OnSelectedActionChanged;
		}

		private void UnitAction_OnSelectedActionChanged(object sender, EventArgs e)
		{
			gameObject.SetActive(UnitActionSystem.Instance.GetSelectedAction().IsTargeted);
		}
	}
}