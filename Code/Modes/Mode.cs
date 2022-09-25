/* Copyright (c) 2021 Valerya Pudova (hww) */

using XiCore.Delegates;
using XiKeyboard.KeyMaps;

namespace XiKeyboard.Modes
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
            onEnableListeners.Call(this);
        }
        /// <summary>
        /// Disable this mode
        /// </summary>
        public virtual void Disable()
        {
            onDisableListeners.Call(this);
        }
        /// <summary>
        /// Enable or disable Disable this mode
        /// </summary>
        public virtual void SetEnable(bool state)
        {
            if (state)
                Enable();
            else
                Disable();
        }
        // ===============================================================================================
        // Object's members
        // ===============================================================================================
        public KeyMap keyMap;           //< This is the curent mode key map
        public readonly string name;    //< The mode's name
        public readonly string help;    //< The mode's help
        private Mode parentMode;     //< The parent mode
        // ===============================================================================================
        // Hooks of mode
        // ===============================================================================================
        private readonly FastAction<Mode> onEnableListeners = new FastAction<Mode>();
        private readonly FastAction<Mode> onDisableListeners= new FastAction<Mode>();
        // ===============================================================================================
        // Statc members
        // ===============================================================================================
        /// <summary>
        /// Null mode. Returned instead of null
        /// </summary>
        public static readonly Mode Null = new Mode ( "null", "None unused mode", KeyMap.GlobalKeymap );
    }

}
