/* Copyright (c) 2021 Valerya Pudova (hww) */

using XiKeyboard;
using UnityEngine;
using XiKeyboard.KeyMaps;
using XiKeyboard.Menu;
using System.Collections;

namespace XiKeyboard.Examples.Menu
{

	public class DMMenuExample_Emacs : MonoBehaviour
	{
		public delegate void Method();

		private const string kHelpText = "Menu navigation\n"
                                       + "  A,S,D,W  Cursors\n"
                                       + "  Q        Quit menu\n"
                                       + "  E        Tollge menu\n"
                                       + "  R        Reset value\n"
									   + "Demo file methods\n"
									   + "  SHIFT+F  Open file menu\n"
									   + "  SHIFT+S  Save\n"
									   + "--------------------\n";

		private string miniBuffer = "";

		private GUISkin skin;

		void Awake()
		{
			skin = Resources.Load<GUISkin>("XiKeyboard/Skins/Default Skin");
			skin.box.normal.background = MakeTex(2, 2, new Color(0f, 0f, 0f, 0.7f));
		}

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
			var fileMenu = MenuMap.MenuBar.CreateMenu("menu-bar/file", "File", "Help text");
			
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


		IEnumerator UpdateMiniBuffer()
		{
			yield return new WaitForSeconds(3f);
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

		void Save()
		{
			miniBuffer = "File Saved\n";
			StopAllCoroutines();
			StartCoroutine(UpdateMiniBuffer());
		}

		void SaveAs()
		{
			miniBuffer = "File Saved As\n";
			StopAllCoroutines();
			StartCoroutine(UpdateMiniBuffer());
		}

		void Export()
		{
			miniBuffer = "File Exported\n";
			StopAllCoroutines();
			StartCoroutine(UpdateMiniBuffer());
		}

	}
}