using System;
using UnityEngine;

namespace VARP.Keyboard
{
    /// <summary>This componet redirecting key events to current buffer</summary>
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

