# Keyboard Manager

The asset for Unity with keyboard manager similar to keyboard manager of Emacs. <sup>Work In Progress</sup> 

## Key Modifyers

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

## Event

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

## Pseudo Keys

The pseudocode looks like unique random key code (non existed in keyboard). For example: "pseudo-1", "pseudo-2",...,"pseudo-N"
The pseudo code has large keycode and the key modifyer _Pseudo_ is in pressed state.

```C#
var pseudo = Event.GetPseudocodeOfName(); // Get random pseudo code
var default = Event.DefaultPseudoCode;    // Get default pseudo code
```

## Humanized Key Name

The key code can be convert to humanize name, or reversed. To declarate the name use method _SetName_

```C#
Event.SetName((int)KeyCode.RightCommand, "\\c-");
var name = Event.GetName((int)KeyCode.RightCommand);  // Return "\\c-"
```

## Key Sequence

The sequence can be defined as array of events. The example below defines the sequence "C-x C-f"

```C#
int[] sequence = new int[2] { Event.MakeEvent((int)KeyCode.X, KeyModifyers.Control), Event.MakeEvent((int)KeyCode.F, KeyModifyers.Control) };
```

Alternative way is parsing the string expression.

```C#
var sequence = ParseExpression("C-x C-f");
```

## Key Map

### Sequence Binding

This is simple keysequence binding to any key. The pressing this key will invoke this sequence.

| Field | Info |
|-------|------|
| string name | Binding's name |
| string help | Binding's help |
[ int[] sequence | The key sequence |

The constructor for sequence requires two fields values.

```C#
SequenceBinding(string name, int[] sequence, string help = null)
```

### Key Map Item

Any object wich can be binded to the keymap have to be based on this class.
    
| Field | Info |
|-------|------|
| int key | Key event |
| object value | Binded value |

The constructor requires those two fields as arguments.

```C#
KeyMapItem(int key, object value)
```

### Key Map

There are two variants of constructor available. One for the ordinary keymap and another for child keymap. When called LockUp method of keymap, and in case if key binding not found, and default binding is not alowed, will be called LockUp method of parent key map. The default binding is the field of each key map, used only when allowed by dedicated argument.

```C#
KeyMap(string title = null, string help = null )
KeyMap(KeyMap parent, string title = null, string help = null )
```
#### Define Local Binding

To define and read local binding means does not look at parent key map.

```C#
var event = Event.MakeEvent((int)KeyCode.A, KeyModifyers.Shift); // Makes S-a event
keyMap.SetLocal(event, "Foo");                                   // Bind to S-a event of this keymap the string "Foo"
var binding = keyMap.GetLocal(event, false);                     // Second argument accept default binding.
```
#### Define Global Binding

The define binding to the sequence use _Define_ method, use event sequence and object to bind as arguments.

```C#
Define(int[] sequence, object value)
``` 

Alternative version of this method dedicated for menu definition, and will buse pseudo codes for this bnding.

```C#
Define(string[] sequence, object value)
```

For example lets define menu _File_ and option _Save_ and bnd to it lambda function.

```C#
Define(new string[]{"File", "Save"}, () => { Debug.Log("Save"); })
```


#### Lock Up Binding

To lockup biding in hierary use _LokupKey_ method.

```C#
KeyMapItem LokupKey(int[] sequence, bool acceptDefaults = false)
```

Additionaly there is version of this method with start and end index in the sequence aray.

```
KeyMapItem LokupKey(int[] sequence, int starts, int ends, bool acceptDefaults = false)
```
## Mode

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

## Buffer

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
