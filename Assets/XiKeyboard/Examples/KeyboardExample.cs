/* Copyright (c) 2021 Valerya Pudova (hww) */

using XiKeyboard;
using XiKeyboard.KeyMaps;
using XiKeyboard.Buffers;
using UnityEngine;
using System.Collections;

public class KeyboardExample : MonoBehaviour {

	const string kHelpText = "USAGE\n"
						   + "S-1      Keystroke 1\n"
						   + "S-2 S-3  Keystroke 2\n"
						   + "--------------------\n";

	string miniBuffer = string.Empty;

	GUISkin skin;

	void Awake()
	{
		skin = Resources.Load<GUISkin>("XiKeyboard/Skins/Default Skin");
		skin.box.normal.background = MakeTex(2, 2, new Color(0f, 0f, 0f, 0.7f));
	}


	// Use this for initialization
	void Start ()
	{
		KeyMap.GlobalKeymap.Define("S-1", "Pressed: S-1");         	// Define keystroke S-1 with text binding "1"
		KeyMap.GlobalKeymap.Define("S-2 S-3", "Pressed: S-2 S-3");	// Define keystroke S-2 S-3 with text binding "2"
		Buffer.OnSequencePressed.Add(OnSequencePressed);            // On press sequence delegate
		Buffer.OnKeyPressed.Add(OnKeyPressed);                      // On press key delegate
	}

	Buffer theBuffer;

	void OnSequencePressed(Buffer buffer, DMKeyMapItem item) {
		theBuffer = buffer;
		miniBuffer = item.value.ToString();                         // Print "Pressed Sequence: N" 	
		StopAllCoroutines();
		StartCoroutine(UpdateMiniBuffer());
		if (item.value is KeyMap)
			return;
		buffer.Clear();
	}

	// Presed key but not detected key sequence"
	void OnKeyPressed(Buffer buffer, KeyEvent evt) {
		theBuffer = buffer;
		miniBuffer = buffer.GetBufferHumanizedString();
		StopAllCoroutines();
		StartCoroutine(UpdateMiniBuffer());
	}

	IEnumerator UpdateMiniBuffer()
	{
		yield return new WaitForSeconds(3f);
		if (theBuffer != null)
		{
			theBuffer.Clear();
			miniBuffer = theBuffer.GetBufferHumanizedString();
		}
		else
        {
			miniBuffer = string.Empty;
		}
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

		InputManager.OnGUI();
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
}
