using System;

namespace Editor.Scripts.HubLocation.ReactiveData
{
	[Serializable]
	public class ReactiveInt
	{
		public delegate void OnValueChanged(int newValue);
		public event OnValueChanged onValueChanged;

		private int _value;

		public ReactiveInt(int resourceAmount)
		{
			_value = resourceAmount;
		}

		public int Value
		{
			get => _value;
			set
			{
				if (_value != value)
				{
					_value = value;
					onValueChanged?.Invoke(_value);
				}
			}
		}
	}
}