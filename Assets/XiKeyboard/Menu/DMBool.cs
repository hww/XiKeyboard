/* Copyright (c) 2021 dr. ext (Vladimir Sigalkin) */

using System;

namespace XiKeyboard
{
	public class DMBool : TDMMenuValue<bool>
	{
		#region Public Methods

		public DMBool(string text, Func<bool> getter, Action<bool> setter = null, string shortcut = null, string help = null) 
			: base(text, getter, setter, shortcut, help)
		{ }

		public override bool ButtonState => _getter();
		public override DMButtonType ButtonType => DMButtonType.Toggle;

		#endregion

		#region Protected Methods

		protected override string ValueToString(bool value) => String.Empty;

		protected override bool ValueIncrement(bool value, bool isShift) => !value;

		protected override bool ValueDecrement(bool value, bool isShift) => !value;

		#endregion
	}
}