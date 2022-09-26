/* Copyright (c) 2021 dr. ext (Vladimir Sigalkin) */

using System;

namespace XiKeyboard.Menu
{
	public class DMString : TMenuValueLine<string>
	{
		#region Public Methods

		public DMString(string text, Func<string> getter, string shortcut = null, string help = null) 
			: base(text, getter, null, shortcut, help)
		{ }

		#endregion

		#region Private Methods

		protected override string ValueToString(string value) => value;

		protected override string ValueIncrement(string value, bool isShift, int idx = 0) => value;

		protected override string ValueDecrement(string value, bool isShift, int idx = 0) => value;

		#endregion
	}
}