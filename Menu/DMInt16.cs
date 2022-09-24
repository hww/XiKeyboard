/* Copyright (c) 2021 dr. ext (Vladimir Sigalkin) */

using System;

namespace XiKeyboard
{
	public class DMInt16 : TDMMenuValue<Int16>
	{
		#region Public Vars

		public Int16 Step = 1;

		public Int16 ShiftStep = 10;

		public string Format = "0";

		#endregion

		#region Public Methods

		public DMInt16(string text, Func<Int16> getter, Action<Int16> setter = null, string shortcut = null, string help = null)
			: base(text, getter, setter, shortcut, help) 
		{ }

		#endregion

		#region Protected Methods

		protected override string ValueToString(Int16 value) => value.ToString(Format);

		protected override Int16 ValueIncrement(Int16 value, bool isShift) => (Int16)(value + (isShift ? ShiftStep : Step));

		protected override Int16 ValueDecrement(Int16 value, bool isShift) => (Int16)(value - (isShift ? ShiftStep : Step));

		#endregion
	}
}