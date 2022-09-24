/* Copyright (c) 2021 Valerya Pudova (hww) : For more information read the license file */

namespace XiKeyboard
{
    public static class DMConfig
    {
        public const string PrefixNormal = " ";
        public const string PrefixCursor = ">";
        public const string SuffixNormal = " ";   //< Suffix for all lines
        public const string SuffixModified = "*"; //< Suffix for modified line
        public const string Space = "  ";          //< Space between title and value

        public const char DashedLineChar = '-';
        public const char NormalLineChar = '─';

    }

	/// <summary>
	/// The stylesheet for menu system
	/// </summary>
	public static class DMColors
	{
		public const string Header = "#eeffff";
		public const string TitleNormal = "#9BB7D4";
		public const string TitleDisabled = "#566573";
		// Checkbox and radio button
		public const string ValueNormal = "#A0DAA9"; 
		public const string ValueDisabled = "#566573";
		// Checkbox and radio button
		public const string ToggleTitleActive = "#F5DF4D";
		public const string ToggleTitleInactive = "#d4d6d5";
		// Suffix and prefix
		public const string Cursor = "#ffffff";
		public const string SuffixNormal = "#eeeeee";
		public const string SuffixModified = "#FDAC53";
		// The cursor
		public const string HorizontalLine = "#939597";

	}
}