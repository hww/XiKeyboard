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
using Event = XiKeyboard.Event;

public class KeyboardDemoC : MonoBehaviour {

	// Use this for initialization
	void Start ()
	{
		KeyMap.GlobalKeymap.Define("S-1", "Pressed: S-1");         	// Define keystroke S-1 with text binding "1"
		KeyMap.GlobalKeymap.Define("S-2 S-3", "Pressed: S-2 S-3");	// Define keystroke S-2 S-3 with text binding "2"
		Buffer.OnSequencePressed.Add(OnSequencePressed);                          	// On press sequence delegate
		Buffer.OnKeyPressed.Add(OnKeyPressed);                                    	// On press key delegate
	}
	
	void OnSequencePressed(Buffer buffer, KeyMapItem item) {
		Debug.Log("{" + item.value + "}");	  // Print "Pressed Sequence: N" 	
	}
	void OnKeyPressed(Buffer buffer, Event evt) {
		Debug.Log(buffer.GetBufferHumanizedString()); // Just display current buffer content		
	}
}
