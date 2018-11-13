using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VARP.Keyboard;

public class KeyboardDemoC : MonoBehaviour {

	// Use this for initialization
	void Start ()
	{
		KeyMap.GlobalKeymap.Define("S-1", "S-1");
		KeyMap.GlobalKeymap.Define("S-2 S-3", "S-2 S-3");
		Buffer.OnSequencePressed.Add(OnSequencePressed);
	}
	
	// Update is called once per frame
	void OnSequencePressed(Buffer buffer, KeyMapItem item) {
		Debug.Log(item.value);		
	}
}
