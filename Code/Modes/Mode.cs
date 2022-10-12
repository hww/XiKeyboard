/* Copyright (c) 2021 Valerya Pudova (hww) */

using XiCore.Delegates;
using XiKeyboard.KeyMaps;

namespace XiKeyboard.Modes
{
    ///-------------------------------------------------------------------------------------------------
    /// <summary>   The keyboard mode. </summary>
    ///
    ///-------------------------------------------------------------------------------------------------

    public class Mode
    {
        ///-------------------------------------------------------------------------------------------------
        /// <summary>   Constructor without parent. </summary>
        ///
        ///
        /// <param name="name">     The name. </param>
        /// <param name="help">     (Optional) The help. </param>
        /// <param name="keyMap">   (Optional) The key map. </param>
        ///-------------------------------------------------------------------------------------------------

        public Mode(string name, string help = null, KeyMap keyMap = null)
        {
            this.name = name;
            this.help = help;
            this.keyMap = keyMap;
        }

        ///-------------------------------------------------------------------------------------------------
        /// <summary>   Construct with parent node. </summary>
        ///
        ///
        /// <param name="parentMode">   The parent mode. </param>
        /// <param name="name">         The name. </param>
        /// <param name="help">         (Optional) The help. </param>
        /// <param name="keyMap">       (Optional) The key map. </param>
        ///-------------------------------------------------------------------------------------------------

        public Mode(Mode parentMode, string name, string help = null, KeyMap keyMap = null)
        {
            this.parentMode = parentMode;
            this.name = name;
            this.help = help;
            this.keyMap = keyMap;
        }

        ///-------------------------------------------------------------------------------------------------
        /// <summary>   Enable this mode. </summary>
        ///
        ///-------------------------------------------------------------------------------------------------

        public virtual void Enable()
        {
            onEnableListeners.Call(this);
        }

        ///-------------------------------------------------------------------------------------------------
        /// <summary>   Disable this mode. </summary>
        ///
        ///
        /// <returns>   An avoid. </returns>
        ///-------------------------------------------------------------------------------------------------

        public virtual void Disable()
        {
            onDisableListeners.Call(this);
        }

        ///-------------------------------------------------------------------------------------------------
        /// <summary>   Enable or disable Disable this mode. </summary>
        ///
        ///
        /// <param name="state">    True to state. </param>
        ///-------------------------------------------------------------------------------------------------

        public virtual void SetEnable(bool state)
        {
            if (state)
                Enable();
            else
                Disable();
        }

        /// <summary>   This is the curent mode key map. </summary>
        public KeyMap keyMap;
        
        /// <summary>   (Immutable) The mode's name. </summary>
        public readonly string name;
        
        /// <summary>   (Immutable) The mode's help. </summary>
        public readonly string help;
        /// <summary>   The parent mode. </summary>
        private Mode parentMode;

        /// <summary>   (Immutable) the on enable listeners. </summary>
        private readonly FastAction<Mode> onEnableListeners = new FastAction<Mode>();

        /// <summary>   (Immutable) the on disable listeners. </summary>
        private readonly FastAction<Mode> onDisableListeners= new FastAction<Mode>();

        ///-------------------------------------------------------------------------------------------------
        /// <summary>
        /// (Immutable)
        /// Null mode. Returned instead of null.
        /// </summary>
        ///-------------------------------------------------------------------------------------------------

        public static readonly Mode Null = new Mode ( "null", "None unused mode", KeyMap.GlobalKeymap );
    }

}
