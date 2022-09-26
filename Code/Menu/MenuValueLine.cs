/* Copyright (c) 2021 dr. ext (Vladimir Sigalkin) */

using System;
using System.Collections.Generic;
using XiKeyboard.KeyMaps;

namespace XiKeyboard.Menu
{

	public abstract class TMenuValueLine<T> : MenuLine
	{
		#region Protected Methods

		protected readonly Func<T> _getter;

		protected readonly Action<T> _setter;

		#endregion

		#region Private Vars

		protected string text;
		protected string help;
		protected string shortcut;
		protected bool _isDefault;
		protected T _defaultValue;

		#endregion

		#region Public Methods

		protected TMenuValueLine(string text, Func<T> getter, Action<T> setter = null, string shortcut = null, string help = null) 
		{
			this.text = text;
			this.help = help;
			this.shortcut = shortcut;
			_getter = getter;
			_setter = setter;
			_defaultValue = getter.Invoke();
			_isDefault = true;
		}

		#endregion

		#region Protected Methods


		public override string Text => text;
		public override string Help => help;
		public override string Shorcut => null;
		public override string Value => ValueToString(_getter());
		public override object Binding => _getter;
		public override bool IsDefault => _isDefault;
		public override bool IsVisible => true;
		public override bool IsEnabled => true;

		public override void OnEvent(MenuEvent menuEvent)
		{
			var evt = menuEvent.eventType;
			var isShift = menuEvent.keyEvent.IsModifier(KeyModifiers.Shift);
			if (evt == MenuEventType.Decrement && _setter != null)
			{
				var value = ValueDecrement(_getter.Invoke(), isShift, menuEvent.vectorIndex);

				ChangeValue(value, false);
			}
			else if (evt == MenuEventType.Increment && _setter != null)
			{
				var value = ValueIncrement(_getter.Invoke(), isShift, menuEvent.vectorIndex);

				ChangeValue(value, false);
			}
			else if (evt == MenuEventType.Reset && _setter != null)
			{
				ChangeValue(_defaultValue, true);
			}
		}
		protected abstract string ValueToString(T value);
		protected abstract T ValueIncrement(T value, bool isShift, int idx = 0);

		protected abstract T ValueDecrement(T value, bool isShift, int idx = 0);

		#endregion

		#region Private Methods

		protected virtual void ChangeValue(T value, bool isReset)
		{
			// Change value.
			_setter.Invoke(value);
			_isDefault = Comparer<T>.Default.Compare(value, _defaultValue) == 0;
		}

		#endregion
	}
}
