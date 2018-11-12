/* Copyright (c) 2016 Valery Alex P. All rights reserved. */

namespace VARP.Keyboard
{
    /// <summary>
    /// Similar ro Enum class defines all key modifyers as bits in the integer word
    /// </summary>
    public static class KeyModifyers
    {
        public static readonly int MaxCode = 1 << 28 - 1;
        public static readonly int Meta = 1 << 27;
        public static readonly int Control = 1 << 26;
        public static readonly int Shift = 1 << 25;
        public static readonly int Hyper = 1 << 24;
        public static readonly int Super = 1 << 23;
        public static readonly int Alt = 1 << 22;
        public static readonly int Pseudo = 1 << 21;
        /// <summary>
        /// Use for masking the modifyer bits
        /// </summary>
        public static readonly int AllModifyers = Control | Shift | Alt | Hyper | Super | Meta;
        /// <summary>
        /// Used internaly for iteration over modyfier. It is replacement for System.Enum.Values(typeof(Modifyers)).
        /// <summary>
        public static readonly int[] AllModifyersList = new int[] { Control, Alt, Shift };
    }
}

