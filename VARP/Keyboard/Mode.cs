using System;
using System.Collections.Generic;
using UnityEngine.UI;

namespace VARP.Keyboard
{
    public class Mode
    {
        private EventHandler onEnableListeners;
        /// <summary>Activate mode listeners</summary>
        public event EventHandler OnEnableListeners
        {
            add
            {
                onEnableListeners -= value;
                onEnableListeners += value;
            }
            remove
            {
                onEnableListeners -= value;
            }
        }

        private EventHandler onDisableListeners;
        /// <summary>Inactivate mode listeners</summary>
        public event EventHandler OnDisableListeners
        {
            add
            {
                onDisableListeners -= value;
                onDisableListeners += value;
            }
            remove
            {
                onDisableListeners -= value;
            }
        }

        /// <summary>Null mode</summary>
        public static Mode Null = new Mode("null", "Empty unused mode", KeyMap.GlobalKeymap);
        /// <summary>This is the curent mode key map</summary>
        public KeyMap keyMap;
        /// <summary>The mode name</summary>
        public readonly string name;
        /// <summary>Mode help</summary>
        public readonly string help;
        /// <summary>When this mode recogni</summary>
        private Mode parentMode;

        public Mode(string name, string help = null, KeyMap keyMap = null)
        {
            this.name = name;
            this.help = help;
            this.keyMap = keyMap;
        }

        public Mode(Mode parentMode, string name, string help = null, KeyMap keyMap = null)
        {
            this.parentMode = parentMode;
            this.name = name;
            this.help = help;
            this.keyMap = keyMap;
        }

        #region Mode

        public virtual void OnEnable()
        {
            if (onEnableListeners != null) onEnableListeners(this, null);
        }

        public virtual void OnDisable()
        {
            if (onDisableListeners != null) onDisableListeners(this, null);
        }

        #endregion
    }


}
