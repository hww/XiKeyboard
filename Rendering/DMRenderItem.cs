using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace XiKeyboard
{
    /// <summary>
    /// The DMRenderItem is the view for the DMMenuLine
    /// </summary>
    public class DMRenderItem
    {
        /// <summary>A reference to the KeyMap</summary>
        public KeyMap keyMap;
        /// <summary>The index of selected line</summary>
        public int selectedLine;

        public int widthOfShortcut;
        public int widthOfValue;
        public int widthOfName;
        public int widthOfNameAnValue;

        // The text items


        /// <summary>
        /// Calculate menu, name, value columns width
        /// </summary>
        public void UpdateWidth(int spaceSize, string[] titles, string[] values)
        {
            widthOfName = 0;
            widthOfValue = 0;
            widthOfNameAnValue = 0;

            for (var i = 0; i < keyMap.Count; i++)
            {
                var line = keyMap[i].value;
                if (line is DMMenuLine)
                {
                    var item = line as DMMenuLine;
                    var txt = item.Text;
                    var val = item.Value;
                    var srt = item.Shorcut;
                    widthOfName = System.Math.Max(widthOfName, txt.Length);
                    widthOfValue = System.Math.Max(widthOfValue, val.Length);
                    widthOfShortcut = System.Math.Max(widthOfShortcut, srt.Length);
                    titles[i] = txt;
                    values[i] = val;
                }
            }
            widthOfNameAnValue = System.Math.Max(widthOfNameAnValue, widthOfName + widthOfValue + spaceSize);
            widthOfName = System.Math.Max(widthOfName, widthOfNameAnValue - widthOfValue - spaceSize);
        }
    }
}
