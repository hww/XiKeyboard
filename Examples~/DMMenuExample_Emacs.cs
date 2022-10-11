/* Copyright (c) 2021 Valerya Pudova (hww) */

using XiKeyboard;
using UnityEngine;
using XiKeyboard.KeyMaps;
using XiKeyboard.Menu;

namespace XiKeyboard.Examples.Menu
{

	public class DMMenuExample_Emacs : MonoBehaviour
	{
		public delegate void Method();

		private string helpText = "USAGE\n"
			+ "SHIFT+F -- Open file menu\n"
			+ "SHIFT+S -- Save\n"
			+ "A,S,D,W -- Cursors\n"
			+ "Q       -- Quit menu\n"
			+ "E       -- Tollge menu\n"
			+ "R       -- Reset value\n";

		private string guiText = "";

		// Use this for initialization the menu system
		private void Start()
		{
			// - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - -
			// Define the menu tree
			// - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - -

			// Define menu as member of MenuBar
			// One way is to define menu from the MenuBar (see below)
			//   KeyMap.MenuBar.CreateMenu ("file", "File", "Help text");
			// The other way is to do it from global menu with full
			// path "menu-bar/file" (see below)
			var fileMenu = KeyMap.GlobalKeymap.CreateMenu("menu-bar/file", "File", "Help text");
			
			// Create save menu item (shortcut will be only displayed and can be omitted)
			// The method Save of this class will be bind to this menu item
			var menuItem1 = new MenuLineSimple("Save", (System.Action)Save, "S-s", "Save current file");
			
			// Define this item as member of File menu 
			fileMenu.AddMenuLine("save", menuItem1);
			
			// Save As menu line
			var menuItem2 = new MenuLineSimple("Save As", (System.Action)SaveAs, null, "Save current file as *");
			fileMenu.AddMenuLine("save-as", menuItem2);
			
			// Export menu line
			var menuItem3 = new MenuLineSimple("Export", (System.Action)Export, null, "Export current file as *");
			fileMenu.AddMenuLine("export", menuItem3);

			// Line separators
			fileMenu.AddMenuLine("-1", new MenuSeparator(MenuSeparator.Type.Space));
			fileMenu.AddMenuLine("-2", new MenuSeparator(MenuSeparator.Type.NoLine));
			fileMenu.AddMenuLine("-3", new MenuSeparator(MenuSeparator.Type.DashedLine));
			fileMenu.AddMenuLine("-4", new MenuSeparator(MenuSeparator.Type.SingleLine));

			// - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - -
			// All abow is creating the menu tree but did not define a 
			// shortcuts in the global kay bindings.
			// - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - -
			// To make S+F to open file menu
			KeyMap.GlobalKeymap.SetLocal("S-f", fileMenu);

			// Tomake S+S as the save shortcut 
			KeyMap.GlobalKeymap.SetLocal(menuItem1.Shorcut, menuItem1.binding);

			// To open menu uncomment next line
			// DM.Open(KeyMap.MenuBar);
		}


		void Update()
		{
			DM.Update();
		}

		void OnGUI()
		{
			Rect rect1 = new Rect(500, 10, 500, 200);
			GUI.Box (rect1, guiText);
			Rect rect2 = new Rect(500, 220, 500, 200);
			GUI.Box (rect2, helpText);
			DM.OnGUI();
		}

		void Save()
		{
			guiText = "File Saved\n";
		}

		void SaveAs()
		{
			guiText = "File Saved As\n";
		}

		void Export()
		{
			guiText = "File Exported\n";
		}

	}
}