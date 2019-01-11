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

namespace VARP.Keyboard
{
    /// <summary>
    /// This is simple keysequence binding to any key.
    /// </summary>
    public class SequenceBinding 
    {
        public string name;              //< Menu name, or key binding name
        public string help;              //< Help information for this bnding
        public readonly int[] sequence;  //< Sequence of events required to invoke it 

        public SequenceBinding(string name, int[] sequence, string help = null)
        {
            this.name = name;
            this.sequence = sequence;
            this.help = help;
        }
    }
    /// <summary>
    /// Any object wich can be binded to the keymap have to be based on this class
    /// </summary>
    public class KeyMapItem
    {
        public static readonly KeyMapItem Empty = new KeyMapItem(0, null);

        public int key;             //< this is the fake key
        public object value;        //< there can be any avaiable value

        public KeyMapItem(int key, object value)
        {
            this.key = key;
            this.value = value;
        }
        /// <summary>
        /// Compare with single event
        /// </summary>
        /// <param name="evt"></param>
        /// <returns></returns>
        public bool Compare(int evt)
        {
            return evt == key;
        }
        public override string ToString()
        {
            return string.Format("key: {0} value: {1}", key, value);
        }
    }

    /// <summary>
    /// This class have to alow to build the tree of the keymaps.
    /// Each keymap contains List of KeyMapItem(s). Additionaly 
    /// the keymap has title and help information as whell it 
    /// reffers to parent keymap.
    /// </summary>
    public class KeyMap 
    {
        /// <summary>
        /// The global keymap. It is represent top level of keymaps
        /// </summary>
        public static readonly KeyMap GlobalKeymap = new KeyMap("global-keymap");

        public string title;                //< title of keymap
        public string help;                 //< help for this item (used for menu only)
        public KeyMap parent;               //< parent key map
        public List<KeyMapItem> items;      //< kay map items
        // This binding can be returned when the requested key binding is not found
        public KeyMapItem defaultBinding;   //< default binding or null. 
        /// <summary>
        /// Get title name of this keymap
        /// </summary>
        public virtual string Title { get { return title; } }
        public virtual string Help { get { return help; } }
        /// <summary>
        /// Create emty keymap
        /// </summary>
        public KeyMap(string title = null, string help = null )
        {
            this.title = title;
            this.help = help;
            this.items = new List<KeyMapItem>();
        }
        /// <summary>
        /// Create empty keymap based on parent keymap
        /// </summary>
        public KeyMap(KeyMap parent, string title = null, string help = null )
        {
            if (parent == null) throw new ArgumentNullException("parent");
            this.title = title;
            this.help = help;
            this.parent = parent;
            items = new List<KeyMapItem>();
        }
        /// <summary>
        /// Copy this keymap to target one
        /// </summary>
        public virtual void CopyTo(KeyMap target)
        {
            target.title = title;
            target.title = help;
            target.parent = parent;
            foreach (var item in items)
                target.SetLocal(item.key, item.value);
        }
        // ===============================================================================================
        // SET & GET bindings in this keymap only
        // ===============================================================================================
        /// <summary>
        /// Set key value pair. Replace existing
        /// </summary>
        public virtual void SetLocal(Event evt, object value)
        {
            if (!evt.IsValid)
                throw new ArgumentOutOfRangeException("evt");
            var binding = new KeyMapItem(evt, value);
            var index = GetIndexOf(evt);
            if (index >= 0)
                items[index] = binding;
            else
                items.Add(binding);

            if (evt == Event.DefaultPseudocode)
                defaultBinding = binding;
        }
        /// <summary>
        ///  Get key binding of this keymap, returns: null or deffault binding (if allowed)
        /// </summary>
        public virtual KeyMapItem GetLocal(Event evt, bool acceptDefaults = false)
        {
            if (!evt.IsValid)
                throw new ArgumentOutOfRangeException("evt");
            var index = GetIndexOf(evt);
            if (index >= 0 && items[index].value != null)
                return items[index];
            return acceptDefaults ? defaultBinding : null;
        }
        /// <summary>
        /// Get index of element which has given sequence
        /// </summary>
        private int GetIndexOf ( int evt )
        {
            for ( var i = 0 ; i < items.Count ; i++ )
            {
                if ( items[ i ].Compare ( evt ) )
                    return i;
            }

            return -1;
        }
        // ===============================================================================================
        // Lockup the keybinding recursively
        // ===============================================================================================
        /// <summary>
        /// Lockup keymap item by full sequence of keys
        /// </summary>
        public virtual KeyMapItem LookupKey(Event[] sequence, bool acceptDefaults = false)
        {
            return LookupKey(sequence, 0, sequence.Length - 1, acceptDefaults);
        }
        /// <summary>
        /// Lockup keymap item by full sequence of keys
        /// </summary>
        /// <param name="sequence">Full sequence of keys</param>
        /// <param name="starts">First index in the sequence</param>
        /// <param name="ends">Last index in the sequence</param>
        /// <param name="acceptDefaults">Allow to return default binding</param>
        /// <returns>KeyMapItem or Null</returns>
        public virtual KeyMapItem LookupKey(Event [] sequence, int starts, int ends, bool acceptDefaults = false)
        {
            if (sequence == null) throw new ArgumentNullException("sequence");
            if (starts < 0 || starts >= sequence.Length) throw new ArgumentOutOfRangeException("starts");
            if (ends < starts || ends >= sequence.Length) throw new ArgumentOutOfRangeException("ends");

            var curentMap = this;
            var tmp = null as KeyMapItem;

            for (var i=starts; i < ends; i++)
            { 
                tmp = curentMap.GetLocal(sequence[i], acceptDefaults);
                if (tmp == null)
                    return curentMap.parent != null ? curentMap.parent.LookupKey(sequence, acceptDefaults) : null;

                var map = tmp.value as KeyMap;
                if (map != null)
                    curentMap = map;
                else
                    return tmp; //< we found binding and it is not key map
            }
            return tmp;
        }
        // ===============================================================================================
        // Define the keybinding recursively
        // ===============================================================================================
        /// <summary>Define list of key-strings. This way used for defining menu</summary>
        public bool Define(string[] sequence, object value)
        {
            var newsequence = Kbd.ParsePseudo(sequence);
            return Define(newsequence, value);
        }
        /// <summary>Define by string expression</summary>
        public bool Define(string sequence, object value)
        {
            var newsequence = Kbd.Parse(sequence);
            return Define(newsequence, value);
        }
        /// <summary>Define sequence with given binding</summary>
        public virtual bool Define(Event [] sequence, object value)
        {
            if (sequence == null) throw new ArgumentNullException("sequence");

            var curentMap = this;
            var lastIndex = sequence.Length - 1;

            for (var i = 0; i < sequence.Length; i++)
            {
                var key = sequence[i];
                var tmp = curentMap.GetLocal(key); // do not alow defaults
                if (tmp == null)
                {
                    // there is no this binding
                    // N.B. Do not look at the parent one!
                    if (i == lastIndex)
                    {
                        // the curentMap is the target map and it does not have definition 
                        curentMap.SetLocal(key, value);
                        return true;
                    }
                    else
                    {
                        // the curentMap is the map in the sequence and it does not have definition 
                        var newMap = new KeyMap();
                        curentMap.SetLocal(key, newMap);
                        curentMap = newMap;
                    }
                }
                else
                {
                    // we found binding in curentMap
                    if (i == lastIndex)
                    {
                        // curentMap is target map, it has binding but we have to redefine it
                        curentMap.SetLocal(key, value);
                    }
                    else
                    {
                        // the curentMap is the map in the sequence and it has definition 
                        var map = tmp.value as KeyMap;
                        if (map != null)
                            curentMap = map;
                        else
                            throw new Exception(string.Format("Expect KeyMap at '{0}' found: '{1}' in: '{2}'", sequence[i], tmp, sequence));
                    }
                }
            }
            throw new Exception("We can\'t be here");
        }
    }
    /// <summary>
    /// If an element of a keymap is a char-table, it counts as holding bindings for all
    /// character events with no modifier element n
    /// is the binding for the character with code n.This is a compact way to record
    /// lots of bindings.A keymap with such a char-table is called a full keymap. Other
    /// keymaps are called sparse keymaps.
    /// </summary>
    public class FullKeymap : KeyMap
    {
        private const int MaxSize = 2048;
        /// <summary>
        /// Create emty keymap
        /// </summary>
        /// <param name="title"></param>
        public FullKeymap(string title = null) : base(title)
        {
        }
        /// <summary>
        /// Create empty keymap based on parent keymap
        /// </summary>
        /// <param name="parent"></param>
        /// <param name="title"></param>
        public FullKeymap(KeyMap parent, string title = null) : base(parent, title)
        {
        }
        /// <summary>
        /// Set key value pair. Replace existing.
        /// </summary>
        /// <param name="evt"></param>
        /// <param name="value"></param>
        public override void SetLocal(Event evt, object value)
        {
            if (!evt.IsValid)
                throw new ArgumentOutOfRangeException("evt");

            // do not support keys with modificators
            if (evt >= KeyModifyers.Alt)
                throw new ArgumentOutOfRangeException("evt");

            // limit by some "rational" number ;)
            if (evt >= MaxSize)
                throw new ArgumentOutOfRangeException("evt");

            if (evt >= items.Count)
            {
                // extend size of the items list
                var large = new List<KeyMapItem>(evt + 10);
                var i = 0;
                foreach (var item in items)
                    large[i++] = item;
                items = large;
            }

            items[evt] = new KeyMapItem(evt, value);
        }
        /// <summary>
        /// Get key binding localy
        /// </summary>
        /// <param name="evt"></param>
        /// <param name="acceptDefaults"></param>
        /// <returns></returns>
        public override KeyMapItem GetLocal(Event evt, bool acceptDefaults = false)
        {
            if (!evt.IsValid)
                throw new ArgumentOutOfRangeException("evt");

            // do not support keys with modificators
            if (evt >= KeyModifyers.Alt)
                throw new ArgumentOutOfRangeException("evt");

            // limit by some "rational" number ;)
            if (evt >= MaxSize)
                throw new ArgumentOutOfRangeException("evt");

            if (evt < items.Count && items[evt] != null && items[evt].value != null)
                return items[evt];

            return acceptDefaults ? defaultBinding : null;
        }
    }
}