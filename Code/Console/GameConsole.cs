using UnityEngine;
using XiKeyboard.Buffers;
using XiKeyboard.KeyMaps;
using XiKeyboard.Modes;

namespace XiKeyboard.Console
{
    ///-------------------------------------------------------------------------------------------------
    /// <summary>   A generic game console. </summary>
    ///
    /// <remarks>   Valery, 10/12/2022. </remarks>
    ///-------------------------------------------------------------------------------------------------

    public class GameConsole 
    {
        public delegate void OnReadLineDelegate(string line);
        public delegate void OnReadKeyDelegate(int key);
        public delegate string OnAutoCompleteDelegate(string line, int caretPosition);

        public static IDisplay display;
        private static OnReadLineDelegate onReadLine;
        private static OnReadKeyDelegate onReadKey;
        private static OnAutoCompleteDelegate onAutoComplete;
        private static Buffer inputBuffer;
        private static ReadLineMode readlineMode;

        ///-------------------------------------------------------------------------------------------------
        /// <summary>   Constructor. </summary>
        ///
        /// <remarks>   Valery, 10/12/2022. </remarks>
        ///-------------------------------------------------------------------------------------------------

        public static void  Initialize()
        {
            readlineMode = new ReadLineMode();
#if FIXME
            readlineMode.OnReadLineListener.Add(OnReadLine);
#endif
            inputBuffer = new Buffer("repl");
            inputBuffer.EnabeMajorMode(readlineMode);
            inputBuffer.SetActive(true);
            Buffer.OnKeyPressed.Add(OnKeyDown);
        }

        #region Terminal interface

        ///-------------------------------------------------------------------------------------------------
        /// <summary>   Receive message on key down. </summary>
        ///
        /// <remarks>   Valery, 10/12/2022. </remarks>
        ///
        /// <param name="buffer">   The buffer. </param>
        /// <param name="evt">      The event. </param>
        ///-------------------------------------------------------------------------------------------------

        public static void OnKeyDown(Buffer buffer, KeyEvent evt)
        {
            if (onReadKey != null)
                onReadKey(evt);
            onReadKey = null;
        }

        ///-------------------------------------------------------------------------------------------------
        /// <summary>   Receive call back from terminal when we enter line of text in the buffer. </summary>
        ///
        /// <remarks>   Valery, 10/12/2022. </remarks>
        ///
        /// <param name="text"> The text. </param>
        ///-------------------------------------------------------------------------------------------------

        public static void OnReadLine(string text)
        {
            if (onReadLine != null)
            {
                onReadLine(text);
                onReadLine = null;
            }
        }

        #endregion

        #region Console

        ///-------------------------------------------------------------------------------------------------
        /// <summary>   Play beep sound. </summary>
        ///
        /// <remarks>   Valery, 10/12/2022. </remarks>
        ///-------------------------------------------------------------------------------------------------

        public void Beep()
        {
            if (display != null) display.Beep();
        }
        public void Clear()
        {
            if (display != null) display.Clear();
        }
        public static void ResetColor()
        {
            if (display != null) display.ResetColor();
        }
        public static void SetColor(Color color)
        {
            if (display != null) display.SetColor(color);
        }
        public static void Write(string message)
        {
            if (display != null) display.Write(message);
        }
        public static void WriteLine()
        {
            if (display != null) display.WriteLine("");
        }
        public static void WriteLine(string message)
        {
            if (display != null) display.WriteLine(message);
        }
        public static void WriteLine(string format, params string[] args)
        {
            if (display != null) display.WriteLine(string.Format(format,args));
        }
        public static int BufferWidth { get { return display.BufferWidth; } }
        public static int BufferHeight { get { return display.BufferHeight; } }
        #endregion

        #region Read Console
        /// <summary>
        /// Read single character
        /// </summary>
        public static void ReadKey(OnReadKeyDelegate onReadChar)
        {
            GameConsole.onReadKey = onReadChar;
        }
        /// <summary>
        /// Read line
        /// </summary>
        /// <param name="prompt"></param>
        public static void ReadLine(OnReadLineDelegate onReadLine)
        {
            GameConsole.onReadLine = onReadLine;
        }
        #endregion
    }

}
