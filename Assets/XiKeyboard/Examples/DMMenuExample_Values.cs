/* Copyright (c) 2021 dr. ext (Vladimir Sigalkin) */

using UnityEngine;
using System;
using XiKeyboard;
using XiKeyboard.Menu;
using XiKeyboard.KeyMaps;

namespace XiKeyboard.Examples.Menu
{
	public class DMMenuExample_Values : MonoBehaviour
	{
		#region Internal Types

		private enum ExampleEnums
		{
			One,
			Two,
			Three
		}

		[Flags]
		private enum ExampleFlags
		{
			One = 1 << 0,
			Two = 1 << 1,
			Three = 1 << 2,
		}

		#endregion

		#region Private Vars

		private string _string = "Hello, World!";

		private byte _uint8;

		private UInt16 _uint16;

		private UInt32 _uint32;

		private UInt64 _uint64;

		private sbyte _int8;

		private Int16 _int16;

		private Int32 _int32;

		private Int64 _int64;

		private float _float;

		private bool _bool;

		private Vector2 _vector2;

		private Vector3 _vector3;

		private Vector4 _vector4;

		private Quaternion _quaternion;

		private Color _color;

		private Vector2Int _vector2Int;

		private Vector3Int _vector3Int;

		private ExampleEnums _enum;

		private ExampleFlags _flags;

		private byte _uint8Storage;

		private UInt16 _uint16Storage;

		private UInt32 _uint32Storage;

		private UInt64 _uint64Storage;

		private sbyte _int8Storage;

		private Int16 _int16Storage;

		private Int32 _int32Storage;

		private Int64 _int64Storage;

		private float _floatStorage;

		private bool _boolStorage;

		private ExampleEnums _enumStorage;

		private ExampleFlags _flagsStorage;

		#endregion

		#region Unity Methods

		private void Start()
		{
			// Simple Menus
			var simpleMenu = KeyMap.GlobalKeymap.CreateMenu("simple", "Simple Menu", "Help for simpe menu");
			simpleMenu.AddMenuLine("-4", new MenuSeparator(MenuSeparator.Type.SingleLine));
			var subMenu = KeyMap.GlobalKeymap.CreateMenu("simple/sub", "Sub Menu", "Help for sub menu menu");

			//DM.Add("Simple Menus/Action", action => Debug.Log("Hello, Action!"), "Simple Action");

			DM.Add("simple/String", () => _string);
			DM.Add("simple/UInt8", () => _uint8, v => _uint8 = v);
			DM.Add("simple/UInt16", () => _uint16, v => _uint16 = v);
			DM.Add("simple/UInt32", () => _uint32, v => _uint32 = v);
			DM.Add("simple/UInt64", () => _uint64, v => _uint64 = v);
			DM.Add("simple/Int8", () => _int8, v => _int8 = v);
			DM.Add("simple/Int16", () => _int16, v => _int16 = v);
			DM.Add("simple/Int32", () => _int32, v => _int32 = v);
			DM.Add("simple/Int64", () => _int64, v => _int64 = v);
			DM.Add("simple/Float", () => _float, v => _float = v).SetPrecision(2);
			simpleMenu.AddMenuLine("-5", new MenuSeparator(MenuSeparator.Type.SingleLine));
			DM.AddEnumFlags("simple/Flags", () => _flags, v => _flags = v);
			simpleMenu.AddMenuLine("-6", new MenuSeparator(MenuSeparator.Type.SingleLine));
			DM.Add("simple/Enum", () => _enum, v => _enum = v);
			simpleMenu.AddMenuLine("-6", new MenuSeparator(MenuSeparator.Type.DashedLine));
			DM.Add("simple/Bool", () => _bool, v => _bool = v, "S-f");

			//DM.Add("simple/Vector 3", () => _vector3, v => _vector3 = v).SetPrecision(2);
			DM.Open(simpleMenu);
		}
		private void Update()
		{
			DM.Update();
		}

		private void OnGUI()
        {
			DM.OnGUI();
        }

        #endregion
    }
}