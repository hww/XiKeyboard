/* Copyright (c) 2021 Valerya Pudova (hww) */

using XiKeyboard;
using UnityEngine;
using XiKeyboard.KeyMaps;
using XiKeyboard.Menu;
using System.Collections;
using XiKeyboard.Buffers;
using XiKeyboard.Assets.XiKeyboard.Code.Libs;

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
			skin.box.normal.background = TextureUtils.MakeTex(2, 2, new Color(0f, 0f, 0f, 0.7f));
		}


		private void Start()
		{
			// - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - -
			// DefineMenuLine the menu tree
			// - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - -

			// DefineMenuLine menu as member of MenuBar
			// One way is to define menu from the MenuBar (see below)
			//   KeyMap.MenuBar.EasyCreateMenu ("file", "File", "Help title");
			// The other way is to do it from global menu with full
			// path "menu-bar/file" (see below)
			var fileMenu = new MenuMap("File", "Help title");
			DM.MenuBar.DefineLine(null, fileMenu);

			// Create save menu item (shortcut will be only displayed and can be omitted)
			// The method Save of this class will be bind to this menu item
			var menuItem1 = new MenuLineSimple("Save", (System.Action)Save, "S-s", "Save current file");
			
			// DefineMenuLine this item as member of File menu 
			fileMenu.DefineLine("save", menuItem1);
			
			// Save As menu line
			var menuItem2 = new MenuLineSimple("Save As", (System.Action)SaveAs, "S-x S-s", "Save current file as *");
			fileMenu.DefineLine("save-as", menuItem2);
			
			// Export menu line
			var menuItem3 = new MenuLineSimple("Export", (System.Action)Export, "S-x S-e", "Export current file as *");
			fileMenu.DefineLine("export", menuItem3);

			// Line separators
			fileMenu.DefineLine("-1", new MenuSeparator(MenuSeparator.Type.Space));
			fileMenu.DefineLine("-2", new MenuSeparator(MenuSeparator.Type.NoLine));
			fileMenu.DefineLine("-3", new MenuSeparator(MenuSeparator.Type.DashedLine));
			fileMenu.DefineLine("-4", new MenuSeparator(MenuSeparator.Type.SingleLine));

			// - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - -
			// All abow is creating the menu tree but did not define a 
			// shortcuts in the global kay bindings.
			// - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - -
			// To make S+F to open file menu
			KeyMap.GlobalKeymap.SetLocal("S-f", fileMenu);

			// Tomake S+S as the save shortcut 
			KeyMap.GlobalKeymap.SetLocal(menuItem1.Shorcut, menuItem1.binding);
			KeyMap.GlobalKeymap.Define(menuItem2.Shorcut, menuItem2.binding);
			KeyMap.GlobalKeymap.Define(menuItem3.Shorcut, menuItem3.binding);

			// - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - -
			// Just to display the message 
			// - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - -

			Buffer.OnSequenceProgress.Add(OnSequencePressed);// On press part of sequence delegate
			Buffer.OnKeyPressed.Add(OnKeyPressed);            // On press key delegate

			// To open menu uncomment next line
			// DM.Open(KeyMap.MenuBar);
		}
		void OnSequencePressed(Buffer buffer, DMKeyMapItem item)
        {
			miniBuffer = buffer.GetBufferHumanizedString();
			StopAllCoroutines();
			StartCoroutine(UpdateMiniBuffer());
		}
		void OnKeyPressed(Buffer buffer, KeyEvent item)
		{
			miniBuffer = buffer.GetBufferHumanizedString();
			StopAllCoroutines();
			StartCoroutine(UpdateMiniBuffer());
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