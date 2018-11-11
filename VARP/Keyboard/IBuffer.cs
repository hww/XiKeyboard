namespace VARP.Keyboard
{
    public interface IBuffer
    {
        void Enable();
        string Name { get; }
        string Help { get; }
        void EnableMajorMode(Mode mode);
        void DisableMajorMode();
        void EnableMinorMode(Mode mode);
        void DisableMinorMode(Mode mode);
        KeyMapItem Lockup(KeyEvent[] sequence, int starts, int ends, bool acceptDefaults);
    }
}