/* Copyright (c) 2021 Valerya Pudova (hww) */

using System;
using UnityEngine;
using XiKeyboard.KeyMaps;
using XiKeyboard.Menu;
using XiKeyboard.Rendering;

namespace XiKeyboard
{
    ///-------------------------------------------------------------------------------------------------
    /// <summary>   The interface to menu and keyboard system. </summary>
    ///

    ///-------------------------------------------------------------------------------------------------

	public static class DM
	{
        ///-------------------------------------------------------------------------------------------------
        /// <summary>   The global keymap. It is represent top level of keymaps. </summary>
        ///
        /// <value> The global keymap. </value>
        ///-------------------------------------------------------------------------------------------------

		public static KeyMap GlobalKeymap => KeyMap.GlobalKeymap;

        ///-------------------------------------------------------------------------------------------------
        /// <summary>   The main menu map tree. </summary>
        ///
        /// <value> The menu bar. </value>
        ///-------------------------------------------------------------------------------------------------

		public static MenuMap MenuBar => MenuMap.MenuBar;

        ///-------------------------------------------------------------------------------------------------
        /// <summary>
        /// (Immutable)
        /// Default menu map Controller.
        /// </summary>
        ///-------------------------------------------------------------------------------------------------

		static readonly MenuController Controller = new MenuController();

		#region Static Public Vars

        ///-------------------------------------------------------------------------------------------------
        /// <summary>   Is menu visible or not. </summary>
        ///
        /// <value> True if this object is visible, false if not. </value>
        ///-------------------------------------------------------------------------------------------------

		public static bool IsVisible => Controller.IsVisible;

		#endregion

		#region Public Methods

        ///-------------------------------------------------------------------------------------------------
        /// <summary>   Static constructor. </summary>
        ///

        ///-------------------------------------------------------------------------------------------------

		static DM()
		{

		}

        ///-------------------------------------------------------------------------------------------------
        /// <summary>   Open default menu. </summary>
        ///

        ///-------------------------------------------------------------------------------------------------

		public static void Open() => Controller.Open();

        ///-------------------------------------------------------------------------------------------------
        /// <summary>   Open the menu map. </summary>
        ///

        ///
        /// <param name="menu"> . </param>
        ///-------------------------------------------------------------------------------------------------

		public static void Open(KeyMap menu) => Controller.Open(menu);

        ///-------------------------------------------------------------------------------------------------
        /// <summary>   Close current mmenu. </summary>
        ///

        ///-------------------------------------------------------------------------------------------------

		public static void Close() => Controller.Close();

        ///-------------------------------------------------------------------------------------------------
        /// <summary>   Toggle visibility. </summary>
        ///

        ///-------------------------------------------------------------------------------------------------

		public static void ToggleVisibility() => Controller.ToggleVisibility();

        ///-------------------------------------------------------------------------------------------------
        /// <summary>   Get the menu name from the path. </summary>
        ///

        ///
        /// <param name="path"> A menu path. </param>
        ///
        /// <returns>   The menu name. </returns>
        ///-------------------------------------------------------------------------------------------------

		private static string GetMenuName(string path)
		{
			var idx = path.LastIndexOf('/');
			return (idx >= 0) ? path.Substring(idx + 1) : path;
		}

        ///-------------------------------------------------------------------------------------------------
        /// <summary>   Get the menu path (aka folder) and get rid of name. </summary>
        ///

        ///
        /// <param name="path"> A menu path. </param>
        ///
        /// <returns>   The menu path. </returns>
        ///-------------------------------------------------------------------------------------------------

		private static string GetMenuPath(string path)
		{
			var idx = path.LastIndexOf('/');
			return (idx >= 0) ? path.Substring(0, idx) : null;
		}

        ///-------------------------------------------------------------------------------------------------
        /// <summary>   Create and define menu map. </summary>
        ///

        ///
        /// <param name="path">     Path to the menu map. </param>
        /// <param name="title">    The title of menu map. </param>
        /// <param name="help">     The help tex for menu map. </param>
        ///
        /// <returns>   A MenuMap. </returns>
        ///-------------------------------------------------------------------------------------------------

		public static MenuMap EasyCreateMenu(string path, string title, string help) => MenuBar.EasyCreateMenu(path, title, help);

        ///-------------------------------------------------------------------------------------------------
        /// <summary>   Adds path. </summary>
        ///

        ///
        /// <param name="path">     A menu path. </param>
        /// <param name="getter">   The getter. </param>
        /// <param name="shortcut"> (Optional) The shortcut. </param>
        /// <param name="help">     (Optional) The help tex for menu map. </param>
        ///
        /// <returns>   A DMString. </returns>
        ///-------------------------------------------------------------------------------------------------

		public static DMString Add(string path, Func<string> getter, string shortcut = null, string help = null)
		{
			var val = new DMString(GetMenuName(path), getter, shortcut, help);
			MenuBar.DefineMenuLine(path, val);
			return val;
		}

        ///-------------------------------------------------------------------------------------------------
        /// <summary>   Adds path. </summary>
        ///

        ///
        /// <param name="path">     A menu path. </param>
        /// <param name="getter">   The getter. </param>
        /// <param name="setter">   (Optional) The setter. </param>
        /// <param name="shortcut"> (Optional) The shortcut. </param>
        /// <param name="help">     (Optional) The help tex for menu map. </param>
        ///
        /// <returns>   A DMString. </returns>
        ///-------------------------------------------------------------------------------------------------

		public static DMBool Add(string path, Func<bool> getter, Action<bool> setter = null, string shortcut = null, string help = null)
		{
			var val = new DMBool(GetMenuName(path), getter, setter, shortcut, help);
			MenuBar.DefineMenuLine(path, val);
			return val;
		}

        ///-------------------------------------------------------------------------------------------------
        /// <summary>   Adds a bool. </summary>
        ///

        ///
        /// <param name="path">     A menu path. </param>
        /// <param name="getter">   The getter. </param>
        /// <param name="setter">   (Optional) The setter. </param>
        /// <param name="shortcut"> (Optional) The shortcut. </param>
        /// <param name="help">     (Optional) The help tex for menu map. </param>
        ///
        /// <returns>   A DMBool. </returns>
        ///-------------------------------------------------------------------------------------------------

		public static DMBool AddBool(string path, Func<bool> getter, Action<bool> setter = null, string shortcut = null, string help = null) =>
			Add(path, getter, setter, shortcut, help);

        ///-------------------------------------------------------------------------------------------------
        /// <summary>   Adds path. </summary>
        ///

        ///
        /// <typeparam name="T">    Generic type parameter. </typeparam>
        /// <param name="path">     A menu path. </param>
        /// <param name="getter">   The getter. </param>
        /// <param name="setter">   (Optional) The setter. </param>
        /// <param name="shortcut"> (Optional) The shortcut. </param>
        /// <param name="help">     (Optional) The help tex for menu map. </param>
        ///
        /// <returns>   A DMString. </returns>
        ///-------------------------------------------------------------------------------------------------

		public static DMEnum<T> Add<T>(string path, Func<T> getter, Action<T> setter = null, string shortcut = null, string help = null) where T : struct, Enum
		{
			var val = new DMEnum<T>(GetMenuName(path), getter, setter, shortcut, help);
			MenuBar.DefineMenuLine(path, val);
			return val;
		}

        ///-------------------------------------------------------------------------------------------------
        /// <summary>   Adds an enum flags. </summary>
        ///

        ///
        /// <typeparam name="T">    Generic type parameter. </typeparam>
        /// <param name="path">     A menu path. </param>
        /// <param name="getter">   The getter. </param>
        /// <param name="setter">   (Optional) The setter. </param>
        /// <param name="shortcut"> (Optional) The shortcut. </param>
        /// <param name="help">     (Optional) The help tex for menu map. </param>
        ///-------------------------------------------------------------------------------------------------

		public static void AddEnumFlags<T>(string path, Func<T> getter, Action<T> setter = null, string shortcut = null, string help = null) where T : struct, Enum
		{
			var type = typeof(T);
			if (type.IsDefined(typeof(FlagsAttribute), false))
			{
				var values = (T[])Enum.GetValues(type);
				for (var i = 0; i < values.Length; i++)
				{
					var value = values[i];

					AddBool(path + " " + value.ToString(),
						() =>
						{
							var intGetter = (int)(object)getter.Invoke();
							var intValue = (int)(object)value;
							return (intGetter & intValue) != 0;
						},
						v =>
						{
							var intGetter = (int)(object)getter.Invoke();
							var intValue = (int)(object)value;
							setter.Invoke((T)(object)(intGetter ^ intValue));
						},
						shortcut,
						help);
				}
			} else
            {
				Debug.LogError("The AddEnumFlags expects a Flags enum");
            }
		}

        ///-------------------------------------------------------------------------------------------------
        /// <summary>   Adds path. </summary>
        ///

        ///
        /// <param name="path">     A menu path. </param>
        /// <param name="getter">   The getter. </param>
        /// <param name="setter">   (Optional) The setter. </param>
        /// <param name="shortcut"> (Optional) The shortcut. </param>
        /// <param name="help">     (Optional) The help tex for menu map. </param>
        ///
        /// <returns>   A DMString. </returns>
        ///-------------------------------------------------------------------------------------------------

		public static DMUInt8 Add(string path, Func<byte> getter, Action<byte> setter = null, string shortcut = null, string help = null)
		{
			var val = new DMUInt8(GetMenuName(path), getter, setter, shortcut, help);
			MenuBar.DefineMenuLine(path, val);
			return val;
		}

        ///-------------------------------------------------------------------------------------------------
        /// <summary>   Adds path. </summary>
        ///

        ///
        /// <param name="path">     A menu path. </param>
        /// <param name="getter">   The getter. </param>
        /// <param name="setter">   (Optional) The setter. </param>
        /// <param name="shortcut"> (Optional) The shortcut. </param>
        /// <param name="help">     (Optional) The help tex for menu map. </param>
        ///
        /// <returns>   A DMString. </returns>
        ///-------------------------------------------------------------------------------------------------

		public static DMUInt16 Add(string path, Func<UInt16> getter, Action<UInt16> setter = null, string shortcut = null, string help = null)
		{
			var val = new DMUInt16(GetMenuName(path), getter, setter, shortcut, help);
			MenuBar.DefineMenuLine(path, val);
			return val;
		}

        ///-------------------------------------------------------------------------------------------------
        /// <summary>   Adds path. </summary>
        ///

        ///
        /// <param name="path">     A menu path. </param>
        /// <param name="getter">   The getter. </param>
        /// <param name="setter">   (Optional) The setter. </param>
        /// <param name="shortcut"> (Optional) The shortcut. </param>
        /// <param name="help">     (Optional) The help tex for menu map. </param>
        ///
        /// <returns>   A DMString. </returns>
        ///-------------------------------------------------------------------------------------------------

		public static DMUInt32 Add(string path, Func<UInt32> getter, Action<UInt32> setter = null, string shortcut = null, string help = null)
        {
			var val = new DMUInt32(GetMenuName(path), getter, setter, shortcut, help);
			MenuBar.DefineMenuLine(path, val);
			return val;
		}

        ///-------------------------------------------------------------------------------------------------
        /// <summary>   Adds path. </summary>
        ///

        ///
        /// <param name="path">     A menu path. </param>
        /// <param name="getter">   The getter. </param>
        /// <param name="setter">   (Optional) The setter. </param>
        /// <param name="shortcut"> (Optional) The shortcut. </param>
        /// <param name="help">     (Optional) The help tex for menu map. </param>
        ///
        /// <returns>   A DMString. </returns>
        ///-------------------------------------------------------------------------------------------------

		public static DMUInt64 Add(string path, Func<UInt64> getter, Action<UInt64> setter = null, string shortcut = null, string help = null)
        {
			var val = new DMUInt64(GetMenuName(path), getter, setter, shortcut, help);
			MenuBar.DefineMenuLine(path, val);
			return val;
		}

        ///-------------------------------------------------------------------------------------------------
        /// <summary>   Adds path. </summary>
        ///

        ///
        /// <param name="path">     A menu path. </param>
        /// <param name="getter">   The getter. </param>
        /// <param name="setter">   (Optional) The setter. </param>
        /// <param name="shortcut"> (Optional) The shortcut. </param>
        /// <param name="help">     (Optional) The help tex for menu map. </param>
        ///
        /// <returns>   A DMString. </returns>
        ///-------------------------------------------------------------------------------------------------

		public static DMInt8 Add(string path, Func<sbyte> getter, Action<sbyte> setter = null, string shortcut = null, string help = null)
		{
			var val = new DMInt8(GetMenuName(path), getter, setter, shortcut, help);
			MenuBar.DefineMenuLine(path, val);
			return val;
		}

        ///-------------------------------------------------------------------------------------------------
        /// <summary>   Adds path. </summary>
        ///

        ///
        /// <param name="path">     A menu path. </param>
        /// <param name="getter">   The getter. </param>
        /// <param name="setter">   (Optional) The setter. </param>
        /// <param name="shortcut"> (Optional) The shortcut. </param>
        /// <param name="help">     (Optional) The help tex for menu map. </param>
        ///
        /// <returns>   A DMString. </returns>
        ///-------------------------------------------------------------------------------------------------

		public static DMInt16 Add(string path, Func<Int16> getter, Action<Int16> setter = null, string shortcut = null, string help = null)
		{
			var val = new DMInt16(GetMenuName(path), getter, setter, shortcut, help);
			MenuBar.DefineMenuLine(path, val);
			return val;
		}

        ///-------------------------------------------------------------------------------------------------
        /// <summary>   Adds path. </summary>
        ///

        ///
        /// <param name="path">     A menu path. </param>
        /// <param name="getter">   The getter. </param>
        /// <param name="setter">   (Optional) The setter. </param>
        /// <param name="shortcut"> (Optional) The shortcut. </param>
        /// <param name="help">     (Optional) The help tex for menu map. </param>
        ///
        /// <returns>   A DMString. </returns>
        ///-------------------------------------------------------------------------------------------------

		public static DMInt32 Add(string path, Func<Int32> getter, Action<Int32> setter = null, string shortcut = null, string help = null)
        {
			var val = new DMInt32(GetMenuName(path), getter, setter, shortcut, help);
			MenuBar.DefineMenuLine(path, val);
			return val;
		}

        ///-------------------------------------------------------------------------------------------------
        /// <summary>   Adds path. </summary>
        ///

        ///
        /// <param name="path">     A menu path. </param>
        /// <param name="getter">   The getter. </param>
        /// <param name="setter">   (Optional) The setter. </param>
        /// <param name="shortcut"> (Optional) The shortcut. </param>
        /// <param name="help">     (Optional) The help tex for menu map. </param>
        ///
        /// <returns>   A DMString. </returns>
        ///-------------------------------------------------------------------------------------------------

		public static DMInt64 Add(string path, Func<Int64> getter, Action<Int64> setter = null, string shortcut = null, string help = null) 		
		{
			var val = new DMInt64(GetMenuName(path), getter, setter, shortcut, help);
			MenuBar.DefineMenuLine(path, val);
			return val;
		}

        ///-------------------------------------------------------------------------------------------------
        /// <summary>   Adds path. </summary>
        ///

        ///
        /// <param name="path">     A menu path. </param>
        /// <param name="getter">   The getter. </param>
        /// <param name="setter">   (Optional) The setter. </param>
        /// <param name="shortcut"> (Optional) The shortcut. </param>
        /// <param name="help">     (Optional) The help tex for menu map. </param>
        ///
        /// <returns>   A DMString. </returns>
        ///-------------------------------------------------------------------------------------------------

		public static DMVector2 Add(string path, Func<Vector2> getter, Action<Vector2> setter = null, string shortcut = null, string help = null)
        {
			var val = new DMVector2(GetMenuName(path), getter, setter, shortcut, help);
			MenuBar.DefineMenuLine(path, val);
			return val;
		}

        ///-------------------------------------------------------------------------------------------------
        /// <summary>   Adds path. </summary>
        ///

        ///
        /// <param name="path">     A menu path. </param>
        /// <param name="getter">   The getter. </param>
        /// <param name="setter">   (Optional) The setter. </param>
        /// <param name="shortcut"> (Optional) The shortcut. </param>
        /// <param name="help">     (Optional) The help tex for menu map. </param>
        ///
        /// <returns>   A DMString. </returns>
        ///-------------------------------------------------------------------------------------------------

		public static DMVector3 Add(string path, Func<Vector3> getter, Action<Vector3> setter = null, string shortcut = null, string help = null)
		{
			var val = new DMVector3(GetMenuName(path), getter, setter, shortcut, help);
			MenuBar.DefineMenuLine(path, val);
			return val;
		}

        ///-------------------------------------------------------------------------------------------------
        /// <summary>   Adds path. </summary>
        ///

        ///
        /// <param name="path">     A menu path. </param>
        /// <param name="getter">   The getter. </param>
        /// <param name="setter">   (Optional) The setter. </param>
        /// <param name="shortcut"> (Optional) The shortcut. </param>
        /// <param name="help">     (Optional) The help tex for menu map. </param>
        ///
        /// <returns>   A DMString. </returns>
        ///-------------------------------------------------------------------------------------------------

		public static DMVector4 Add(string path, Func<Vector4> getter, Action<Vector4> setter = null, string shortcut = null, string help = null)
		{
			var val = new DMVector4(GetMenuName(path), getter, setter, shortcut, help);
			MenuBar.DefineMenuLine(path, val);
			return val;
		}

        ///-------------------------------------------------------------------------------------------------
        /// <summary>   Adds path. </summary>
        ///

        ///
        /// <param name="path">     A menu path. </param>
        /// <param name="getter">   The getter. </param>
        /// <param name="setter">   (Optional) The setter. </param>
        /// <param name="shortcut"> (Optional) The shortcut. </param>
        /// <param name="help">     (Optional) The help tex for menu map. </param>
        ///
        /// <returns>   A DMString. </returns>
        ///-------------------------------------------------------------------------------------------------

		public static DMQuaternion Add(string path, Func<Quaternion> getter, Action<Quaternion> setter = null, string shortcut = null, string help = null)
		{
			var val = new DMQuaternion(GetMenuName(path), getter, setter, shortcut, help);
			MenuBar.DefineMenuLine(path, val);
			return val;
		}

        ///-------------------------------------------------------------------------------------------------
        /// <summary>   Adds path. </summary>
        ///

        ///
        /// <param name="path">     A menu path. </param>
        /// <param name="getter">   The getter. </param>
        /// <param name="setter">   (Optional) The setter. </param>
        /// <param name="shortcut"> (Optional) The shortcut. </param>
        /// <param name="help">     (Optional) The help tex for menu map. </param>
        ///
        /// <returns>   A DMString. </returns>
        ///-------------------------------------------------------------------------------------------------

		public static DMColor Add(string path, Func<Color> getter, Action<Color> setter = null, string shortcut = null, string help = null)
		{
			var val = new DMColor(GetMenuName(path), getter, setter, shortcut, help);
			MenuBar.DefineMenuLine(path, val);
			return val;
		}

        ///-------------------------------------------------------------------------------------------------
        /// <summary>   Adds path. </summary>
        ///

        ///
        /// <param name="path">     A menu path. </param>
        /// <param name="getter">   The getter. </param>
        /// <param name="setter">   (Optional) The setter. </param>
        /// <param name="shortcut"> (Optional) The shortcut. </param>
        /// <param name="help">     (Optional) The help tex for menu map. </param>
        ///
        /// <returns>   A DMString. </returns>
        ///-------------------------------------------------------------------------------------------------

		public static DMVector2Int Add(string path, Func<Vector2Int> getter, Action<Vector2Int> setter = null, string shortcut = null, string help = null)
		{
			var val = new DMVector2Int(GetMenuName(path), getter, setter, shortcut, help);
			MenuBar.DefineMenuLine(path, val);
			return val;
		}

        ///-------------------------------------------------------------------------------------------------
        /// <summary>   Adds path. </summary>
        ///

        ///
        /// <param name="path">     A menu path. </param>
        /// <param name="getter">   The getter. </param>
        /// <param name="setter">   (Optional) The setter. </param>
        /// <param name="shortcut"> (Optional) The shortcut. </param>
        /// <param name="help">     (Optional) The help tex for menu map. </param>
        ///
        /// <returns>   A DMString. </returns>
        ///-------------------------------------------------------------------------------------------------

		public static DMVector3Int Add(string path, Func<Vector3Int> getter, Action<Vector3Int> setter = null, string shortcut = null, string help = null)
		{
			var val = new DMVector3Int(GetMenuName(path), getter, setter, shortcut, help);
			MenuBar.DefineMenuLine(path, val);
			return val;
		}

        ///-------------------------------------------------------------------------------------------------
        /// <summary>   Adds path. </summary>
        ///

        ///
        /// <param name="path">     A menu path. </param>
        /// <param name="getter">   The getter. </param>
        /// <param name="setter">   (Optional) The setter. </param>
        /// <param name="shortcut"> (Optional) The shortcut. </param>
        /// <param name="help">     (Optional) The help tex for menu map. </param>
        ///
        /// <returns>   A DMString. </returns>
        ///-------------------------------------------------------------------------------------------------

		public static DMFloat Add(string path, Func<float> getter, Action<float> setter = null, string shortcut = null, string help = null)
		{
			var val = new DMFloat(GetMenuName(path), getter, setter, shortcut, help);
			MenuBar.DefineMenuLine(path, val);
			return val;
		}

        ///-------------------------------------------------------------------------------------------------
        /// <summary>   Produce the sequence which can  be binded to a key. </summary>
        ///

        ///
        /// <param name="name">     The name. </param>
        /// <param name="sequence"> The sequence. </param>
        /// <param name="help">     (Optional) The help tex for menu map. </param>
        ///
        /// <returns>   A DMSequenceBinding. </returns>
        ///-------------------------------------------------------------------------------------------------

		public static DMSequenceBinding MakeSequenceBinding(string name, int[] sequence, string help = null) =>
            new DMSequenceBinding(name, sequence, help);

        #endregion

        ///-------------------------------------------------------------------------------------------------
        /// <summary>   Called every frame, if the MonoBehaviour is enabled. </summary>
        ///

        ///-------------------------------------------------------------------------------------------------

        public static void Update()
		{
			(Controller as IMenuRender_Update).Update();
		}

        ///-------------------------------------------------------------------------------------------------
        /// <summary>   Called for rendering and handling GUI events. </summary>
        ///

        ///-------------------------------------------------------------------------------------------------

		public static void OnGUI()
        {
			(Controller as IMenuRender_OnGUI).OnGUI();
		}
	}
}
