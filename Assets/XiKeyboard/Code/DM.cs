/* Copyright (c) 2021 Valerya Pudova (hww) */

using System;
using UnityEngine;
using XiCore.StringTools;
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
        /// <summary>   Get the menu name from the keystroke. </summary>
        ///
        ///
        /// <param name="path"> A menu keystroke. </param>
        ///
        /// <returns>   The menu name. </returns>
        ///-------------------------------------------------------------------------------------------------

		private static string GetMenuName(string path)
		{
			var idx = path.LastIndexOf('/');
			return (idx >= 0) ? path.Substring(idx + 1) : path;
		}

        ///-------------------------------------------------------------------------------------------------
        /// <summary>   Get the menu keystroke (aka folder) and get rid of name. </summary>
        ///
        /// <remarks>   Valery,. </remarks>
        ///
        /// <param name="path"> A menu keystroke. </param>
        ///
        /// <returns>   The menu keystroke. </returns>
        ///-------------------------------------------------------------------------------------------------

		private static string GetMenuPath(string path)
		{
			var idx = path.LastIndexOf('/');
			return (idx >= 0) ? path.Substring(0, idx) : null;
		}

        ///-------------------------------------------------------------------------------------------------
        /// <summary>   Creates a menu. </summary>
        ///
        /// <remarks>   Valery,. </remarks>
        ///
        /// <param name="map">          The key map where to add this line. </param>
        /// <param name="path">         A menu keystroke. </param>
        /// <param name="title">        The title. </param>
        /// <param name="help">         The help. </param>
        /// <param name="afterEvent">   (Optional) The after event. </param>
        ///
        /// <returns>   The new menu. </returns>
        ///-------------------------------------------------------------------------------------------------

        public static MenuMap CreateMenu(KeyMap map, string path, string title, string help, string afterEvent = null)
        {
            var menuMap = new MenuMap(title, help);
            var after = afterEvent == null ? KeyEvent.None : KeyEvent.GetPseudoCode(afterEvent);
            var sequence = KBD.ParsePseudo(path.Split("/"));
            map.Define(sequence, menuMap, after);
            return menuMap;
        }

        ///-------------------------------------------------------------------------------------------------
        /// <summary>   DefineMenuLine list of key-strings. This way used for defining menu. </summary>
        ///
        /// <remarks>   Valery, 10/12/2022. </remarks>
        ///
        /// <param name="map">          The key map where to add this line. </param>
        /// <param name="keystroke">    Full pathname of the file. </param>
        /// <param name="binding">      The line. </param>
        ///
        /// <returns>   True if it succeeds, false if it fails. </returns>
        ///-------------------------------------------------------------------------------------------------

        public static bool DefineKey(KeyMap map, string keystroke, object binding)
        {
            return map.Define(keystroke, binding);
        }

        ///-------------------------------------------------------------------------------------------------
        /// <summary>   DefineMenuLine after. </summary>
        ///
        /// <remarks>   Valery,. </remarks>
        ///
        /// <param name="map">      The key map where to add this line. </param>
        /// <param name="path">     Full pathname of the file. </param>
        /// <param name="line">     The menu line. </param>
        /// <param name="after">    (Optional) Name of event. </param>
        ///
        /// <returns>   True if it succeeds, false if it fails. </returns>
        ///-------------------------------------------------------------------------------------------------

        public static bool DefineMenuLine(MenuMap map, string path, MenuLine line, string after = null)
        {
            if (path == null) path = Humanizer.Decamelize(line.Title);
            return map.DefineLine(path, line, after);
        }

        ///-------------------------------------------------------------------------------------------------
        /// <summary>   DefineMenuLine after. </summary>
        ///
        /// <remarks>   Valery,. </remarks>
        ///
        /// <param name="map">      The key map where to add this line. </param>
        /// <param name="path">     Full pathname of the file. </param>
        /// <param name="binding">  The line. </param>
        /// <param name="after">    (Optional) Name of event. </param>
        ///
        /// <returns>   True if it succeeds, false if it fails. </returns>
        ///-------------------------------------------------------------------------------------------------

        public static bool DefineMenuLine(MenuMap map, string path, MenuMap binding, string after = null)
        {
            return map.DefineLine(path, binding, after);
        }

        ///-------------------------------------------------------------------------------------------------
        /// <summary>   Adds keystroke. </summary>
        ///
        /// <remarks>   Valery,. </remarks>
        ///
        /// <param name="type"> The type. </param>
        ///
        /// <returns>   A DMString. </returns>
        ///
        /// ### <param name="title">    The title. </param>
        ///-------------------------------------------------------------------------------------------------

        public static MenuLine MenuLine(MenuSeparator.Type type)
        {
            return new MenuSeparator(MenuSeparator.Type.SingleLine);
        }

        ///-------------------------------------------------------------------------------------------------
        /// <summary>   Adds keystroke. </summary>
        ///
        /// <remarks>   Valery,. </remarks>
        ///
        /// <param name="title">    The title. </param>
        /// <param name="getter">   The getter. </param>
        /// <param name="shortcut"> (Optional) The shortcut. </param>
        /// <param name="help">     (Optional) The help tex for menu map. </param>
        ///
        /// <returns>   A DMString. </returns>
        ///-------------------------------------------------------------------------------------------------

		public static DMString MenuLine(string title, Func<string> getter, string shortcut = null, string help = null)
		{
            return new DMString(title, getter, shortcut, help);
		}

        ///-------------------------------------------------------------------------------------------------
        /// <summary>   Adds keystroke. </summary>
        ///
        /// <remarks>   Valery,. </remarks>
        ///
        /// <param name="title">    The title. </param>
        /// <param name="getter">   The getter. </param>
        /// <param name="setter">   (Optional) The setter. </param>
        /// <param name="shortcut"> (Optional) The shortcut. </param>
        /// <param name="help">     (Optional) The help tex for menu map. </param>
        ///
        /// <returns>   A DMString. </returns>
        ///-------------------------------------------------------------------------------------------------

		public static DMBool MenuLine(string title, Func<bool> getter, Action<bool> setter = null, string shortcut = null, string help = null)
		{
			return new DMBool(title, getter, setter, shortcut, help);
		}

        ///-------------------------------------------------------------------------------------------------
        /// <summary>   Adds keystroke. </summary>
        ///
        /// <remarks>   Valery,. </remarks>
        ///
        /// <typeparam name="T">    Generic type parameter. </typeparam>
        /// <param name="title">    The title. </param>
        /// <param name="getter">   The getter. </param>
        /// <param name="setter">   (Optional) The setter. </param>
        /// <param name="shortcut"> (Optional) The shortcut. </param>
        /// <param name="help">     (Optional) The help tex for menu map. </param>
        ///
        /// <returns>   A DMString. </returns>
        ///
        /// ### <param name="path"> A menu keystroke. </param>
        ///-------------------------------------------------------------------------------------------------

		public static DMEnum<T> MenuLine<T>(string title, Func<T> getter, Action<T> setter = null, string shortcut = null, string help = null) where T : struct, Enum
		{
			return new DMEnum<T>(title, getter, setter, shortcut, help);
		}

        ///-------------------------------------------------------------------------------------------------
        /// <summary>   Adds an enum flags. </summary>
        ///
        /// <remarks>   Valery,. </remarks>
        ///
        /// <typeparam name="T">    Generic type parameter. </typeparam>
        /// <param name="map">      The key map where to add this line. </param>
        /// <param name="path">     A menu keystroke. </param>
        /// <param name="title">    The title. </param>
        /// <param name="getter">   The getter. </param>
        /// <param name="setter">   (Optional) The setter. </param>
        /// <param name="shortcut"> (Optional) The shortcut. </param>
        /// <param name="help">     (Optional) The help tex for menu map. </param>
        ///-------------------------------------------------------------------------------------------------

		public static void DefineEnumFlags<T>(KeyMap map,string path, string title, Func<T> getter, Action<T> setter = null, string shortcut = null, string help = null) where T : struct, Enum
		{
            if (path == null) path = Humanizer.Decamelize(title);
            var type = typeof(T);
			if (type.IsDefined(typeof(FlagsAttribute), false))
			{
				var values = (T[])Enum.GetValues(type);
				for (var i = 0; i < values.Length; i++)
				{
					var value = values[i];
                    Func<bool> fgetter = () =>
                    {
                        var intGetter = (int)(object)getter.Invoke();
                        var intValue = (int)(object)value;
                        return (intGetter & intValue) != 0;
                    };
                    Action<bool> fsetter = (bool v) =>
                    {
                        var intGetter = (int)(object)getter.Invoke();
                        var intValue = (int)(object)value;
                        setter.Invoke((T)(object)(intGetter ^ intValue));
                    };
                    var seq = KBD.ParsePseudo(path.Split("/"));
                    map.Define(path, MenuLine(title + " " + value.ToString(), fgetter, fsetter, shortcut, help));
				}
			} else
            {
				Debug.LogError("The DefineEnumFlags expects a Flags enum");
            }
		}

        ///-------------------------------------------------------------------------------------------------
        /// <summary>   Adds keystroke. </summary>
        ///
        /// <remarks>   Valery,. </remarks>
        ///
        /// <param name="title">    The title. </param>
        /// <param name="getter">   The getter. </param>
        /// <param name="setter">   (Optional) The setter. </param>
        /// <param name="shortcut"> (Optional) The shortcut. </param>
        /// <param name="help">     (Optional) The help tex for menu map. </param>
        ///
        /// <returns>   A DMString. </returns>
        ///
        /// ### <param name="path"> A menu keystroke. </param>
        ///-------------------------------------------------------------------------------------------------

		public static DMUInt8 MenuLine(string title, Func<byte> getter, Action<byte> setter = null, string shortcut = null, string help = null)
		{
			return new DMUInt8(title, getter, setter, shortcut, help);
		}

        ///-------------------------------------------------------------------------------------------------
        /// <summary>   Adds keystroke. </summary>
        ///
        /// <remarks>   Valery,. </remarks>
        ///
        /// <param name="title">    The title. </param>
        /// <param name="getter">   The getter. </param>
        /// <param name="setter">   (Optional) The setter. </param>
        /// <param name="shortcut"> (Optional) The shortcut. </param>
        /// <param name="help">     (Optional) The help tex for menu map. </param>
        ///
        /// <returns>   A DMString. </returns>
        ///
        /// ### <param name="path"> A menu keystroke. </param>
        ///-------------------------------------------------------------------------------------------------

		public static DMUInt16 MenuLine(string title, Func<UInt16> getter, Action<UInt16> setter = null, string shortcut = null, string help = null)
		{
			return new DMUInt16(title, getter, setter, shortcut, help);
		}

        ///-------------------------------------------------------------------------------------------------
        /// <summary>   Adds keystroke. </summary>
        ///
        ///
        /// <param name="path">     A menu keystroke. </param>
        /// <param name="getter">   The getter. </param>
        /// <param name="setter">   (Optional) The setter. </param>
        /// <param name="shortcut"> (Optional) The shortcut. </param>
        /// <param name="help">     (Optional) The help tex for menu map. </param>
        ///
        /// <returns>   A DMString. </returns>
        ///-------------------------------------------------------------------------------------------------

		public static DMUInt32 MenuLine(string title, Func<UInt32> getter, Action<UInt32> setter = null, string shortcut = null, string help = null)
        {
			return new DMUInt32(title, getter, setter, shortcut, help);
		}

        ///-------------------------------------------------------------------------------------------------
        /// <summary>   Adds keystroke. </summary>
        ///
        /// <remarks>   Valery,. </remarks>
        ///
        /// <param name="title">    The title. </param>
        /// <param name="getter">   The getter. </param>
        /// <param name="setter">   (Optional) The setter. </param>
        /// <param name="shortcut"> (Optional) The shortcut. </param>
        /// <param name="help">     (Optional) The help tex for menu map. </param>
        ///
        /// <returns>   A DMString. </returns>
        ///
        /// ### <param name="path"> A menu keystroke. </param>
        ///-------------------------------------------------------------------------------------------------

		public static DMUInt64 MenuLine(string title, Func<UInt64> getter, Action<UInt64> setter = null, string shortcut = null, string help = null)
        {
			return new DMUInt64(title, getter, setter, shortcut, help);
		}

        ///-------------------------------------------------------------------------------------------------
        /// <summary>   Adds keystroke. </summary>
        ///
        /// <remarks>   Valery,. </remarks>
        ///
        /// <param name="title">    The title. </param>
        /// <param name="getter">   The getter. </param>
        /// <param name="setter">   (Optional) The setter. </param>
        /// <param name="shortcut"> (Optional) The shortcut. </param>
        /// <param name="help">     (Optional) The help tex for menu map. </param>
        ///
        /// <returns>   A DMString. </returns>
        ///
        /// ### <param name="path"> A menu keystroke. </param>
        ///-------------------------------------------------------------------------------------------------

		public static DMInt8 MenuLine(string title, Func<sbyte> getter, Action<sbyte> setter = null, string shortcut = null, string help = null)
		{
			return new DMInt8(title, getter, setter, shortcut, help);
		}

        ///-------------------------------------------------------------------------------------------------
        /// <summary>   Adds keystroke. </summary>
        ///
        /// <remarks>   Valery,. </remarks>
        ///
        /// <param name="title">    The title. </param>
        /// <param name="getter">   The getter. </param>
        /// <param name="setter">   (Optional) The setter. </param>
        /// <param name="shortcut"> (Optional) The shortcut. </param>
        /// <param name="help">     (Optional) The help tex for menu map. </param>
        ///
        /// <returns>   A DMString. </returns>
        ///
        /// ### <param name="path"> A menu keystroke. </param>
        ///-------------------------------------------------------------------------------------------------

		public static DMInt16 MenuLine(string title, Func<Int16> getter, Action<Int16> setter = null, string shortcut = null, string help = null)
		{
			return new DMInt16(title, getter, setter, shortcut, help);
		}

        ///-------------------------------------------------------------------------------------------------
        /// <summary>   Adds keystroke. </summary>
        ///
        /// <remarks>   Valery,. </remarks>
        ///
        /// <param name="title">    The title. </param>
        /// <param name="getter">   The getter. </param>
        /// <param name="setter">   (Optional) The setter. </param>
        /// <param name="shortcut"> (Optional) The shortcut. </param>
        /// <param name="help">     (Optional) The help tex for menu map. </param>
        ///
        /// <returns>   A DMString. </returns>
        ///
        /// ### <param name="path"> A menu keystroke. </param>
        ///-------------------------------------------------------------------------------------------------

		public static DMInt32 MenuLine(string title, Func<Int32> getter, Action<Int32> setter = null, string shortcut = null, string help = null)
        {
			return new DMInt32(title, getter, setter, shortcut, help);
		}

        ///-------------------------------------------------------------------------------------------------
        /// <summary>   Adds keystroke. </summary>
        ///
        /// <remarks>   Valery,. </remarks>
        ///
        /// <param name="title">    The title. </param>
        /// <param name="getter">   The getter. </param>
        /// <param name="setter">   (Optional) The setter. </param>
        /// <param name="shortcut"> (Optional) The shortcut. </param>
        /// <param name="help">     (Optional) The help tex for menu map. </param>
        ///
        /// <returns>   A DMString. </returns>
        ///
        /// ### <param name="path"> A menu keystroke. </param>
        ///-------------------------------------------------------------------------------------------------

		public static DMInt64 MenuLine(string title, Func<Int64> getter, Action<Int64> setter = null, string shortcut = null, string help = null) 		
		{
			return new DMInt64(title, getter, setter, shortcut, help);
		}

        ///-------------------------------------------------------------------------------------------------
        /// <summary>   Adds keystroke. </summary>
        ///
        /// <remarks>   Valery,. </remarks>
        ///
        /// <param name="title">    The title. </param>
        /// <param name="getter">   The getter. </param>
        /// <param name="setter">   (Optional) The setter. </param>
        /// <param name="shortcut"> (Optional) The shortcut. </param>
        /// <param name="help">     (Optional) The help tex for menu map. </param>
        ///
        /// <returns>   A DMString. </returns>
        ///
        /// ### <param name="path"> A menu keystroke. </param>
        ///-------------------------------------------------------------------------------------------------

		public static DMVector2 MenuLine(string title, Func<Vector2> getter, Action<Vector2> setter = null, string shortcut = null, string help = null)
        {
			return new DMVector2(title, getter, setter, shortcut, help);
		}

        ///-------------------------------------------------------------------------------------------------
        /// <summary>   Adds keystroke. </summary>
        ///
        /// <remarks>   Valery,. </remarks>
        ///
        /// <param name="title">    The title. </param>
        /// <param name="getter">   The getter. </param>
        /// <param name="setter">   (Optional) The setter. </param>
        /// <param name="shortcut"> (Optional) The shortcut. </param>
        /// <param name="help">     (Optional) The help tex for menu map. </param>
        ///
        /// <returns>   A DMString. </returns>
        ///
        /// ### <param name="path"> A menu keystroke. </param>
        ///-------------------------------------------------------------------------------------------------

		public static DMVector3 MenuLine(string title, Func<Vector3> getter, Action<Vector3> setter = null, string shortcut = null, string help = null)
		{
			return new DMVector3(title, getter, setter, shortcut, help);
		}

        ///-------------------------------------------------------------------------------------------------
        /// <summary>   Adds keystroke. </summary>
        ///
        /// <remarks>   Valery,. </remarks>
        ///
        /// <param name="title">    The title. </param>
        /// <param name="getter">   The getter. </param>
        /// <param name="setter">   (Optional) The setter. </param>
        /// <param name="shortcut"> (Optional) The shortcut. </param>
        /// <param name="help">     (Optional) The help tex for menu map. </param>
        ///
        /// <returns>   A DMString. </returns>
        ///
        /// ### <param name="path"> A menu keystroke. </param>
        ///-------------------------------------------------------------------------------------------------

		public static DMVector4 MenuLine(string title, Func<Vector4> getter, Action<Vector4> setter = null, string shortcut = null, string help = null)
		{
			return new DMVector4(title, getter, setter, shortcut, help);
		}

        ///-------------------------------------------------------------------------------------------------
        /// <summary>   Adds keystroke. </summary>
        ///
        /// <remarks>   Valery,. </remarks>
        ///
        /// <param name="title">    The title. </param>
        /// <param name="getter">   The getter. </param>
        /// <param name="setter">   (Optional) The setter. </param>
        /// <param name="shortcut"> (Optional) The shortcut. </param>
        /// <param name="help">     (Optional) The help tex for menu map. </param>
        ///
        /// <returns>   A DMString. </returns>
        ///
        /// ### <param name="path"> A menu keystroke. </param>
        ///-------------------------------------------------------------------------------------------------

		public static DMQuaternion MenuLine(string title, Func<Quaternion> getter, Action<Quaternion> setter = null, string shortcut = null, string help = null)
		{
			return new DMQuaternion(title, getter, setter, shortcut, help);
		}

        ///-------------------------------------------------------------------------------------------------
        /// <summary>   Adds keystroke. </summary>
        ///
        /// <remarks>   Valery,. </remarks>
        ///
        /// <param name="title">    The title. </param>
        /// <param name="getter">   The getter. </param>
        /// <param name="setter">   (Optional) The setter. </param>
        /// <param name="shortcut"> (Optional) The shortcut. </param>
        /// <param name="help">     (Optional) The help tex for menu map. </param>
        ///
        /// <returns>   A DMString. </returns>
        ///
        /// ### <param name="path"> A menu keystroke. </param>
        ///-------------------------------------------------------------------------------------------------

		public static DMColor MenuLine(string title, Func<Color> getter, Action<Color> setter = null, string shortcut = null, string help = null)
		{
			return new DMColor(title, getter, setter, shortcut, help);
		}

        ///-------------------------------------------------------------------------------------------------
        /// <summary>   Adds keystroke. </summary>
        ///
        /// <remarks>   Valery,. </remarks>
        ///
        /// <param name="title">    The title. </param>
        /// <param name="getter">   The getter. </param>
        /// <param name="setter">   (Optional) The setter. </param>
        /// <param name="shortcut"> (Optional) The shortcut. </param>
        /// <param name="help">     (Optional) The help tex for menu map. </param>
        ///
        /// <returns>   A DMString. </returns>
        ///
        /// ### <param name="path"> A menu keystroke. </param>
        ///-------------------------------------------------------------------------------------------------

		public static DMVector2Int MenuLine(string title, Func<Vector2Int> getter, Action<Vector2Int> setter = null, string shortcut = null, string help = null)
		{
			return new DMVector2Int(title, getter, setter, shortcut, help);
		}

        ///-------------------------------------------------------------------------------------------------
        /// <summary>   Adds keystroke. </summary>
        ///
        /// <remarks>   Valery,. </remarks>
        ///
        /// <param name="title">    The title. </param>
        /// <param name="getter">   The getter. </param>
        /// <param name="setter">   (Optional) The setter. </param>
        /// <param name="shortcut"> (Optional) The shortcut. </param>
        /// <param name="help">     (Optional) The help tex for menu map. </param>
        ///
        /// <returns>   A DMString. </returns>
        ///
        /// ### <param name="path"> A menu keystroke. </param>
        ///-------------------------------------------------------------------------------------------------

		public static DMVector3Int MenuLine(string title, Func<Vector3Int> getter, Action<Vector3Int> setter = null, string shortcut = null, string help = null)
		{
			return new DMVector3Int(title, getter, setter, shortcut, help);
		}

        ///-------------------------------------------------------------------------------------------------
        /// <summary>   Adds keystroke. </summary>
        ///
        /// <remarks>   Valery,. </remarks>
        ///
        /// <param name="title">    The title. </param>
        /// <param name="getter">   The getter. </param>
        /// <param name="setter">   (Optional) The setter. </param>
        /// <param name="shortcut"> (Optional) The shortcut. </param>
        /// <param name="help">     (Optional) The help tex for menu map. </param>
        ///
        /// <returns>   A DMString. </returns>
        ///
        /// ### <param name="path"> A menu keystroke. </param>
        ///-------------------------------------------------------------------------------------------------

		public static DMFloat MenuLine(string title, Func<float> getter, Action<float> setter = null, string shortcut = null, string help = null)
		{
			return new DMFloat(title, getter, setter, shortcut, help);
		}

        ///-------------------------------------------------------------------------------------------------
        /// <summary>   Produce the sequence which can  be binded to a key. </summary>
        ///
        /// <remarks>   Valery,. </remarks>
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
