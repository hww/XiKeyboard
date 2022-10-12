/* Copyright (c) 2021 Valerya Pudova (hww) */

using UnityEngine;
using XiKeyboard.Buffers;

namespace XiKeyboard.KeyMaps
{
    ///-------------------------------------------------------------------------------------------------
    /// <summary>   This component redirecting key events to current buffer. </summary>
    ///
    ///-------------------------------------------------------------------------------------------------

    internal static class InputManager 
    {
        /// <summary>   The previous key. </summary>
        private static KeyEvent _previousKey;
        /// <summary>   The repeat time. </summary>
        private static float _repeatTime;
        /// <summary>   (Immutable) the repeat delay. </summary>
        private const float kRepeatDelay = 0.75f;
        /// <summary>   (Immutable) the repeat interval. </summary>
        private const float kRepeatInterval = 0.1f;

        ///-------------------------------------------------------------------------------------------------
        /// <summary>   Make a key from the current Unity 3D input manager. </summary>
        ///
        ///
        /// <param name="currentTime">  The current time. </param>
        ///
        /// <returns>   The key. </returns>
        ///-------------------------------------------------------------------------------------------------

        private static KeyEvent GetKey(float currentTime)
        {
            var evt = UnityEngine.Event.current;
            KeyEvent keyEvt = KeyEvent.None;

            if (evt.type == EventType.KeyDown || evt.type == EventType.KeyUp)
            {
                if (evt.keyCode == KeyCode.None) return KeyEvent.None;
                if (evt.keyCode >= KeyCode.RightShift && evt.keyCode <= KeyCode.RightWindows) return KeyEvent.None;
                // read modifiers
                var modifiers = 0;
                if (evt.shift) modifiers |= KeyModifiers.Shift;
                if (evt.control) modifiers |= KeyModifiers.Control;
                if (evt.alt) modifiers |= KeyModifiers.Alt;
                // create the event
                keyEvt = KeyEvent.MakeEvent((int)evt.keyCode, modifiers);
                // initialize autorepeater
                if (evt.type == EventType.KeyUp)
                {
                    if (_previousKey == keyEvt)
                        _previousKey = KeyEvent.None;
                }
                else if (evt.type == EventType.KeyDown)
                {
                    _previousKey = keyEvt;
                    _repeatTime = currentTime + kRepeatDelay;
                    return keyEvt;
                }
            }
            // 
            if (_previousKey != KeyEvent.None && _repeatTime < currentTime)
            {
                _repeatTime = currentTime + kRepeatInterval;
                return _previousKey;
            }
            return KeyEvent.None;
        }

        ///-------------------------------------------------------------------------------------------------
        /// <summary>   Unity will call it to deliver message. </summary>
        ///
        ///-------------------------------------------------------------------------------------------------

        public static void OnGUI()
        {
            var evt = GetKey(Time.unscaledTime);
            if (evt != KeyEvent.None)
            {
                // send to current buffer
                Buffer.CurrentBuffer.OnKeyDown(evt);
            }
        }
    }
}

