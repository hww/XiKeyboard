using System;
using System.Collections.Generic;
using UnityEngine;
using XiKeyboard.Notifications;

namespace XiKeyboard
{
    public static class DM
	{


		#region Static Public Vars


		// Container
		public static readonly DMContainer Container = new DMContainer("Debug Menu");

		public static DMBranch Root => Container.Root;

		public static bool IsVisible => Container.IsVisible;

		public static IDMRender Render
		{
			get => Container.Render;
			set => Container.Render = value;
		}

		// Notice
		public static IDMNotice Notice = new DMDefaultNotice();

		#endregion

		#region Public Methods

		static DM()
		{

		}

		public static void Open() => Container.Open();

		public static void Open(KeyMap branch) => Container.Open(branch);

		public static void Back() => Container.Back();

		public static void Notify(DMMenuLine item, Color? nameColor = null, Color? valueColor = null) => Notice?.Notify(item, nameColor, valueColor);

		// Branch
		public static KeyMap Add(string path, string description = "", int order = 0) =>
			Container.Add(path, description, order);

		// String
		public static DMString Add(string path, Func<string> getter, int order = 0) =>
			Container.Add(path, getter, order);

		// Action
		//public static DMAction Add(string path, Action<EventTag> action, string description = "", int order = 0) =>
		//	Container.Add(path, action, description, order);

		// Bool
		public static DMBool Add(string path, Func<bool> getter, Action<bool> setter = null, int order = 0) =>
			Container.Add(path, getter, setter, order);

		// Enum
		public static DMEnum<T> Add<T>(string path, Func<T> getter, Action<T> setter = null, int order = 0) where T : struct, Enum =>
			Container.Add(path, getter, setter, order);

		// UInt8
		public static DMUInt8 Add(string path, Func<byte> getter, Action<byte> setter = null, int order = 0) =>
			Container.Add(path, getter, setter, order);

		// UInt16
		public static DMUInt16 Add(string path, Func<UInt16> getter, Action<UInt16> setter = null, int order = 0) =>
			Container.Add(path, getter, setter, order);

		// UInt32
		public static DMUInt32 Add(string path, Func<UInt32> getter, Action<UInt32> setter = null, int order = 0) =>
			Container.Add(path, getter, setter, order);

		// UInt64
		public static DMUInt64 Add(string path, Func<UInt64> getter, Action<UInt64> setter = null, int order = 0) =>
			Container.Add(path, getter, setter, order);

		// Int8
		public static DMInt8 Add(string path, Func<sbyte> getter, Action<sbyte> setter = null, int order = 0) =>
			Container.Add(path, getter, setter, order);

		// Int16
		public static DMInt16 Add(string path, Func<Int16> getter, Action<Int16> setter = null, int order = 0) =>
			Container.Add(path, getter, setter, order);

		// Int32
		public static DMInt32 Add(string path, Func<Int32> getter, Action<Int32> setter = null, int order = 0) =>
			Container.Add(path, getter, setter, order);

		// Int64
		public static DMInt64 Add(string path, Func<Int64> getter, Action<Int64> setter = null, int order = 0) =>
			Container.Add(path, getter, setter, order);

		// Float
		//public static DMFloat Add(string path, Func<float> getter, Action<float> setter = null, int order = 0) =>
		//	Container.Add(path, getter, setter, order);
		//
		//// Vector 2
		//public static DMVector2 Add(string path, Func<Vector2> getter, Action<Vector2> setter = null, int order = 0) =>
		//	Container.Add(path, getter, setter, order);
		//
		//// Vector 3
		//public static DMVector3 Add(string path, Func<Vector3> getter, Action<Vector3> setter = null, int order = 0) =>
		//	Container.Add(path, getter, setter, order);
		//
		//// Vector 4
		//public static DMVector4 Add(string path, Func<Vector4> getter, Action<Vector4> setter = null, int order = 0) =>
		//	Container.Add(path, getter, setter, order);
		//
		//// Quaternion
		//public static DMQuaternion Add(string path, Func<Quaternion> getter, Action<Quaternion> setter = null, int order = 0) =>
		//	Container.Add(path, getter, setter, order);
		//
		//// Color
		//public static DMColor Add(string path, Func<Color> getter, Action<Color> setter = null, int order = 0) =>
		//	Container.Add(path, getter, setter, order);

		// Vector 2 Int
		//public static DMVector2Int Add(string path, Func<Vector2Int> getter, Action<Vector2Int> setter = null, int order = 0) =>
		//	Container.Add(path, getter, setter, order);
		//
		//// Vector 3 Int
		//public static DMVector3Int Add(string path, Func<Vector3Int> getter, Action<Vector3Int> setter = null, int order = 0) =>
		//	Container.Add(path, getter, setter, order);

		// Dynamic
		//public static DMBranch Add<T>(string path, Func<IEnumerable<T>> getter, Action<DMBranch, T> buildCallback = null, Func<T, string> nameCallback = null, string description = "", int order = 0) =>
		//	Container.Add(path, getter, buildCallback, nameCallback, description, order);

		#endregion

		#region Private Methods

		private static void Update() => Container.Update();

		private static void OnGUI() => Container.OnGUI();

		#endregion
	}
}
