/* Copyright (c) 2021 dr. ext (Vladimir Sigalkin) */


namespace XiKeyboard.Rendering
{
	public enum MenuRenderOptions
	{
		Default
	}

	public interface IMenuRender
	{
		#region Methods

		// Debug Menu Hooks
		void RenderMenu(IMenuController controller, MenuPanelRepresentation menu, MenuRenderOptions options = MenuRenderOptions.Default);

		#endregion
	}

	public interface IMenuRender_OnGUI
	{
		#region Methods

		void OnGUI();

		#endregion
	}

	public interface IMenuRender_Update
	{
		#region Methods

		void Update();

		#endregion
	}
}
