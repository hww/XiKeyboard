using System;
using System.Collections;
using System.Collections.Generic;
using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.VR;

namespace VARP.Keyboard
{
    public class Buffer : IBuffer, IOnKeyDown
    {
        private EventHandler onEnableListeners;
        /// <summary>Listeners on enabling this buffer</summary>
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
        /// <summary>Listeners on disabling this buffer</summary>
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

        /// <summary>Current active buffer</summary>
        private static Buffer curentBuffer;
        /// <summary>Null buffer</summary>
        private static readonly Buffer Null = new Buffer("null", "Empty unused buffer");
        /// <summary>Buffer name</summary>
        private readonly string name;
        /// <summary>Buffer help</summary>
        private readonly string help;
        /// <summary>Buffer's major mode</summary>
        private Mode majorMode;
        /// <summary>Collection of buffer's minor modes</summary>
        private readonly List<Mode> minorModes = new List<Mode>();
        /// <summary>Iput buffer is an array of input keys</summary>
        private readonly InputBuffer inputBuffer = new InputBuffer();

        public Buffer([NotNull] string name, string help = null)
        {
            if (name == null) throw new ArgumentNullException("name");
            this.name = name;
            this.help = help;
            this.majorMode = Mode.Null;
        }

        #region IBuffer

        /// <summary>
        /// Main entry of all keys. Will find the binding for the curen mode
        /// end evaluate it
        /// </summary>
        /// <param name="evt"></param>
        public bool OnKeyDown(int evt)
        {
            inputBuffer.OnKeyDown(evt);
            var result = Lockup(inputBuffer.buffer, 0, inputBuffer.Count, true);
            if (result == null || result.value == null)
            {
                inputBuffer.Clear(); // no reason to continue
                return false;
            }

            Eval(result);
            return true;
        }

        /// <summary>
        /// Evaluate the keybinding. 
        /// </summary>
        /// <param name="item"></param>
        private void Eval(KeyMapItem item)
        {
            var value = item.value;
            
            if (value is KeyMap)
            {
                var o = value as KeyMap;
                // KeyMap without title is just keyMap
                if (o.Title == null)
                    return;
                // KeyMap with title behave as menu
                UiManager.I.CreateMenu(o, Vector3.zero, 200f);
            }
            else if (value is NativeFunction)                    
            {
                // native function
                var o = value as NativeFunction;
                var returns = o.Call();
                inputBuffer.Clear();
            }
            else if (value is string)
            {
                // string expression
                var o = value as string;
                NativeFunctionRepl.Instance.Evaluate(o);
            }
            else if (value is MenuLineBaseComplex)
            {

            }
            else if (value is MenuLineBaseSimple)
            {

            }
        }

        private void Eval(string function)
        {
            string[] args = function.Split(' ');
            
            var func = NativeFunction.Lockup(function);
        }

        public void Enable()
        {
            CurentBuffer = this;
        }

        protected virtual void OnEnable()
        {
            if (onEnableListeners != null) onEnableListeners(this, null);
        }

        protected virtual void OnDisable()
        {
            if (onDisableListeners != null) onDisableListeners(this, null);
        }

        public string Name {
            get { return name; } 
        }

        public string Help
        {
            get { return help; } 
        }

        public void EnabeMinorMode(Mode mode)
        {
            if (minorModes.Contains(mode))
                return;
            mode.OnEnable();
            minorModes.Add(mode);
        }

        public void DisableMinorMode(Mode mode)
        {
            if (!minorModes.Contains(mode))
                return;
            mode.OnDisable();
            minorModes.Remove(mode);
        }

        public void EnabeMajorMode(Mode mode)
        {
            mode.OnEnable();
            majorMode = mode;
        }

        public void DisableMajorMode()
        {
            if (majorMode == null) return;
            majorMode.OnDisable();
            majorMode = Mode.Null;
        }

        public KeyMapItem Lockup([NotNull] int[] sequence, int starts, int ends, bool acceptDefaults)
        {
            if (sequence == null) throw new ArgumentNullException("sequence");
            if (starts < 0 || starts >= sequence.Length) throw new ArgumentOutOfRangeException("starts");
            if (ends < starts || ends >= sequence.Length) throw new ArgumentOutOfRangeException("ends");

            foreach (var minorMode in minorModes)
            {
                var minorItem = minorMode.keyMap.LokupKey(inputBuffer.buffer, 0, inputBuffer.Count, acceptDefaults);
                if (minorItem != null)
                    return minorItem;
            }

            var majorItem = majorMode.keyMap.LokupKey(inputBuffer.buffer, 0, inputBuffer.Count, acceptDefaults);
            if (majorItem != null)
                return majorItem;

            return KeyMap.GlobalKeymap.LokupKey(inputBuffer.buffer, 0, inputBuffer.Count, acceptDefaults);
        }

        #endregion

        public static Buffer CurentBuffer
        {
            get { return curentBuffer ?? Null; }
            set
            {
                CurentBuffer.OnDisable();
                curentBuffer = value ?? Null;
                curentBuffer.OnEnable();
            }
        }

        #region Nested Types

        /// <summary>The line of characters collected in the array</summary>
        public class InputBuffer
        {
            /// <summary>Collection of input keys</summary>
            public readonly int[] buffer = new int[32];
            /// <summary>Constructor</summary>
            public InputBuffer()
            {
                Count = 0;
            }
            /// <summary>Called every key down event</summary>
            public virtual void OnKeyDown(int evt)
            {
                if (Count >= buffer.Length)
                    Clear();
                buffer[Count++] = evt;
            }
            /// <summary>Clear this buffer</summary>
            public void Clear()
            {
                Count = 0;
            }
            /// <summary>Get size of this buffer</summary>
            public int Count { get; private set; }
            /// <summary>Print buffer content</summary>
            public override string ToString()
            {
                var s = "";
                for (var i = 0; i < Count; i++)
                {
                    s += Event.GetName(buffer[i]);
                }
                return s;
            }
        }

        #endregion
    }

}

