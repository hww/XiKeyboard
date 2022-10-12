/* Copyright (c) 2021 Valerya Pudova (hww) */

using XiKeyboard.KeyMaps;
using XiKeyboard.Menu;

namespace XiKeyboard.Rendering
{
    ///-------------------------------------------------------------------------------------------------
    /// <summary>   The MenuPanelRepresentation is the view model the keymap. </summary>
    ///
    ///-------------------------------------------------------------------------------------------------

    public class MenuPanelRepresentation
    {
        /// <summary>   (Immutable) the maximum menu lines. </summary>
        const int MAX_MENU_LINES = 64;

        /// <summary>   A parent menu. </summary>
        public MenuPanelRepresentation parent;

        /// <summary>   A reference to the KeyMap. </summary>
        public KeyMap keyMap;

        /// <summary>   The index of selected selectedLine. </summary>
        public int lineIndex;

        /// <summary>   The vector editor index or -1 for disabled. </summary>
        public int columnIndex = -1;

        /// <summary>   Width of the value. </summary>
        public int widthOfValue;
        /// <summary>   Name of the width of. </summary>
        public int widthOfName;
        /// <summary>   Width of the line. </summary>
        public int widthOfLine;


        /// <summary>   The title. </summary>
        public string title;
        /// <summary>   The menu lines. </summary>
        public MenuLineRepresentation[] items;
        public int Count;

        ///-------------------------------------------------------------------------------------------------
        /// <summary>   Constructor. </summary>
        ///
        ///
        /// <param name="parent">   A parent menu. </param>
        /// <param name="keyMap">   A reference to the KeyMap. </param>
        ///-------------------------------------------------------------------------------------------------

        public MenuPanelRepresentation(MenuPanelRepresentation parent, KeyMap keyMap)
        {
            this.parent = parent;
            this.keyMap = keyMap;
            items = new MenuLineRepresentation[MAX_MENU_LINES];
            lineIndex = 0;
            columnIndex = -1;
        }

        ///-------------------------------------------------------------------------------------------------
        /// <summary>   Calculate menu, name, value columns width. </summary>
        ///
        ///
        /// <param name="spaceSize">    Size of the space. </param>
        ///-------------------------------------------------------------------------------------------------

        public void PreRenderMenu(int spaceSize)
        {
            title = keyMap.Title;
            widthOfName = title.Length;
            widthOfValue = 0;
            widthOfLine = 0;

            for (var i = 0; i < Count; i++)
                items[i] = new MenuLineRepresentation();

            Count = 0;

            for (var i = 0; i < keyMap.Count; i++)
            {
                var line = keyMap[i].value;
                if (line is MenuLine)
                {
                    var menuLine = line as MenuLine;
                    var lineTitle = menuLine.Text;
                    var vectorCount = menuLine.Count;
                    var lineValue = string.Empty;
                    if (vectorCount == 1)
                    {
                        lineValue = menuLine.Value;
                    }
                    else
                    {
                        var selectedElement = columnIndex % vectorCount;
                        if (lineIndex == Count && selectedElement >= 0)
                        {
                            // Render highlighted vector element

                            lineValue += "(";
                            for (var j = 0; j < vectorCount; j++)
                            {
                                if (j == selectedElement)
                                    lineValue += string.Format("({0})", menuLine.GetValue(j));
                                else
                                    lineValue += string.Format(" {0} ", menuLine.GetValue(j));
                            }
                            lineValue += ")";
                        }
                        else
                        {
                            // Render vector as value only
                            lineValue += "(";
                            for (var j = 0; j < vectorCount; j++)
                                lineValue += string.Format(" {0} ", menuLine.GetValue(j));
                            lineValue += ")";
                        }
                    }
                    var val = menuLine.Value;
                    if (lineTitle != null)
                        widthOfName = System.Math.Max(widthOfName, lineTitle.Length);
                    if (lineValue != null)
                        widthOfValue = System.Math.Max(widthOfValue, lineValue.Length);

                    items[Count++] = new MenuLineRepresentation()
                    {
                        title = lineTitle,
                        value = lineValue,
                        line = menuLine
                    };
                }
                else if (line is KeyMap)
                {
                    var item = line as KeyMap;
                    var txt = item.Title + "...";
                    var val = string.Empty; // TODO Make shortcut
                    if (txt != null)
                        widthOfName = System.Math.Max(widthOfName, txt.Length);
                    if (val != null)
                        widthOfValue = System.Math.Max(widthOfValue, val.Length);
                    // Create the record
                    items[Count++] = new MenuLineRepresentation()
                    {
                        title = txt,
                        value = val,
                        line = item
                    };
                }
            }
            widthOfLine = System.Math.Max(widthOfLine, widthOfName + widthOfValue + spaceSize);
            lineIndex = GetSelectedLineIndex(lineIndex, true);

        }

        ///-------------------------------------------------------------------------------------------------
        /// <summary>
        /// Find the best position for selected selectedLine Skip all sepoarators and disabled lines.
        /// </summary>
        ///
        ///
        /// <param name="index">    Zero-based index of the. </param>
        /// <param name="forward">  TRUE - for forward moving. </param>
        ///
        /// <returns>   The selected line index. </returns>
        ///-------------------------------------------------------------------------------------------------

        private int GetSelectedLineIndex(int index, bool forward)
        {
            if (index >= Count)
            {
                index = Count - 1;
                forward = false;
            }
            else if (index < 0)
            {
                index = 0;
                forward = true;
            }
            if (forward)
            {
                for (; index < Count; index++)
                {
                    var line = items[index].line;
                    if (line == null)
                        continue;
                    if (line is MenuSeparator)
                        continue;
                    if (line is KeyMap)
                        return index;
                    if (line is MenuLine && (line as MenuLine).IsEnabled)
                        return index;
                }
            }
            for (; index >= 0; index--)
            {
                var line = items[index].line;
                if (line == null)
                    continue;
                if (line is MenuSeparator)
                    continue;
                if (line is KeyMap)
                    return index;
                if (line is MenuLine && (line as MenuLine).IsEnabled)
                    return index;
            }
            return -1; // There is nothing to select
        }

        ///-------------------------------------------------------------------------------------------------
        /// <summary>   Executes the 'event' action. </summary>
        ///
        ///
        /// <param name="menuEvent">    The menu event. </param>
        ///-------------------------------------------------------------------------------------------------

        public void OnEvent(MenuEvent menuEvent)
        {
            var eventType = menuEvent.eventType;
            var isShift = menuEvent.keyEvent.IsModifier(KeyModifiers.Shift);

            if (eventType == MenuLine.MenuEventType.Open)
            {
                lineIndex = 0;
                columnIndex = -1;
            }
            // There are two posibilities: a normal navigation and changig values
            // or the vector navigation and values

            var line = items[lineIndex].line as MenuLine;
            if (line != null)
            {
                if (line.Count == 1)
                {
                    // The normal option is not a vector
                    columnIndex = 0;
                }
                else
                {
                    // Change value of the vector
                    MenuEvent menuEventVector = menuEvent;

                    // The vector editor is active or inactivated
                    switch (eventType)
                    {
                        case MenuLine.MenuEventType.Left:
                            if (--columnIndex < -1)
                                columnIndex = -1;
                            return;

                        case MenuLine.MenuEventType.Right:
                            if (++columnIndex >= line.Count)
                                columnIndex = line.Count - 1;
                            return;
                    }

                    if (columnIndex >= 0)
                    {
                        // The vector editor is activated
                        switch (eventType)
                        {
                            case MenuLine.MenuEventType.Up:
                                menuEventVector.vectorIndex = columnIndex;
                                menuEventVector.eventType = MenuLine.MenuEventType.Increment;
                                (items[lineIndex].line as MenuLine)?.OnEvent(menuEventVector);
                                return;

                            case MenuLine.MenuEventType.Down:
                                menuEventVector.vectorIndex = columnIndex;
                                menuEventVector.eventType = MenuLine.MenuEventType.Decrement;
                                (items[lineIndex].line as MenuLine)?.OnEvent(menuEventVector);
                                return;

                            case MenuLine.MenuEventType.Reset:
                                (items[lineIndex].line as MenuLine)?.OnEvent(menuEvent);
                                break;
                        }
                        return;
                    }
                }
            }


            // Normal menu navigation
            switch (eventType)
            {
                case MenuLine.MenuEventType.Up:
                    columnIndex = -1;
                    lineIndex--;
                    lineIndex = GetSelectedLineIndex(lineIndex, false);
                    return;

                case MenuLine.MenuEventType.Down:
                    columnIndex = -1;
                    lineIndex++;
                    lineIndex = GetSelectedLineIndex(lineIndex, true);
                    return;
            }

            if (lineIndex < 0)
                return;

            if (items[lineIndex].line is KeyMap)
            {
                switch (eventType)
                {
                    case MenuLine.MenuEventType.Right:
                        DM.Open(items[lineIndex].line as KeyMap);
                        return;
                }

            }
            else if (items[lineIndex].line is MenuLine)
            {
                MenuEvent menuEventValue = menuEvent;
                switch (eventType)
                {
                    case MenuLine.MenuEventType.Left:
                        menuEventValue.eventType = MenuLine.MenuEventType.Decrement;
                        (items[lineIndex].line as MenuLine)?.OnEvent(menuEventValue);
                        break;

                    case MenuLine.MenuEventType.Right:
                        menuEventValue.eventType = MenuLine.MenuEventType.Increment;
                        (items[lineIndex].line as MenuLine)?.OnEvent(menuEventValue);
                        break;

                    case MenuLine.MenuEventType.Reset:
                        (items[lineIndex].line as MenuLine)?.OnEvent(menuEvent);
                        break;
                }
            }
        }
    }
}
