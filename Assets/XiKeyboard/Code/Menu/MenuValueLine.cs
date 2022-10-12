/* Copyright (c) 2021 dr. ext (Vladimir Sigalkin)*/

using System;
using System.Collections.Generic;
using XiKeyboard.KeyMaps;

namespace XiKeyboard.Menu
{
    ///-------------------------------------------------------------------------------------------------
    /// <summary>   A menu value line. </summary>
    ///

    ///
    /// <typeparam name="T">    Generic type parameter. </typeparam>
    ///-------------------------------------------------------------------------------------------------

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

        ///-------------------------------------------------------------------------------------------------
        /// <summary>   Specialized constructor for use only by derived class. </summary>
        ///

        ///
        /// <param name="text">     The text. </param>
        /// <param name="getter">   The getter. </param>
        /// <param name="setter">   (Optional) The setter. </param>
        /// <param name="shortcut"> (Optional) The shortcut. </param>
        /// <param name="help">     (Optional) The help. </param>
        ///-------------------------------------------------------------------------------------------------

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

        ///-------------------------------------------------------------------------------------------------
        /// <summary>   Executes the 'event' action. </summary>
        ///

        ///
        /// <param name="menuEvent">    The menu event. </param>
        ///-------------------------------------------------------------------------------------------------

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

        ///-------------------------------------------------------------------------------------------------
        /// <summary>   Value to string. </summary>
        ///

        ///
        /// <param name="value">    The value. </param>
        ///
        /// <returns>   A string. </returns>
        ///-------------------------------------------------------------------------------------------------

		protected abstract string ValueToString(T value);

        ///-------------------------------------------------------------------------------------------------
        /// <summary>   Value increment. </summary>
        ///

        ///
        /// <param name="value">    The value. </param>
        /// <param name="isShift">  True if is shift, false if not. </param>
        /// <param name="idx">      (Optional) Zero-based index of the. </param>
        ///
        /// <returns>   A T. </returns>
        ///-------------------------------------------------------------------------------------------------

		protected abstract T ValueIncrement(T value, bool isShift, int idx = 0);

        ///-------------------------------------------------------------------------------------------------
        /// <summary>   Value decrement. </summary>
        ///

        ///
        /// <param name="value">    The value. </param>
        /// <param name="isShift">  True if is shift, false if not. </param>
        /// <param name="idx">      (Optional) Zero-based index of the. </param>
        ///
        /// <returns>   A T. </returns>
        ///-------------------------------------------------------------------------------------------------

		protected abstract T ValueDecrement(T value, bool isShift, int idx = 0);

		#endregion

		#region Private Methods

        ///-------------------------------------------------------------------------------------------------
        /// <summary>   Change value. </summary>
        ///

        ///
        /// <param name="value">    The value. </param>
        /// <param name="isReset">  True if is reset, false if not. </param>
        ///-------------------------------------------------------------------------------------------------

		protected virtual void ChangeValue(T value, bool isReset)
		{
			// Change value.
			_setter.Invoke(value);
			_isDefault = Comparer<T>.Default.Compare(value, _defaultValue) == 0;
		}

		#endregion
	}
}
