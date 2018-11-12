/* Copyright (c) 2016 Valery Alex P. All rights reserved. */

namespace VARP.Keyboard
{
    /// <summary>
    /// Any object which can receive the key event
    /// </summary>
    public interface IOnKeyDown
    {
        bool OnKeyDown ( int evt );
    }
}
