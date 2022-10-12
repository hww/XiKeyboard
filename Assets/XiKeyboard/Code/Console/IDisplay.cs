using UnityEngine;

namespace XiKeyboard
{
    ///-------------------------------------------------------------------------------------------------
    /// <summary>   Interface for generic display. </summary>
    ///
    ///-------------------------------------------------------------------------------------------------

    public interface IDisplay
    {
        ///-------------------------------------------------------------------------------------------------
        /// <summary>   Write text to the input field. </summary>
        ///
        /// <param name="message">  The message to write. </param>
        ///-------------------------------------------------------------------------------------------------

        void Write ( string message );

        ///-------------------------------------------------------------------------------------------------
        /// <summary>   Write text to the input field and add new line. </summary>
        ///
        /// <param name="message">  The message to write. </param>
        ///-------------------------------------------------------------------------------------------------

        void WriteLine ( string message );

        /// <summary>   Produce beep sound if it is defined. </summary>
        void Beep ( );

        /// <summary>   Clear terminal screen. </summary>
        void Clear ( );

        /// <summary>   Reset foreground color to defaut. </summary>
        void ResetColor ( );

        ///-------------------------------------------------------------------------------------------------
        /// <summary>   Set foreground color. </summary>
        ///
        /// <param name="color">    The color. </param>
        ///-------------------------------------------------------------------------------------------------

        void SetColor ( Color color );

        ///-------------------------------------------------------------------------------------------------
        /// <summary>   Set background color. </summary>
        ///
        /// <param name="color">    The color. </param>
        ///-------------------------------------------------------------------------------------------------

        void SetBackgroundColor ( Color color );

        ///-------------------------------------------------------------------------------------------------
        /// <summary>   Set cursor position. </summary>
        ///
        /// <param name="x">    The x coordinate. </param>
        /// <param name="y">    The y coordinate. </param>
        ///-------------------------------------------------------------------------------------------------

        void SetCursor ( int x, int y );

        ///-------------------------------------------------------------------------------------------------
        /// <summary>   get buffer width. </summary>
        ///
        /// <value> The width of the buffer. </value>
        ///-------------------------------------------------------------------------------------------------

        int BufferWidth { get; }

        ///-------------------------------------------------------------------------------------------------
        /// <summary>   get buffer heght. </summary>
        ///
        /// <value> The height of the buffer. </value>
        ///-------------------------------------------------------------------------------------------------

        int BufferHeight { get; }
    }
}
