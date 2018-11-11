using System;
using System.Collections.Generic;
using JetBrains.Annotations;
using UnityEngine;

namespace VARP.Keyboard
{

    public partial struct KeyEvent
    {
        /// <summary>The keycode include modifyers</summary>
        public int Code;

        public KeyEvent(char c) { Code = c; }
        public KeyEvent(int code) { Code = code; }
        public KeyEvent(int code, int modifyers) { Code = MakeEventCode(code, modifyers); }
        public KeyEvent(KeyCode code) { Code = (int)code; }
        public KeyEvent(KeyEvent evt) { Code = evt.Code; }
        public KeyEvent([NotNull] string expression) { Code = ParseExpression(expression) ; }
        
        public bool IsControlCode() { return (Code < 32); }
        public bool IsModifyer(int modifyers) { return (Code & modifyers) == modifyers; }
        public bool IsValid() { return Code >= 0 && Code < KeyModifyers.MaxCode; }
        public bool IsNotValid() { return Code < 0 && Code >= KeyModifyers.MaxCode; }
        public bool HasModifyers() { return Code >= KeyModifyers.Alt; }
        public KeyEvent GetModifyers() { return new KeyEvent(Code & KeyModifyers.AllModifyers); }
        public KeyEvent GetKeyCode() { return new KeyEvent(Code & ~KeyModifyers.AllModifyers); }
 
        // -- Cast to other type  --------------------------------------------------------------
        
        // explicit conversion: var str = (string)fooName
        public static explicit operator string ( KeyEvent evt ) { return evt.ToString(); }

        // implicit conversion: KeyEvent name = EName.None
        public static implicit operator KeyEvent ( KeyCode keyCode ) { return new KeyEvent(keyCode); }
        
        // implicit conversion: KeyEvent name = 12
        public static implicit operator KeyEvent ( int keyCode ) { return new KeyEvent(keyCode); }
        
        // -- Comparison ------------------------------------------------------------------------

        public override bool Equals ( object obj )
        {
            if ( obj == null || GetType ( ) != obj.GetType ( ) )
                return false;
            return Code == ((KeyEvent)obj).Code ;
        }
        public override int GetHashCode ( ) { return Code; }
        public static bool operator == ( KeyEvent x, KeyEvent y ) { return x.Code == y.Code; }
        public static bool operator != ( KeyEvent x, KeyEvent y ) { return x.Code != y.Code; }
        
        // -- Casting ---------------------------------------------------------------------------
        
        /// <summary>
        /// Get name of key code code 
        /// </summary>
        public string GetName()
        {
            var keyModifyers = GetModifyers();
            var keyCodeOnly = GetKeyCode();
            var modifyerName = string.Empty;
            if (keyModifyers != 0)
            {
                foreach (var m in KeyModifyers.AllModifyersList)
                {
                    if (IsModifyer(m))
                    {
                        string name;
                        if (KeyCodeToNameTable.TryGetValue(m, out name))
                            modifyerName += name;
                        else
                            throw new Exception(string.Format("Unexpected modifyer in keycode '{0:X}'", this));
                    }
                }
            }
            string keyCodeName;
            if (KeyCodeToNameTable.TryGetValue(keyCodeOnly, out keyCodeName))
                return modifyerName + keyCodeName;
            else if (IsControlCode())
                return string.Format("^{0}", (char)(keyCodeOnly.Code + 0x40));
            else
                throw new Exception(string.Format("Unexpected keycode '{0:X}'", Code));
        }
        
        // -- Debuging ---------------------------------------------------------------------------
        
        public override string ToString() { return Code.ToString( ); }
    }

}

  