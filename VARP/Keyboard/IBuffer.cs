namespace VARP.Keyboard
{
    public interface IBuffer
    {
        void Enable();
        string Name { get; }
        string Help { get; }
        void EnabeMajorMode(Mode mode);
        void DisableMajorMode();
        void EnabeMinorMode(Mode mode);
        void DisableMinorMode(Mode mode);
        KeyMapItem Lockup(int[] sequence, int starts, int ends, bool acceptDefaults);
    }
}