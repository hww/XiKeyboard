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

using UnityEngine;

namespace XiKeyboard
{
    /// <summary>This component redirecting key events to current buffer</summary>
    public class KeyInputManager
    {
        private static KeyEvent _previousKey;
        private static float _repeatTime;
        private const float kRepeatDelay = 0.75f;
        private const float kRepeatInterval = 0.1f;

        /// <summary>
        /// Make a key from the current Unity 3D input manager
        /// </summary>
        /// <returns></returns>
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


        /// <summary>Unity will call it to deliver message</summary>
        public static void OnGUI()
        {
            var evt = GetKey(Time.unscaledTime);
            if (evt != KeyEvent.None)
            {
                // send to current buffer
                KeyBuffer.CurrentBuffer.OnKeyDown(evt);
            }
        }
    }
}

