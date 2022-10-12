/* Copyright (c) 2021 Valerya Pudova (hww) */

using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using UnityEngine;

namespace XiKeyboard.KeyMaps
{
    public partial struct KeyEvent
    {
        private int code;
        public KeyEvent(int code) { this.code = code; }
        public static implicit operator int(KeyEvent evt){ return evt.code; }
        public static implicit operator KeyEvent(int code) { return new KeyEvent(code); }
        public static implicit operator KeyEvent(KeyCode code) { return new KeyEvent((int)code); }
        public static implicit operator KeyEvent(string expression){ return ParseExpression(expression); }

        /// <summary>Check if code is valid</summary>
        public bool IsValid => code>= 0 && code < KeyModifiers.MaxCode;

        /// <summary>Get name of key code code</summary>
        public string Name => GetName(this);

        /// <summary>Get modifiers of this event</summary>
        public KeyEvent Modifiers => code & KeyModifiers.AllModifiers;

        /// <summary>Get code of this event</summary>
        public KeyEvent KeyCode => code & ~KeyModifiers.AllModifiers;

        /// <summary>Get code as key code</summary>
        public KeyCode AsKeyCode => (KeyCode)(code & ~KeyModifiers.AllModifiers);


        /// <summary>Check if the given keycode is with the given modifier mask</summary>
        public bool IsModifier(int modifiers) { return (code & modifiers) == modifiers; }

        /// <summary>
        /// None event
        /// </summary>
        public static readonly KeyEvent None = new KeyEvent();
    }

    public partial struct KeyEvent
    {   
        /// <summary>
        /// Create nev event from code and use new modifiers
        /// </summary>
        /// <param name="keyCode"></param>
        /// <param name="modifiers"></param>
        public static KeyEvent MakeEvent(KeyCode keyCode, int modifiers)
        {
            return MakeEvent((int)keyCode, modifiers);
        }
        /// <summary>
        /// Create nev event from code and use new modifiers
        /// </summary>
        /// <param name="keyCode"></param>
        /// <param name="modifiers"></param>
        public static KeyEvent MakeEvent(int keyCode, int modifiers)
        {
            var code = keyCode & ~KeyModifiers.AllModifiers;
            if (code > 32 && code < 255)
            {
                // ASCII
                if ((modifiers & KeyModifiers.AllModifiers) == KeyModifiers.Control)
                    return code & 0x1F;
                else
                    return code | modifiers;
            }
            else
            {
                return code | modifiers;
            }
        }

        // ===============================================================================================
        // PseudoCode Generator 
        //
        // The pseudo code looks like unique random key code (non existed in keyboard). Think about 
        // like it is: "pseudo-1", "pseudo-2",...,"pseudo-N"
        // Pseudo code has large keycode and the key modifier Pseudo added
        // ===============================================================================================
        /// <summary>
        /// this pseudocode is reserved word "default" used for 
        /// default binding in keymaps
        /// </summary>
        public static KeyEvent DefaultPseudoCode { get; private set; }
        /// <summary>Incremental value used for pseudo codes generator.</summary>
        private static int pseudoCodeIndex = 0;
        /// <summary>generate new pseudo code</summary>
        public static KeyEvent GetPseudoCode(string name)
        {
            KeyEvent code;
            if (NameToKeyCodeTable.TryGetValue(name, out code))
                return code;
            code = pseudoCodeIndex++ | KeyModifiers.Pseudo;
            SetName(code, name);
            return code;
        }
        // ===============================================================================================
        // Conversion Tables
        // Those tables alow to convert keycode to the name, and reversed.
        // ===============================================================================================
        /// <summary>Table to convert name to the key code</summary>
        public static Dictionary<string, KeyEvent> NameToKeyCodeTable
        {
            get {
                if (nameToKeyCodeTable == null)
                    Initialize ();
                return nameToKeyCodeTable;
            }
        }
        /// <summary>Table to convert key code to the code name</summary>
        public static Dictionary<KeyEvent, string> KeyCodeToNameTable
        {
            get {
                if (keyCodeToNameTable == null)
                    Initialize ();
                return keyCodeToNameTable;
            }
        }
        /// <summary>
        /// Declare new key code name
        /// SetName((int)KeyCode.RightCommand, "\\c-");
        /// </summary>
        public static void SetName(KeyEvent keyCode, string name)
        {
            var modifiers = keyCode.Modifiers;
            var keyCodeOnly = keyCode.KeyCode;
            // The key code can be modifyer or the key
            // but it can't be bought
            Debug.Assert(modifiers == 0 || keyCodeOnly == 0, keyCode);
            NameToKeyCodeTable[name] = keyCode;
            KeyCodeToNameTable[keyCode] = name;
        }
        /// <summary>Get name of key code code</summary>
        public static string GetName(KeyEvent evt)
        {
            var keyModifiers = evt.Modifiers;
            var keyCodeOnly = evt.KeyCode;
            var modifierName = string.Empty;
            if (keyModifiers != 0)
            {
                foreach (var m in KeyModifiers.AllModifiersList)
                {
                    if (keyModifiers.IsModifier(m))
                    {
                        string name;
                        if (KeyCodeToNameTable.TryGetValue(m, out name))
                            modifierName += name;
                        else
                            throw new Exception($"Unexpected modifier in keycode '{evt:X}'");
                    }
                }
            }
            string keyCodeName;
            if (KeyCodeToNameTable.TryGetValue(keyCodeOnly, out keyCodeName))
                return modifierName + keyCodeName;
            else if (keyCodeOnly < 32 && keyModifiers == 0)
                return $"^{(char) (keyCodeOnly + 0x40)}";
            else
                throw new Exception($"Unexpected keycode '{evt:X}'");
        }
        /// <summary>Get keycode by name</summary>
        private static KeyEvent GetKeyCodeInternal(string name)
        {
            KeyEvent code;
            if (NameToKeyCodeTable.TryGetValue(name, out code))
                return code;
            throw new Exception($"Expected key code name, found '{name}'");
        }
        // ===============================================================================================
        // EMACS keycode sequence parser
        // For converting from expression like: "C-x C-v" to convert to the KeyEvent[]
        // ===============================================================================================
        /// <summary>
        /// Supports multiple or single tokens
        /// When multiple tokens each one separated with ' ' space
        /// </summary>
        /// <param name="expression"></param>
        /// <returns></returns>
        public static KeyEvent[] ParseSequence([NotNull] string expression)
        {
            if (expression == null) throw new ArgumentNullException(nameof(expression));
            if (expression == string.Empty) throw new ArgumentException(nameof(expression));

            var sequence = new List<KeyEvent>();
            var tags = expression.Split(' ');

            foreach (var s in tags)
            {
                if (string.IsNullOrEmpty(s))
                    continue;

                var evt = KeyEvent.ParseExpression(s);

                if (evt >= 0)
                {
                    sequence.Add(evt);
                }
                else
                {
                    // This case will be translated as string
                    // "abcd" => "abcd"
                    foreach (var c in s)
                    {
                        sequence.Add(c);
                    }
                }
            }
            return sequence.ToArray();
        }
        // ===============================================================================================
        // EMACS keycode expressions parser
        // For converting from expression like: "C-v" to convert to the keycode
        // ===============================================================================================
        /// <summary>Parse the expression without spaces</summary>
        public static KeyEvent ParseExpression(string expression)
        {
            if (expression == null) 
                throw new ArgumentNullException(nameof(expression));
            if (expression == string.Empty) 
                throw new ArgumentException(nameof(expression));

            // There is the testing for the modifier C- A- S-
            var m = expression[0];
            if (m == 'C' || m == 'A' || m == 'S')
            {
                var evt = ParseWordWithModifiers(expression);
                if (evt >= 0)
                {
 
                    return evt;
                }
            }
            // There is test for named character Shift, LeftAlt, Space
            return GetKeyCodeInternal(expression);
        }
        /// <summary>
        /// Parse string expression EMACS style and build the keycode
        /// Support only single token with '-' character inside
        /// Such as C- A- S- the function produce an exception
        /// it the expression is badly format
        /// </summary>
        private static KeyEvent ParseWordWithModifiers(string expression)
        {
            if (expression == null) throw new ArgumentNullException(nameof(expression));

            var modifiers = 0;
            var sufix = string.Empty;

            var index = 0;
            while (index < expression.Length)
            {
                var c1 = expression[index];                    // C,A,S,...
                var i2 = index + 1;

                if (i2 < expression.Length)
                {
                    var c2 = expression[i2];                   // Here is '-'

                    if (c2 == '-')
                    {
                        index += 2;                             // skip c1 and c2
                        switch (c1)
                        {
                            case 'C':
                                modifiers |= KeyModifiers.Control;
                                break;

                            case 'S':
                                modifiers |= KeyModifiers.Shift;
                                break;

                            case 'A':
                                modifiers |= KeyModifiers.Alt;
                                break;

                            case 'M':
                                modifiers |= KeyModifiers.Meta;
                                break;

                            case 's':
                                modifiers |= KeyModifiers.Super;
                                break;

                            case 'H':
                                modifiers |= KeyModifiers.Hyper;
                                break;

                            default:
                                sufix = expression.Substring(index);
                                goto DONE;
                        }
                    }
                    else
                    {
                        sufix = expression.Substring(index); // include c1
                        goto DONE;
                    }
                }
                else
                {
                    sufix = expression.Substring(index);
                    goto DONE;
                }
            }
            DONE:
            if (sufix != string.Empty)
            {
                var tmp = GetKeyCodeInternal(sufix);
                if (tmp >= 0)
                {
                    if (modifiers == KeyModifiers.Control && tmp < 256)
                        return tmp.code & 0x1F;
                    else
                        return MakeEvent(tmp.code, modifiers);
                }
                throw new Exception(string.Format("Expected character after C-,A-,S- found '{0}' in expression '{0:X}'", sufix, expression));
            }
            else
            {
                return modifiers;
            }
        }
        // ===============================================================================================
        // The initialization block
        // ===============================================================================================
        /// <summary>Initialize the class<summary>
        public static void Initialize ( )
        {
            nameToKeyCodeTable = new Dictionary<string, KeyEvent> ( );
            keyCodeToNameTable = new Dictionary<KeyEvent, string> ( );

            for ( var i = (int)' ' ; i < (int)( '0' ) ; i++ )
                SetName ( i, ( (char)i ).ToString ( ) );
            for ( var i = (int)'0' ; i <= (int)( '9' ) ; i++ )
                SetName ( i, ( (char)i ).ToString ( ) );
            for ( var i = (int)'a' ; i < (int)( 'z' ) ; i++ )
                SetName ( i, ( (char)i ).ToString ( ) );
            
            SetName(UnityEngine.KeyCode.RightArrow, "right");
            SetName(UnityEngine.KeyCode.LeftArrow, "left");
            SetName(UnityEngine.KeyCode.UpArrow, "up");
            SetName(UnityEngine.KeyCode.DownArrow, "down");

            SetName( KeyModifiers.Shift, "S-" );
            SetName ( KeyModifiers.Control, "C-" );
            SetName ( KeyModifiers.Alt, "A-" );
            SetName ( KeyModifiers.Pseudo, "P-" );
            SetName ( KeyModifiers.Pseudo, "\\P-" );
            SetName ( UnityEngine.KeyCode.CapsLock, "\\_-" );
            SetName ( UnityEngine.KeyCode.Numlock, "\\N-" );
            SetName ( UnityEngine.KeyCode.LeftControl, "\\C-" );
            SetName ( UnityEngine.KeyCode.LeftAlt, "\\A-" );
            SetName ( UnityEngine.KeyCode.LeftShift, "\\S-" );
            SetName ( UnityEngine.KeyCode.LeftWindows, "\\W-" );
            SetName ( UnityEngine.KeyCode.LeftCommand, "\\c-" );
            SetName ( UnityEngine.KeyCode.RightControl, "\\C-" );
            SetName ( UnityEngine.KeyCode.RightAlt, "\\A-" );
            SetName ( UnityEngine.KeyCode.RightShift, "\\S-" );
            SetName ( UnityEngine.KeyCode.RightWindows, "\\W-" );
            SetName ( UnityEngine.KeyCode.RightCommand, "\\c-" );
            // pseudo-code for default binding.
            DefaultPseudoCode = GetPseudoCode ( "default" );
        }
        private static Dictionary<string, KeyEvent> nameToKeyCodeTable;
        private static Dictionary<KeyEvent, string> keyCodeToNameTable;
    }
}

  