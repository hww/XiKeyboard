/* Copyright (c) 2016 Valery Alex P. All rights reserved. */

using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;
using VARP.delegates;

namespace VARP.Keyboard
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
        /// <summary>
        /// Enable this buffer and makes it current
        /// </summary>
        public void Enable ( ) { CurentBuffer = this;  }
        /// <summary>
        /// Get name of this buffer
        /// </summary>
        public string Name { get { return name; } }
        /// <summary>
        /// Get help for this buffer
        /// </summary>
        public string Help { get { return help; } }
        /// <summary>
        /// Enable minor mode in this buffer
        /// </summary>
        /// <param name="mode"></param>
        public void EnabeMinorMode ( Mode mode )
        {
            if ( minorModes.Contains ( mode ) )
                return;
            mode.Enable ( );
            minorModes.Add ( mode );
        }
        /// <summary>
        /// Disable minor mode in this buffer
        /// </summary>
        /// <param name="mode"></param>
        public void DisableMinorMode ( Mode mode )
        {
            if ( !minorModes.Contains ( mode ) )
                return;
            mode.Disable ( );
            minorModes.Remove ( mode );
        }
        /// <summary>
        /// Enable major mode in this buffer
        /// </summary>
        /// <param name="mode"></param>
        public void EnabeMajorMode ( Mode mode )
        {
            mode.Enable ( );
            majorMode = mode;
        }
        /// <summary>
        /// Disable major mode in this buffer
        /// </summary>
        public void DisableMajorMode ( )
        {
            if ( majorMode == null )
                return;
            majorMode.Disable ( );
            majorMode = Mode.Null;
        }
        /// <summary>
        /// Lockup sequence for this buffer.
        /// </summary>
        public KeyMapItem Lookup ( int[] sequence, int starts, int ends, bool acceptDefaults )
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
        /// <summary>
        /// Get current buffer string
        /// </summary>
        /// <returns></returns>
        public string GetBufferString()
        {
            return textBuffer.GetBufferString();
        }
        /// <summary>
        /// Get buffer substring
        /// </summary>
        /// <param name="starts"></param>
        /// <param name="ends"></param>
        public string GetBufferSubString(int starts, int ends)
        {
            return textBuffer.GetBufferSubString(starts, ends);
        }
        /// <summary>
        /// Get curent cursor position
        /// </summary>
        /// <returns></returns>
        public int Point
        {
            get { return textBuffer.Point; }
            set { textBuffer.Point = value; }
        }
        /// <summary>
        /// Get current selection
        /// </summary>
        public void GetSelection(out int begin, out int end)
        {
            textBuffer.GetSelection(out begin, out end);
        }
        /// <summary>
        ///  Set selection
        /// </summary>
        /// <param name="begin"></param>
        /// <param name="end"></param>
        public void SetSelection(int begin, int end)
        {
            textBuffer.SetSelection( begin, end);
        }
        #endregion

        #region Lockup the keybinding recursively
        /// <summary>
        /// Main entry of all keys. Will find the binding for the curen mode
        /// end evaluate it
        /// </summary>
        public bool OnKeyDown(int evt)
        {
            textBuffer.InsertCharacter(evt);
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
            set {
                CurentBuffer.OnDisable ( );
                curentBuffer = value ?? Null;
                curentBuffer.OnEnable ( );
            }
        }
        private static Buffer curentBuffer;
        private static readonly Buffer Null = new Buffer ( "null", "Empty unused buffer" );
        #endregion

        #region Enable/Disable Hooks
        /// <summary>
        /// On enable buffer hook
        /// </summary>
        public FastAction<Buffer> OnEnableListeners = new FastAction<Buffer>();
        /// <summary>
        /// On disable buffer hook
        /// </summary>
        public FastAction<Buffer> OnDisableListeners = new FastAction<Buffer>();
        /// <summary>
        /// When some key sequence found
        /// </summary>
        public FastAction<Buffer,KeyMapItem> OnSequencePressed = new FastAction<Buffer, KeyMapItem>();
        #endregion
    }
}

