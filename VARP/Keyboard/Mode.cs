/* Copyright (c) 2016 Valery Alex P. All rights reserved. */

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
