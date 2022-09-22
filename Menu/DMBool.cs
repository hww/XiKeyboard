/* Copyright (c) 2021 dr. ext (Vladimir Sigalkin) */

using System;

namespace XiKeyboard
{
	public class DMBool : DMValue<bool>
	{
		#region Public Methods

		public DMBool(string text, Func<bool> getter, Action<bool> setter = null, string shortcut = null, string help = null) 
			: base(text, getter, setter, shortcut, help)
		{ }

		#endregion

		#region Protected Methods

		protected override string ValueToString(bool value) => value ? "True" : "False";

		protected override bool ValueIncrement(bool value, bool isShift) => !value;

		protected override bool ValueDecrement(bool value, bool isShift) => !value;

		#endregion
	}
}