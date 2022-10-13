/* Copyright (c) 2021 Valerya Pudova (hww) */

using System;
using XiKeyboard.KeyMaps;

namespace XiKeyboard
{
    ///-------------------------------------------------------------------------------------------------
    /// <summary>   The line of characters collected in the array. </summary>
    ///
    ///-------------------------------------------------------------------------------------------------

    public class TextBuffer 
    {
        ///-------------------------------------------------------------------------------------------------
        /// <summary>   Constructor. </summary>
        ///
        ///
        /// <param name="capacity"> (Optional) The capacity. </param>
        ///-------------------------------------------------------------------------------------------------

        public TextBuffer ( int capacity = 256 )
        {
            buffer = new KeyEvent[ capacity ];
            BufferSize = 0;
        }

        ///-------------------------------------------------------------------------------------------------
        /// <summary>
        /// On key down event will add the character to the buffer. In case if buffer is overflowed it
        /// will clear this buffer.
        /// </summary>
        ///
        ///
        /// <param name="evt">  . </param>
        ///-------------------------------------------------------------------------------------------------

        public void InsertCharacter ( KeyEvent evt )
        {
            // TODO if there is selection
            // insert character
            for (var i = Point; i < BufferSize - 1; i++)
                buffer[i + 1] = buffer[i];
            buffer[ Point ] = evt;
            BufferSize++;
            Point++;
            isModified = true;
        }

        ///-------------------------------------------------------------------------------------------------
        /// <summary>   Set character at the point. </summary>
        ///
        ///
        /// <param name="evt">  . </param>
        ///-------------------------------------------------------------------------------------------------

        public void OverrideCharacter(KeyEvent evt)
        {
            buffer[Point] = evt;
            isModified = true;
        }

        ///-------------------------------------------------------------------------------------------------
        /// <summary>   Clear buffer. </summary>
        ///
        ///-------------------------------------------------------------------------------------------------

        public void Clear ( )
        {
            for (var i = 0; i < buffer.Length; i++)
                buffer[i] = 0;
            bufferSize = 0;
            point = 0;
            sequenceStarts = 0;
            isModified = true;
        }

        ///-------------------------------------------------------------------------------------------------
        /// <summary>   Clears the sequence. </summary>
        ///
        ///-------------------------------------------------------------------------------------------------

        public void ClearSequence()
        {
            bufferSize = point = sequenceStarts;
        }

        ///-------------------------------------------------------------------------------------------------
        /// <summary>   Get buffer size. How many characters in the buffer. </summary>
        ///
        /// <exception cref="SystemException">  Thrown when a System error condition occurs. </exception>
        ///
        /// <value> The size of the buffer. </value>
        ///-------------------------------------------------------------------------------------------------

        public int BufferSize
        {
            get => bufferSize;
            set
            {
                if (value >= buffer.Length)
                    throw new SystemException("Count can't be more that buffer size");
                bufferSize = value;
            }
        }

        ///-------------------------------------------------------------------------------------------------
        /// <summary>   Where are sequence similar to ""C-x C-f"" starts. </summary>
        ///
        /// <value> The sequence starts. </value>
        ///-------------------------------------------------------------------------------------------------

        public int SequenceStarts
        {
            get => sequenceStarts;
            set
            {
                UnityEngine.Debug.Assert(value <= BufferSize);
                sequenceStarts = value;
            }
        }

        ///-------------------------------------------------------------------------------------------------
        /// <summary>   Point is the position of cursor in the title. </summary>
        ///
        /// <exception cref="SystemException">  Thrown when a System error condition occurs. </exception>
        ///
        /// <value> The point. </value>
        ///-------------------------------------------------------------------------------------------------

        public int Point
        {
            get => point;
            set
            {
                if (Point >= BufferSize)
                    throw new SystemException("Point can't be more that count");
                UnityEngine.Debug.Assert(value <= BufferSize);
                point = value;
            }
        }

        ///-------------------------------------------------------------------------------------------------
        /// <summary>   Goto character. </summary>
        ///
        ///
        /// <param name="position"> The position. </param>
        ///-------------------------------------------------------------------------------------------------

        public void GotoChar(int position)
        {
            Point = position;
        }

        ///-------------------------------------------------------------------------------------------------
        /// <summary>   Forward character. </summary>
        ///
        ///
        /// <param name="count">    (Optional) Number of. </param>
        ///-------------------------------------------------------------------------------------------------

        public void ForwardChar(int count = 1)
        {
            Point = System.Math.Min(bufferSize, point + count);
        }

        ///-------------------------------------------------------------------------------------------------
        /// <summary>   Backward character. </summary>
        ///
        ///
        /// <param name="count">    (Optional) Number of. </param>
        ///-------------------------------------------------------------------------------------------------

        public void BackwardChar(int count = 1)
        {
            Point = System.Math.Max(0, point - count);
        }

        ///-------------------------------------------------------------------------------------------------
        /// <summary>   Convert buffer to the string. </summary>
        ///
        ///
        /// <returns>   A string that represents this object. </returns>
        ///-------------------------------------------------------------------------------------------------

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

        ///-------------------------------------------------------------------------------------------------
        /// <summary>
        /// Get current buffer string. The result length should be exactly same as source.
        /// </summary>
        ///
        ///
        /// <returns>   The buffer string. </returns>
        ///-------------------------------------------------------------------------------------------------

        public string GetBufferString()
        {
            var s = "";
            for (var i = 0; i < BufferSize; i++)
            {
                var codeName = KeyEvent.GetName(buffer[i]);
                s += codeName.Length == 1 ? codeName : "?";
            }
            return s;
        }

        ///-------------------------------------------------------------------------------------------------
        /// <summary>
        /// Get current buffer string. The result lenght should be exactly same as source.
        /// </summary>
        ///
        ///
        /// <returns>   The buffer humanized string. </returns>
        ///-------------------------------------------------------------------------------------------------

        public string GetBufferHumanizedString    ()
        {
            var s = "";
            for (var i = 0; i < BufferSize; i++)
                s += KeyEvent.GetName(buffer[i]);
            return s;
        }

        ///-------------------------------------------------------------------------------------------------
        /// <summary>
        /// Get buffer substring. The result lenght should be exactly same as source
        /// </summary>
        /// </summary>
        ///
        ///
        /// <param name="starts">   . </param>
        /// <param name="ends">     . </param>
        ///
        /// <returns>   The buffer sub string. </returns>
        ///-------------------------------------------------------------------------------------------------

        public string GetBufferSubString(int starts, int ends)
        {
            UnityEngine.Debug.Assert(starts >= 0 && starts < BufferSize);
            UnityEngine.Debug.Assert(ends >= 0 && ends < BufferSize);
            var s = "";
            for (var i = starts; i < ends + 1; i++)
            {
                var codeName = KeyEvent.GetName(buffer[i]);
                s += codeName.Length == 1 ? codeName : "?";
            }
            return s;
        }

        ///-------------------------------------------------------------------------------------------------
        /// <summary>   Get selection. </summary>
        ///
        ///
        /// <param name="starts">   [out] . </param>
        /// <param name="ends">     [out] . </param>
        ///-------------------------------------------------------------------------------------------------

        public void GetSelection(out int starts, out int ends)
        {
            starts = selectionStart;
            ends = selectionEnd;
        }

        ///-------------------------------------------------------------------------------------------------
        /// <summary>   Set selection. </summary>
        ///
        ///
        /// <param name="starts">   . </param>
        /// <param name="ends">     . </param>
        ///-------------------------------------------------------------------------------------------------

        public void SetSelection(int starts, int ends)
        {
            selectionStart = starts;
            selectionEnd = ends;
        }

        ///-------------------------------------------------------------------------------------------------
        /// <summary> (Immutable) Sequence of events inside of this buffer. </summary>
        ///-------------------------------------------------------------------------------------------------

        public readonly KeyEvent[] buffer;
        /// <summary>   Position of first character in the kbd-map's sequence as: "C-x C-f". </summary>
        private int sequenceStarts;
        /// <summary>   Position of entry point. </summary>
        private int point;
        /// <summary>   Current title size. </summary>
        private int bufferSize;
        /// <summary>   Selection marker. </summary>
        private int selectionStart;
        /// <summary>   Selection marker. </summary>
        private int selectionEnd;
        /// <summary>   Modified or not. </summary>
        public bool isModified;
    }

}

