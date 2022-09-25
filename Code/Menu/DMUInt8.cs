/* Copyright (c) 2021 dr. ext (Vladimir Sigalkin) */

using System;

namespace XiKeyboard.Menu
{
	public class DMUInt8 : MenuValueLine<byte>
	{
		#region Public Vars

		public byte Step = 1;

		public byte ShiftStep = 10;

		public string Format = "0";

		#endregion

		#region Public Methods

		public DMUInt8(string text, Func<byte> getter, Action<byte> setter = null, string shortcut = null, string help = null)
			: base(text, getter, setter, shortcut, help)
		{
		}

		#endregion

		#region Protected Methods

		protected override string ValueToString(byte value) => value.ToString(Format);

		protected override byte ValueIncrement(byte value, bool isShift) =>
			(byte)(value + (isShift ? ShiftStep : Step));

		protected override byte ValueDecrement(byte value, bool isShift) =>
			(byte)(value - (isShift ? ShiftStep : Step));

		#endregion
	}
}