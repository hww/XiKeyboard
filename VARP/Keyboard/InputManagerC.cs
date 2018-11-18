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
using UnityEngine;

namespace VARP.Keyboard
{
    /// <summary>This component redirecting key events to current buffer</summary>
    public class InputManagerC : MonoBehaviour
    {
        /// <summary>Unity will call it to deliver message</summary>
        private void OnGUI()
        {
            var evt = UnityEngine.Event.current;
            if (evt.type == EventType.KeyDown)
            {
                if (evt.keyCode == KeyCode.None) return;
                if (evt.keyCode >= KeyCode.RightShift && evt.keyCode <= KeyCode.RightWindows) return;
                // read modifiers
                int modifyers = 0;
                if (evt.shift) modifyers |= KeyModifyers.Shift;
                if (evt.control) modifyers |= KeyModifyers.Control;
                if (evt.alt) modifyers |= KeyModifyers.Alt;
                // create the event
                var keyevt = Event.MakeEvent((int)evt.keyCode, modifyers);
                // send to curent buffer
                if (Buffer.CurentBuffer.OnKeyDown(keyevt))
                    return;
            }
        }
    }
}

