# Keyboard Manager

The asset for Unity with keyboard manager similar to keyboard manager of Emacs. <sup>Work In Progress</sup> 

# Key Modifyers

Key modifyers encoded as most significant bits of integer value. The virutal key _Pseudo_ used to generate pseudo keys<sup>Read Below</sup>.

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

# Event

The KeyEvent is container with KeyCode and key modfyers. The modifyers packed to most significant bits. Every key press will convert KeyCode to the KeyEvent and send to current input buffer<sup>Read Below</sup>. 

To create new event there is  method _MakeEvent_.

```C#
var event = Event.MakeEvent((int)KeyCode.A, KeyModifyers.Shift); // Makes S-a event
Event.IsModifyer(event, KeyModifyers.Shift);                     // Return true
Event.IsModifyer(event, KeyModifyers.Control);                   // Return false
Event.IsValie(event);                                            // Return true
var keyCode = Event.GetKeyCode(event);                           // Return KeyCode.A as integer
var keyModf = Event.GetModifyers(event);                         // Return KeyModifyers.Shift
```

# Pseudo Keys

The pseudocode looks like unique random key code (non existed in keyboard). For example: "pseudo-1", "pseudo-2",...,"pseudo-N"
The pseudo code has large keycode and the key modifyer _Pseudo_ is in pressed state.

```C#
var pseudo = Event.GetPseudocodeOfName(); // Get random pseudo code
var default = Event.DefaultPseudoCode;    // Get default pseudo code
```

# Humanized Key Name

The key code can be convert to humanize name, or reversed. To declarate the name use method _SetName_

```C#
Event.SetName((int)KeyCode.RightCommand, "\\c-");
var name = Event.GetName((int)KeyCode.RightCommand);  // Return "\\c-"
```

# Key Sequence

The sequence can be defined as array of events. The example below defines the sequence "C-x C-f"

```C#
int[] sequence = new int[2] { Event.MakeEvent((int)KeyCode.X, KeyModifyers.Control), Event.MakeEvent((int)KeyCode.F, KeyModifyers.Control) };
```

Alternative way is parsing the string expression.

```C#
var sequence = ParseExpression("C-x C-f");
```

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

To lockup keysequence in the buffer use method _Lockup_ with arguments: key sequence, start index of sequence, end index of sequence and accept or not default key binding<sup>read KeyMap chapter</sup>. The method returns the _KeyMapItem_ ofject in case of recognized sequence.

```C#
  KeyMapItem Lockup([NotNull] int[] sequence, int starts, int ends, bool acceptDefaults)
```
