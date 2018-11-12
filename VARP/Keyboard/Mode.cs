/* Copyright (c) 2016 Valery Alex P. All rights reserved. */

using System;

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
            if (onEnableListeners != null)
                onEnableListeners (this, null);
        }
        /// <summary>
        /// Disable this mode
        /// </summary>
        public virtual void Disable()
        {
            if (onDisableListeners != null)
                onDisableListeners (this, null);
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
        private EventHandler onEnableListeners;
        public event EventHandler OnEnableListeners
        {
            add {
                onEnableListeners -= value;
                onEnableListeners += value;
            }
            remove {
                onEnableListeners -= value;
            }
        }

        private EventHandler onDisableListeners;
        public event EventHandler OnDisableListeners
        {
            add {
                onDisableListeners -= value;
                onDisableListeners += value;
            }
            remove {
                onDisableListeners -= value;
            }
        }
        // ===============================================================================================
        // Statc members
        // ===============================================================================================
        /// <summary>
        /// Null mode. Returned instead of null
        /// </summary>
        public static Mode Null = new Mode ( "null", "Empty unused mode", KeyMap.GlobalKeymap );
    }


}
