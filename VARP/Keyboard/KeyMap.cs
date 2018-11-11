using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using JetBrains.Annotations;

namespace VARP.Keyboard
{
    /// <summary>
    /// This is simple keysequence binding to any key.
    /// </summary>
    public class SequenceBinding 
    {
        public string name;
        public string help;
        public readonly KeyEvent[] sequence;

        public SequenceBinding(string name, KeyEvent[] sequence, string help = null)
        {
            this.name = name;
            this.sequence = sequence;
            this.help = help;
        }
    }

    /// <summary>
    /// Any class wich can be in the keymap have to be
    /// based on this class
    /// </summary>
    public class KeyMapItem
    {
        public static readonly KeyMapItem Empty = new KeyMapItem(0, null);

        public KeyEvent key;  //< this is the fake key
        public object value;  //< there can be any avaiable value

        public KeyMapItem(int key, object value)
        {
            this.key = new KeyEvent(key);
            this.value = value;
        }
        
        public KeyMapItem(KeyEvent key, object value)
        {
            this.key = key;
            this.value = value;
        }

        /// <summary>
        /// Compare with single event
        /// </summary>
        /// <param name="evt"></param>
        /// <returns></returns>
        public bool Compare(KeyEvent evt) { return evt == key; }

        public override string ToString() { return string.Format("key: {0} value: {1}", key, value); }
    }
 

    /// <summary>
    /// This class have to alow to build the tree of the keymaps
    /// </summary>
    public class KeyMap 
    {
        public static readonly KeyMap GlobalKeymap = new KeyMap("global-keymap");

        public string title;                //< title of keymap
        public string help;                 //< help for this item (used for menu only)
        public KeyMap parent;               //< parent key map
        public List<KeyMapItem> items;      //< kay map items
        public KeyMapItem defaultBinding;   //< default binding or null

        public virtual string Title { get { return title; } }
        public virtual string Help { get { return help; } }

        /// <summary>
        /// Create emty keymap
        /// </summary>
        /// <param name="title"></param>
        public KeyMap(string title = null)
        {
            this.title = title;
            items = new List<KeyMapItem>();
        }

        /// <summary>
        /// Create empty keymap based on parent keymap
        /// </summary>
        /// <param name="parent"></param>
        /// <param name="title"></param>
        public KeyMap([NotNull] KeyMap parent, string title = null)
        {
            if (parent == null) throw new ArgumentNullException("parent");
            this.title = title;
            this.parent = parent;
            items = new List<KeyMapItem>();
        }

        public KeyMap(string title, string help = null) : this(title)
        {
            this.title = help;
        }

        public KeyMap(KeyMap parent, string title, string help = null) : this(parent, title)
        {
            this.title = help;
        }

        public virtual void CopyTo(KeyMap other)
        {
            other.title = title;
            other.title = help;
            other.parent = parent;
            foreach (var item in items)
                other.SetLocal(item.key, item.value);
        }

        /// <summary>
        /// Get index of element which has given sequence
        /// </summary>
        /// <param name="evt"></param>
        /// <returns></returns>
        private int GetIndexOf(KeyEvent evt)
        {
            for (var i = 0; i < items.Count; i++)
            {
                if (items[i].Compare(evt))
                    return i;
            }

            return -1;
        }

        /// <summary>
        /// Set key value pair. Replace existing
        /// </summary>
        /// <param name="evt"></param>
        /// <param name="value"></param>
        public virtual void SetLocal(KeyEvent evt, object value)
        {
            if (evt.IsNotValid())
                throw new ArgumentOutOfRangeException("evt");
            var binding = new KeyMapItem(evt, value);
            var index = GetIndexOf(evt);
            if (index >= 0)
                items[index] = binding;
            else
                items.Add(binding);

            if (evt == KeyEvent.DefaultPseudoCode)
                defaultBinding = binding;
        }

        public virtual KeyMapItem GetLocal(KeyEvent evt, bool acceptDefaults = false)
        {
            if (evt.IsNotValid())
                throw new ArgumentOutOfRangeException("evt");
            var index = GetIndexOf(evt);
            if (index >= 0 && items[index].value != null)
                return items[index];

            return acceptDefaults ? defaultBinding : null;
        }

        #region Use full expression to define and lookup the definition

        public virtual KeyMapItem LokupKey(KeyEvent[] sequence, bool acceptDefaults = false)
        {
            return LokupKey(sequence, 0, sequence.Length - 1, acceptDefaults);
        }

        public virtual KeyMapItem LokupKey([NotNull] KeyEvent[] sequence, int starts, int ends, bool acceptDefaults = false)
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
                    return curentMap.parent != null ? curentMap.parent.LokupKey(sequence, acceptDefaults) : null;

                var map = tmp.value as KeyMap;
                if (map != null)
                    curentMap = map;
                else
                    return tmp; //< we found binding and it is not key map
            }
            return tmp;
        }

        // this way used for defining menu
        public bool Define(string[] sequence, object value)
        {
            var newsequence = Kbd.ParsePseudo(sequence);
            return Define(newsequence, value);
        }

        public virtual bool Define([NotNull] KeyEvent[] sequence, object value)
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

        #endregion
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
        public FullKeymap([NotNull] KeyMap parent, string title = null) : base(parent, title)
        {
        }

        /// <summary>
        /// Set key value pair. Replace existing.
        /// </summary>
        /// <param name="evt"></param>
        /// <param name="value"></param>
        public override void SetLocal(KeyEvent evt, object value)
        {
            if (evt.IsNotValid())
                throw new ArgumentOutOfRangeException("evt");

            // do not support keys with modificators
            if (evt.HasModifyers())
                throw new ArgumentOutOfRangeException("evt");

            // limit by some "rational" number ;)
            if (evt.Code >= MaxSize)
                throw new ArgumentOutOfRangeException("evt");

            if (evt.Code >= items.Count)
            {
                // extend size of the items list
                var large = new List<KeyMapItem>(evt.Code + 10);
                var i = 0;
                foreach (var item in items)
                    large[i++] = item;
                items = large;
            }

            items[evt.Code] = new KeyMapItem(evt, value);
        }

        public override KeyMapItem GetLocal(KeyEvent evt, bool acceptDefaults = false)
        {
            if (evt.IsNotValid())
                throw new ArgumentOutOfRangeException("evt");

            // do not support keys with modificators
            if (evt.HasModifyers())
                throw new ArgumentOutOfRangeException("evt");

            var code = evt.Code;
            // limit by some "rational" number ;)
            if (code >= MaxSize)
                throw new ArgumentOutOfRangeException("evt");

            if (code < items.Count && items[code] != null && items[code].value != null)
                return items[code];

            return acceptDefaults ? defaultBinding : null;
        }
      
    }

}