/* Copyright (c) 2021 Valerya Pudova (hww) */

using XiKeyboard.Modes;
using XiKeyboard.KeyMaps;

namespace XiKeyboard.Buffers
{
    ///-------------------------------------------------------------------------------------------------
    /// <summary>
    /// Each buffer is like a recipient of events. And only one of them receive events in this
    /// moment. As every buffer has its own modes activated, to switch buffer means switch the modes
    /// too.
    /// </summary>
    ///
    ///-------------------------------------------------------------------------------------------------

    public interface IBuffer
    {
        ///-------------------------------------------------------------------------------------------------
        /// <summary>   Sets an active. </summary>
        ///
        /// <param name="state">    True to state. </param>
        ///-------------------------------------------------------------------------------------------------

        void SetActive ( bool state );

        ///-------------------------------------------------------------------------------------------------
        /// <summary>   Gets the name. </summary>
        ///
        /// <value> The name. </value>
        ///-------------------------------------------------------------------------------------------------

        string Name { get; }

        ///-------------------------------------------------------------------------------------------------
        /// <summary>   Gets the help. </summary>
        ///
        /// <value> The help. </value>
        ///-------------------------------------------------------------------------------------------------

        string Help { get; }

        ///-------------------------------------------------------------------------------------------------
        /// <summary>   Enabe major mode. </summary>
        ///
        /// <param name="mode"> The mode. </param>
        ///-------------------------------------------------------------------------------------------------

        void EnabeMajorMode ( Mode mode );

        /// <summary>   Disables the major mode. </summary>
        void DisableMajorMode ( );

        ///-------------------------------------------------------------------------------------------------
        /// <summary>   Enabe minor mode. </summary>
        ///
        /// <param name="mode"> The mode. </param>
        ///-------------------------------------------------------------------------------------------------

        void EnabeMinorMode ( Mode mode );

        ///-------------------------------------------------------------------------------------------------
        /// <summary>   Disables the minor mode. </summary>
        ///
        /// <param name="mode"> The mode. </param>
        ///-------------------------------------------------------------------------------------------------

        void DisableMinorMode ( Mode mode );

        ///-------------------------------------------------------------------------------------------------
        /// <summary>   Looks up a given key to find its associated value. </summary>
        ///
        /// <param name="sequence">         The sequence. </param>
        /// <param name="starts">           The starts. </param>
        /// <param name="ends">             The ends. </param>
        /// <param name="acceptDefaults">   True to accept defaults. </param>
        ///
        /// <returns>   A DMKeyMapItem. </returns>
        ///-------------------------------------------------------------------------------------------------

        DMKeyMapItem Lookup ( KeyEvent [] sequence, int starts, int ends, bool acceptDefaults );

        ///-------------------------------------------------------------------------------------------------
        /// <summary>   Gets buffer string. </summary>
        ///
        /// <returns>   The buffer string. </returns>
        ///-------------------------------------------------------------------------------------------------

        string GetBufferString();

        ///-------------------------------------------------------------------------------------------------
        /// <summary>   Gets buffer sub string. </summary>
        ///
        /// <param name="starts">   The starts. </param>
        /// <param name="ends">     The ends. </param>
        ///
        /// <returns>   The buffer sub string. </returns>
        ///-------------------------------------------------------------------------------------------------

        string GetBufferSubString(int starts, int ends);

        ///-------------------------------------------------------------------------------------------------
        /// <summary>   Gets or sets the point. </summary>
        ///
        /// <value> The point. </value>
        ///-------------------------------------------------------------------------------------------------

        int Point { get; set; }

        ///-------------------------------------------------------------------------------------------------
        /// <summary>   Gets a selection. </summary>
        ///
        /// <param name="begin">    [out] The begin. </param>
        /// <param name="end">      [out] The end. </param>
        ///-------------------------------------------------------------------------------------------------

        void GetSelection(out int begin, out int end);

        ///-------------------------------------------------------------------------------------------------
        /// <summary>   Sets a selection. </summary>
        ///
        /// <param name="starts">   The starts. </param>
        /// <param name="ends">     The ends. </param>
        ///-------------------------------------------------------------------------------------------------

        void SetSelection(int starts, int ends);
    }
}

