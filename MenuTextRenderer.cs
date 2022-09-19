using System;
using System.Text;
using UnityEditor;
using XiCore.StringTools;

namespace XiKeyboard
{
    public class MenuTextRenderer
    {
        private const string SUFFIX = " ";
        private const string PREFIX = " ";
        private const string PREFIX_SELECTED = ">";
        private const string SPACE = "  ";
        private const char CHAR_SQUARE_DOT = '▪';
        private const char CHAR_DASHED_LINE = '-';
        private const char CHAR_LIGHT_HORIZONTAL = '─';
            
        private const  string upperLineFormat  = "┌{0}┐";
        private const  string middleLineFormat = "├{0}┤";
        private const  string bottomLineFormat = "└{0}┘";
        private const  string menuLineFormat   = "│{0}│";
        
        
        private static readonly BetterStringBuilder stringBuilder = new BetterStringBuilder(80*40);

        public enum MenuOptions
        {
            Default
        }
        
        public static string RenderMenu(KeyMap menu, int selected, MenuOptions options = MenuOptions.Default)
        {
            stringBuilder.Clear();
            var count = menu.Count;
            var txtItems = new string[count];
            var valItems = new string[count];
            var txtWidth = 0;
            var valWidth = 0;
            var txt = string.Empty;
            var val = string.Empty;
            for (var i = 0; i < count; i++)
            {
                var line = menu[i].value;
                if (line is MenuLine)
                {
                    var item = line as MenuLine;
                    txt = item.Text;
                    val = item.Shorcut;
                }
                
                if (txt != null)
                    txtWidth = Math.Max(txtWidth, txt.Length);
                if (val != null)
                    valWidth = Math.Max(valWidth, val.Length);

                txtItems[i] = txt;
                valItems[i] = val;
            }
            var itemWidth = txtWidth + valWidth + SPACE.Length;
            var lineWidth = itemWidth + SUFFIX.Length + PREFIX.Length;

            string spaceLine = null;
            string dashedLine = null;
            string singleLine = null;
            string justLine = new string(CHAR_LIGHT_HORIZONTAL, lineWidth);
            
            stringBuilder.AppendLine(string.Format( upperLineFormat, justLine));
            
            string itemFormat1 = $"{{0}}{{1,-{itemWidth}}}{{2}}";
            string itemFormat2 = $"{{0}}{{1,-{txtWidth}}}{SPACE}{{2,-{valWidth}}}{{3}}";

            
            for (var i = 0; i < count; i++)
            {
                var line = menu[i].value;
                var isSelected = i == selected;

                
                if (line is MenuSeparator)
                {
                    var item = line as MenuSeparator;
                    switch (item.type)
                    {
                        case MenuSeparator.Type.NoLine:
                            break;
                        case MenuSeparator.Type.Space:
                            if (spaceLine == null) 
                                spaceLine = string.Format(menuLineFormat, new string(' ', lineWidth));
                            stringBuilder.AppendLine(spaceLine);
                            break;
                        case MenuSeparator.Type.SingleLine:
                            if (singleLine == null) 
                                singleLine = string.Format(middleLineFormat,justLine);
                            stringBuilder.AppendLine(singleLine);
                            break;
                        case MenuSeparator.Type.DashedLine:
                            if (dashedLine == null) 
                                dashedLine = string.Format(middleLineFormat, new string(CHAR_DASHED_LINE, lineWidth));
                            stringBuilder.AppendLine(dashedLine);
                            break;
                        default:
                            throw new Exception();
                    }
                }
                else
                {
                    var prefix = isSelected ? PREFIX_SELECTED : PREFIX;
                    string item;
                    if (valItems[i] == null)
                        item = string.Format(itemFormat1, prefix, txtItems[i], SUFFIX);
                    else if (txtItems[i] != null)
                        item = string.Format(itemFormat2, prefix, txtItems[i], valItems[i], SUFFIX);                            
                    else
                        throw new ArgumentOutOfRangeException();
                    stringBuilder.AppendLine(string.Format(menuLineFormat, item));
                }
            }
            stringBuilder.AppendLine(string.Format( bottomLineFormat, justLine));
            return stringBuilder.ToString();
        }
        
    }
}