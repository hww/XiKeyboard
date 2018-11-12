/* Copyright (c) 2016 Valery Alex P. All rights reserved. */

namespace VARP.Keyboard
{
    /// <summary>
    /// Each buffer is like a recipient of events. And only one of them receive events in this moment.
    /// As every buffer has its own modes activated, to switch buffer means switch the modes too.
    /// </summary>
    public interface IBuffer
    {
        void Enable ( );
        string Name { get; }
        string Help { get; }
        void EnabeMajorMode ( Mode mode );
        void DisableMajorMode ( );
        void EnabeMinorMode ( Mode mode );
        void DisableMinorMode ( Mode mode );
        KeyMapItem Lockup ( int[] sequence, int starts, int ends, bool acceptDefaults );
        string GetBufferString();
        string GetBufferSubString(int starts, int ends);
        int Point { get; set; }
        void GetSelection(out int begin, out int end);
        void SetSelection(int starts, int ends);
    }
}

