/* Copyright (c) 2021 Valerya Pudova (hww) */

using XiKeyboard;
using UnityEngine;

namespace XiKeyboard.Examples.Menu
{

	public class DMMenuExample_Emacs : MonoBehaviour
	{
		public delegate void Method();

		// Use this for initialization the menu system
		private void Start()
		{

			// Define menu as member of MainMenu
			var fileMenu = KeyMap.GlobalKeymap.CreateMenu("file", "File", "Help for file menu");

			// Create save menu item (shortcut will be only displayed and can be omitted)
			// The method Save of this class will be bind to this menu item
			var menuItem1 = new DMMenuLineSimple("Save", (System.Action)Save, "S-s", "Save current file");
			// Define this item as member of File menu 
			fileMenu.AddMenuLine("save", menuItem1);
			// Save As menu line
			var menuItem2 = new DMMenuLineSimple("Save As", (System.Action)SaveAs, null, "Save current file as *");
			fileMenu.AddMenuLine("save-as", menuItem2);
			// Export menu line
			var menuItem3 = new DMMenuLineSimple("Export", (System.Action)Export, null, "Export current file as *");
			fileMenu.AddMenuLine("export", menuItem3);

			// Line separators
			fileMenu.AddMenuLine("-1", new DMMenuSeparator(DMMenuSeparator.Type.Space));
			fileMenu.AddMenuLine("-2", new DMMenuSeparator(DMMenuSeparator.Type.NoLine));
			fileMenu.AddMenuLine("-3", new DMMenuSeparator(DMMenuSeparator.Type.DashedLine));
			fileMenu.AddMenuLine("-4", new DMMenuSeparator(DMMenuSeparator.Type.SingleLine));

			// Now make shortcuts for menu options
			// Open file menu by C+F 
			KeyMap.GlobalKeymap.SetLocal("C-f", fileMenu);
			// Save file by C+S 
			KeyMap.GlobalKeymap.SetLocal(menuItem1.Shorcut, menuItem1.binding);

			DM.Open(fileMenu);
		}
		void Update()
		{
			DM.Update();
		}

		void OnGUI()
		{
			DM.OnGUI();
		}

		void Save()
		{
			Debug.Log("File Saved");
		}

		void SaveAs()
		{
			Debug.Log("File Saved As");
		}

		void Export()
		{
			Debug.Log("File Exported");
		}
	}
}