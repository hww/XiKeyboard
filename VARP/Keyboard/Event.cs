/* Copyright (c) 2016 Valery Alex P. All rights reserved. */

using System;
using System.Collections.Generic;
using JetBrains.Annotations;
using UnityEngine;

namespace VARP.Keyboard
{
    public static partial class Event
    {
        /// <summary>
        /// Check if the given keycode is with the given modifyer mask
        /// </summary>
        /// <param name="evt"></param>
        /// <param name="modifyers"></param>
        /// <returns></returns>
        public static bool IsModifyer(int evt, int modifyers)
        {
            return (evt & modifyers) == modifyers;
        }
        /// <summary>
        /// Check if code is valid
        /// </summary>
        /// <param name="evt"></param>
        /// <returns></returns>
        public static bool IsValid(int evt)
        {
            return evt>= 0 && evt < KeyModifyers.MaxCode;
        }
        /// <summary>
        /// Create nev event from code and use new modifyers
        /// </summary>
        /// <param name="keyCode"></param>
        /// <param name="modifyers"></param>
        public static int MakeEvent(int keyCode, int modifyers)
        {
            var code = keyCode & ~KeyModifyers.AllModifyers;
            if (code > 32 && code < 255)
            {
                // ASCII
                if ((modifyers & KeyModifyers.AllModifyers) == KeyModifyers.Control)
                    return code & 0x1F;
                else
                    return code | modifyers;
            }
            else
            {
                return code | modifyers;
            }
        }
        /// <summary>
        /// Get modifyers of this event
        /// </summary>
        /// <returns></returns>
        public static int GetModifyers(int evt)
        {
            return evt & KeyModifyers.AllModifyers;
        }
        /// <summary>
        /// Get code of this event
        /// </summary>
        /// <param name="evt"></param>
        /// <returns></returns>
        public static int GetKeyCode(int evt)
        {
            return evt & ~KeyModifyers.AllModifyers;
        }
        // ===============================================================================================
        // PseudoCode Generator 
        //
        // The pseudocode looks like unique random key code (non existed in keyboard). Think about 
        // like it is: "pseudo-1", "pseudo-2",...,"pseudo-N"
        // Pseudo code has large keycode and the key modifyer Pseudo added
        // ===============================================================================================
        /// <summary>
        /// this pseudocode is reserved word "default" used for 
        /// default binding in keymaps
        /// </summary>
        public static int DefaultPseudoCode { get; private set; }
        /// <summary>
        /// Incremental value used for pseudocodes generator.
        /// </summary>
        private static int pseudoCodeIndex = 0;
        /// <summary>
        /// generate new pseudocode
        /// </summary>
        public static int GetPseudocodeOfName(string name)
        {
            int code;
            if (NameToKeyCodeTable.TryGetValue(name, out code))
                return code;
            code = pseudoCodeIndex++ | KeyModifyers.Pseudo;
            SetName(code, name);
            return code;
        }
        // ===============================================================================================
        // Conversion Tables
        // Those tables alow to convert keycode to the name, and reversed.
        // ===============================================================================================
        /// <summary>
        /// Table to convert name to the key code 
        /// </summary>
        public static Dictionary<string, int> NameToKeyCodeTable
        {
            get {
                if (nameToKeyCodeTable == null)
                    Initialize ();
                return nameToKeyCodeTable;
            }
        }
        /// <summary>
        /// Table to convert key code to the code name 
        /// </summary>
        public static Dictionary<int, string> KeyCodeToNameTable
        {
            get {
                if (keyCodeToNameTable == null)
                    Initialize ();
                return keyCodeToNameTable;
            }
        }
        /// <summary>
        /// Declarate new key code name
        /// SetName((int)KeyCode.RightCommand, "\\c-");
        /// </summary>
        public static void SetName(int keyCode, string name)
        {
            var modifyers = keyCode & KeyModifyers.AllModifyers;
            var keyCodeOnly = keyCode - modifyers;
            // The key code can be modifyer or the key
            // but it can't be bought
            UnityEngine.Debug.Assert(modifyers == 0 || keyCodeOnly == 0, keyCode);
            NameToKeyCodeTable[name] = keyCode;
            KeyCodeToNameTable[keyCode] = name;
        }
        /// <summary>
        /// Get name of key code code 
        /// </summary>
        public static string GetName(int keyCode)
        {
            var keyModifyers = keyCode & KeyModifyers.AllModifyers;
            var keyCodeOnly = keyCode - keyModifyers;
            var modifyerName = string.Empty;
            if (keyModifyers != 0)
            {
                foreach (var m in KeyModifyers.AllModifyersList)
                {
                    if (IsModifyer(keyModifyers, m))
                    {
                        string name;
                        if (KeyCodeToNameTable.TryGetValue(m, out name))
                            modifyerName += name;
                        else
                            throw new Exception(string.Format("Unexpected modifyer in keycode '{0:X}'", keyCode));
                    }
                }
            }
            string keyCodeName;
            if (KeyCodeToNameTable.TryGetValue(keyCodeOnly, out keyCodeName))
                return modifyerName + keyCodeName;
            else if (keyCodeOnly < 32 && keyModifyers == 0)
                return string.Format("^{0}", (char)(keyCodeOnly + 0x40));
            else
                throw new Exception(string.Format("Unexpected keycode '{0:X}'", keyCode));
        }
        /// <summary>
        /// Get keycode by name
        /// </summary>
        private static int GetKeyCodeInternal(string name)
        {
            int code;
            if (NameToKeyCodeTable.TryGetValue(name, out code))
                return code;
            throw new Exception(string.Format("Expected key code name, found '{0}'", name));
        }
        // ===============================================================================================
        // EMACS keycode expressions parser
        // For convertion from expression like: "C-v" to convert to the keycode
        // ===============================================================================================
        /// <summary>
        /// Parse the expression without spaces
        /// </summary>
        /// <param name="expression"></param>
        /// <returns></returns>
        public static int ParseExpression(string expression)
        {
            if (expression == null) throw new ArgumentNullException("expression");
            if (expression == string.Empty) throw new ArgumentException("expression");

            // There is the testing for the modifyer C- A- S-
            var m = expression[0];
            if (m == 'C' || m == 'A' || m == 'S')
            {
                var evt = ParseWordWithModifyers(expression);
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
        /// it the expression is badly formate
        /// </summary>
        private static int ParseWordWithModifyers(string expression)
        {
            if (expression == null) throw new ArgumentNullException("expression");

            var modifyers = 0;
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
                                modifyers |= KeyModifyers.Control;
                                break;

                            case 'S':
                                modifyers |= KeyModifyers.Shift;
                                break;

                            case 'A':
                                modifyers |= KeyModifyers.Alt;
                                break;

                            case 'M':
                                modifyers |= KeyModifyers.Meta;
                                break;

                            case 's':
                                modifyers |= KeyModifyers.Super;
                                break;

                            case 'H':
                                modifyers |= KeyModifyers.Hyper;
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
                    if (modifyers == KeyModifyers.Control && tmp < 256)
                        return tmp & 0x1F;
                    else
                        return MakeEvent(tmp, modifyers);
                }
                throw new Exception(string.Format("Expected character after C-,A-,S- found '{0}' in expression '{0:X}'", sufix, expression));
            }
            else
            {
                return modifyers;
            }
        }
        // ===============================================================================================
        // The initialization block
        // ===============================================================================================
        /// <summary>
        /// Initialize the class
        /// <summary>
        public static void Initialize ( )
        {
            nameToKeyCodeTable = new Dictionary<string, int> ( );
            keyCodeToNameTable = new Dictionary<int, string> ( );

            foreach ( var keyCode in Enum.GetValues ( typeof ( KeyCode ) ) )
                SetName ( (int)keyCode, keyCode.ToString ( ) );

            for ( var i = (int)'a' ; i < (int)( 'z' ) ; i++ )
                SetName ( i, ( (char)i ).ToString ( ) );

            SetName ( KeyModifyers.Shift, "S-" );
            SetName ( KeyModifyers.Control, "C-" );
            SetName ( KeyModifyers.Alt, "A-" );

            SetName ( (int)KeyCode.CapsLock, "\\_-" );
            SetName ( (int)KeyCode.Numlock, "\\N-" );

            SetName ( (int)KeyCode.LeftControl, "\\C-" );
            SetName ( (int)KeyCode.LeftAlt, "\\A-" );
            SetName ( (int)KeyCode.LeftShift, "\\S-" );
            SetName ( (int)KeyCode.LeftWindows, "\\W-" );
            SetName ( (int)KeyCode.LeftCommand, "\\c-" );


            SetName ( (int)KeyCode.RightControl, "\\C-" );
            SetName ( (int)KeyCode.RightAlt, "\\A-" );
            SetName ( (int)KeyCode.RightShift, "\\S-" );
            SetName ( (int)KeyCode.RightWindows, "\\W-" );
            SetName ( (int)KeyCode.RightCommand, "\\c-" );
            // pseudocode for default binding.
            DefaultPseudoCode = GetPseudocodeOfName ( "default" );
            SetName ( KeyModifyers.Pseudo, "P-" );
            SetName ( KeyModifyers.Pseudo, "\\P-" );
        }
        private static Dictionary<string, int> nameToKeyCodeTable;
        private static Dictionary<int, string> keyCodeToNameTable;
    }
}

  