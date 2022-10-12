/* Copyright (c) 2016 Valery Alex P. All rights reserved. */

using System;
using System.Collections.Generic;
using XiCore.Delegates;
using XiCore.StringTools;
using XiKeyboard.KeyMaps;
using XiKeyboard.Modes;

namespace XiKeyboard.Buffers
{
    ///-------------------------------------------------------------------------------------------------
    /// <summary>
    /// This class is one of input Modes. It allow to complete the word by TAB, and navigate in
    /// history by arrow keys.
    /// </summary>
    ///
    ///-------------------------------------------------------------------------------------------------

    public class ReadLineMode : Mode
    {
        ///-------------------------------------------------------------------------------------------------
        /// <summary>   Constructors. </summary>
        ///
        /// <remarks>   Valery, 10/12/2022. </remarks>
        ///-------------------------------------------------------------------------------------------------

        public ReadLineMode () : base("readline")
        {
#if FIXME
            keyMap = new KeyMap();
           keyMap.Define(KBD.ParseSequence("Tab"),  AutoComplete);
           keyMap.Define(KBD.ParseSequence("UpArrow"), HistoryUp);
           keyMap.Define(KBD.ParseSequence("DownArrow"), HistoryUp);
           keyMap.Define(KBD.ParseSequence("C-c"), ControlC);
#endif
        }
#if FIXME

        /// <summary>   Callback when line was entered. </summary>
        public FastAction<string> OnReadLineListener = new FastAction<string>();

        ///-------------------------------------------------------------------------------------------------
        /// <summary>
        /// Callback when TAB pressed but no one completition found
        /// @string line
        /// @int caretPosition.
        /// </summary>
        ///-------------------------------------------------------------------------------------------------

        public FastAction<string, int> OnAutoCompletionDelegate = new FastAction<string, int>();

        ///-------------------------------------------------------------------------------------------------
        /// <summary>
        /// (Immutable)
        /// The istory buffer.
        /// </summary>
        ///-------------------------------------------------------------------------------------------------

        private readonly ReadLineHistory historyBuffer = new ReadLineHistory();

        ///-------------------------------------------------------------------------------------------------
        /// <summary>
        /// Auto complete
        /// </summary>
        ///
        ///
        /// <param name="args"> The arguments. </param>
        ///
        /// <returns>   An object. </returns>
        ///-------------------------------------------------------------------------------------------------

        private void AutoComplete(object[] args)
        {
            if (OnAutoCompletionDelegate == null)
                return;

            string text = null;
            int caretPosition = 0;
            //if ( GameConsole.GetInputLine(out text, out caretPosition))
            {
                if (caretPosition == 0) 
                    return;

                // only if the field is focused
                var variants = OnAutoCompletionDelegate(text, caretPosition);
                if (variants.Length == 1)
                {
                    var prefix = variants[0];
                    var result = prefix + text.Substring(caretPosition);
                    GameConsole.SetInputLine(result, prefix.Length, true);
                }
                else if (variants.Length > 1)
                {
                    var lines = StingTableBuilder.BuildTable(variants, GameConsole.BufferWidth, 2);
                    foreach (var line in lines)
                    {
                        GameConsole.WriteLine(line);
                    }
                }
            }
        }

        ///-------------------------------------------------------------------------------------------------
        /// <summary>   move inhistory up. </summary>
        ///
        ///
        /// <param name="args"> The arguments. </param>
        ///
        /// <returns>   An object. </returns>
        ///-------------------------------------------------------------------------------------------------

        private void HistoryUp(object[] args)
        {
            var str = historyBuffer.GetCurentLine( );
            historyBuffer.MoveToPreviousLine ( );
            if (str != null)
                GameConsole.SetInputLine(str, str.Length, true);
        }

        ///-------------------------------------------------------------------------------------------------
        /// <summary>   move inhistory down. </summary>
        ///
        ///
        /// <param name="args"> The arguments. </param>
        ///
        /// <returns>   An object. </returns>
        ///-------------------------------------------------------------------------------------------------

        private void HistoryDown(object[] args)
        {
            var str = historyBuffer.GetCurentLine ( );
            historyBuffer.MoveToNextLine ( );
            if (str != null)
                GameConsole.SetInputLine(str, str.Length, true);
        }

        ///-------------------------------------------------------------------------------------------------
        /// <summary>   move inhistory down. </summary>
        ///
        ///
        /// <param name="args"> The arguments. </param>
        ///
        /// <returns>   An object. </returns>
        ///-------------------------------------------------------------------------------------------------

        private object ControlC(object[] args)
        {
            var str = "";
            var caretPosition = 0;
            GameConsole.WriteLine ( str + "C-c" );
            if ( GameConsole.GetInputLine(out str, out caretPosition))
            {
                GameConsole.WriteLine(str + "C-c");
                GameConsole.SetInputLine("", 0, true);
            }
            return null;
        }
    }

    ///-------------------------------------------------------------------------------------------------
    /// <summary>   This clas is the history buffer for ReadLine mode. </summary>
    ///
    ///-------------------------------------------------------------------------------------------------

    public class ReadLineHistory
    {
        ///-------------------------------------------------------------------------------------------------
        /// <summary>   Constroctor. </summary>
        ///
        ///-------------------------------------------------------------------------------------------------

        public ReadLineHistory()
        { }

        ///-------------------------------------------------------------------------------------------------
        /// <summary>   Get history list. </summary>
        ///
        ///
        /// <returns>   the arrays of expressions in history. </returns>
        ///-------------------------------------------------------------------------------------------------

        public List<string> GetAllHistory ( )
        {
            return historyBuffer;
        }

        ///-------------------------------------------------------------------------------------------------
        /// <summary>   Get curent line from history or null. </summary>
        ///
        ///
        /// <returns>   The curent line. </returns>
        ///-------------------------------------------------------------------------------------------------

        public string GetCurentLine ( )
        {
            return ( historyPosition < historyBuffer.Count && historyPosition >= 0 ) ? historyBuffer[ historyPosition ] : null;
        }

        ///-------------------------------------------------------------------------------------------------
        /// <summary>   Add single word to the history. </summary>
        ///
        ///
        /// <param name="text"> The text. </param>
        ///-------------------------------------------------------------------------------------------------

        public void AddHistory ( string text )
        {
            MoveToLastLine ( );
            // only different text will be added
            if ( GetCurentLine ( ) != text )
            {
                historyBuffer.Add ( text );
                MoveToLastLine ( );
            }
        }

        ///-------------------------------------------------------------------------------------------------
        /// <summary>   Add multiple words to the history. </summary>
        ///
        ///
        /// <param name="history">  The history. </param>
        ///-------------------------------------------------------------------------------------------------

        public void AddHistory ( string[] history )
        {
            foreach ( var s in history )
                historyBuffer.Add ( s );
            MoveToLastLine ( );
        }

        ///-------------------------------------------------------------------------------------------------
        /// <summary>   Add list of strings to the history. </summary>
        ///
        ///
        /// <param name="history">  The history. </param>
        ///-------------------------------------------------------------------------------------------------

        public void AddHistory ( List<string> history )
        {
            foreach ( var s in history )
                historyBuffer.Add ( s );
            MoveToLastLine ( );
        }

        ///-------------------------------------------------------------------------------------------------
        /// <summary>   Clear all histiry. </summary>
        ///
        ///-------------------------------------------------------------------------------------------------

        public void ClearHistory ( )
        {
            historyBuffer.Clear ( );
            MoveToLastLine ( );
        }

        ///-------------------------------------------------------------------------------------------------
        /// <summary>
        /// Navigation methods
        /// </summary>
        ///
        ///-------------------------------------------------------------------------------------------------

        public void MoveToNextLine ( )
        {
            if ( historyPosition < historyBuffer.Count - 1 )
                historyPosition++;
        }

        ///-------------------------------------------------------------------------------------------------
        /// <summary>   Move to previous line. </summary>
        ///
        ///-------------------------------------------------------------------------------------------------

        public void MoveToPreviousLine ( )
        {
            if ( historyPosition > 0 )
                historyPosition--;
        }

        ///-------------------------------------------------------------------------------------------------
        /// <summary>   Move to last line. </summary>
        ///
        ///-------------------------------------------------------------------------------------------------

        private void MoveToLastLine ( )
        {
            historyPosition = historyBuffer.Count - 1;
        }

        ///-------------------------------------------------------------------------------------------------
        /// <summary>
        /// (Immutable)
        /// Objet's fields
        /// </summary>
        ///-------------------------------------------------------------------------------------------------

        private readonly List<string> historyBuffer = new List<string> ( 100 );

        /// <summary>   The history position. </summary>
        private int historyPosition;
#endif
    }

}