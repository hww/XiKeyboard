/* Copyright (c) 2016 Valery Alex P. All rights reserved. */


namespace XiKeyboard
{
    /// <summary>
    /// This class is one of input Modes. It allow to complete the word by TAB, and navigate 
    /// in history by arrow keys
    /// </summary>
    public class ReadLineMode : KeyMode
    {
   
        // ===============================================================================================
        // Constructors
        // ===============================================================================================
        public ReadLineMode () : base("readline")
        {
           // keyMap = new KeyMap();
           // keyMap.Define(KBD.ParseSequence("Tab"), new NativeFunction("AutoComplete", AutoComplete));
           // keyMap.Define(KBD.ParseSequence("UpArrow"), new NativeFunction("HistoryUp", HistoryUp));
           // keyMap.Define(KBD.ParseSequence("DownArrow"), new NativeFunction("HistoryUp", HistoryDown));
           // keyMap.Define(KBD.ParseSequence("C-c"), new NativeFunction("ControlC", ControlC));
        }     /*
        // ===============================================================================================
        // Delegates
        // ===============================================================================================
        /// <summary>
        /// Callback when line was entered
        /// </summary>
        public Action<string> OnReadLineListener = new Action<string>(string obj);
        /// <summary>
        /// Callback when TAB pressed but no one completition found
        /// @string line
        /// @int caretPosition
        /// </summary>
        public Action<string, int> OnAutoCompletionDelegate = new Action<string, int>();
        /// <summary>
        /// The istory buffer
        /// </summary>
        private readonly ReadLineHistory historyBuffer = new ReadLineHistory();
        // ===============================================================================================
        // KeyMap
        // ===============================================================================================


        private object AutoComplete(object[] args)
        {
            if (OnAutoCompletionDelegate.Count == 0)
                return null;

            string text = null;
            int caretPosition = 0;
            //if ( GameConsole.GetInputLine(out text, out caretPosition))
            {
                if (caretPosition == 0) return null;

                // only if the field is focused
                var variants = OnAutoCompletionDelegate(text, caretPosition);
                if (variants.Length == 1)
                {
                    var prefix = variants[0];
                    var result = prefix + text.Substring(caretPosition);
 //FIX!                   GameConsole.SetInputLine(result, prefix.Length, true);
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
            return null;
        }
        // move inhistory up
        private object HistoryUp(object[] args)
        {
            var str = historyBuffer.GetCurentLine( );
            historyBuffer.MoveToPreviousLine ( );
            if (str == null) return null;
       //     GameConsole.SetInputLine(str, str.Length, true);
            return null;
        }
        // move inhistory down
        private object HistoryDown(object[] args)
        {
            var str = historyBuffer.GetCurentLine ( );
            historyBuffer.MoveToNextLine ( );
            if (str == null) return null;
    //        GameConsole.SetInputLine(str, str.Length, true);
            return null;
        }
        // move inhistory down
        private object ControlC(object[] args)
        {
            var str = "";
            var caretPosition = 0;
            GameConsole.WriteLine ( str + "C-c" );
         //   if ( GameConsole.GetInputLine(out str, out caretPosition))
            {
     //           GameConsole.WriteLine(str + "C-c");
    //            GameConsole.SetInputLine("", 0, true);
            }
            return null;
        }
    }
    /// <summary>
    /// This clas is the history buffer for ReadLine mode
    /// </summary>
    public class ReadLineHistory
    {
        /// <summary>
        /// Constroctor
        /// </summary>
        public ReadLineHistory()
        { }
        // ===============================================================================================
        // Navigating trought history
        // the arrays of expressions in history
        // ===============================================================================================
        /// <summary>
        /// Get history list
        /// </summary>
        public List<string> GetAllHistory ( )
        {
            return historyBuffer;
        }
        /// <summary>
        /// Get curent line from history or null
        /// </summary>
        public string GetCurentLine ( )
        {
            return ( historyPosition < historyBuffer.Count && historyPosition >= 0 ) ? historyBuffer[ historyPosition ] : null;
        }
        /// <summary>
        /// Add single word to the history
        /// </summary>
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
        /// <summary>
        /// Add multiple words to the history
        /// </summary>
        public void AddHistory ( string[] history )
        {
            foreach ( var s in history )
                historyBuffer.Add ( s );
            MoveToLastLine ( );
        }
        /// <summary>
        /// Add list of strings to the history
        /// </summary>
        public void AddHistory ( List<string> history )
        {
            foreach ( var s in history )
                historyBuffer.Add ( s );
            MoveToLastLine ( );
        }
        /// <summary>
        /// Clear all histiry
        /// </summary>
        public void ClearHistory ( )
        {
            historyBuffer.Clear ( );
            MoveToLastLine ( );
        }
        // ===============================================================================================
        // Navigation methods
        // ===============================================================================================
        public void MoveToNextLine ( )
        {
            if ( historyPosition < historyBuffer.Count - 1 )
                historyPosition++;
        }
        public void MoveToPreviousLine ( )
        {
            if ( historyPosition > 0 )
                historyPosition--;
        }
        private void MoveToLastLine ( )
        {
            historyPosition = historyBuffer.Count - 1;
        }
        // ===============================================================================================
        // Objet's fields
        // ===============================================================================================
        private readonly List<string> historyBuffer = new List<string> ( 100 );
        private int historyPosition;
        */
    }

}