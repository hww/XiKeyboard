# Keyboard Manager

The asset for Unity with keyboard manager similar to keyboard manager of Emacs. <sup>Work In Progress</sup> 

# Mode

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
