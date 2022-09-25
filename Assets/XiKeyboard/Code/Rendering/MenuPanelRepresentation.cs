/* Copyright (c) 2021 Valerya Pudova (hww) */

using XiKeyboard.KeyMaps;
using XiKeyboard.Menu;

namespace XiKeyboard.Rendering
{
    /// <summary>
    /// The MenuPanelRepresentation is the view model the keymap
    /// </summary>
    public class MenuPanelRepresentation
    {
        const int MAX_MENU_LINES = 64;

        /// <summary>A parent menu</summary>
        public MenuPanelRepresentation parent;
        /// <summary>A reference to the KeyMap</summary>
        public KeyMap keyMap;
        /// <summary>The index of selected line</summary>
        public int selectedLine;

        public bool isChanged;

        public int widthOfValue;
        public int widthOfName;
        public int widthOfLine;

        public string title;
        /// <summary>The menu lines</summary> 
        public MenuLineRepresentation[] items;
        public int Count;

        public MenuPanelRepresentation(MenuPanelRepresentation parent, KeyMap keyMap)
        {
            this.parent = parent;
            this.keyMap = keyMap;
            items = new MenuLineRepresentation[MAX_MENU_LINES];
            selectedLine = 0;
        }

        /// <summary>
        /// Calculate menu, name, value columns width
        /// </summary>
        public void Update(IMenuController controller, int spaceSize)
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
                    var item = line as MenuLine;
                    var txt = item.Text;
                    var val = item.Value;
                    if (txt != null)
                        widthOfName = System.Math.Max(widthOfName, txt.Length);
                    if (val != null)
                        widthOfValue = System.Math.Max(widthOfValue, val.Length);
                    items[Count++] = new MenuLineRepresentation()
                    {
                        title = txt,
                        value = val,
                        line = item
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
            selectedLine = GetSelectedLineIndex(selectedLine, true);
        }

        /// <summary>
        /// Find the best position for selected line
        /// Skip all sepoarators and disabled lines
        /// </summary>
        /// <param name="forward"></param>
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

        public void OnEvent(IMenuController controller)
        {
            var menuEvt = controller.GetMenuEvt();
            var isShift = controller.GetKeyEvent().IsModifier(KeyModifiers.Shift);

            switch (menuEvt)
            {
                case MenuLine.MenuEvent.Up:
                    selectedLine--;
                    selectedLine = GetSelectedLineIndex(selectedLine, false);
                    return;

                case MenuLine.MenuEvent.Down:
                    selectedLine++;
                    selectedLine = GetSelectedLineIndex(selectedLine, true);
                    return;
            }

            if (selectedLine < 0)
                return;

            if (items[selectedLine].line is KeyMap)
            {
                switch (menuEvt)
                {
                    case MenuLine.MenuEvent.Right:
                        DM.Open(items[selectedLine].line as KeyMap);
                        return;
                }

            }
            else if (items[selectedLine].line is MenuLine)
            {
                switch (menuEvt)
                {
                    case MenuLine.MenuEvent.Left:
                        (items[selectedLine].line as MenuLine)?.OnEvent(controller);
                        break;

                    case MenuLine.MenuEvent.Right:
                        (items[selectedLine].line as MenuLine)?.OnEvent(controller);
                        break;

                    case MenuLine.MenuEvent.Reset:
                        (items[selectedLine].line as MenuLine)?.OnEvent(controller);
                        break;
                }
            }
        }
    }
}
