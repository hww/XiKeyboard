/* Copyright (c) 2021 dr. ext (Vladimir Sigalkin) */

using UnityEngine;

using System;
using XiKeyboard.Libs;

namespace XiKeyboard.Menu
{
	public abstract class DMIntVector<TStruct> : TMenuValueLine<TStruct> where TStruct : struct, IFormattable
	{
		#region Public Vars

		public int Step = 1;

		public string Format;

		#endregion

		#region Private Vars


		#endregion

		#region Public Methods

		#endregion

		#region Protected Methods

		protected DMIntVector(string text, Func<TStruct> getter, Action<TStruct> setter = null, string shortcut = null, string help = null)
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
		}

		public override void OnEvent(MenuEvent menuEvent)
		{
			base.OnEvent(menuEvent);
		}
		public override int Count => StructUtils.GetFieldsCount(typeof(TStruct));
		public override string GetValue(int idx)
		{
			var val = StructFieldGetter(_getter.Invoke(), idx);
			return val.ToString(Format, null);
		}
		protected sealed override string ValueToString(TStruct value) => value.ToString(Format, null);

		protected sealed override TStruct ValueIncrement(TStruct value, bool isShift, int idx = 0)
		{
			var count = StructUtils.GetFieldsCount(typeof(TStruct));
			var i = idx % count;
			StructFieldSetter(ref value, i, StructFieldGetter(value, i) + Step);
			return value;
		}

		protected sealed override TStruct ValueDecrement(TStruct value, bool isShift, int idx = 0)
		{
			var count = StructUtils.GetFieldsCount(typeof(TStruct));
			var i = idx % count;
			StructFieldSetter(ref value, i, StructFieldGetter(value, i) - Step);

			return value;
		}

		protected abstract int StructFieldGetter(TStruct vector, int fieldIndex);

		protected abstract void StructFieldSetter(ref TStruct vector, int fieldIndex, int value);

		protected override void ChangeValue(TStruct value, bool isReset)
		{
			// Change value.
			_setter.Invoke(value);
			var count = StructUtils.GetFieldsCount(typeof(TStruct));
			_isDefault = true;
			for (var i = 0; i < count; i++)
				_isDefault &= StructFieldGetter(value, i) == StructFieldGetter(_defaultValue, i);
		}

		#endregion

		#region Private Methods

		#endregion
	}

	public class DMVector2Int : DMIntVector<Vector2Int>
	{
		#region Public Methods

		public DMVector2Int(string text, Func<Vector2Int> getter, Action<Vector2Int> setter = null, string shortcut = null, string help = null)
			: base(text, getter, setter, shortcut, help)
		{
		}

		#endregion

		#region Protected Methods

		protected override int StructFieldGetter(Vector2Int vector, int fieldIndex) => vector[fieldIndex];

		protected override void StructFieldSetter(ref Vector2Int vector, int fieldIndex, int value) =>
			vector[fieldIndex] = value;

		#endregion
	}

	public class DMVector3Int : DMIntVector<Vector3Int>
	{
		#region Public Methods

		public DMVector3Int(string text, Func<Vector3Int> getter, Action<Vector3Int> setter = null, string shortcut = null, string help = null)
			: base(text, getter, setter, shortcut, help)
		{
		}

		#endregion

		#region Protected Methods

		protected override int StructFieldGetter(Vector3Int vector, int fieldIndex) => vector[fieldIndex];

		protected override void StructFieldSetter(ref Vector3Int vector, int fieldIndex, int value) =>
			vector[fieldIndex] = value;

		#endregion
	}
}