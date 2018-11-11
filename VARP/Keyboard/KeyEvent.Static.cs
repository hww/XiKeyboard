using System;
using System.Collections.Generic;
using JetBrains.Annotations;
using UnityEngine;

namespace VARP.Keyboard
{
    public partial struct KeyEvent
    {
        /// <summary>
        /// Create nev event from code and use new modifyers
        /// </summary>
        /// <param name="keyCode"></param>
        /// <param name="modifyers"></param>
        public static int MakeEventCode(int keyCode, int modifyers)
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

        #region PseudoCode Generator
        
        /// <summary>
        /// this pseudocode is reserved word "default" used for 
        /// default binding in keymaps
        /// </summary>
        public static KeyEvent DefaultPseudoCode { get; private set; }
        /// <summary>
        /// pseudocodes generator
        /// </summary>
        private static int pseudoCodeIndex = 0;
        
        /// <summary>
        /// generate new pseudocode
        /// </summary>
        public static KeyEvent GetPseudocodeOfName(string name)
        {
            KeyEvent code;
            if (NameToKeyCodeTable.TryGetValue(name, out code))
                return code;
            code = new KeyEvent(pseudoCodeIndex++ | KeyModifyers.Pseudo);
            SetName(code, name);
            return code;
        }

        #endregion

        private static bool Initialized;
        private static Dictionary<string, KeyEvent> NameToKeyCodeTable = new Dictionary<string, KeyEvent>();
        private static Dictionary<KeyEvent, string> KeyCodeToNameTable = new Dictionary<KeyEvent, string>();

        /// <summary>
        /// Initialize the class
        /// </summary>
        public static void Initialize()
        {
            Debug.Assert(Initialized == false);
            
            Initialized = true;
            
            foreach (var keyCode in Enum.GetValues(typeof(KeyCode)))
                SetName((int) keyCode, keyCode.ToString());

            for (var i = (int) 'a'; i < (int) ('z'); i++)
                SetName(i, ((char) i).ToString());

            SetName(KeyModifyers.Shift, "S-");
            SetName(KeyModifyers.Control, "C-");
            SetName(KeyModifyers.Alt, "A-");

            SetName(KeyCode.CapsLock, "\\_-");
            SetName(KeyCode.Numlock, "\\N-");

            SetName(KeyCode.LeftControl, "\\C-");
            SetName(KeyCode.LeftAlt, "\\A-");
            SetName(KeyCode.LeftShift, "\\S-");
            SetName(KeyCode.LeftWindows, "\\W-");
            SetName(KeyCode.LeftCommand, "\\c-");

            SetName(KeyCode.RightControl, "\\C-");
            SetName(KeyCode.RightAlt, "\\A-");
            SetName(KeyCode.RightShift, "\\S-");
            SetName(KeyCode.RightWindows, "\\W-");
            SetName(KeyCode.RightCommand, "\\c-");

            // pseudocode for default binding.
            DefaultPseudoCode = GetPseudocodeOfName("default");
            SetName(KeyModifyers.Pseudo, "P-");
            SetName(KeyModifyers.Pseudo, "\\P-");
        }

        /// <summary>
        /// Declarate new key code name
        /// </summary>
        public static void SetName(KeyEvent keyCode, string name)
        {
            var modifyers = keyCode.GetModifyers();
            var keyCodeOnly = keyCode.GetKeyCode();
            // The key code can be modifyer or the key
            // but it can't be bought
            UnityEngine.Debug.Assert(modifyers == 0 || keyCodeOnly == 0, keyCode);
            NameToKeyCodeTable[name] = keyCode;
            KeyCodeToNameTable[keyCode] = name;
        }

        public static string GetName(KeyEvent keyCode)
        {
            var modifyers = keyCode.GetModifyers();
            var keyCodeOnly = keyCode.GetKeyCode();

            var modifyerName = string.Empty;
            if (modifyers != 0)
            {
                foreach (var m in KeyModifyers.AllModifyersList)
                {
                    if (modifyers.IsModifyer(m))
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
            else if (keyCodeOnly.Code < 32 && modifyers.Code == 0)
                return string.Format("^{0}", (char) (keyCodeOnly.Code + 0x40));
            else
                throw new Exception(string.Format("Unexpected keycode '{0:X}'", keyCode));
        }

        /// <summary>
        /// Get keycode by name
        /// </summary>
        private static int GetKeyCodeInternal(string name)
        {
            KeyEvent code;
            if (NameToKeyCodeTable.TryGetValue(name, out code))
                return code.Code;
            throw new Exception(string.Format("Expected key code name, found '{0}'", name));
        }

        /// <summary>
        /// Parse the expression without spaces
        /// </summary>
        /// <param name="expression"></param>
        public static int ParseExpression([NotNull] string expression)
        {
            if (expression == null) throw new ArgumentNullException("expression");
            if (expression == string.Empty) throw new ArgumentException("expression");

            // There is the testing for the modifyer C- A- S-
            var m = expression[0];
            if (m == 'C' || m == 'A' || m == 'S')
            {
                var evtCode = ParseWordWithModifyers(expression);
                if (evtCode >= 0)
                {
                    return evtCode;
                }
            }

            // There is test for named character Shift, LeftAlt, Space
            return GetKeyCodeInternal(expression);
        }

        /// <summary>
        /// Support only single token with '-' character inside
        /// Such as C- A- S- the function produce an exception
        /// it the expression is badly formate
        /// </summary>
        private static int ParseWordWithModifyers([NotNull] string expression)
        {
            if (expression == null) throw new ArgumentNullException("expression");

            var modifyers = 0;
            var sufix = string.Empty;

            var index = 0;
            while (index < expression.Length)
            {
                var c1 = expression[index]; // C,A,S,...
                var i2 = index + 1;

                if (i2 < expression.Length)
                {
                    var c2 = expression[i2]; // Here is '-'

                    if (c2 == '-')
                    {
                        index += 2; // skip c1 and c2
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
                        return MakeEventCode(tmp, modifyers);
                }

                throw new Exception(string.Format("Expected character after C-,A-,S- found '{0}' in expression '{0:X}'",
                    sufix, expression));
            }
            else
            {
                return modifyers;
            }
        }
    }
}