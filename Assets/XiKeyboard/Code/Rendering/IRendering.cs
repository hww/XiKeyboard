/* Copyright (c) 2021 dr. ext (Vladimir Sigalkin) */


namespace XiKeyboard.Rendering
{
    ///-------------------------------------------------------------------------------------------------
    /// <summary>   Values that represent menu render options. </summary>
    ///
    ///-------------------------------------------------------------------------------------------------

	public enum MenuRenderOptions
	{
		Default
	}

    ///-------------------------------------------------------------------------------------------------
    /// <summary>   Interface for menu render. </summary>
    ///
    ///-------------------------------------------------------------------------------------------------

	public interface IMenuRender
	{
		#region Methods

        ///-------------------------------------------------------------------------------------------------
        /// <summary>   Debug Menu Hooks. </summary>
        ///
        /// <param name="menu">     The menu. </param>
        /// <param name="options">  (Optional) Options for controlling the operation. </param>
        ///-------------------------------------------------------------------------------------------------

		void RenderMenu(MenuPanelRepresentation menu, MenuRenderOptions options = MenuRenderOptions.Default);

		#endregion
	}

    ///-------------------------------------------------------------------------------------------------
    /// <summary>   Interface for menu render on graphical user interface. </summary>
    ///
    ///-------------------------------------------------------------------------------------------------

	public interface IMenuRender_OnGUI
	{
		#region Methods

        /// <summary>   Executes the 'graphical user interface' action. </summary>
		void OnGUI();

		#endregion
	}

    ///-------------------------------------------------------------------------------------------------
    /// <summary>   Interface for menu render update. </summary>
    ///
    ///-------------------------------------------------------------------------------------------------

	public interface IMenuRender_Update
	{
		#region Methods

        /// <summary>   Updates this object. </summary>
		void Update();

		#endregion
	}
}
