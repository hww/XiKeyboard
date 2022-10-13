/* Copyright (c) 2021 Valerya Pudova (hww) */

using System;
using System.Collections.Generic;
using XiKeyboard.Menu;
using XiCore.StringTools;

namespace XiKeyboard.KeyMaps
{
    ///-------------------------------------------------------------------------------------------------
    /// <summary>
    /// This is simple key sequence binding to any key. When the key will be pressed it does poroduce
    /// the sequence.
    /// </summary>
    ///
    /// <remarks>   Valery, 10/12/2022. </remarks>
    ///-------------------------------------------------------------------------------------------------

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

    ///-------------------------------------------------------------------------------------------------
    /// <summary>   Any object which can be bind to the keymap have to be based on this class. </summary>
    ///
    /// <remarks>   Valery, 10/12/2022. </remarks>
    ///-------------------------------------------------------------------------------------------------

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

        ///-------------------------------------------------------------------------------------------------
        /// <summary>   Compare with single event. </summary>
        ///
        /// <remarks>   Valery, 10/12/2022. </remarks>
        ///
        /// <param name="evt">  . </param>
        ///
        /// <returns>   True if it succeeds, false if it fails. </returns>
        ///-------------------------------------------------------------------------------------------------

        public bool Compare(int evt)
        {
            return evt == key;
        }
        public override string ToString()
        {
            return $"key: {key} value: {value}";
        }
    }

    ///-------------------------------------------------------------------------------------------------
    /// <summary>   A dm iterator. Used for addressing to menu item </summary>
    ///
    /// <remarks>   Valery,. </remarks>
    ///-------------------------------------------------------------------------------------------------

    public struct DMIterator
    {
        /// <summary>   The key map. </summary>
        private KeyMap map;

        /// <summary>   Zero-based index of the item. </summary>
        private int idx;

        ///-------------------------------------------------------------------------------------------------
        /// <summary>   Constructor. </summary>
        ///
        /// <remarks>   Valery,. </remarks>
        ///
        /// <param name="keyMap">   The key map. </param>
        ///-------------------------------------------------------------------------------------------------

        public DMIterator(KeyMap keyMap, int index = 0)
        {
            this.map = keyMap;
            this.idx = index;
        }

        ///-------------------------------------------------------------------------------------------------
        /// <summary>   Gets the key map. </summary>
        ///
        /// <value> The key map. </value>
        ///-------------------------------------------------------------------------------------------------

        public KeyMap KeyMap => map;

        ///-------------------------------------------------------------------------------------------------
        /// <summary>   Gets the zero-based index of this object. </summary>
        ///
        /// <value> The index. </value>
        ///-------------------------------------------------------------------------------------------------

        public int Index => idx;

        ///-------------------------------------------------------------------------------------------------
        /// <summary>   Gets the item. </summary>
        ///
        /// <value> The item. </value>
        ///-------------------------------------------------------------------------------------------------

        public DMKeyMapItem Item => map?[idx];

        ///-------------------------------------------------------------------------------------------------
        /// <summary>   Gets a value indicating whether this object is end. </summary>
        ///
        /// <value> True if this object is end, false if not. </value>
        ///-------------------------------------------------------------------------------------------------

        public bool IsEnd => (map == null) || (idx < 0) || (idx >= map.Count);

        ///-------------------------------------------------------------------------------------------------
        /// <summary>   Increment operator. </summary>
        ///
        /// <remarks>   Valery,. </remarks>
        ///
        /// <param name="a">    A DMIterator to process. </param>
        ///
        /// <returns>   The result of the operation. </returns>
        ///-------------------------------------------------------------------------------------------------

        public static DMIterator operator ++(DMIterator a)
        {
            a.idx ++;
            return a;
        }

        ///-------------------------------------------------------------------------------------------------
        /// <summary>   Decrement operator. </summary>
        ///
        /// <remarks>   Valery,. </remarks>
        ///
        /// <param name="a">    A DMIterator to process. </param>
        ///
        /// <returns>   The result of the operation. </returns>
        ///-------------------------------------------------------------------------------------------------

        public static DMIterator operator --(DMIterator a)
        {
            a.idx--;
            return a;
        }
    }

    ///-------------------------------------------------------------------------------------------------
    /// <summary>
    /// This class have to allow to build the tree of the keymaps. Each keymap contains List of
    /// DMKeyMapItem(s). Additionally the keymap has title and help information as well it refers to
    /// parent keymap.
    /// </summary>
    ///
    /// <remarks>   Valery, 10/12/2022. </remarks>
    ///-------------------------------------------------------------------------------------------------

    public class KeyMap 
    {
        ///-------------------------------------------------------------------------------------------------
        /// <summary>
        /// (Immutable)
        /// The global keymap. It is represent top level of keymaps.
        /// </summary>
        ///-------------------------------------------------------------------------------------------------

        public static readonly KeyMap GlobalKeymap = new KeyMap("global-keymap");


        /// <summary>   title of keymap. </summary>
        public string title;
        /// <summary>   help for this item (used for menu only) </summary>
        public string help;
        /// <summary>   parent key map. </summary>
        public KeyMap parent;
        /// <summary>   key map items. </summary>
        public List<DMKeyMapItem> items;
        /// <summary>   This binding can be returned when the requested key binding is not found. </summary>
        public DMKeyMapItem defaultBinding;
        public bool isMenu;

        ///-------------------------------------------------------------------------------------------------
        /// <summary>   Get title name of this keymap. </summary>
        ///
        /// <value> The title. </value>
        ///-------------------------------------------------------------------------------------------------

        public virtual string Title => title;

        ///-------------------------------------------------------------------------------------------------
        /// <summary>   Get the help for this keymap. </summary>
        ///
        /// <value> The help. </value>
        ///-------------------------------------------------------------------------------------------------

        public virtual string Help => help;

        ///-------------------------------------------------------------------------------------------------
        /// <summary>   Get binding's quantity. </summary>
        ///
        /// <value> The count. </value>
        ///-------------------------------------------------------------------------------------------------

        public int Count => items.Count;

        ///-------------------------------------------------------------------------------------------------
        /// <summary>   Convert this object into a string representation. </summary>
        ///
        /// <remarks>   Valery, 10/12/2022. </remarks>
        ///
        /// <returns>   A string that represents this object. </returns>
        ///-------------------------------------------------------------------------------------------------

        public override string ToString()
        {
            return $"KeyMap Title={Title} Count={Count}";
        }

        ///-------------------------------------------------------------------------------------------------
        /// <summary>   Get key binding with this index. </summary>
        ///
        /// <param name="index">    . </param>
        ///
        /// <returns>   The indexed item. </returns>
        ///-------------------------------------------------------------------------------------------------

        public DMKeyMapItem this[int index] => items[index];

        ///-------------------------------------------------------------------------------------------------
        /// <summary>   Create emty keymap. </summary>
        ///
        /// <remarks>   Valery, 10/12/2022. </remarks>
        ///
        /// <param name="title">    (Optional) The title. </param>
        /// <param name="help">     (Optional) The help. </param>
        ///-------------------------------------------------------------------------------------------------

        public KeyMap(string title = null, string help = null )
        {
            this.title = title;
            this.help = help;
            this.items = new List<DMKeyMapItem>();
        }

        ///-------------------------------------------------------------------------------------------------
        /// <summary>   Create empty keymap based on parent keymap. </summary>
        ///
        /// <remarks>   Valery, 10/12/2022. </remarks>
        ///
        /// <exception cref="ArgumentNullException">    Thrown when one or more required arguments are
        ///                                             null. </exception>
        ///
        /// <param name="parent">   The parent. </param>
        /// <param name="title">    (Optional) The title. </param>
        /// <param name="help">     (Optional) The help. </param>
        ///-------------------------------------------------------------------------------------------------

        public KeyMap(KeyMap parent, string title = null, string help = null )
        {
            this.title = title;
            this.help = help;
            this.parent = parent ?? throw new ArgumentNullException(nameof(parent));
            items = new List<DMKeyMapItem>();
        }

        ///-------------------------------------------------------------------------------------------------
        /// <summary>   Copy this keymap to target one. </summary>
        ///
        /// <remarks>   Valery, 10/12/2022. </remarks>
        ///
        /// <param name="target">   Target for the. </param>
        ///-------------------------------------------------------------------------------------------------

        public virtual void CopyTo(KeyMap target)
        {
            target.title = title;
            target.title = help;
            target.parent = parent;
            foreach (var item in items)
                target.SetLocal(item.key, item.value);
        }

        ///-------------------------------------------------------------------------------------------------
        /// <summary>   Set key value pair. Replace existing. </summary>
        ///
        /// <remarks>   Valery, 10/12/2022. </remarks>
        ///
        /// <exception cref="ArgumentOutOfRangeException">  Thrown when one or more arguments are outside
        ///                                                 the required range. </exception>
        ///
        /// <param name="evt">          The event. </param>
        /// <param name="value">        The value. </param>
        /// <param name="afterEvent">   (Optional) The afterEvent. </param>
        ///-------------------------------------------------------------------------------------------------

        public virtual void SetLocal(KeyEvent evt, object value, int afterEvent = 0)
        {
            if (!evt.IsValid)
                throw new ArgumentOutOfRangeException(nameof(evt));
            var binding = new DMKeyMapItem(evt, value);

            var index = GetIndexOf(evt);
            if (index >= 0)
            {
                items[index] = binding;
            }
            else
            {
                if (afterEvent > 0)
                {
                    var aferIndex = GetIndexOf(afterEvent);
                    items.Insert(aferIndex+1, binding);
                }
                else
                {
                    items.Add(binding);
                }
            }
            if (evt == KeyEvent.DefaultPseudoCode)
                defaultBinding = binding;
        }

        ///-------------------------------------------------------------------------------------------------
        /// <summary>
        /// Get key binding of this keymap, returns: null or default binding (if allowed)
        /// </summary>
        ///
        /// <remarks>   Valery, 10/12/2022. </remarks>
        ///
        /// <exception cref="ArgumentOutOfRangeException">  Thrown when one or more arguments are outside
        ///                                                 the required range. </exception>
        ///
        /// <param name="evt">              The event. </param>
        /// <param name="acceptDefaults">   (Optional) True to accept defaults. </param>
        ///
        /// <returns>   The local. </returns>
        ///-------------------------------------------------------------------------------------------------

        public virtual DMKeyMapItem GetLocal(KeyEvent evt, bool acceptDefaults = false)
        {
            if (!evt.IsValid)
                throw new ArgumentOutOfRangeException(nameof(evt));
            var index = GetIndexOf(evt);
            if (index >= 0 && items[index].value != null)
                return items[index];
            return acceptDefaults ? defaultBinding : null;
        }

        ///-------------------------------------------------------------------------------------------------
        /// <summary>   Get index of element which has given sequence. </summary>
        ///
        /// <remarks>   Valery, 10/12/2022. </remarks>
        ///
        /// <param name="evt">  The event. </param>
        ///
        /// <returns>   The index of. </returns>
        ///-------------------------------------------------------------------------------------------------

        private int GetIndexOf ( int evt )
        {
            for ( var i = 0 ; i < items.Count ; i++ )
            {
                if ( items[ i ].Compare ( evt ) )
                    return i;
            }
            return -1;
        }

        ///-------------------------------------------------------------------------------------------------
        /// <summary>   Lockup keymap item by full sequence of keys. </summary>
        ///
        /// <remarks>   Valery, 10/12/2022. </remarks>
        ///
        /// <param name="sequence">         The sequence. </param>
        /// <param name="acceptDefaults">   (Optional) True to accept defaults. </param>
        ///
        /// <returns>   A DMKeyMapItem. </returns>
        ///-------------------------------------------------------------------------------------------------

        public virtual DMKeyMapItem LookupKey(KeyEvent[] sequence, bool acceptDefaults = false)
        {
            return LookupKey(sequence, 0, sequence.Length - 1, acceptDefaults);
        }

        ///-------------------------------------------------------------------------------------------------
        /// <summary>   Lockup keymap item by full sequence of keys. </summary>
        ///
        /// <remarks>   Valery, 10/12/2022. </remarks>
        ///
        /// <exception cref="ArgumentNullException">        Thrown when one or more required arguments
        ///                                                 are null. </exception>
        /// <exception cref="ArgumentOutOfRangeException">  Thrown when one or more arguments are outside
        ///                                                 the required range. </exception>
        ///
        /// <param name="sequence">         Full sequence of keys. </param>
        /// <param name="starts">           First index in the sequence. </param>
        /// <param name="ends">             Last index in the sequence. </param>
        /// <param name="acceptDefaults">   (Optional) Allow to return default binding. </param>
        ///
        /// <returns>   DMKeyMapItem or Null. </returns>
        ///-------------------------------------------------------------------------------------------------

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

        ///-------------------------------------------------------------------------------------------------
        /// <summary>   DefineMenuLine by string expression with '/' separator. </summary>
        ///
        /// <remarks>   Valery, 10/12/2022. </remarks>
        ///
        /// <param name="path">         Full pathname of the file. </param>
        /// <param name="value">        The line. </param>
        /// <param name="afterEvent">   (Optional) Add new event after this event. </param>
        ///
        /// <returns>   A MenuLine. </returns>
        ///-------------------------------------------------------------------------------------------------

        public bool DefinePseudo(string path, object value, int afterEvent = 0)
        {
            var sequence = path.Split('/');
            var newSequence = KBD.ParsePseudo(sequence);
            return Define(newSequence, value, afterEvent);
        }

        ///-------------------------------------------------------------------------------------------------
        /// <summary>   DefineMenuLine list of key-strings. This way used for defining menu. </summary>
        ///
        /// <remarks>   Valery, 10/12/2022. </remarks>
        ///
        /// <param name="sequence">     The sequence. </param>
        /// <param name="value">        The value. </param>
        /// <param name="afterEvent">   (Optional) Add new event after this event. </param>
        ///
        /// <returns>   True if it succeeds, false if it fails. </returns>
        ///-------------------------------------------------------------------------------------------------

        public bool DefinePseudo(string[] sequence, object value, int afterEvent = 0)
        {
            var newSequence = KBD.ParsePseudo(sequence);
            return Define(newSequence, value, afterEvent);
        }

        ///-------------------------------------------------------------------------------------------------
        /// <summary>   DefineMenuLine by string expression. </summary>
        ///
        /// <remarks>   Valery, 10/12/2022. </remarks>
        ///
        /// <param name="sequence">     The sequence. </param>
        /// <param name="value">        The value. </param>
        /// <param name="afterEvent">   (Optional) Add new event after this event. </param>
        ///
        /// <returns>   True if it succeeds, false if it fails. </returns>
        ///-------------------------------------------------------------------------------------------------

        public bool Define(string sequence, object value,  int afterEvent = 0)
        {
            var newSequence = KBD.ParseSequence(sequence);
            return Define(newSequence, value, afterEvent);
        }

        ///-------------------------------------------------------------------------------------------------
        /// <summary>   DefineMenuLine sequence with given binding. </summary>
        ///
        /// <remarks>   Valery, 10/12/2022. </remarks>
        ///
        /// <exception cref="ArgumentNullException">    Thrown when one or more required arguments are
        ///                                             null. </exception>
        /// <exception cref="Exception">                Thrown when an exception error condition occurs. </exception>
        ///
        /// <param name="sequence">     The sequence. </param>
        /// <param name="value">        The value. </param>
        /// <param name="afterEvent">   (Optional) Add new event after this event. </param>
        ///
        /// <returns>   True if it succeeds, false if it fails. </returns>
        ///-------------------------------------------------------------------------------------------------

        public virtual bool Define(KeyEvent [] sequence, object value, int afterEvent = 0)
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
                        currentMap.SetLocal(key, value, afterEvent);
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
                        currentMap.SetLocal(key, value, afterEvent);
                        UnityEngine.Debug.LogWarning($"Overwrite menu item [{KBD.ConvertToString(sequence)}]");
                        return true;
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
    }

    ///-------------------------------------------------------------------------------------------------
    /// <summary>
    /// If an element of a keymap is a char-table, it counts as holding bindings for all character
    /// events with no modifier element n is the binding for the character with code n.This is a
    /// compact way to record lots of bindings.A keymap with such a char-table is called a full
    /// keymap. Other keymaps are called sparse keymaps.
    /// </summary>
    ///
    /// <remarks>   Valery, 10/12/2022. </remarks>
    ///-------------------------------------------------------------------------------------------------

    public class FullKeymap : KeyMap
    {
        private const int MaxSize = 2048;

        ///-------------------------------------------------------------------------------------------------
        /// <summary>   Create emty keymap. </summary>
        ///
        /// <remarks>   Valery, 10/12/2022. </remarks>
        ///
        /// <param name="title">    (Optional) </param>
        ///-------------------------------------------------------------------------------------------------

        public FullKeymap(string title = null) : base(title)
        {
        }

        ///-------------------------------------------------------------------------------------------------
        /// <summary>   Create empty keymap based on parent keymap. </summary>
        ///
        /// <remarks>   Valery, 10/12/2022. </remarks>
        ///
        /// <param name="parent">   . </param>
        /// <param name="title">    (Optional) </param>
        ///-------------------------------------------------------------------------------------------------

        public FullKeymap(KeyMap parent, string title = null) : base(parent, title)
        {
        }

        ///-------------------------------------------------------------------------------------------------
        /// <summary>   Set key value pair. Replace existing. </summary>
        ///
        /// <remarks>   Valery, 10/12/2022. </remarks>
        ///
        /// <exception cref="ArgumentOutOfRangeException">  Thrown when one or more arguments are outside
        ///                                                 the required range. </exception>
        ///
        /// <param name="evt">          The event to bind. </param>
        /// <param name="value">        The value to bind. </param>
        /// <param name="afterEvent">   (Optional) Unused. </param>
        ///-------------------------------------------------------------------------------------------------

        public override void SetLocal(KeyEvent evt, object value, int afterEvent = 0)
        {
            UnityEngine.Debug.Assert(afterEvent == 0);
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

        ///-------------------------------------------------------------------------------------------------
        /// <summary>   Get key binding locally. </summary>
        ///
        /// <remarks>   Valery, 10/12/2022. </remarks>
        ///
        /// <exception cref="ArgumentOutOfRangeException">  Thrown when one or more arguments are outside
        ///                                                 the required range. </exception>
        ///
        /// <param name="evt">              . </param>
        /// <param name="acceptDefaults">   (Optional) </param>
        ///
        /// <returns>   The local. </returns>
        ///-------------------------------------------------------------------------------------------------

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

    ///-------------------------------------------------------------------------------------------------
    /// <summary>   The menu map is the same as key map but just separated to the other class </summary>
    ///
    /// <remarks>   Valery, 10/12/2022. </remarks>
    ///-------------------------------------------------------------------------------------------------

    public class MenuMap : KeyMap
    {   
        /// <summary>   The global menu bar. There can be more than one if you need. </summary>
        public static MenuMap MenuBar;

        ///-------------------------------------------------------------------------------------------------
        /// <summary>   Static constructor. </summary>
        ///
        /// <remarks>   Valery, 10/12/2022. </remarks>
        ///-------------------------------------------------------------------------------------------------

        static MenuMap()
        {
            MenuBar = new MenuMap("Menu Bar", "The global menu bar");
            string[] name = { "menu-bar" };
            var newSequence = KBD.ParsePseudo(name);
            KeyMap.GlobalKeymap.Define(newSequence, MenuBar);
        }

        ///-------------------------------------------------------------------------------------------------
        /// <summary>   Create emty keymap. </summary>
        ///
        /// <remarks>   Valery, 10/12/2022. </remarks>
        ///
        /// <param name="title">    (Optional) The title. </param>
        /// <param name="help">     (Optional) The help. </param>
        ///-------------------------------------------------------------------------------------------------

        public MenuMap(string title = null, string help = null) : base(title, help)
        {
        }

        ///-------------------------------------------------------------------------------------------------
        /// <summary>   SetLocal menu line by string expression. </summary>
        ///
        /// <remarks>   Valery, 10/12/2022. </remarks>
        ///
        /// <param name="path">         The path of item or null. </param>
        /// <param name="line">         The line. </param>
        /// <param name="afterEvent">   (Optional) The after event. </param>
        ///
        /// <returns>   A MenuLine. </returns>
        ///-------------------------------------------------------------------------------------------------

        public bool DefineLine(string path, MenuLine line, string afterEvent = null)
        {
            if (path == null) path = Humanizer.Decamelize(line.Title);
            var after = afterEvent==null ? KeyEvent.None : KeyEvent.GetPseudoCode(afterEvent);
            var sequence = path.Split('/');
            var code = KBD.ParsePseudo(sequence);
            return Define(code, line, after);
        }

        ///-------------------------------------------------------------------------------------------------
        /// <summary>   Define line. </summary>
        ///
        /// <remarks>   Valery,. </remarks>
        ///
        /// <param name="path">         Full pathname of the file. </param>
        /// <param name="map">          The map. </param>
        /// <param name="afterEvent">   (Optional) The after event. </param>
        ///
        /// <returns>   True if it succeeds, false if it fails. </returns>
        ///-------------------------------------------------------------------------------------------------

        public bool DefineLine(string path, MenuMap map, string afterEvent = null)
        {
            if (path == null) path = Humanizer.Decamelize(map.Title);
            var after = afterEvent == null ? KeyEvent.None : KeyEvent.GetPseudoCode(afterEvent);
            var sequence = path.Split('/');
            var code = KBD.ParsePseudo(sequence);
            return Define(code, map, after);
        }

        ///-------------------------------------------------------------------------------------------------
        /// <summary>   SetLocal menu line by string expression. </summary>
        ///
        /// <remarks>   Valery, 10/12/2022. </remarks>
        ///
        /// <param name="name">         The name. </param>
        /// <param name="line">         The line. </param>
        /// <param name="afterEvent">   (Optional) The after event. </param>
        ///
        /// <returns>   A MenuLine. </returns>
        ///-------------------------------------------------------------------------------------------------

        public MenuLine SetLocal(string name, MenuLine line, int afterEvent = 0)
        {
            var code = KeyEvent.GetPseudoCode(name);
            SetLocal(code, line, afterEvent);
            return line;
        }

        ///-------------------------------------------------------------------------------------------------
        /// <summary>   Convert this object into a string representation. </summary>
        ///
        /// <remarks>   Valery, 10/12/2022. </remarks>
        ///
        /// <returns>   A string that represents this object. </returns>
        ///-------------------------------------------------------------------------------------------------

        public override string ToString()
        {
            return $"MenuMap Title={Title} Count={Count}";
        }
    }

}