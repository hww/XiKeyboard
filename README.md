# Keyboard Manager

The asset for Unity 3D with keyboard manager similar to Emacs. <sup>Work In Progress</sup> 

## Introduction

The example below shows how the API can be used to define key sequences. Each key press will print current buffer to log. And in case of two sequences will be printed "Pressed Sequence: N" text (where N is 1 or 2)

```C#
void Start ()
{
    KeyMap.GlobalKeymap.Define("S-1", "1");          // Define keystroke S-1 with text binding "1"
    KeyMap.GlobalKeymap.Define("S-2 S-3", "2");      // Define keystroke S-2 S-3 with text binding "2"
    Buffer.OnSequencePressed.Add(OnSequencePressed); // On press sequence delagate
    Buffer.OnKeyPressed.Add(OnKeyPressed);           // On press key delegate
}
	
void OnSequencePressed(Buffer buffer, KeyMapItem item) {
    Debug.Log("Pressed Sequence: " + item.value);    // Print "Pressed Sequence: N" 
}

void OnKeyPressed(Buffer buffer, Event evt) {
    Debug.Log(buffer.GetBufferHumanizedString());    // Just display current bufer content
}
 ```

Can be binded any value, for example: GameObject, lambda function or menu item.

## Key Modifiers

Key modifiers encoded as most significant bits of integer value. The virtual key _Pseudo_ used to generate pseudo keys<sup>Read Below</sup>.

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

The key Event is container with key code and key modifier. For every pressed key will the key code will be packed with modifier to Event and submit to current input buffer<sup>Read Below</sup>. 

To create new event there is _MakeEvent_ method.

```C#
var event = Event.MakeEvent(KeyCode.A, KeyModifyers.Shift);      // Makes S-a event
```

To check event's modifiers there is _IsModifyer_ method.

```C#
event.IsModifyer(event, KeyModifyers.Shift);                     // Return true
event.IsModifyer(event, KeyModifyers.Control);                   // Return false
```

Other methods of Event you can see below.

```C#
var name = event.Name;                                           // Return S-a
var valid = event.IsValid;                                       // Return true if the keycode is valid
var keyCode = event.KeyCode;                                     // Return KeyCode.A as integer
var keyModf = event.Modifyers;                                   // Return KeyModifyers.Shift
```

## Pseudo Keys

The pseudo codes are virtual keys with unique names. Each pseudocode has a key modifier _Pseudo_ is in pressed state. 

```C#
var pseudo = Event.GetPseudocode("Foo");       // Get random pseudo code with unique name "Foo"
var default = Event.DefaultPseudoCode;         // Get default pseudo code. It has name "default"
```

## Humanized Key Name

The key code can be converted to humanize name, or reversed. To define the name use method _SetName_

```C#
Event.SetName((int)KeyCode.RightCommand, "\\c-");
var name = Event.GetName((int)KeyCode.RightCommand);  // Return "\\c-"
```

## Key Sequence

The sequence can be defined as array of events. The example below defines the sequence "C-x C-f"

```C#
Event[] sequence = new Event[2] { 
    Event.MakeEvent((int)KeyCode.X, KeyModifyers.Control), 
    Event.MakeEvent((int)KeyCode.F, KeyModifyers.Control) 
    };
```

Alternative way is parsing the expression <sup>similar to Emacs</sup>.

```C#
var sequence = Kbd.ParseExpression("C-x C-f");
```

## Key Map

There are two variants of constructor available. One for the ordinary key-map and another for child key map. When called LookUp method of key-map, and in case if key binding not found, and default binding is not alowed, will be called LookUp method of parent key map. The default binding is the field of each key map, used only when allowed by dedicated argument.

```C#
KeyMap(string title = null, string help = null )
KeyMap(KeyMap parent, string title = null, string help = null )
```

#### Key Map Item

This object link a key event with other item: 

- other keymap
- sequence binding
- menu item
- lambda function
- anything else... 
 
KeyMapItem contains the next fields.

| Field         | Info         |
|---------------|--------------|
| int key       | Key event    |
| object value  | Binded value |

The constructor requires those two fields as arguments.

```C#
KeyMapItem(int key, object value)
```

#### Define Local Binding

To define and read local binding means does not look at parent key map.

```C#
var event = Event.MakeEvent((int)KeyCode.A, KeyModifyers.Shift); // Makes S-a event
keyMap.SetLocal(event, "Foo");                                   // Bind to S-a event of this key map the string "Foo"
var binding = keyMap.GetLocal(event, false);                     // Second argument accept default binding.
```
#### Define Global Binding

The define binding to the sequence use _Define_ method, use event sequence and object to bind as arguments.

```C#
Define(int[] sequence, object value);
Define(string expression, object value);
``` 

Alternative version of this method dedicated for menu definition, and will use pseudo codes for this binding.

```C#
Define(string[] sequence, object value)
```

For example lets define menu _File_ and option _Save_ and bind to it a menu item.<sup>Read Menu chapter</sup>

```C#
Define(new string[]{"File", "Save"}, menuItem )
```

#### Lookup Binding

To lockup biding in hierarchy use _LokupKey_ method.

```C#
KeyMapItem LokupKey(int[] sequence, bool acceptDefaults = false)
```

Additionaly there is version of this method with start and end index in the sequence aray.

```
KeyMapItem LokupKey(int[] sequence, int starts, int ends, bool acceptDefaults = false)
```

## Global Key Map

The default global key map, can be used in most cases without creating additional key-maps.

```C#
var globalKeyMap = KeyMap.GlobalKeymap;
```

## Full Keymap

If an element of a key map is a char-table, it counts as holding bindings for all character events with no modifier element n is the binding for the character with code n. This is a compact way to record lots of bindings. A key map with such a char-table is called a full key-map. Other key-maps are called sparse key-maps.

#### Sequence Binding

This is simple keysequence binding to any key. The pressing this key will invoke this sequence.

| Field          | Info             |
|----------------|------------------|
| string name    | Binding's name   |
| string help    | Binding's help   |
| int[] sequence | The key sequence |

The constructor for sequence requires two fields values.

```C#
SequenceBinding(string name, int[] sequence, string help = null)
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

To get parrent mode use _parentMode_ field and to read key-map use _keyMap_ field. 

## Buffer

Buffer is similar to text input line. There is only one current buffer is active for input. To create new buffer.

```C#
var buffer = new Buffer("REPL", "Evaluate LISP command");
```

To activate buffer use _Enable_ method. After activation current buffer will be accessible via static property _Buffer.CurrentBuffer_

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

To lockup key sequence in the buffer use method _Lookup_ with arguments: key sequence, start index of sequence, end index of sequence and accept or not default key binding<sup>read KeyMap chapter</sup>. The method returns the _KeyMapItem_ object in case of recognized sequence.

```C#
  KeyMapItem Lookup([NotNull] int[] sequence, int starts, int ends, bool acceptDefaults)
```

To get current string in the buffer use _GetBufferString_ and _GetBufferSubString_ methods.
To select substring in the buffer use _SetSelection_ and to read selection _GetSelection_ methods. The first method set begin and end position of selection, but last method returns current positions.

Additoanal delegates _OnSequencePressed_ will be called when one of sequence bindings triggered.

```C#
OnSequencePressed.Add((Buffer buffer, KeyMapItem item) => 
{
    Debug.LogFormat("The buffer {0} detected keys sequence with binding {1}", buffer.name, item.value);
});
```

## Menu

The key maps in EMACS also used for making menus. The menu by self is a key map. Where instead of key events used pseudo codes, but menu items as bindings.

There are several variants of menu items available.

| Menu Item | Description |
|-----------|-------------|
|MenuLineBase| Abstract class for all menu items |
|MenuLineBaseSimple| Very simple menu item. Has fields: text, help, shortcut and binding |
|MenuLineBaseComplex| Advanced menu item has various of additional options and delegates |
|MenuSeparator| Just a line to separate menu items |

### MenuSeparator

Has only one field with type of separator: NoLine, Space, SingleLine, DashedLine.

```C#
var menuLine = new MenuSeparator(MenuSeparator.Type.SingleLine);
```

### MenuLineBase

```C#
public abstract class MenuLineBase
{
    public virtual string Text { get { return null; } }
    public virtual string Help { get { return null; } }
    public virtual string Shortcut { get { return null; } }
    public virtual object Binding { get { return null; } }
}
```

### MenuLineBaseSimple

Has two constructors.

```C#
MenuLineBaseSimple(string text, string shortcut = null, string help = null)
MenuLineBaseSimple(string text, object binding, string shortcut = null, string help = null) 
```

And this menu item has next fields:

```C#
protected string text;      // Menu Text
protected string help;      // Menu Help
protected string shortcut;  // Menu Shortcut/Value Text 
public object binding;      // Menu Biding
```

### MenuLineBaseComplex

The fields of complex menu item:

```C#
protected string text;                      // Menu Text
protected string help;                      // Menu Help
protected string shortcut;                  // Shortcut Text or Value text. Will be rendered at right side. 
public readonly object binding;             // Binding to menu item
public readonly ButtonType buttonType;      // Enum value NoButton, Toggle, Radio
public readonly Filter filter;              // Delegate to get filtered menu item<sup>See below</sup>
public readonly Precodition enable;         // Delegate to get status of activity (normal/grayed out)
public readonly Precodition visible;        // Delegate to get status of visibility (show/hide)  
public readonly Precodition buttonState;    // Delegate to get button state
```

The constructors for this menu item:

```C#
MenuLineBaseComplex(string text,                                  // Menu text
                    string shortcut = null,                       // Menu shortcut only for screen
                    string help = null)                           // Menu help 

MenuLineBaseComplex(string text,                                  // Menu text
                    object binding,                               // Binding to menu: other menu, function, etc
                    string shortcut = null,                       // Menu shortcut only for screen
                    string help = null)                           // Menu text

MenuLineBaseComplex(string text,                                  // Menu text
                    object binging,                               // Binding to menu: other menu, function, etc
                    Precodition enable = null,                    // Predicate: is this menu active
                    Precodition visible = null,                   // Predicate: is this menu visible
                    Filter filter = null,                         // Filter: Method to compute actual menu item
                    string shortcut = null,                       // Menu shortcut only for screen
                    string help = null)                           // Menu help 

MenuLineBaseComplex(string text,                                  // Menu text
                    object binging,                               // Binding to menu: other menu, function, etc
                    Precodition enable = null,                    // Predicate: is this menu active
                    Precodition visible = null,                   // Predicate: is this menu visible
                    Filter filter = null,                         // Filter: Method to compute actual menu item
                    ButtonType buttonType = ButtonType.NoButton,  // Button Type
                    Precodition buttonState = null,               // Predicate: is this button pressed
                    string shortcut = null,                       // Menu shortcut only for screen
                    string help = null)                           // Menu help     
```

Lets make example of menu definition. 
- Define _File_ menu
- Define _Save_ item at _File_ menu 
- Append _File_ menu to _MainMenu_
- Add keyboar shorcuts to _File_ menu and _Save_ menu item.

```C#
// Create file menu
var menu = new KeyMap("File", "File Menu" );
// Define menu as member of MainMenu
KeyMap.GlobalKeyMap.Define(new string[] { "MainMenu", "File" }, menu );
// Create save menu item (shortcut will be only displayed and can be omitted)
// The method Save of this class will be binded to this menu item
var menuItem = MenuLineBaseSimple("Save", Save, "C-s", "Save current file") 
// Define this item as member of File menu 
KeyMap.GlobalKeyMap.Define(new string[] { "MainMenu", "File", "Save" }, menItemu );
// Open file menu by Alt+F 
KeyMap.GlobalKeymap.SetLocal("A-f", menu);                  
// Save file by C+S 
KeyMap.GlobalKeymap.SetLocal("C-s", menuItem.binding);                  
```

But this can be done by shorter way. 

<sup>To Do ...</sup>

