/* Copyright (c) 2021 dr. ext (Vladimir Sigalkin) */

using System;
using System.Collections.Generic;

namespace XiKeyboard.Menu
{
	public abstract class MenuValueLine<T> : MenuLine
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
		private T _defaultValue;

		#endregion

		#region Public Methods

		protected MenuValueLine(string text, Func<T> getter, Action<T> setter = null, string shortcut = null, string help = null) 
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
		public override string Shorcut => ValueToString(_getter());
		public override object Binding => _getter;
		public override bool IsDefault => _isDefault;
		public override bool IsVisible => true;
		public override bool IsEnabled => true;

		public override void OnEvent(MenuEvent evt, bool isShift)
		{
			if (evt == MenuEvent.Left && _setter != null)
			{
				var value = ValueDecrement(_getter.Invoke(), isShift);

				ChangeValue(value, false);
			}
			else if (evt == MenuEvent.Right && _setter != null)
			{
				var value = ValueIncrement(_getter.Invoke(), isShift);

				ChangeValue(value, false);
			}
			else if (evt == MenuEvent.Reset && _setter != null)
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
			_isDefault = Comparer<T>.Default.Compare(value, _defaultValue) == 0;
		}

		#endregion
	}
}
