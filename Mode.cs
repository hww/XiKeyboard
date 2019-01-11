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
using TMPro;

namespace VARP.Keyboard
{
    public class Mode
    {
        /// <summary>
        /// Constructor without parent
        /// </summary>
        public Mode(string name, string help = null, KeyMap keyMap = null)
        {
            this.name = name;
            this.help = help;
            this.keyMap = keyMap;
        }
        /// <summary>
        /// Construct with parent node
        /// </summary>
        public Mode(Mode parentMode, string name, string help = null, KeyMap keyMap = null)
        {
            this.parentMode = parentMode;
            this.name = name;
            this.help = help;
            this.keyMap = keyMap;
        }
        /// <summary>
        /// Enable this mode
        /// </summary>
        public virtual void Enable()
        {
            OnEnableListeners.Call(this);
        }
        /// <summary>
        /// Disable this mode
        /// </summary>
        public virtual void Disable()
        {
            OnDisableListeners.Call(this);
        }
        // ===============================================================================================
        // Object's members
        // ===============================================================================================
        public KeyMap keyMap;           //< This is the curent mode key map
        public readonly string name;    //< The mode's name
        public readonly string help;    //< The mode's help
        private Mode parentMode;        //< The parent mode
        // ===============================================================================================
        // Hooks of mode
        // ===============================================================================================
        private readonly FastAction<Mode> OnEnableListeners = new FastAction<Mode>();
        private readonly FastAction<Mode> OnDisableListeners= new FastAction<Mode>();
        // ===============================================================================================
        // Statc members
        // ===============================================================================================
        /// <summary>
        /// Null mode. Returned instead of null
        /// </summary>
        public static Mode Null = new Mode ( "null", "Empty unused mode", KeyMap.GlobalKeymap );
    }

}
