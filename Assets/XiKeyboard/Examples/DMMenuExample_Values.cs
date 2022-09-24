/* Copyright (c) 2021 dr. ext (Vladimir Sigalkin) */

using UnityEngine;
using System;
using XiKeyboard;

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
			var simpleMenu = KeyMap.GlobalKeymap.CreateMenu("simple", "Simple Menu", "Help for file menu");

			//DM.Add("Simple Menus/Action", action => Debug.Log("Hello, Action!"), "Simple Action");
			simpleMenu.AddMenuLine("-4", new DMMenuSeparator(DMMenuSeparator.Type.SingleLine));

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
			DM.Add("simple/Bool", () => _bool, v => _bool = v);
			DM.Add("simple/Enum", () => _enum, v => _enum = v);
			simpleMenu.AddMenuLine("-5", new DMMenuSeparator(DMMenuSeparator.Type.SingleLine));
			DM.AddRadio("simple/Flags", () => _flags, v => _flags = v);
			simpleMenu.AddMenuLine("-6", new DMMenuSeparator(DMMenuSeparator.Type.SingleLine));

			//DM.Add("simple/Vector 2", () => _vector2, v => _vector2 = v).SetPrecision(2);
			//DM.Add("simple/Vector 3", () => _vector3, v => _vector3 = v).SetPrecision(2);
			//DM.Add("simple/Vector 4", () => _vector4, v => _vector4 = v).SetPrecision(2);
			//DM.Add("simple/Quaternion", () => _quaternion, v => _quaternion = v).SetPrecision(2);
			//DM.Add("simple/Color", () => _color, v => _color = v).SetPrecision(2);
			//DM.Add("simple/Vector 2 Int", () => _vector2Int, v => _vector2Int = v);
			//DM.Add("simple/Vector 3 Int", () => _vector3Int, v => _vector3Int = v);

			// Storage
			//DM.Add("simple/UInt8", () => _uint8Storage, v => _uint8Storage = v, order: 1).SetStorage(storage);
			//DM.Add("simple/UInt16", () => _uint16Storage, v => _uint16Storage = v, order: 2).SetStorage(storage);
			//DM.Add("simple/UInt32", () => _uint32Storage, v => _uint32Storage = v, order: 3).SetStorage(storage);
			//DM.Add("simple/UInt64", () => _uint64Storage, v => _uint64Storage = v, order: 4).SetStorage(storage);
			//DM.Add("simple/Int8", () => _int8Storage, v => _int8Storage = v, order: 5).SetStorage(storage);
			//DM.Add("simple/Int16", () => _int16Storage, v => _int16Storage = v, order: 6).SetStorage(storage);
			//DM.Add("simple/Int32", () => _int32Storage, v => _int32Storage = v, order: 7).SetStorage(storage);
			//DM.Add("simple/Int64", () => _int64Storage, v => _int64Storage = v, order: 8).SetStorage(storage);
			//DM.Add("simple/Float", () => _floatStorage, v => _floatStorage = v, order: 9).SetPrecision(2).SetStorage(storage);
			//DM.Add("simple/Bool", () => _boolStorage, v => _boolStorage = v, order: 10).SetStorage(storage);
			//DM.Add("simple/Enum", () => _enumStorage, v => _enumStorage = v, order: 11).SetStorage(storage);
			//DM.Add("simple/Flags", () => _flagsStorage, v => _flagsStorage = v, order: 12).SetStorage(storage);

			// Dynamic
			//DM.Add("Dynamic Transforms", FindObjectsOfType<Transform>, (branch, transform) =>
			//{
			//	branch.Add("Name", a => { Debug.Log(transform); });
			//});

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