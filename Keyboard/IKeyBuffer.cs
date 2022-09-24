/* Copyright (c) 2021 Valerya Pudova (hww) */

namespace XiKeyboard
{
    /// <summary>
    /// Each buffer is like a recipient of events. And only one of them receive events in this moment.
    /// As every buffer has its own modes activated, to switch buffer means switch the modes too.
    /// </summary>
    public interface IKeyBuffer
    {
        void SetActive ( bool state );
        string Name { get; }
        string Help { get; }
        void EnabeMajorMode ( KeyMode mode );
        void DisableMajorMode ( );
        void EnabeMinorMode ( KeyMode mode );
        void DisableMinorMode ( KeyMode mode );
        DMKeyMapItem Lookup ( KeyEvent [] sequence, int starts, int ends, bool acceptDefaults );
        string GetBufferString();
        string GetBufferSubString(int starts, int ends);
        int Point { get; set; }
        void GetSelection(out int begin, out int end);
        void SetSelection(int starts, int ends);
    }
}

