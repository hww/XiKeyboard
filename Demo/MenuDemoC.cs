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

using TMPro;
using VARP.Keyboard;
using UnityEngine;
using Event = VARP.Keyboard.Event;

public class MenuDemoC : MonoBehaviour
{

	public delegate void Method();

	public TextMeshProUGUI textMeshPro;
	
	// Use this for initialization
	void Start ()
	{
		Buffer.OnSequencePressed.Add(OnSequencePressed);                          	// On press sequence delegate
		Buffer.OnKeyPressed.Add(OnKeyPressed);                                    	// On press key delegate
		Buffer.OnPseudoPressed.Add(OnPsedoPressed);								    // On keymap was selected
		
		// Define menu as member of MainMenu
		var fileMenu = KeyMap.GlobalKeymap.DefineMenu("main-menu/file", "File", "Help for file menu" );
		// Open file menu by C+F 
		KeyMap.GlobalKeymap.SetLocal("C-f", fileMenu);                  
		// Create save menu item (shortcut will be only displayed and can be omitted)
		// The method Save of this class will be bind to this menu item
		var menuItem = new MenuLineSimple("Save", (Method) Save, "C-s", "Save current file");
		// Define this item as member of File menu 
		KeyMap.GlobalKeymap.DefineMenuLine("main-menu/file/save", menuItem );
		// Save file by C+S 
		KeyMap.GlobalKeymap.SetLocal(menuItem.Shorcut, menuItem.binding);                  
		// Save As menu line
		var menuItem1 = new MenuLineSimple("Save As", (Method) Save, null, "Save current file as *");
		KeyMap.GlobalKeymap.DefineMenuLine("main-menu/file/save-as", menuItem1 );
		// Export menu line
		var menuItem2 = new MenuLineSimple("Export", (Method) Save, null, "Export current file as *");
		KeyMap.GlobalKeymap.DefineMenuLine("main-menu/file/export", menuItem2 );
		// Line separators
		KeyMap.GlobalKeymap.DefineMenuLine("main-menu/file/-1", new MenuSeparator(MenuSeparator.Type.Space) );
		KeyMap.GlobalKeymap.DefineMenuLine("main-menu/file/-2", new MenuSeparator(MenuSeparator.Type.NoLine) );
		KeyMap.GlobalKeymap.DefineMenuLine("main-menu/file/-3", new MenuSeparator(MenuSeparator.Type.DashedLine) );
		KeyMap.GlobalKeymap.DefineMenuLine("main-menu/file/-4", new MenuSeparator(MenuSeparator.Type.SingleLine) );
	}

	void OnSequencePressed(Buffer buffer, KeyMapItem item) {
		if (item.value is Method)
			(item.value as Method).Invoke();
		else if (item.value is KeyMap)
			OnPsedoPressed(buffer, item);
		else
			Debug.Log("{" + item.value + "}");	  // Print "Pressed Sequence: N" 	
	}
	void OnKeyPressed(Buffer buffer, Event evt) {
		Debug.Log(buffer.GetBufferHumanizedString()); // Just display current buffer content		
	}

	void OnPsedoPressed(Buffer buffer, KeyMapItem item)
	{
		Debug.Log("{menu:" + item.value + "}");
		textMeshPro.text = MenuTextRenderer.RenderMenu(item.value as KeyMap, 0);
	}
	
	void Save()
	{
		Debug.Log("File Saved");
	}

}
