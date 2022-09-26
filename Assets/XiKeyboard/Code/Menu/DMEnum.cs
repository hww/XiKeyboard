/* Copyright (c) 2021 dr. ext (Vladimir Sigalkin) */

using System;

namespace XiKeyboard.Menu
{
	public class DMEnum<T> : TMenuValueLine<T> where T : struct, Enum
	{
		#region Static Private Methods

		private static T NextEnum(T value)
		{
			var length = Enum.GetValues(typeof(T)).Length;
			var index = Convert.ToInt64(value) + 1;

			if (index >= length)
				index = 0;

			return (T)Enum.ToObject(typeof(T), index);
		}

		private static T PrevEnum(T value)
		{
			var length = Enum.GetValues(typeof(T)).Length;
			var index = Convert.ToInt64(value) - 1;

			if (index < 0)
				index = length - 1;

			return (T)Enum.ToObject(typeof(T), index);
		}

		#endregion


		#region Public Methods

		public DMEnum(string text, Func<T> getter, Action<T> setter = null, string shortcut = null, string help = null)
			: base(text, getter, setter, shortcut, help)
		{

			var type = typeof(T);
			if (type.IsDefined(typeof(FlagsAttribute), false))
				UnityEngine.Debug.LogError("Do not use DMEnum for flags enum");
		}

		#endregion

		#region Protected Methods

		public override void OnEvent(XiKeyboard.MenuEvent controller)
		{
			base.OnEvent(controller);
		}

		protected override string ValueToString(T value)
		{
			return value.ToString();
		}

		protected override T ValueIncrement(T value, bool isShift, int idx = 0) => NextEnum(value);

		protected override T ValueDecrement(T value, bool isShift, int idx = 0) => PrevEnum(value);

		#endregion

	}
}