using System;

namespace XiKeyboard
{

	public abstract class DMValue<T> : DMMenuLine
	{
		#region Protected Methods

		protected readonly Func<T> _getter;

		protected readonly Action<T> _setter;

		#endregion

		#region Private Vars

		protected string text;
		protected string help;
		protected string shortcut;
		private T _defaultValue;

		#endregion

		#region Public Methods

		protected DMValue(string text, Func<T> getter, Action<T> setter = null, string shortcut = null, string help = null) 
		{
			this.text = text;
			this.help = help;
			this.shortcut = shortcut;
			_getter = getter;
			_setter = setter;
			_defaultValue = getter.Invoke();
		}

		#endregion

		#region Protected Methods

		protected override void OnEvent(EventTag evt, bool shift)
		{
			if (evt == EventTag.Left && _setter != null)
			{
				var value = ValueDecrement(_getter.Invoke(), shift);

				ChangeValue(value, false);
			}
			else if (evt == EventTag.Right && _setter != null)
			{
				var value = ValueIncrement(_getter.Invoke(), shift);

				ChangeValue(value, false);
			}
			else if (evt == EventTag.Reset && _setter != null)
			{
				ChangeValue(_defaultValue, true);
			}
		}

		protected abstract string ValueToString(T value);

		protected abstract T ValueIncrement(T value, bool isShift);

		protected abstract T ValueDecrement(T value, bool isShift);

		#endregion

		#region Private Methods

		private void ChangeValue(T value, bool isReset)
		{
			// Change value.
			_setter.Invoke(value);
		}

		#endregion
	}
}
