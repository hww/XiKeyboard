/* Copyright (c) 2021 Valerya Pudova (hww) */

namespace XiKeyboard.KeyMaps
{
    ///-------------------------------------------------------------------------------------------------
    /// <summary>   Any object which can receive the key event. </summary>
    ///
    ///-------------------------------------------------------------------------------------------------

    public interface IOnKeyDown
    {
        ///-------------------------------------------------------------------------------------------------
        /// <summary>   Executes the 'key down' action. </summary>
        ///
        /// <param name="evt">  The event. </param>
        ///
        /// <returns>   True if it succeeds, false if it fails. </returns>
        ///-------------------------------------------------------------------------------------------------

        bool OnKeyDown ( int evt );
    }
}
