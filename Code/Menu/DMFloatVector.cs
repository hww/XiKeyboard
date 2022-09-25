﻿/* Copyright (c) 2021 dr. ext (Vladimir Sigalkin) */

using UnityEngine;

using System;
using XiKeyboard.Libs;
using XiKeyboard.KeyMaps;

namespace XiKeyboard.Menu
{
	public abstract class DMFloatVector<TStruct> : MenuValueLine<TStruct> where TStruct : struct, IFormattable
	{
		#region Public Vars

		public string Format;

		public int Step = 1;

		#endregion

		#region Private Vars

		private int _precision;

		private int _floatPointScale;

		#endregion

		#region Public Vars

		public void SetPrecision(int value)
		{
			_precision = Mathf.Clamp(value, 0, FloatFormats.Formats.Length - 1);
			_floatPointScale = (int) Mathf.Pow(10, _precision);
		}

		#endregion

		#region Protected Methods
		
		protected DMFloatVector(string text, Func<TStruct> getter, Action<TStruct> setter = null, string shortcut = null, string help = null)
			: base(text, getter, setter, shortcut, help)
		{
			if (setter != null)
			{
				var names = StructUtils.GetFieldsNames(typeof(TStruct));
				var count = StructUtils.GetFieldsCount(typeof(TStruct));

				for (var i = 0; i < count; i++)
				{
					//var fieldIndex = i;
					//_fields[i] = _fieldsBranch.Add(names[i],
					//	() => StructFieldGetter(getter.Invoke(), fieldIndex), v =>
					//	{
					//		var vector = getter.Invoke();
					//		StructFieldSetter(ref vector, fieldIndex, v);
					//		setter.Invoke(vector);
					//	}, i);
				}

			}

			SetPrecision(2);
		}

		public override void OnEvent(IMenuController controller)
		{
			base.OnEvent(controller);
		}

		protected sealed override string ValueToString(TStruct value) =>
			value.ToString(string.IsNullOrEmpty(Format) ? FloatFormats.Formats[_precision] : Format, null);

		protected sealed override TStruct ValueIncrement(TStruct value, bool isShift)
		{
			var count = StructUtils.GetFieldsCount(typeof(TStruct));
			for (var i = 0; i < count; i++)
			{
				StructFieldSetter(ref value, i,
					(Mathf.Floor(StructFieldGetter(value, i) * _floatPointScale + 0.1f) + Step) / _floatPointScale);
			}

			return value;
		}

		protected sealed override TStruct ValueDecrement(TStruct value, bool isShift)
		{
			var count = StructUtils.GetFieldsCount(typeof(TStruct));
			for (var i = 0; i < count; i++)
			{
				StructFieldSetter(ref value, i,
					(Mathf.Floor(StructFieldGetter(value, i) * _floatPointScale + 0.1f) - Step) / _floatPointScale);
			}

			return value;
		}

		protected abstract float StructFieldGetter(TStruct vector, int fieldIndex);

		protected abstract void StructFieldSetter(ref TStruct vector, int fieldIndex, float value);

		#endregion

		#region Private Methods



		#endregion
	}

	public class DMVector2 : DMFloatVector<Vector2>
	{
		#region Public Methods

		public DMVector2(string text, Func<Vector2> getter, Action<Vector2> setter = null, string shortcut = null, string help = null)
			: base(text, getter, setter, shortcut, help)
		{ 
		}

		#endregion

		#region Protected Methods

		protected override float StructFieldGetter(Vector2 vector, int fieldIndex) => vector[fieldIndex];

		protected override void StructFieldSetter(ref Vector2 vector, int fieldIndex, float value) =>
			vector[fieldIndex] = value;

		#endregion
	}

	public class DMVector3 : DMFloatVector<Vector3>
	{
		#region Public Methods

		public DMVector3(string text, Func<Vector3> getter, Action<Vector3> setter = null, string shortcut = null, string help = null)
			: base(text, getter, setter, shortcut, help)
		{
		}

		#endregion

		#region Protected Methods

		protected override float StructFieldGetter(Vector3 vector, int fieldIndex) => vector[fieldIndex];

		protected override void StructFieldSetter(ref Vector3 vector, int fieldIndex, float value) =>
			vector[fieldIndex] = value;

		#endregion
	}

	public class DMVector4 : DMFloatVector<Vector4>
	{
		#region Public Methods

		public DMVector4(string text, Func<Vector4> getter, Action<Vector4> setter = null, string shortcut = null, string help = null)
			: base(text, getter, setter, shortcut, help)
		{
		}

		#endregion

		#region Protected Methods

		protected override float StructFieldGetter(Vector4 vector, int fieldIndex) => vector[fieldIndex];

		protected override void StructFieldSetter(ref Vector4 vector, int fieldIndex, float value) =>
			vector[fieldIndex] = value;

		#endregion
	}

	public class DMQuaternion : DMFloatVector<Quaternion>
	{
		#region Public Methods

		public DMQuaternion(string text, Func<Quaternion> getter, Action<Quaternion> setter = null, string shortcut = null, string help = null)
			: base(text, getter, setter, shortcut, help)
		{
		}

		#endregion

		#region Protected Methods

		protected override float StructFieldGetter(Quaternion vector, int fieldIndex) => vector[fieldIndex];

		protected override void StructFieldSetter(ref Quaternion vector, int fieldIndex, float value) =>
			vector[fieldIndex] = value;

		#endregion
	}

	public class DMColor : DMFloatVector<Color>
	{
		#region Public Methods

		public DMColor(string text, Func<Color> getter, Action<Color> setter = null, string shortcut = null, string help = null)
			: base(text, getter, setter, shortcut, help)
		{
		}

		#endregion

		#region Protected Methods

		protected override float StructFieldGetter(Color vector, int fieldIndex) => vector[fieldIndex];

		protected override void StructFieldSetter(ref Color vector, int fieldIndex, float value) =>
			vector[fieldIndex] = value;

		#endregion
	}
}