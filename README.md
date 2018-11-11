# Keyboard Manager

The asset for Unity with keyboard manager similar to keyboard manager of Emacs. <sup>Work In Progress</sup> 

# Key Even

The KeyEvent is container with KeyCode and key modfyers. The modifyers packed to most significant bits. Every key press will convert KeyCode to the KeyEvent and send to current input buffer<sup>Read Below</sup>

| Modifier                 | Bit         | 
|--------------------------|-------------|
| KeyModifyers.MaxCode     | 1 << 28 - 1 |
| KeyModifyers.Meta        | 1 << 27     |
| KeyModifyers.Control     | 1 << 26     |
| KeyModifyers.Shift       | 1 << 25     |
| KeyModifyers.Hyper       | 1 << 24     |
| KeyModifyers.Super       | 1 << 23     |
| KeyModifyers.Alt         | 1 << 22     |
| KeyModifyers.Pseudo      | 1 << 21     |

# Mode

To create new mode use constructors.

```C#
// Create new mode with name, help and given key map
public Mode(string name, string help = null, KeyMap keyMap = null)
// Create new child mode with name, help and given key map 
public Mode(Mode parentMode, string name, string help = null, KeyMap keyMap = null)
```

To read name and help

```C#
Debug.Log(mode.name);
Debug.Log(mode.help);
```

To add remove listeners use delegates  _OnEnableListeners_ and _OnDisableListeners_.

```C#
mode.OnEnableListeners += () => { Debug.Log("Enabled"); };
mode.OnDisableListeners += () => { Debug.Log("Disabled"); };
mode.Enable();  // Print "Enabled"
mode.Disable(); // Print "Disabled"
```

To get parrent mode use _parentMode_ field and to read keymap use _keyMap_ field. 

# Buffer

Buffer is similar to text input line. There is only one current buffer is active for input. To create new bufffer.

```C#
var buffer = new Buffer("REPL", "Evaluate LISP command");
```

To activate buffer use _Enable_ method. After activation current buffer will be accesible via static property _Buffer.CurrentBuffer_

```C#
buffer.Enable();
Debug.Log(Buffer.CurrentBuffer.Name); // will print "REPL"
Debug.Log(Buffer.CurrentBuffer.Help); // will print "Evaluate LISP command"
```

There are two delegates available _OnEnableListeners_ and _OnDisableListeners_.

```C#
buffer.OnEnableListeners += () => { Debug.Log("Enabled"); };
buffer.OnDisableListeners += () => { Debug.Log("Disabled"); };
buffer.Enable();  // Print "Enabled"
buffer.Disable(); // Print "Disabled"
```

Every buffer could have single major and multiple minor modes.

```C#
EnableMajorMode(mode); // Enable major mode
DisableMajorMode()     // Disable major mode
EnableMinorMode(mode)  // Enable minor mode
DisableMinorMode(mode) // Disable minor mode
```

To lockup keysequence in the buffer use method _Lockup_ with arguments: key sequence, start index of sequence, end index of sequence and accept or not default mode. The method returns the _KeyMapItem_ ofject in case of recognized sequence.

```C#
  KeyMapItem Lockup([NotNull] int[] sequence, int starts, int ends, bool acceptDefaults)
```
