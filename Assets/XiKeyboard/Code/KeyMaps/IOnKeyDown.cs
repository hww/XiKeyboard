﻿/* Copyright (c) 2021 Valerya Pudova (hww) */

namespace XiKeyboard.KeyMaps
{
    /// <summary>
    /// Any object which can receive the key event
    /// </summary>
    public interface IOnKeyDown
    {
        bool OnKeyDown ( int evt );
    }
}