using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace XiKeyboard
{
	public enum DMMenuOptions
	{
		Default
	}

	public interface IDMRender
	{
		#region Methods

		// Debug Menu Hooks
		void RenderMenu(DMRenderItem menu, DMMenuOptions options = DMMenuOptions.Default);

		#endregion
	}

	public interface IDMRender_OnGUI
	{
		#region Methods

		void OnGUI();

		#endregion
	}

	public interface IDMRender_Update
	{
		#region Methods

		void Update();

		#endregion
	}
}
