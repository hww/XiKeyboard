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

namespace VARP.Keyboard
{

    /// <summary>
    /// The line of characters collected in the array
    /// </summary>
    public class TextBuffer 
    {
        /// <summary>
        /// Constructor
        /// </summary>
        public TextBuffer ( int capacity = 256 )
        {
            buffer = new Event[ capacity ];
            BufferSize = 0;
        }
        // ===========================================================================================
        // Working with buffer
        // ===========================================================================================
        /// <summary>
        /// On key down event will add the character to the buffer.
        /// In case if buffer is overflowed it will clear this buffer
        /// </summary>
        public void InsertCharacter ( Event evt )
        {
            //TODO if there is selection
            // insert character
            for (var i = Point; i < BufferSize - 1; i++)
                buffer[i + 1] = buffer[i];
            buffer[ Point ] = evt;
            BufferSize++;
            Point++;
            isModifyed = true;
        }
        /// <summary>
        /// Set character at the point
        /// </summary>
        /// <param name="evt"></param>
        public void OverrideCharacter(Event evt)
        {
            buffer[Point] = evt;
            isModifyed = true;
        }
        /// <summary>
        /// Clear buffer
        /// </summary>
        public void Clear ( )
        {
            for (var i = 0; i < buffer.Length; i++)
                buffer[i] = 0;
            bufferSize = 0;
            point = 0;
            sequenceStarts = 0;
            isModifyed = true;
        }
        /// <summary>
        /// Get buffer size. How many characters in the buffer
        /// </summary>
        public int BufferSize
        {
            get { return bufferSize; }
            set
            {
                if (value >= buffer.Length)
                    throw new SystemException("Count can't be more that buffer size");
                bufferSize = value;
            }
        }
        // ===========================================================================================
        // Required for scanning in keyboard map
        // ===========================================================================================
        /// <summary>Where are sequence similar to ""C-x C-f"" starts</summary>
        public int SequenceStarts
        {
            get { return sequenceStarts; }
            set
            {
                UnityEngine.Debug.Assert(value <= BufferSize);
                sequenceStarts = value;
            }
        }
        // ===========================================================================================
        // Point
        // ===========================================================================================
        /// <summary>
        /// Point is the position of cursor in the text
        /// </summary>
        public int Point
        {
            get { return point; }
            set
            {
                if (Point >= BufferSize)
                    throw new SystemException("Point can't be more that count");
                UnityEngine.Debug.Assert(value <= BufferSize);
                point = value;
            }
        }
        public void GotoChar(int position)
        {
            Point = position;
        }
        public void ForwardChar(int count = 1)
        {
            Point = System.Math.Min(bufferSize, point + count);
        }
        public void BackwardChar(int count = 1)
        {
            Point = System.Math.Max(0, point - count);
        }
        //TODO! MOVE WORD
        // ===========================================================================================
        // Conversion
        // ===========================================================================================
        /// <summary>
        /// Convert buffer to the string
        /// </summary>
        public override string ToString ( )
        {
            var s = "\"";
            for ( var i = 0 ; i < 16 ; i++ )
            {
                s += buffer[ i ].Name;
            }
    
            if (BufferSize > 20)
                s += "...";
            s += "\"";
            return s;
        }
        /// <summary>
        /// Get current buffer string. The result lenght should be exactly same as source
        /// </summary>
        public string GetBufferString()
        {
            var s = "";
            for (var i = 0; i < BufferSize; i++)
            {
                var codeName = Event.GetName(buffer[i]);
                s += codeName.Length == 1 ? codeName : "?";
            }
            return s;
        }
        /// <summary>
        /// Get current buffer string. The result lenght should be exactly same as source
        /// </summary>
        public string GetBufferHumanizedString    ()
        {
            var s = "";
            for (var i = 0; i < BufferSize; i++)
                s += Event.GetName(buffer[i]);
            return s;
        }
        /// Get buffer substring. The result lenght should be exactly same as source
        /// </summary>
        /// <param name="starts"></param>
        /// <param name="ends"></param>
        public string GetBufferSubString(int starts, int ends)
        {
            UnityEngine.Debug.Assert(starts >= 0 && starts < BufferSize);
            UnityEngine.Debug.Assert(ends >= 0 && ends < BufferSize);
            var s = "";
            for (var i = starts; i < ends + 1; i++)
            {
                var codeName = Event.GetName(buffer[i]);
                s += codeName.Length == 1 ? codeName : "?";
            }
            return s;
        }
        // ===========================================================================================
        // Selection
        // ===========================================================================================
        /// <summary>
        /// Get selection
        /// </summary>
        public void GetSelection(out int starts, out int ends)
        {
            starts = selectionStart;
            ends = selectionEnd;
        }
        /// <summary>
        /// Set selection
        /// </summary>
        public void SetSelection(int starts, int ends)
        {
            selectionStart = starts;
            selectionEnd = ends;
        }
        // ===========================================================================================
        // Members
        // ===========================================================================================
        // sequence of events inside of this buffer
        public readonly Event[] buffer;
        // position of first character in the kbd-map's seuquencem as: "C-x C-f"
        private int sequenceStarts;
        // position of entry point
        private int point;
        // current text size
        private int bufferSize;
        // selection marker
        private int selectionStart;
        // selection marker
        private int selectionEnd;
        // modifyed or not
        public bool isModifyed;
    }

}

