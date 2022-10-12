/* Copyright (c) 2021 Valerya Pudova (hww) */

namespace XiKeyboard.KeyMaps
{
    ///-------------------------------------------------------------------------------------------------
    /// <summary>
    /// Similar ro Enum class defines all key Modifiers as bits in the integer word.
    /// </summary>
    ///
    ///-------------------------------------------------------------------------------------------------

    public static class KeyModifiers
    {
        public static readonly int None = 0;
        public static readonly int MaxCode = 1 << 28 - 1;
        public static readonly int Meta = 1 << 27;
        public static readonly int Control = 1 << 26;
        public static readonly int Shift = 1 << 25;
        public static readonly int Hyper = 1 << 24;
        public static readonly int Super = 1 << 23;
        public static readonly int Alt = 1 << 22;
        public static readonly int Pseudo = 1 << 21;

        ///-------------------------------------------------------------------------------------------------
        /// <summary>
        /// (Immutable)
        /// Use for masking the modifier bits.
        /// </summary>
        ///-------------------------------------------------------------------------------------------------

        public static readonly int AllModifiers = Control | Shift | Alt | Hyper | Super | Meta;

        ///-------------------------------------------------------------------------------------------------
        /// <summary>   (Immutable)
        ///             Used internally for iteration over modifier. It is replacement for
        ///             System.Enum.Values(typeof(Modifiers)). </summary>
        /// <summary>   . </summary>
        ///-------------------------------------------------------------------------------------------------

        public static readonly int[] AllModifiersList = { Control, Alt, Shift };
    }
}

