// =============================================================================
// MIT License
//
// Copyright (c) [2018] [Valeriya Pudova]
//
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:
//
// The above copyright notice and this permission notice shall be included in all
// copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
// SOFTWARE.
// =============================================================================


using XiKeyboard;
using UnityEngine;
using Event = XiKeyboard.KeyEvent;
using Type = System.Type;

public class MenuDemoC : MonoBehaviour
{
	public delegate void Method();
	private DMRender renderer;

	// To initialize the renderer
	private void OnEnable()
    {
		renderer = new DMRender();
	}

    // Use this for initialization the menu system
    void Start ()
	{
		
		// Define menu as member of MainMenu
		var fileMenu = KeyMap.GlobalKeymap.CreateMenu("main-menu/file", "File", "Help for file menu" );
		
		// Create save menu item (shortcut will be only displayed and can be omitted)
		// The method Save of this class will be bind to this menu item
		var menuItem1 = new DMMenuLineSimple("Save", (Method) Save, "C-s", "Save current file");
		// Define this item as member of File menu 
		fileMenu.AddMenuLine("save", menuItem1 );
		// Save As menu line
		var menuItem2 = new DMMenuLineSimple("Save As", (Method) SaveAs, null, "Save current file as *");
		fileMenu.AddMenuLine("save-as", menuItem2 );
		// Export menu line
		var menuItem3 = new DMMenuLineSimple("Export", (Method) Export, null, "Export current file as *");
		fileMenu.AddMenuLine("export", menuItem3 );

		// Line separators
		fileMenu.AddMenuLine("-1", new DMMenuSeparator(DMMenuSeparator.Type.Space) );
		fileMenu.AddMenuLine("-2", new DMMenuSeparator(DMMenuSeparator.Type.NoLine) );
		fileMenu.AddMenuLine("-3", new DMMenuSeparator(DMMenuSeparator.Type.DashedLine) );
		fileMenu.AddMenuLine("-4", new DMMenuSeparator(DMMenuSeparator.Type.SingleLine) );

		// Now make shortcuts for menu options
		// Open file menu by C+F 
		KeyMap.GlobalKeymap.SetLocal("C-f", fileMenu);                  
		// Save file by C+S 
		KeyMap.GlobalKeymap.SetLocal(menuItem1.Shorcut, menuItem1.binding);

		(renderer as IDMRender).RenderMenu(fileMenu, 0);
	}
	void Update()
	{
		(renderer as IDMRender_Update).Update();
	}

	void OnGUI()
	{
		(renderer as IDMRender_OnGUI).OnGUI();
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
