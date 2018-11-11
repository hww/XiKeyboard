using System;
using System.Collections.Generic;
using JetBrains.Annotations;
using UnityEngine;

namespace VARP.Keyboard
{

    public partial struct KeyEvent
    {
        public int Code;

        public KeyEvent(char c) { Code = c; }
        public KeyEvent(int code) { Code = code; }
        public KeyEvent(int code, int modifyers) { Code = MakeEventCode(code, modifyers); }
        public KeyEvent(KeyCode code) { Code = (int)code; }
        public KeyEvent(KeyEvent evt) { Code = evt.Code; }
        public KeyEvent([NotNull] string expression) { Code = ParseExpression(expression) ; }
        
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
        
        // -- Debuging ---------------------------------------------------------------------------
        
        public override string ToString() { return Code.ToString( ); }
    }

}

  