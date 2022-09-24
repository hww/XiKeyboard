using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace XiKeyboard
{
	public class DMFloat : TDMMenuValue<float>
	{
		#region Public Vars

		public string Format;

		public int Step = 1;

		public int ShiftStep = 10;

		#endregion

		#region Private Vars

		private int _precision;

		private int _floatPointScale;

		#endregion

		#region Public Methods
		
		public DMFloat(string text, Func<float> getter, Action<float> setter = null, string shortcut = null, string help = null)
			: base(text, getter, setter, shortcut, help)
		{
			SetPrecision(2);
		}

		public DMFloat SetPrecision(int value)
		{
			_precision = Mathf.Clamp(value, 0, FloatFormats.Formats.Length - 1);
			_floatPointScale = (int)Mathf.Pow(10, _precision);

			return this;
		}

		#endregion

		#region Private Methods

		protected override string ValueToString(float value) => value.ToString(string.IsNullOrEmpty(Format) ? FloatFormats.Formats[_precision] : Format);

		protected override float ValueIncrement(float value, bool isShift) => (Mathf.Floor(value * _floatPointScale + 0.1f) + (isShift ? ShiftStep : Step)) / _floatPointScale;

		protected override float ValueDecrement(float value, bool isShift) => (Mathf.Floor(value * _floatPointScale + 0.1f) - (isShift ? ShiftStep : Step)) / _floatPointScale;

		#endregion
	}
}
