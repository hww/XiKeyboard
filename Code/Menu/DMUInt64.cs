/* Copyright (c) 2021 dr. ext (Vladimir Sigalkin) */

using System;

namespace XiKeyboard.Menu
{
	public class DMUInt64 : TMenuValueLine<UInt64>
	{
		#region Public Vars

		public UInt64 Step = 1;

		public UInt64 ShiftStep = 10;

		public string Format = "0";

		#endregion

		#region Public Methods

		public DMUInt64(string text, Func<UInt64> getter, Action<UInt64> setter = null, string shortcut = null, string help = null)
			: base(text, getter, setter, shortcut, help) 
		{ }

		#endregion

		#region Protected Methods

		protected override string ValueToString(UInt64 value) => value.ToString(Format);

		protected override UInt64 ValueIncrement(UInt64 value, bool isShift, int idx = 0) => value + (isShift ? ShiftStep : Step);

		protected override UInt64 ValueDecrement(UInt64 value, bool isShift, int idx = 0) => value - (isShift ? ShiftStep : Step);

		#endregion
	}
}