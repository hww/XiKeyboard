using System;
using System.Collections.Generic;
using System.Text;
using UnityEditor;
using UnityEngine;
using XiCore.StringTools;
using XiKeyboard.Notifications;

namespace XiKeyboard
{


    public class DMRender : IDMRender, IDMRender_OnGUI, IDMRender_Update
    {
        private const string PREFIX = " ";
        private const string PREFIX_SELECTED = ">";
        private const string SUFFIX = " ";
        private const string SPACE = "  ";

        private const char CHAR_DASHED_LINE = '-';
        private const char CHAR_LINE = '─';

        private readonly BetterStringBuilder stringBuilder = new BetterStringBuilder(80 * 40);
        private string menuText;
        private GUISkin menuSkin;
        private KeyBuffer menuBuffer;
        private KeyMode menuMode;
        private KeyMap menuMap;
        private DMRenderItem globalMenu;

        private Stack<DMRenderItem> menuStack;
        private DMRenderItem Curent => menuStack.Count == 0 ? null : menuStack.Peek();

        private bool isVisible;

        public DMRender()
        {
            menuSkin = Resources.Load<GUISkin>("XiKeyboard/Skins/Default Skin");
            menuText = String.Empty;
            menuStack = new Stack<DMRenderItem>(16);
            menuMap = new KeyMap("menu-map", "The menu key map");
            menuBuffer = new KeyBuffer("menu-buffer", "The menu input buffer");
            menuMode = new KeyMode("menu-mode", "The menu main mode", menuMap);
            globalMenu = new DMRenderItem() { keyMap = KeyMap.GlobalKeymap, selectedLine = 0 };

            KeyMap.GlobalKeymap.SetLocal("q", menuControlEvent);
            KeyMap.GlobalKeymap.SetLocal("e", menuControlEvent);
            KeyMap.GlobalKeymap.SetLocal("r", menuControlEvent);
            KeyMap.GlobalKeymap.SetLocal("s", menuControlEvent);
            KeyMap.GlobalKeymap.SetLocal("d", menuControlEvent);
            // the next two works only in the menu mode
            menuMap.SetLocal("w", menuControlEvent);
            menuMap.SetLocal("a", menuControlEvent);
            menuMap.SetLocal("S-w", menuControlEvent);
            menuMap.SetLocal("S-a", menuControlEvent);

            KeyBuffer.OnSequencePressed.Add(OnSequencePressed);  // On press sequence delegate
            KeyBuffer.OnKeyPressed.Add(OnKeyPressed);            // On press key delegate
            KeyBuffer.OnPseudoPressed.Add(OnPseudoPressed);      // On keymap was selected
        }

        ~DMRender()
        {
            menuSkin = null;
            menuText = null;
            menuText = null;
            menuMap = null;
            menuBuffer = null;
            menuMode = null;
            globalMenu = null;
            KeyBuffer.OnSequencePressed.Remove(OnSequencePressed);  // On press sequence delegate
            KeyBuffer.OnKeyPressed.Remove(OnKeyPressed);            // On press key delegate
            KeyBuffer.OnPseudoPressed.Remove(OnPseudoPressed);      // On keymap was selecte
        }
        void Toggle()
        {
            isVisible = !isVisible;
            menuBuffer.SetActive(isVisible);
        }

        void Open(KeyMap menu = null)
        {
            if (menu == null)
            {
                (this as IDMRender).RenderMenu(globalMenu);
            }
            else
            {
                // Unwind all menu items to the selected menu
                var menuList = menuStack.ToArray();
                for (int i = 0; i < menuList.Length; i++)
                {
                    if (menuList[i].keyMap == menu)
                    {
                        var diffNum = (menuStack.Count - 1) - i;
                        for (var j = 0; j < diffNum; j++)
                            menuStack.Pop();
                        // Render the menu at the top of stack
                        (this as IDMRender).RenderMenu(Curent);
                        return;
                    }
                }
                // Create a menu at the top of stack
                (this as IDMRender).RenderMenu(new DMRenderItem() { keyMap = menu, selectedLine = 0 });
            }
        }
        void Close()
        {
            menuStack.Pop();
            var cur = Curent;
            if (cur != null)
            {
                (this as IDMRender).RenderMenu(cur);
            }
            else
                isVisible = false;
        }
        void Redraw()
        {
            var cur = Curent;
            if (cur != null)
                (this as IDMRender).RenderMenu(cur);
            else
                isVisible = false;
        }
        const int MAX_MENU_LINES = 64;
        private static string[] txtItems = new string[MAX_MENU_LINES];
        private static string[] valItems = new string[MAX_MENU_LINES];
        void IDMRender.RenderMenu(DMRenderItem menuItem, DMMenuOptions options = DMMenuOptions.Default)
        {

            stringBuilder.Clear();

            var keyMap = menuItem.keyMap;
            var count = menuItem.keyMap.Count;
            // Calculate te width and read all texts from

            menuItem.UpdateWidth(SPACE.Length, txtItems, valItems);
            var lineWidth = menuItem.widthOfNameAnValue + SUFFIX.Length + PREFIX.Length;

            string spaceLine = null;
            string dashedLine = null;
            string singleLine = new string(CHAR_LINE, lineWidth);

            // Prepair a formatting lines
            string itemFormat1 = $"{{0}}<color={{3}}>{{1,-{menuItem.widthOfNameAnValue}}}</color>{{2}}";
            string itemFormat2 = $"{{0}}<color={{4}}>{{1,-{menuItem.widthOfName}}}</color>{SPACE}<color={{5}}>{{2,{menuItem.widthOfValue}}}</color>{{3}}";

            // Render a menu header with separator
            var header = string.Format(itemFormat1, PREFIX, keyMap.title, SUFFIX, DMColors.HeaderDefault);
            stringBuilder.AppendLine(header);
            stringBuilder.AppendLine(singleLine);
            
            // Render the mnu items
            for (var i = 0; i < count; i++)
            {
                var line = keyMap[i].value;
                var isSelected = i == menuItem.selectedLine;

                if (line is DMMenuSeparator)
                {
                    var item = line as DMMenuSeparator;
                    switch (item.type)
                    {
                        case DMMenuSeparator.Type.NoLine:
                            break;
                        case DMMenuSeparator.Type.Space:
                            if (spaceLine == null)
                                spaceLine = new string(' ', lineWidth);
                            stringBuilder.AppendLine(spaceLine);
                            break;
                        case DMMenuSeparator.Type.SingleLine:
                            stringBuilder.AppendLine(singleLine);
                            break;
                        case DMMenuSeparator.Type.DashedLine:
                            if (dashedLine == null)
                                dashedLine = new string(CHAR_DASHED_LINE, lineWidth);
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
                    stringBuilder.AppendLine(item);
                }
            }

            menuText = stringBuilder.ToString();
        }

        void IDMRender_Update.Update()
        {
            DN.Update();
        }

        void IDMRender_OnGUI.OnGUI()
        {
            KeyInputManager.OnGUI();
            DN.OnGUI();

            if (!isVisible)
                return;

            GUI.skin = menuSkin;

            var textSize = GUI.skin.label.CalcSize(new GUIContent(menuText)) + new Vector2(10, 10);
            var position = new Vector2(20, 20);
            var rect = new Rect(position, textSize);

            GUI.Box(rect, GUIContent.none);

            rect.x += 5f;
            rect.width -= 5f * 2f;
            rect.y += 5f;
            rect.height -= 5f * 2f;

            GUI.Label(rect, menuText);
        }

        public delegate void Method();

        // The user typed the keystroke
        void OnSequencePressed(KeyBuffer buffer, DMKeyMapItem item)
        {
            if (item.value is Method)
            {
                (item.value as Method).Invoke();
            }
            else if (item.value == menuControlEvent)
            {
                var evt = new KeyEvent(item.key);
                OnEvent(evt);

            }
            else if (item.value is KeyMap)
                OnPseudoPressed(buffer, item);
            else
                Debug.Log("{" + item.value + "}");  // Print "Pressed Sequence: N" 	
        }

        readonly KeyEvent EventE = KeyEvent.MakeEvent(KeyCode.E, KeyModifiers.None);
        public void OnEvent(KeyEvent evt)
        {
            var key = evt.AsKeyCode;
            if (key == KeyCode.E)
                Toggle();
            if (key == KeyCode.Q)
                Close ();

            var cur = Curent;
            if (cur != null)
            {
                var shift = evt.IsModifier(KeyModifiers.Shift);
                if (isVisible)
                {

                }
                else
                {
                    if (shift)
                    {
                        if (key == KeyCode.A)
                            return DebugMenuItem.EvenTag.Left;
                        if (key == KeyCode.D)
                            return DebugMenuItem.EvenTag.Right;
                        if (key == KeyCode.R)
                            return DebugMenuItem.EvenTag.Reset;
                    }
                }
            }
        }

        void OnKeyPressed(KeyBuffer buffer, KeyEvent evt)
        {
            Debug.Log(buffer.GetBufferHumanizedString());   // Just display current buffer content		
        }

        void OnPseudoPressed(KeyBuffer buffer, DMKeyMapItem item)
        {
            Debug.Log("{menu:" + item.value + "}");
           // (this as IDMRender).RenderMenu(item.value as KeyMap, 0);
        }

        readonly object menuControlEvent = new object();
    }
}