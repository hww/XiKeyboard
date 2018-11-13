using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VARP.Keyboard;
using Event = VARP.Keyboard.Event;

public class KeyboardDemoC : MonoBehaviour {

	// Use this for initialization
	void Start ()
	{
		KeyMap.GlobalKeymap.Define("S-1", "Pressed: S-1");
		KeyMap.GlobalKeymap.Define("S-2 S-3", "Pressed: S-2 S-3");
		Buffer.OnSequencePressed.Add(OnSequencePressed);
		Buffer.OnKeyPressed.Add(OnKeyPressed);
	}
	
	// Update is called once per frame
	void OnSequencePressed(Buffer buffer, KeyMapItem item) {
		Debug.Log(item.value);		
	}
	// Update is called once per frame
	void OnKeyPressed(Buffer buffer, Event evt) {
		Debug.Log(buffer.GetBufferHumanizedString());		
	}
}
