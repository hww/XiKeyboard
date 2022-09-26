/* Copyright (c) 2021 dr. ext (Vladimir Sigalkin) */

using System;

namespace XiKeyboard.Menu
{
	public class DMInt32 : TMenuValueLine<Int32>
	{
		#region Public Vars

		public Int32 Step = 1;

		public Int32 ShiftStep = 10;

		public string Format = "0";

		#endregion

		#region Public Methods

		public DMInt32(string text, Func<Int32> getter, Action<Int32> setter = null, string shortcut = null, string help = null)
			: base(text, getter, setter, shortcut, help)
		{
		}

		#endregion

		#region Protected Methods

		protected override string ValueToString(Int32 value) => value.ToString(Format);

		protected override Int32 ValueIncrement(Int32 value, bool isShift, int idx = 0) => value + (isShift ? ShiftStep : Step);

		protected override Int32 ValueDecrement(Int32 value, bool isShift, int idx = 0) => value - (isShift ? ShiftStep : Step);

		#endregion
	}
}