using System;
using System.Text;
using UnityEditor;
using VARP.StringTools;

namespace VARP.Keyboard
{
    public class MenuTextRenderer
    {
        public static readonly MenuOptions DefaultOptions = new MenuOptions();
        private const int SPACE_BETWEEN_COLUMNS = 1;
        private const int SPACE_BETWEEN_EDGES = 1;

        public class MenuOptions
        {
            public int textWidth = 20;
            public int valueWidth = 10;
            public int Width => textWidth + valueWidth;
        }
        
        public string RenderMenu(KeyMap menu, int selected, MenuOptions options = null)
        {
            var sb = new StringBuilder();
            var count = menu.Count;
            var txtItems = new string[count];
            var valItems = new string[count];
            var txtWidth = 0;
            var valWidth = 0;
            for (var i = 0; i < count; i++)
            {
                var line = menu[i].value;
                if (line is MenuLineSimple)
                {
                    var item = line as MenuLineSimple;
                    txtItems[i] = item.Text;
                    valItems[i] = item.Shorcut;
                    txtWidth = Math.Max(txtWidth, txtItems[i].Length);
                    valWidth = Math.Max(valWidth, valItems[i].Length);
                }
                else if (line is MenuLineComplex)
                {
                    var item = line as MenuLineComplex;
                    txtItems[i] = item.Text;
                    valItems[i] = item.Shorcut;
                    txtWidth = Math.Max(txtWidth, txtItems[i].Length);
                    valWidth = Math.Max(valWidth, valItems[i].Length);
                }
            }
            var itemWidth = txtWidth + valWidth + SPACE_BETWEEN_COLUMNS;
            var lineWidth = itemWidth + SPACE_BETWEEN_EDGES * 2;

            string itemFormat2 =
                $"|{{0,{SPACE_BETWEEN_EDGES}}}{{1,{txtWidth}}}{{2,{SPACE_BETWEEN_COLUMNS}}}{{3,{valWidth}}}{{4,{SPACE_BETWEEN_EDGES}}}|";
            string itemFormat1 =
                $"|{{0,{SPACE_BETWEEN_EDGES}}}{{1,{itemWidth}}}{{2,{SPACE_BETWEEN_EDGES}}}|";
            
            string lineFormat = "|{0}|";
            string spaceLine = null;
            string singleLine = null;
            string dashedLine = null;
            
            for (var i = 0; i < count; i++)
            {
                var line = menu[i].value;
                var isSelected = i == selected;
                var prefix = isSelected ? '>' : ' ';
                var suffix = isSelected ? '<' : ' ';
                
                if (line is MenuSeparator)
                {
                    var item = line as MenuSeparator;
                    switch (item.type)
                    {
                        case MenuSeparator.Type.NoLine:
                            break;
                        case MenuSeparator.Type.Space:
                            if (spaceLine == null) spaceLine = string.Format(lineFormat, new string(' ', lineWidth));
                            sb.AppendLine(spaceLine);
                            break;
                        case MenuSeparator.Type.SingleLine:
                            if (singleLine == null) singleLine = string.Format(lineFormat, new string('-', lineWidth));
                            sb.AppendLine(singleLine);
                            break;
                        case MenuSeparator.Type.DashedLine:
                            if (dashedLine == null) dashedLine = string.Format(lineFormat, new string('~', lineWidth));
                            sb.AppendLine(singleLine);
                            break;
                        default:
                            if (valItems[i] == null)
                                sb.AppendLine(string.Format(itemFormat2, prefix, txtItems[i], ' ', valItems[i], suffix));
                            else if (txtItems[i] != null)
                                sb.AppendLine(string.Format(itemFormat1, prefix, txtItems[i], suffix));                            
                            else
                                throw new ArgumentOutOfRangeException();
                            break;
                    }
                }
            }
            return sb.ToString();
        }
        
    }
}