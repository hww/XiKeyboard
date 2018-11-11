namespace VARP.Keyboard
{
    public static class KeyModifyers
    {
        public const int MaxCode = 1 << 28 - 1;
        public const int Meta = 1 << 27;
        public const int Control = 1 << 26;
        public const int Shift = 1 << 25;
        public const int Hyper = 1 << 24;
        public const int Super = 1 << 23;
        public const int Alt = 1 << 22;
        public const int Pseudo = 1 << 21;    

        /// <summary>
        /// Use for masking the modifyer bits
        /// </summary>
        public const int AllModifyers = Control | Shift | Alt | Hyper | Super | Meta;

        /// <summary>
        /// Used internaly for iteration over modyfier. It is replacement for System.Enum.Values(typeof(Modifyers)).
        /// </summary>
        public static readonly int[] AllModifyersList = new int[] { Control, Alt, Shift };
    }
}