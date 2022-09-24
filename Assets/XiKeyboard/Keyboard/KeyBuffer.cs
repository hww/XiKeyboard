/* Copyright (c) 2021 Valerya Pudova (hww) */

using System;
using System.Collections.Generic;
using XiCore.Delegates;
using UnityEngine;

namespace XiKeyboard
{
    /// <summary>
    /// Each buffer is like a recipient of events. And only one of them receive events in this moment.
    /// As every buffer has its own modes activated, to switch buffer means switch the modes too.
    /// </summary>
    public sealed class KeyBuffer : IKeyBuffer
    {
        /// <summary>
        /// New buffer with name and optionally help info
        /// </summary>
        public KeyBuffer(string name, string help = null)
        {
            this.name = name ?? throw new ArgumentNullException(nameof(name));
            this.help = help;
            this.majorMode = KeyMode.Null;
        }

        #region IBuffer methods
        /// <summary>Enable this buffer and makes it current</summary>
        public void SetActive(bool state)
        {
            (currentBuffer ?? Null).OnDisable ( );
            currentBuffer = state ? this : Null;
            currentBuffer.OnEnable ( );
        }
        /// <summary>Get name of this buffer</summary>
        public string Name => name;

        /// <summary>Get help for this buffer</summary>
        public string Help => help;

        /// <summary>Enable minor mode in this buffer</summary>
        public void EnabeMinorMode ( KeyMode mode )
        {
            if ( minorModes.Contains ( mode ) )
                return;
            mode.Enable ( );
            minorModes.Add ( mode );
        }
        /// <summary>Disable minor mode in this buffer</summary>
        public void DisableMinorMode ( KeyMode mode )
        {
            if ( !minorModes.Contains ( mode ) )
                return;
            mode.Disable ( );
            minorModes.Remove ( mode );
        }
        /// <summary>Enable major mode in this buffer</summary>
        public void EnabeMajorMode ( KeyMode mode )
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
            majorMode = KeyMode.Null;
        }
        /// <summary>Lockup sequence for this buffer./// </summary>
        public DMKeyMapItem Lookup ( KeyEvent [] sequence, int starts, int ends, bool acceptDefaults )
        {
            if ( sequence == null )
                throw new ArgumentNullException ( nameof(sequence) );
            if ( starts < 0 || starts >= sequence.Length )
                throw new ArgumentOutOfRangeException ( nameof(starts) );
            if ( ends < starts || ends >= sequence.Length )
                throw new ArgumentOutOfRangeException ( nameof(ends) );
            // Minor modes searcg
            foreach ( var minorMode in minorModes )
            {
                var minorItem = minorMode.keyMap.LookupKey ( textBuffer.buffer, starts, textBuffer.BufferSize, acceptDefaults );
                if ( minorItem != null )
                    return minorItem;
            }
            // Major mode search
            var majorItem = majorMode.keyMap.LookupKey ( textBuffer.buffer, starts, textBuffer.BufferSize, acceptDefaults );
            if ( majorItem != null )
                return majorItem;
            // Global bindings search
            return KeyMap.GlobalKeymap.LookupKey ( textBuffer.buffer, starts, textBuffer.BufferSize, acceptDefaults );
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
            get => textBuffer.Point;
            set => textBuffer.Point = value;
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

        /// <summary>Main entry of all keys. Will find the binding for the current mode and evaluate it</summary>
        public bool OnKeyDown(KeyEvent evt)
        {
            textBuffer.InsertCharacter(evt);

            var result = Lookup(textBuffer.buffer, textBuffer.SequenceStarts, textBuffer.BufferSize, true);
            if (result == null)
            {
                // next time will scan from next character because nothing interesting before
                textBuffer.SequenceStarts = textBuffer.BufferSize;
                OnKeyPressed.Call(this, evt);
                return true;
            }
            if (result.value == null)
            {
                // the binding found but it does not do anything, then undo last sequence
                Debug.Log("Found sequence without binding " + result);
                textBuffer.ClearSequence();
                OnKeyPressed.Call(this, evt);
                return true;
            }
            if (result.value is KeyMap)
            {
                OnKeyPressed.Call(this, evt);
                return true;
            }
            if (result.IsPseudo)
            {
                // no reason to continue
                textBuffer.ClearSequence();
                OnPseudoPressed.Call(this, result);
                return true;
            }
            textBuffer.ClearSequence();
            OnSequencePressed.Call(this, result);
            return true;
        }
        // when buffer is enabling this method will be called
        private void OnEnable()
        {
            onEnableListeners.Call(this);
        }
        // when buffer is disabling this method will be called
        private void OnDisable()
        {
            onEnableListeners.Call(this);
        }
        #endregion

        #region Object's members

        private readonly string name;
        private readonly string help;
        private KeyMode majorMode;
        private readonly List<KeyMode> minorModes = new List<KeyMode> ( );
        private readonly TextBuffer textBuffer = new TextBuffer ( );
        #endregion

        #region Static members
        /// <summary>
        /// There is only one current buffer exists. This method returns or set current buffer
        /// </summary>
        public static KeyBuffer CurrentBuffer => currentBuffer ?? Null;

        private static KeyBuffer currentBuffer;
        private static readonly KeyBuffer Null = new KeyBuffer ( "null", "None unused buffer" );
        #endregion

        #region Enable/Disable Hooks
        /// <summary>On enable buffer hook</summary>
        public readonly FastAction<KeyBuffer> onEnableListeners = new FastAction<KeyBuffer>();
        /// <summary>On disable buffer hook</summary>
        public readonly FastAction<KeyBuffer> onDisableListeners = new FastAction<KeyBuffer>();
        /// <summary>When some key sequence found</summary>
        public static readonly FastAction<KeyBuffer,DMKeyMapItem> OnSequencePressed = new FastAction<KeyBuffer, DMKeyMapItem>();
        /// <summary>When keymap found</summary>
        public static readonly FastAction<KeyBuffer,DMKeyMapItem> OnPseudoPressed = new FastAction<KeyBuffer, DMKeyMapItem>();
        /// <summary>When key pressed</summary>
        public static readonly FastAction<KeyBuffer,KeyEvent> OnKeyPressed = new FastAction<KeyBuffer,KeyEvent>();
        #endregion
    }
}

