/* Copyright (c) 2021 dr. ext (Vladimir Sigalkin) */

using System;

namespace XiKeyboard.Menu
{
	public class DMBool : TMenuValueLine<bool>
	{
		#region Public Methods

		public DMBool(string text, Func<bool> getter, Action<bool> setter = null, string shortcut = null, string help = null) 
			: base(text, getter, setter, shortcut, help)
		{ }

		public override bool ButtonState => _getter();
		public override DMButtonType ButtonType => DMButtonType.Toggle;
		public override string Shorcut => shortcut;
		public override string Value => shortcut;

		#endregion

		#region Protected Methods

		protected override string ValueToString(bool value) => String.Empty;

		protected override bool ValueIncrement(bool value, bool isShift, int idx = 0) => !value;

		protected override bool ValueDecrement(bool value, bool isShift, int idx = 0) => !value;

		#endregion
	}
}