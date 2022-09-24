/* Copyright (c) 2021 Valerya Pudova (hww) */

using XiKeyboard;
using UnityEngine;
using KeyEvent = XiKeyboard.KeyEvent;

public class KeyboardDemoC : MonoBehaviour {

	// Use this for initialization
	void Start ()
	{
		KeyMap.GlobalKeymap.Define("S-1", "Pressed: S-1");         	// Define keystroke S-1 with text binding "1"
		KeyMap.GlobalKeymap.Define("S-2 S-3", "Pressed: S-2 S-3");	// Define keystroke S-2 S-3 with text binding "2"
		KeyBuffer.OnSequencePressed.Add(OnSequencePressed);         // On press sequence delegate
		KeyBuffer.OnKeyPressed.Add(OnKeyPressed);                   // On press key delegate
	}
	
	void OnSequencePressed(KeyBuffer buffer, DMKeyMapItem item) {
		Debug.Log("{" + item.value + "}");	          // Print "Pressed Sequence: N" 	
	}
	void OnKeyPressed(KeyBuffer buffer, KeyEvent evt) {
		Debug.Log(buffer.GetBufferHumanizedString()); // Just display current buffer content		
	}

	void OnGUI()
    {
		KeyInputManager.OnGUI();
    }
}
