/* Copyright (c) 2021 Valerya Pudova (hww) */

using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using UnityEngine;

namespace XiKeyboard.KeyMaps
{
    ///-------------------------------------------------------------------------------------------------
    /// <summary>   A key event. </summary>
    ///
    ///-------------------------------------------------------------------------------------------------

    public partial struct KeyEvent
    {
        /// <summary>   The code. </summary>
        private int code;

        ///-------------------------------------------------------------------------------------------------
        /// <summary>   Constructor. </summary>
        ///

        ///
        /// <param name="code"> The code. </param>
        ///-------------------------------------------------------------------------------------------------

        public KeyEvent(int code) { this.code = code; }

        ///-------------------------------------------------------------------------------------------------
        /// <summary>   Implicit cast that converts the given KeyEvent to an int. </summary>
        ///

        ///
        /// <param name="evt">  The event. </param>
        ///
        /// <returns>   The result of the operation. </returns>
        ///-------------------------------------------------------------------------------------------------

        public static implicit operator int(KeyEvent evt){ return evt.code; }

        ///-------------------------------------------------------------------------------------------------
        /// <summary>   Implicit cast that converts the given int to a KeyEvent. </summary>
        ///

        ///
        /// <param name="code"> The code. </param>
        ///
        /// <returns>   The result of the operation. </returns>
        ///-------------------------------------------------------------------------------------------------

        public static implicit operator KeyEvent(int code) { return new KeyEvent(code); }

        ///-------------------------------------------------------------------------------------------------
        /// <summary>   Implicit cast that converts the given KeyCode to a KeyEvent. </summary>
        ///

        ///
        /// <param name="code"> The code. </param>
        ///
        /// <returns>   The result of the operation. </returns>
        ///-------------------------------------------------------------------------------------------------

        public static implicit operator KeyEvent(KeyCode code) { return new KeyEvent((int)code); }

        ///-------------------------------------------------------------------------------------------------
        /// <summary>   Implicit cast that converts the given string to a KeyEvent. </summary>
        ///

        ///
        /// <param name="expression">   The expression. </param>
        ///
        /// <returns>   The result of the operation. </returns>
        ///-------------------------------------------------------------------------------------------------

        public static implicit operator KeyEvent(string expression){ return ParseExpression(expression); }

        ///-------------------------------------------------------------------------------------------------
        /// <summary>   Check if code is valid. </summary>
        ///
        /// <value> True if this object is valid, false if not. </value>
        ///-------------------------------------------------------------------------------------------------

        public bool IsValid => code>= 0 && code < KeyModifiers.MaxCode;

        ///-------------------------------------------------------------------------------------------------
        /// <summary>   Gets the code. </summary>
        ///
        /// <value> The code. </value>
        ///-------------------------------------------------------------------------------------------------

        public KeyEvent Code => code;

        ///-------------------------------------------------------------------------------------------------
        /// <summary>   Get name of key code code. </summary>
        ///
        /// <value> The name. </value>
        ///-------------------------------------------------------------------------------------------------

        public string Name => GetName(this);

        ///-------------------------------------------------------------------------------------------------
        /// <summary>   Get modifiers of this event. </summary>
        ///
        /// <value> The modifiers. </value>
        ///-------------------------------------------------------------------------------------------------

        public KeyEvent Modifiers => code & KeyModifiers.AllModifiers;

        ///-------------------------------------------------------------------------------------------------
        /// <summary>   Get code of this event. </summary>
        ///
        /// <value> The key code. </value>
        ///-------------------------------------------------------------------------------------------------

        public KeyEvent KeyCode => code & ~KeyModifiers.AllModifiers;

        ///-------------------------------------------------------------------------------------------------
        /// <summary>   Get code as key code. </summary>
        ///
        /// <value> as key code. </value>
        ///-------------------------------------------------------------------------------------------------

        public KeyCode AsKeyCode => (KeyCode)(code & ~KeyModifiers.AllModifiers);

        ///-------------------------------------------------------------------------------------------------
        /// <summary>   Check if the given keycode is with the given modifier mask. </summary>
        ///

        ///
        /// <param name="modifiers">    Get modifiers of this event. </param>
        ///
        /// <returns>   True if modifier, false if not. </returns>
        ///-------------------------------------------------------------------------------------------------

        public bool IsModifier(int modifiers) { return (code & modifiers) == modifiers; }

        ///-------------------------------------------------------------------------------------------------
        /// <summary>
        /// (Immutable)
        /// None event.
        /// </summary>
        ///-------------------------------------------------------------------------------------------------

        public static readonly KeyEvent None = new KeyEvent();
    }

    ///-------------------------------------------------------------------------------------------------
    /// <summary>   A key event. </summary>
    ///
    ///-------------------------------------------------------------------------------------------------

    public partial struct KeyEvent
    {   
        ///-------------------------------------------------------------------------------------------------
        /// <summary>   Create nev event from code and use new modifiers. </summary>
        ///

        ///
        /// <param name="keyCode">      . </param>
        /// <param name="modifiers">    . </param>
        ///
        /// <returns>   A KeyEvent. </returns>
        ///-------------------------------------------------------------------------------------------------

        public static KeyEvent MakeEvent(KeyCode keyCode, int modifiers)
        {
            return MakeEvent((int)keyCode, modifiers);
        }

        ///-------------------------------------------------------------------------------------------------
        /// <summary>   Create nev event from code and use new modifiers. </summary>
        ///
        /// <remarks>   Valery, 10/12/2022. </remarks>
        ///
        /// <param name="keyCode">      . </param>
        /// <param name="modifiers">    . </param>
        ///
        /// <returns>   A KeyEvent. </returns>
        ///-------------------------------------------------------------------------------------------------

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

        ///-------------------------------------------------------------------------------------------------
        /// <summary>   this pseudocode is reserved word "default" used for default binding in keymaps. </summary>
        ///
        /// <value> The default pseudo code. </value>
        ///-------------------------------------------------------------------------------------------------

        public static KeyEvent DefaultPseudoCode { get; private set; }
        /// <summary>   Incremental value used for pseudo codes generator. </summary>
        private static int pseudoCodeIndex = 0;

        ///-------------------------------------------------------------------------------------------------
        /// <summary>
        /// PseudoCode Generator
        /// 
        /// The pseudo code looks like unique random key code (non existed in keyboard). Think about like
        /// it is: "pseudo-1", "pseudo-2",...,"pseudo-N" Pseudo code has large keycode and the key
        /// modifier Pseudo added.
        /// </summary>
        ///
        /// <remarks>   Valery, 10/12/2022. </remarks>
        ///
        /// <param name="name"> The name. </param>
        ///
        /// <returns>   The pseudo code. </returns>
        ///-------------------------------------------------------------------------------------------------

        public static KeyEvent GetPseudoCode(string name)
        {
            KeyEvent code;
            if (NameToKeyCodeTable.TryGetValue(name, out code))
                return code;
            code = pseudoCodeIndex++ | KeyModifiers.Pseudo;
            SetName(code, name);
            return code;
        }

        ///-------------------------------------------------------------------------------------------------
        /// <summary>   Table to convert name to the key code. </summary>
        ///
        /// <value> The name to key code table. </value>
        ///-------------------------------------------------------------------------------------------------

        public static Dictionary<string, KeyEvent> NameToKeyCodeTable
        {
            get {
                if (nameToKeyCodeTable == null)
                    Initialize ();
                return nameToKeyCodeTable;
            }
        }

        ///-------------------------------------------------------------------------------------------------
        /// <summary>   Table to convert key code to the code name. </summary>
        ///
        /// <value> The key code to name table. </value>
        ///-------------------------------------------------------------------------------------------------

        public static Dictionary<KeyEvent, string> KeyCodeToNameTable
        {
            get {
                if (keyCodeToNameTable == null)
                    Initialize ();
                return keyCodeToNameTable;
            }
        }

        ///-------------------------------------------------------------------------------------------------
        /// <summary>   Declare new key code name SetName((int)KeyCode.RightCommand, "\\c-"); </summary>
        ///
        ///
        /// <param name="keyCode">  . </param>
        /// <param name="name">     The name. </param>
        ///-------------------------------------------------------------------------------------------------

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

        ///-------------------------------------------------------------------------------------------------
        /// <summary>   Get name of key code code. </summary>
        ///
        ///
        /// <exception cref="Exception">    Thrown when an exception error condition occurs. </exception>
        ///
        /// <param name="evt">  The event. </param>
        ///
        /// <returns>   The name. </returns>
        ///-------------------------------------------------------------------------------------------------

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

        ///-------------------------------------------------------------------------------------------------
        /// <summary>   Get keycode by name. </summary>
        ///
        ///
        /// <exception cref="Exception">    Thrown when an exception error condition occurs. </exception>
        ///
        /// <param name="name"> The name. </param>
        ///
        /// <returns>   The key code internal. </returns>
        ///-------------------------------------------------------------------------------------------------

        private static KeyEvent GetKeyCodeInternal(string name)
        {
            KeyEvent code;
            if (NameToKeyCodeTable.TryGetValue(name, out code))
                return code;
            throw new Exception($"Expected key code name, found '{name}'");
        }

        ///-------------------------------------------------------------------------------------------------
        /// <summary>
        /// EMACS keycode expressions parser for converting from expression like: "C-v" to convert to the
        /// keycode Parse the expression without spaces.
        /// </summary>
        ///
        ///
        /// <exception cref="ArgumentNullException">    Thrown when one or more required arguments are
        ///                                             null. </exception>
        /// <exception cref="ArgumentException">        Thrown when one or more arguments have
        ///                                             unsupported or illegal values. </exception>
        ///
        /// <param name="expression">   The expression. </param>
        ///
        /// <returns>   A KeyEvent. </returns>
        ///-------------------------------------------------------------------------------------------------

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

        ///-------------------------------------------------------------------------------------------------
        /// <summary>
        /// Parse string expression EMACS style and build the keycode Support only single token with '-'
        /// character inside Such as C- A- S- the function produce an exception it the expression is
        /// badly format.
        /// </summary>
        ///
        ///
        /// <exception cref="ArgumentNullException">    Thrown when one or more required arguments are
        ///                                             null. </exception>
        /// <exception cref="Exception">                Thrown when an exception error condition occurs. </exception>
        ///
        /// <param name="expression">   The expression. </param>
        ///
        /// <returns>   A KeyEvent. </returns>
        ///-------------------------------------------------------------------------------------------------

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

        ///-------------------------------------------------------------------------------------------------
        /// <summary>   Initialize the class&lt;summary&gt; </summary>
        ///
        ///-------------------------------------------------------------------------------------------------

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
        /// <summary>   The name to key code table. </summary>
        private static Dictionary<string, KeyEvent> nameToKeyCodeTable;
        /// <summary>   The key code to name table. </summary>
        private static Dictionary<KeyEvent, string> keyCodeToNameTable;
    }
}

  