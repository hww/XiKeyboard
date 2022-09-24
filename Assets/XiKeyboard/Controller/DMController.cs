/* Copyright (c) 2021 Valerya Pudova (hww) */

using System.Collections.Generic;
using UnityEngine;

namespace XiKeyboard
{
    /// <summary>
    /// The menu manager controller
    /// Initialize the menu system and input stream
    /// then control the visibitily of menu
    /// </summary>
    internal class DMController : IDMRender_OnGUI, IDMRender_Update
    {
        public delegate void Method();
        private KeyBuffer menuBuffer;
        private KeyMode menuMode;
        private KeyMap menuMap;
        private DMMenuRender menuRenderer;
        private DMMenuPanelRepresentation currentMenu;


        private bool isVisible;

        public DMController()
        {
            menuRenderer = new DMMenuRender();
            menuMap = new KeyMap("menu-map", "The menu key map");
            menuMode = new KeyMode("menu-mode", "The menu main mode", menuMap);
            menuBuffer = new KeyBuffer("menu-buffer", "The menu input buffer");
            menuBuffer.EnabeMajorMode(menuMode);

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

        ~DMController()
        {
            menuRenderer = null;
            menuMap = null;
            menuBuffer = null;
            menuMode = null;
            currentMenu = null;
            KeyBuffer.OnSequencePressed.Remove(OnSequencePressed);  // On press sequence delegate
            KeyBuffer.OnKeyPressed.Remove(OnKeyPressed);            // On press key delegate
            KeyBuffer.OnPseudoPressed.Remove(OnPseudoPressed);      // On keymap was selecte
        }

        public bool IsVisible => isVisible;

        public DMMenuPanelRepresentation Current => currentMenu;

        // Just render current menu if it is defined 
        // do not check the visibility -- render it
        void Redraw()
        {
            if (currentMenu != null)
                (menuRenderer as IDMRender).RenderMenu(currentMenu);
        }

        void SetVisibility(bool vis)
        {
            if (vis && currentMenu == null)
                currentMenu = new DMMenuPanelRepresentation(null, KeyMap.GlobalKeymap);
            isVisible = vis;
            menuBuffer.SetActive(isVisible);
        }

        public void ToggleVisibility()
        {
            SetVisibility(!isVisible);
        }

        public void Open(KeyMap menu = null)
        {
            if (menu == null)
                menu = KeyMap.GlobalKeymap;

            if (ContainsMenu(menu))
                CloseAllMenusUpTo(menu);
            else
                currentMenu = new DMMenuPanelRepresentation(Current, menu);
            SetVisibility(true);
            (menuRenderer as IDMRender).RenderMenu(currentMenu);
        }
        /// <summary>
        /// Unwind all menu items to the selected menu
        /// </summary>
        /// <param name="menu"></param>
        private void CloseAllMenusUpTo(KeyMap menu)
        {
            if (currentMenu != null)
            {
                var current = currentMenu;
                while (current != null)
                {
                    if (current.keyMap == menu)
                        return;
                    var parent = current.parent;
                    current.parent = null;
                    current = parent;
                }
            }
            currentMenu = new DMMenuPanelRepresentation(null, menu);
        }

        private bool ContainsMenu(KeyMap menu)
        {
            var current = currentMenu;
            while (current != null)
            {
                if (current.keyMap == menu)
                    return true;
                current = current.parent;
            }
            return false;
        }

        public void Close()
        {
            if (currentMenu == null)
                return;
            var parent = currentMenu.parent;
            currentMenu.parent = null;
            currentMenu = parent;
            if (currentMenu != null)
            {
                SetVisibility(true);
                (menuRenderer as IDMRender).RenderMenu(currentMenu);
            }
            else
            {
                SetVisibility(false);
            }
        }

        void IDMRender_OnGUI.OnGUI()
        {
            KeyInputManager.OnGUI();
            if (isVisible)
                (menuRenderer as IDMRender_OnGUI).OnGUI();
        }

        float readrawAt = 0;
        void IDMRender_Update.Update()
        {
            if (Time.unscaledTime>readrawAt)
            {
                readrawAt = Time.unscaledTime + 0.5f;
                Redraw();
            }
        }

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
        public void OnEvent(KeyEvent keyEvent)
        {
            var key = keyEvent.AsKeyCode;
            if (key == KeyCode.E)
                ToggleVisibility();
            if (key == KeyCode.Q)
                Close();

            if (currentMenu != null)
            {
                var shift = keyEvent.IsModifier(KeyModifiers.Shift);
                DMMenuLine.DMEvent menuEvt = DMMenuLine.DMEvent.None;

                if (isVisible)
                {
                    if (key == KeyCode.W)
                        menuEvt = DMMenuLine.DMEvent.Up;
                    if (key == KeyCode.S)
                        menuEvt = DMMenuLine.DMEvent.Down;
                    if (key == KeyCode.A)
                        menuEvt = DMMenuLine.DMEvent.Left;
                    if (key == KeyCode.D)
                        menuEvt = DMMenuLine.DMEvent.Right;
                    if (key == KeyCode.R)
                        menuEvt = DMMenuLine.DMEvent.Reset;
                }
                else
                {
                    if (shift)
                    {
                        if (key == KeyCode.A)
                            menuEvt = DMMenuLine.DMEvent.Left;
                        if (key == KeyCode.D)
                            menuEvt = DMMenuLine.DMEvent.Right;
                        if (key == KeyCode.R)
                            menuEvt = DMMenuLine.DMEvent.Reset;
                    }
                }
                if (currentMenu != null)
                {
                    currentMenu.OnEvent(menuEvt, shift);
                    Redraw();
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
