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
		private const string kHelpText = "Menu navigation\n"
									   + "  A,S,D,W  Cursors\n"
									   + "  Q        Quit menu\n"
									   + "  E        Tollge menu\n"
									   + "  R        Reset value\n"
									   + "Demo file methods\n"
									   + "  SHIFT+f  Open simple menu\n"
									   + "  SHIFT+g  Open submmenu\n"
									   + "  SHIFT+b  Change boolean value\n"
									   + "--------------------\n";

		private string miniBuffer = "";

		private GUISkin skin;

		void Awake()
		{
			skin = Resources.Load<GUISkin>("XiKeyboard/Skins/Default Skin");
			skin.box.normal.background = MakeTex(2, 2, new Color(0f, 0f, 0f, 0.7f));
		}

		private void OnEnable()
		{
			// Simple Menus
			var simpleMenu = MenuMap.MenuBar.EasyCreateMenu("menu-bar/simple", "Simple Menu", "Help for simpe menu");
			
			var subMenu = MenuMap.MenuBar.EasyCreateMenu("menu-bar/simple/sub", "Sub Menu", "Help for sub menu menu");

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
			DM.Add("simple/Bool", () => _bool, v => _bool = v, "S-b");
			simpleMenu.AddMenuLine("-7", new MenuSeparator(MenuSeparator.Type.SingleLine));
			DM.Add("simple/Vector 2", () => _vector2, v => _vector2 = v).SetPrecision(2);
			DM.Add("simple/Vector 3", () => _vector3, v => _vector3 = v).SetPrecision(2);
			DM.Add("simple/Vector 4", () => _vector4, v => _vector4 = v).SetPrecision(2);
			DM.Add("simple/Quaternion", () => _quaternion, v => _quaternion = v).SetPrecision(2);
			DM.Add("simple/Color", () => _color, v => _color = v).SetPrecision(2);
			DM.Add("simple/Vector 2 Int", () => _vector2Int, v => _vector2Int = v);
			DM.Add("simple/Vector 3 Int", () => _vector3Int, v => _vector3Int = v);

			DM.Add("simple/sub/String", () => _string);

			// - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - -
			// All abow is creating the menu tree but did not define a 
			// shortcuts in the global kay bindings.
			// - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - -
			// To make S+F to open file menu
			KeyMap.GlobalKeymap.SetLocal("S-f", simpleMenu);
			KeyMap.GlobalKeymap.SetLocal("S-g", subMenu);

			// To display menu uncomment it 
			// DM.Open(simpleMenu);
		}
		private void Update()
		{
			DM.Update();
		}

		void OnGUI()
		{
			GUI.skin = skin;
			var menuText = kHelpText + "> " + miniBuffer;

			var textSize = GUI.skin.label.CalcSize(new GUIContent(menuText)) + new Vector2(10, 10);
			var position = new Vector2(500, 20);
			var rect = new Rect(position, textSize);

			GUI.Box(rect, GUIContent.none, GUI.skin.box);

			rect.x += 5f;
			rect.width -= 5f * 2f;
			rect.y += 5f;
			rect.height -= 5f * 2f;

			GUI.Label(rect, menuText);

			DM.OnGUI();
		}

		private Texture2D MakeTex(int width, int height, Color col)
		{
			Color[] pix = new Color[width * height];
			for (int i = 0; i < pix.Length; ++i)
			{
				pix[i] = col;
			}
			Texture2D result = new Texture2D(width, height);
			result.SetPixels(pix);
			result.Apply();
			return result;
		}
		#endregion
	}
}