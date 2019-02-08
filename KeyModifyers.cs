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

