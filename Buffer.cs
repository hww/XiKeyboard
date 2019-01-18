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
using System.Collections.Generic;
using Plugins.VARP.delegates;
using UnityEngine;

namespace Plugins.VARP.Keyboard
{
    /// <summary>
    /// Each buffer is like a recipient of events. And only one of them receive events in this moment.
    /// As every buffer has its own modes activated, to switch buffer means switch the modes too.
    /// </summary>
    public partial class Buffer : IBuffer
    {
        /// <summary>
        /// New buffer with name and optinaly help info
        /// </summary>
        public Buffer(string name, string help = null)
        {
            if (name == null) throw new ArgumentNullException("name");
            this.name = name;
            this.help = help;
            this.majorMode = Mode.Null;
        }

        #region IBuffer methods
        /// <summary>Enable this buffer and makes it current</summary>
        public void SetActive(bool state)
        {
            (curentBuffer ?? Null).OnDisable ( );
            curentBuffer = state ? this : Null;
            curentBuffer.OnEnable ( );
        }
        /// <summary>Get name of this buffer</summary>
        public string Name { get { return name; } }
        /// <summary>Get help for this buffer</summary>
        public string Help { get { return help; } }

        /// <summary>Enable minor mode in this buffer</summary>
        public void EnabeMinorMode ( Mode mode )
        {
            if ( minorModes.Contains ( mode ) )
                return;
            mode.Enable ( );
            minorModes.Add ( mode );
        }
        /// <summary>Disable minor mode in this buffer</summary>
        public void DisableMinorMode ( Mode mode )
        {
            if ( !minorModes.Contains ( mode ) )
                return;
            mode.Disable ( );
            minorModes.Remove ( mode );
        }
        /// <summary>Enable major mode in this buffer</summary>
        public void EnabeMajorMode ( Mode mode )
        {
            mode.Enable ( );
            majorMode = mode;
        }
        /// <summary>Disable major mode in this buffer</summary>
        public void DisableMajorMode ( )
        {
            if ( majorMode == null )
                return;
            majorMode.Disable ( );
            majorMode = Mode.Null;
        }
        /// <summary>Lockup sequence for this buffer./// </summary>
        public KeyMapItem Lookup ( Event [] sequence, int starts, int ends, bool acceptDefaults )
        {
            if ( sequence == null )
                throw new ArgumentNullException ( "sequence" );
            if ( starts < 0 || starts >= sequence.Length )
                throw new ArgumentOutOfRangeException ( "starts" );
            if ( ends < starts || ends >= sequence.Length )
                throw new ArgumentOutOfRangeException ( "ends" );
            // Minor modes searcg
            foreach ( var minorMode in minorModes )
            {
                var minorItem = minorMode.keyMap.LookupKey ( textBuffer.buffer, 0, textBuffer.BufferSize, acceptDefaults );
                if ( minorItem != null )
                    return minorItem;
            }
            // Major mode search
            var majorItem = majorMode.keyMap.LookupKey ( textBuffer.buffer, 0, textBuffer.BufferSize, acceptDefaults );
            if ( majorItem != null )
                return majorItem;
            // Global bindings search
            return KeyMap.GlobalKeymap.LookupKey ( textBuffer.buffer, 0, textBuffer.BufferSize, acceptDefaults );
        }
        /// <summary>Get current buffer string</summary>
        public string GetBufferString() { return textBuffer.GetBufferString(); }
        /// <summary>Get current buffer humanized string</summary>
        public string GetBufferHumanizedString() { return textBuffer.GetBufferHumanizedString(); }
        /// <summary>Get buffer substring</summary>
        public string GetBufferSubString(int starts, int ends)
        {
            return textBuffer.GetBufferSubString(starts, ends);
        }
        /// <summary>Get curent cursor position</summary>
        public int Point
        {
            get { return textBuffer.Point; }
            set { textBuffer.Point = value; }
        }
        /// <summary>Get current selection</summary>
        public void GetSelection(out int begin, out int end)
        {
            textBuffer.GetSelection(out begin, out end);
        }
        /// <summary>Set selection</summary>
        public void SetSelection(int begin, int end)
        {
            textBuffer.SetSelection( begin, end);
        }
        #endregion

        #region Lockup the keybinding recursively

        /// <summary>Main entry of all keys. Will find the binding for the curen modeend evaluate it</summary>
        public bool OnKeyDown(Event evt)
        {
            textBuffer.InsertCharacter(evt);
            OnKeyPressed.Call(this, evt);
            var result = Lookup(textBuffer.buffer, textBuffer.SequenceStarts, textBuffer.BufferSize, true);
            if (result == null)
            {
                // next time will scan from next character because nothing interesting before
                textBuffer.SequenceStarts = textBuffer.BufferSize;
                return true;
            }
            else if (result.value == null)
            {
                // the binding found but it does not do anything, then undo last sequence
                textBuffer.BufferSize = textBuffer.SequenceStarts;
                Debug.Log("Found sequence without bindng " + result.ToString());
                textBuffer.Clear(); // no reason to continue
            }
            OnSequencePressed.Call(this, result);
            return true;
        }
        // when buffer is enabling this method will be called
        protected virtual void OnEnable()
        {
            OnEnableListeners.Call(this);
        }
        // when buffer is disabling this method will be called
        protected virtual void OnDisable()
        {
            OnEnableListeners.Call(this);
        }
        #endregion

        #region Object's members

        private readonly string name;
        private readonly string help;
        private Mode majorMode;
        private readonly List<Mode> minorModes = new List<Mode> ( );
        private readonly TextBuffer textBuffer = new TextBuffer ( );
        #endregion

        #region Static members
        /// <summary>
        /// There is only one current buffer exists. This method returns or set curent buffer
        /// </summary>
        public static Buffer CurentBuffer
        {
            get { return curentBuffer ?? Null; }
        }
        private static Buffer curentBuffer;
        private static readonly Buffer Null = new Buffer ( "null", "Empty unused buffer" );
        #endregion

        #region Enable/Disable Hooks
        /// <summary>On enable buffer hook</summary>
        public readonly FastAction<Buffer> OnEnableListeners = new FastAction<Buffer>();
        /// <summary>On disable buffer hook</summary>
        public readonly FastAction<Buffer> OnDisableListeners = new FastAction<Buffer>();
        /// <summary>When some key sequence found</summary>
        public static readonly FastAction<Buffer,KeyMapItem> OnSequencePressed = new FastAction<Buffer, KeyMapItem>();
        /// <summary>When key pressed</summary>
        public static readonly FastAction<Buffer,Event> OnKeyPressed = new FastAction<Buffer,Event>();
        #endregion
    }
}

