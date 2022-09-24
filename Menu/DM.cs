/* Copyright (c) 2021 Valerya Pudova (hww) */

using System;
using UnityEngine;

namespace XiKeyboard
{
	/// <summary>
	/// The interface to menu and keyboard system
	/// </summary>
	public static class DM
	{

		static readonly DMController controller = new DMController();

		#region Static Public Vars

		public static KeyMap Global => KeyMap.GlobalKeymap;
		public static DMMenuPanelRepresentation Root => controller.Root;
		public static DMMenuPanelRepresentation Current => controller.Current;

		public static bool IsVisible => controller.IsVisible;

		#endregion

		#region Public Methods

		static DM()
		{

		}

		public static void Initialize()
		{

		}

		public static void Open() => controller.Open();

		public static void Open(KeyMap branch) => controller.Open(branch);

		public static void Close() => controller.Close();

		private static string GetName(string path)
		{
			var idx = path.LastIndexOf('/');
			return (idx >= 0) ? path.Substring(idx + 1) : path;
		}
		private static string GetPath(string path)
		{
			var idx = path.LastIndexOf('/');
			return (idx >= 0) ? path.Substring(0, idx) : null;
		}

		// Branch
		public static KeyMap CreateMenu(string path, string title, string help) =>
			KeyMap.GlobalKeymap.CreateMenu(path, title, help);

		// Action
		//public static DMAction Add(string path, Action<DMEvent> action, string description = "", int order = 0) =>
		//	Container.Add(path, action, description, order);

		public static DMString Add(string path, Func<string> getter, string shortcut = null, string help = null)
		{
			var val = new DMString(GetName(path), getter, shortcut, help);
			Global.DefineMenuLine(path, val);
			return val;
		}

		public static DMBool Add(string path, Func<bool> getter, Action<bool> setter = null, string shortcut = null, string help = null)
		{
			var val = new DMBool(GetName(path), getter, setter, shortcut, help);
			Global.DefineMenuLine(path, val);
			return val;
		}

		public static DMEnum<T> Add<T>(string path, Func<T> getter, Action<T> setter = null, string shortcut = null, string help = null) where T : struct, Enum
		{
			var val = new DMEnum<T>(GetName(path), getter, setter, shortcut, help);
			Global.DefineMenuLine(path, val);
			return val;
		}

		public static DMUInt8 Add(string path, Func<byte> getter, Action<byte> setter = null, string shortcut = null, string help = null)
		{
			var val = new DMUInt8(GetName(path), getter, setter, shortcut, help);
			Global.DefineMenuLine(path, val);
			return val;
		}

		public static DMUInt16 Add(string path, Func<UInt16> getter, Action<UInt16> setter = null, string shortcut = null, string help = null)
		{
			var val = new DMUInt16(GetName(path), getter, setter, shortcut, help);
			Global.DefineMenuLine(path, val);
			return val;
		}

		public static DMUInt32 Add(string path, Func<UInt32> getter, Action<UInt32> setter = null, string shortcut = null, string help = null)
        {
			var val = new DMUInt32(GetName(path), getter, setter, shortcut, help);
			Global.DefineMenuLine(path, val);
			return val;
		}

		public static DMUInt64 Add(string path, Func<UInt64> getter, Action<UInt64> setter = null, string shortcut = null, string help = null)
        {
			var val = new DMUInt64(GetName(path), getter, setter, shortcut, help);
			Global.DefineMenuLine(path, val);
			return val;
		}

		public static DMInt8 Add(string path, Func<sbyte> getter, Action<sbyte> setter = null, string shortcut = null, string help = null)
		{
			var val = new DMInt8(GetName(path), getter, setter, shortcut, help);
			Global.DefineMenuLine(path, val);
			return val;
		}

		public static DMInt16 Add(string path, Func<Int16> getter, Action<Int16> setter = null, string shortcut = null, string help = null)
		{
			var val = new DMInt16(GetName(path), getter, setter, shortcut, help);
			Global.DefineMenuLine(path, val);
			return val;
		}

		public static DMInt32 Add(string path, Func<Int32> getter, Action<Int32> setter = null, string shortcut = null, string help = null)
        {
			var val = new DMInt32(GetName(path), getter, setter, shortcut, help);
			Global.DefineMenuLine(path, val);
			return val;
		}

		public static DMInt64 Add(string path, Func<Int64> getter, Action<Int64> setter = null, string shortcut = null, string help = null) 		
		{
			var val = new DMInt64(GetName(path), getter, setter, shortcut, help);
			Global.DefineMenuLine(path, val);
			return val;
		}

		public static DMFloat Add(string path, Func<float> getter, Action<float> setter = null, string shortcut = null, string help = null)
        {
			var val = new DMFloat(GetName(path), getter, setter, shortcut, help);
			Global.DefineMenuLine(path, val);
			return val;
		}


		//public static DMVector2 Add(string path, Func<Vector2> getter, Action<Vector2> setter = null, int order = 0) =>
		//	Container.Add(GetName(path), getter, setter, shortcut, help);
		//
		//// Vector 3
		//public static DMVector3 Add(string path, Func<Vector3> getter, Action<Vector3> setter = null, int order = 0) =>
		//	Container.Add(GetName(path), getter, setter, shortcut, help);
		//
		//// Vector 4
		//public static DMVector4 Add(string path, Func<Vector4> getter, Action<Vector4> setter = null, int order = 0) =>
		//	Container.Add(GetName(path), getter, setter, shortcut, help);
		//
		//// Quaternion
		//public static DMQuaternion Add(string path, Func<Quaternion> getter, Action<Quaternion> setter = null, int order = 0) =>
		//	Container.Add(GetName(path), getter, setter, shortcut, help);
		//
		//// Color
		//public static DMColor Add(string path, Func<Color> getter, Action<Color> setter = null, int order = 0) =>
		//	Container.Add(GetName(path), getter, setter, shortcut, help);

		// Vector 2 Int
		//public static DMVector2Int Add(string path, Func<Vector2Int> getter, Action<Vector2Int> setter = null, int order = 0) =>
		//	Container.Add(GetName(path), getter, setter, shortcut, help);
		//
		//// Vector 3 Int
		//public static DMVector3Int Add(string path, Func<Vector3Int> getter, Action<Vector3Int> setter = null, int order = 0) =>
		//	Container.Add(GetName(path), getter, setter, shortcut, help);

		// Dynamic
		//public static DMBranch Add<T>(string path, Func<IEnumerable<T>> getter, Action<DMBranch, T> buildCallback = null, Func<T, string> nameCallback = null, string description = "", int order = 0) =>
		//	Container.Add(GetName(path), getter, buildCallback, nameCallback, description, order);

		// Produce the sequence which can  be binded to a key
		public static DMSequenceBinding MakeSequenceBinding(string name, int[] sequence, string help = null) =>
            new DMSequenceBinding(name, sequence, help);

        #endregion


        public static void Update()
		{
			(controller as IDMRender_Update).Update();
		}

		public static void OnGUI()
        {
			if (IsVisible)
				(controller as IDMRender_OnGUI).OnGUI();
		}


	}
}
