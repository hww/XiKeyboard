/* Copyright (c) 2021 dr. ext (Vladimir Sigalkin) */

using System;

namespace XiKeyboard.Menu
{
	public class DMInt64 : MenuValueLine<Int64>
	{
		#region Public Vars

		public Int64 Step = 1;

		public Int64 ShiftStep = 10;

		public string Format = "0";

		#endregion

		#region Public Methods

		public DMInt64(string text, Func<Int64> getter, Action<Int64> setter = null, string shortcut = null, string help = null)
			: base(text, getter, setter, shortcut, help)
		{ }

		#endregion

		#region Protected Methods

		protected override string ValueToString(Int64 value) => value.ToString(Format);

		protected override Int64 ValueIncrement(Int64 value, bool isShift) => value + (isShift ? ShiftStep : Step);

		protected override Int64 ValueDecrement(Int64 value, bool isShift) => value - (isShift ? ShiftStep : Step);

		#endregion
	}
}