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

using System;
using System.Collections.Generic;
using UnityEngine;

namespace XiKeyboard
{
    public partial struct Event
    {
        private int code;
        public Event(int code) { this.code = code; }
        public static implicit operator int(Event evt){ return evt.code; }
        public static implicit operator Event(int code) { return new Event(code); }
        public static implicit operator Event(KeyCode code) { return new Event((int)code); }
        public static implicit operator Event(string expression){ return ParseExpression(expression); }
        
        /// <summary>Check if code is valid</summary>
        public bool IsValid => code>= 0 && code < KeyModifiers.MaxCode;

        /// <summary>Get name of key code code</summary>
        public string Name => GetName(this);

        /// <summary>Get modifiers of this event</summary>
        public Event Modifiers => code & KeyModifiers.AllModifiers;

        /// <summary>Get code of this event</summary>
        public Event KeyCode => code & ~KeyModifiers.AllModifiers;

        /// <summary>Check if the given keycode is with the given modifier mask</summary>
        public bool IsModifier(int modifiers) { return (code & modifiers) == modifiers; }

    }

    public partial struct Event
    {    
        /// <summary>
        /// Create nev event from code and use new modifiers
        /// </summary>
        /// <param name="keyCode"></param>
        /// <param name="modifiers"></param>
        public static Event MakeEvent(int keyCode, int modifiers)
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
        public static Event DefaultPseudoCode { get; private set; }
        /// <summary>Incremental value used for pseudo codes generator.</summary>
        private static int pseudoCodeIndex = 0;
        /// <summary>generate new pseudo code</summary>
        public static Event GetPseudoCode(string name)
        {
            Event code;
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
        public static Dictionary<string, Event> NameToKeyCodeTable
        {
            get {
                if (nameToKeyCodeTable == null)
                    Initialize ();
                return nameToKeyCodeTable;
            }
        }
        /// <summary>Table to convert key code to the code name</summary>
        public static Dictionary<Event, string> KeyCodeToNameTable
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
        public static void SetName(Event keyCode, string name)
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
        public static string GetName(Event evt)
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
        private static Event GetKeyCodeInternal(string name)
        {
            Event code;
            if (NameToKeyCodeTable.TryGetValue(name, out code))
                return code;
            throw new Exception($"Expected key code name, found '{name}'");
        }
        // ===============================================================================================
        // EMACS keycode expressions parser
        // For converting from expression like: "C-v" to convert to the keycode
        // ===============================================================================================
        /// <summary>Parse the expression without spaces</summary>
        public static Event ParseExpression(string expression)
        {
            if (expression == null) throw new ArgumentNullException(nameof(expression));
            if (expression == string.Empty) throw new ArgumentException(nameof(expression));

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
        private static Event ParseWordWithModifiers(string expression)
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
            nameToKeyCodeTable = new Dictionary<string, Event> ( );
            keyCodeToNameTable = new Dictionary<Event, string> ( );

            for ( var i = (int)' ' ; i < (int)( '0' ) ; i++ )
                SetName ( i, ( (char)i ).ToString ( ) );
            for ( var i = (int)'0' ; i <= (int)( '9' ) ; i++ )
                SetName ( i, ( (char)i ).ToString ( ) );
            for ( var i = (int)'a' ; i < (int)( 'z' ) ; i++ )
                SetName ( i, ( (char)i ).ToString ( ) );
            
            SetName ( KeyModifiers.Shift, "S-" );
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
        private static Dictionary<string, Event> nameToKeyCodeTable;
        private static Dictionary<Event, string> keyCodeToNameTable;
    }
}

  