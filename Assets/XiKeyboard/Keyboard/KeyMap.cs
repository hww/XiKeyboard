/* Copyright (c) 2021 Valerya Pudova (hww) */

using System;
using System.Collections.Generic;

namespace XiKeyboard
{

    /// <summary>
    /// This is simple key sequence binding to any key.
    /// When the key will be pressed it does poroduce the sequence
    /// </summary>
    public class DMSequenceBinding 
    {
        public string name;              //< Menu name, or key binding name
        public string help;              //< Help information for this binding
        public readonly int[] sequence;  //< Sequence of events required to invoke it 

        public DMSequenceBinding(string name, int[] sequence, string help = null)
        {
            this.name = name;
            this.sequence = sequence;
            this.help = help;
        }
    }

    /// <summary>
    /// Any object which can be bind to the keymap have to be based on this class
    /// </summary>
    public class DMKeyMapItem
    {
        public static readonly DMKeyMapItem Empty = new DMKeyMapItem(0, null);

        public int key;             //< this is the fake key
        public object value;        //< there can be any available value

        public bool IsPseudo => (key & KeyModifiers.Pseudo) > 0;
        public DMKeyMapItem(int key, object value)
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
            return $"key: {key} value: {value}";
        }
    }

    /// <summary>
    /// This class have to allow to build the tree of the keymaps.
    /// Each keymap contains List of DMKeyMapItem(s). Additionally 
    /// the keymap has title and help information as well it 
    /// refers to parent keymap.
    /// </summary>
    public class KeyMap 
    {
        /// <summary>
        /// The global keymap. It is represent top level of keymaps
        /// </summary>
        public static readonly KeyMap GlobalKeymap = new KeyMap("global-keymap");

        public string title;                  //< title of keymap
        public string help;                   //< help for this item (used for menu only)
        public KeyMap parent;                 //< parent key map
        public List<DMKeyMapItem> items;      //< key map items
        // This binding can be returned when the requested key binding is not found
        public DMKeyMapItem defaultBinding;   //< default binding or null. 
        
        /// <summary>
        /// Get title name of this keymap
        /// </summary>
        public virtual string Title => title;

        /// <summary>
        /// Get the help for this keymap
        /// </summary>
        public virtual string Help => help;

        /// <summary>
        /// Get binding's quantity 
        /// </summary>
        public int Count => items.Count;

        /// <summary>
        /// Get key binding with this index
        /// </summary>
        /// <param name="index"></param>
        public DMKeyMapItem this[int index] => items[index];

        /// <summary>
        /// Create emty keymap
        /// </summary>
        public KeyMap(string title = null, string help = null )
        {
            this.title = title;
            this.help = help;
            this.items = new List<DMKeyMapItem>();
        }
        /// <summary>
        /// Create empty keymap based on parent keymap
        /// </summary>
        public KeyMap(KeyMap parent, string title = null, string help = null )
        {
            this.title = title;
            this.help = help;
            this.parent = parent ?? throw new ArgumentNullException(nameof(parent));
            items = new List<DMKeyMapItem>();
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
        public virtual void SetLocal(KeyEvent evt, object value)
        {
            if (!evt.IsValid)
                throw new ArgumentOutOfRangeException(nameof(evt));
            var binding = new DMKeyMapItem(evt, value);
            var index = GetIndexOf(evt);
            if (index >= 0)
                items[index] = binding;
            else
                items.Add(binding);

            if (evt == KeyEvent.DefaultPseudoCode)
                defaultBinding = binding;
        }
        
        /// <summary>
        ///  Get key binding of this keymap, returns: null or default binding (if allowed)
        /// </summary>
        public virtual DMKeyMapItem GetLocal(KeyEvent evt, bool acceptDefaults = false)
        {
            if (!evt.IsValid)
                throw new ArgumentOutOfRangeException(nameof(evt));
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
        // Lockup the key binding recursively
        // ===============================================================================================
        /// <summary>
        /// Lockup keymap item by full sequence of keys
        /// </summary>
        public virtual DMKeyMapItem LookupKey(KeyEvent[] sequence, bool acceptDefaults = false)
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
        /// <returns>DMKeyMapItem or Null</returns>
        public virtual DMKeyMapItem LookupKey(KeyEvent [] sequence, int starts, int ends, bool acceptDefaults = false)
        {
            if (sequence == null) throw new ArgumentNullException(nameof(sequence));
            if (starts < 0 || starts >= sequence.Length) throw new ArgumentOutOfRangeException(nameof(starts));
            if (ends < starts || ends >= sequence.Length) throw new ArgumentOutOfRangeException(nameof(ends));

            var currentMap = this;
            var tmp = null as DMKeyMapItem;

            for (var i=starts; i < ends; i++)
            { 
                tmp = currentMap.GetLocal(sequence[i], acceptDefaults);
                if (tmp == null)
                    return currentMap.parent != null ? currentMap.parent.LookupKey(sequence, acceptDefaults) : null;

                if (tmp.value is KeyMap map)
                    currentMap = map;
                else
                    return tmp; //< we found binding and it is not key map
            }
            return tmp;
        }
        // ===============================================================================================
        // Define the key binding recursively
        // ===============================================================================================
        /// <summary>Define list of key-strings. This way used for defining menu</summary>
        public bool DefinePseudo(string[] sequence, object value)
        {
            var newSequence = KBD.ParsePseudo(sequence);
            return Define(newSequence, value);
        }
        /// <summary>Define by string expression</summary>
        public bool Define(string sequence, object value)
        {
            var newSequence = KBD.ParseSequence(sequence);
            return Define(newSequence, value);
        }
        /// <summary>Define sequence with given binding</summary>
        public virtual bool Define(KeyEvent [] sequence, object value)
        {
            if (sequence == null) throw new ArgumentNullException(nameof(sequence));

            var currentMap = this;
            var lastIndex = sequence.Length - 1;

            for (var i = 0; i < sequence.Length; i++)
            {
                var key = sequence[i];
                var tmp = currentMap.GetLocal(key); // do not allow defaults
                if (tmp == null)
                {
                    // there is no this binding
                    // N.B. Do not look at the parent one!
                    if (i == lastIndex)
                    {
                        // the currentMap is the target map and it does not have definition 
                        currentMap.SetLocal(key, value);
                        return true;
                    }
                    else
                    {
                        // the currentMap is the map in the sequence and it does not have definition 
                        var newMap = new KeyMap();
                        currentMap.SetLocal(key, newMap);
                        currentMap = newMap;
                    }
                }
                else
                {
                    // we found binding in currentMap
                    if (i == lastIndex)
                    {
                        // currentMap is target map, it has binding but we have to redefine it
                        currentMap.SetLocal(key, value);
                    }
                    else
                    {
                        // the currentMap is the map in the sequence and it has definition 
                        var map = tmp.value as KeyMap;
                        if (map != null)
                            currentMap = map;
                        else
                            throw new Exception($"Expect KeyMap at '{sequence[i]}' found: '{tmp}' in: '{sequence}'");
                    }
                }
            }
            throw new Exception("We can\'t be here");
        }
        // ===============================================================================================
        // Define menu map
        // ===============================================================================================
        /// <summary>Define list of key-strings. This way used for defining menu</summary>
        public KeyMap CreateMenu(string path, string title, string help)
        {
            var menu = new KeyMap(title, help );
            var sequence = path.Split('/');
            var newSequence = KBD.ParsePseudo(sequence);
            Define(newSequence, menu);
            return menu;
        }
        
        /// <summary>Define by string expression with '/' separator</summary>
        public DMMenuLine DefineMenuLine(string path, DMMenuLine line)
        {
            var sequence = path.Split('/');
            var newSequence = KBD.ParsePseudo(sequence);
            Define(newSequence, line);
            return line;
        }
        
        /// <summary>Define by string expression</summary>
        public DMMenuLine AddMenuLine(string name, DMMenuLine line)
        {
            var code = KeyEvent.GetPseudoCode(name);
            SetLocal(code, line);
            return line;
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
        public override void SetLocal(KeyEvent evt, object value)
        {
            if (!evt.IsValid)
                throw new ArgumentOutOfRangeException(nameof(evt));

            // do not support keys with modifiers
            if (evt >= KeyModifiers.Alt)
                throw new ArgumentOutOfRangeException(nameof(evt));

            // limit by some "rational" number ;)
            if (evt >= MaxSize)
                throw new ArgumentOutOfRangeException(nameof(evt));

            if (evt >= items.Count)
            {
                // extend size of the items list
                var large = new List<DMKeyMapItem>(evt + 10);
                var i = 0;
                foreach (var item in items)
                    large[i++] = item;
                items = large;
            }

            items[evt] = new DMKeyMapItem(evt, value);
        }
        /// <summary>
        /// Get key binding locally
        /// </summary>
        /// <param name="evt"></param>
        /// <param name="acceptDefaults"></param>
        /// <returns></returns>
        public override DMKeyMapItem GetLocal(KeyEvent evt, bool acceptDefaults = false)
        {
            if (!evt.IsValid)
                throw new ArgumentOutOfRangeException(nameof(evt));

            // do not support keys with modifiers
            if (evt >= KeyModifiers.Alt)
                throw new ArgumentOutOfRangeException(nameof(evt));

            // limit by some "rational" number ;)
            if (evt >= MaxSize)
                throw new ArgumentOutOfRangeException(nameof(evt));

            if (evt < items.Count && items[evt] != null && items[evt].value != null)
                return items[evt];

            return acceptDefaults ? defaultBinding : null;
        }
    }
}