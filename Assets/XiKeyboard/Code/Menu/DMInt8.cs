/* Copyright (c) 2021 dr. ext (Vladimir Sigalkin) */

using System;

namespace XiKeyboard.Menu
{
	public class DMInt8 : MenuValueLine<sbyte>
	{
		#region Public Vars

		public sbyte Step = 1;

		public sbyte ShiftStep = 10;

		public string Format = "0";

		#endregion

		#region Public Methods

		public DMInt8(string text, Func<sbyte> getter, Action<sbyte> setter = null, string shortcut = null, string help = null)
			: base(text, getter, setter, shortcut, help) 
		{ }

		#endregion

		#region Protected Methods

		protected override string ValueToString(sbyte value) => value.ToString(Format);

		protected override sbyte ValueIncrement(sbyte value, bool isShift) =>
			(sbyte)(value + (isShift ? ShiftStep : Step));

		protected override sbyte ValueDecrement(sbyte value, bool isShift) =>
			(sbyte)(value - (isShift ? ShiftStep : Step));

		#endregion
	}
}