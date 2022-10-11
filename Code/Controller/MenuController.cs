﻿/* Copyright (c) 2021 Valerya Pudova (hww) */

using System.Collections.Generic;
using UnityEngine;
using XiKeyboard.Buffers;
using XiKeyboard.KeyMaps;
using XiKeyboard.Menu;
using XiKeyboard.Modes;
using XiKeyboard.Rendering;

namespace XiKeyboard
{
    public struct MenuEvent
    {
        public KeyEvent keyEvent;
        public MenuLine.MenuEventType eventType;
        public int vectorIndex;
    }

    /// <summary>
    /// The menu manager controller
    /// Initialize the menu system and input stream
    /// then control the visibitily of menu
    /// </summary>
    internal class MenuController : IMenuRender_OnGUI, IMenuRender_Update
    {
        #region Public
        public MenuController()
        {
            menuRenderer = new MenuRender();
            menuMap = MenuMap.MenuBar;
            //menuMap = new KeyMap("menu-map", "The menu key map");
            menuMode = new Mode("menu-mode", "The menu main mode", menuMap);
            menuBuffer = new Buffer("menu-buffer", "The menu input buffer");
            menuBuffer.EnabeMajorMode(menuMode);

            KeyMap.GlobalKeymap.SetLocal("q", menuControlEvent);
            KeyMap.GlobalKeymap.SetLocal("e", menuControlEvent);
            KeyMap.GlobalKeymap.SetLocal("r", menuControlEvent);
            KeyMap.GlobalKeymap.SetLocal("s", menuControlEvent);
            KeyMap.GlobalKeymap.SetLocal("d", menuControlEvent);
            KeyMap.GlobalKeymap.SetLocal("x", menuControlEvent);

            // the next two works only in the menu mode
            menuMap.SetLocal("w", menuControlEvent);
            menuMap.SetLocal("a", menuControlEvent);
            menuMap.SetLocal("S-w", menuControlEvent);
            menuMap.SetLocal("S-a", menuControlEvent);

            Buffer.OnSequencePressed.Add(OnSequencePressed);  // On press sequence delegate
            Buffer.OnSequenceProgress.Add(OnSequenceProgress);// On press part of sequence delegate
            Buffer.OnKeyPressed.Add(OnKeyPressed);            // On press key delegate
            Buffer.OnPseudoPressed.Add(OnPseudoPressed);      // On keymap was selected
        }

        public bool IsVisible => isVisible;
        public MenuPanelRepresentation Current => currentMenu;

        public void ToggleVisibility()
        {
            SetVisibility(!isVisible);
        }

        public void Open(KeyMap menu = null)
        {
            if (menu == null)
                menu = MenuMap.MenuBar;

            if (ContainsMenu(menu))
                CloseAllMenusUpTo(menu);
            else
                currentMenu = new MenuPanelRepresentation(Current, menu);
            SetVisibility(true);
            controllerEvent.eventType = MenuLine.MenuEventType.Open;
            currentMenu.OnEvent(controllerEvent);
            (menuRenderer as IMenuRender).RenderMenu(currentMenu);
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
                (menuRenderer as IMenuRender).RenderMenu(currentMenu);
            }
            else
            {
                SetVisibility(false);
            }
        }

        public void OnEvent(KeyEvent keyEvent)
        {
            MenuEvent menuEvent = new MenuEvent();
            menuEvent.keyEvent = keyEvent;

            var key = keyEvent.AsKeyCode;
            if (key == KeyCode.E)
                ToggleVisibility();
            if (key == KeyCode.Q)
                Close();


            if (currentMenu != null)
            {
                var shift = keyEvent.IsModifier(KeyModifiers.Shift);
                menuEvent.eventType = MenuLine.MenuEventType.None;

                if (isVisible)
                {
                    if (key == KeyCode.W)
                        menuEvent.eventType = MenuLine.MenuEventType.Up;
                    if (key == KeyCode.S)
                        menuEvent.eventType = MenuLine.MenuEventType.Down;
                    if (key == KeyCode.A)
                        menuEvent.eventType = MenuLine.MenuEventType.Left;
                    if (key == KeyCode.D)
                        menuEvent.eventType = MenuLine.MenuEventType.Right;
                    if (key == KeyCode.R)
                        menuEvent.eventType = MenuLine.MenuEventType.Reset;
                }
                else
                {
                    if (shift)
                    {
                        if (key == KeyCode.A)
                            menuEvent.eventType = MenuLine.MenuEventType.Left;
                        if (key == KeyCode.D)
                            menuEvent.eventType = MenuLine.MenuEventType.Right;
                        if (key == KeyCode.R)
                            menuEvent.eventType = MenuLine.MenuEventType.Reset;
                    }
                }
                if (currentMenu != null)
                {
                    currentMenu.OnEvent(menuEvent);
                    Redraw();
                }
            }
        }

        #endregion

        #region Protected

        #endregion

        #region Private
        private Buffer menuBuffer;
        private Mode menuMode;
        private KeyMap menuMap;
        private MenuRender menuRenderer;
        private MenuPanelRepresentation currentMenu;
        private bool isVisible;
        private MenuEvent controllerEvent;
        readonly object menuControlEvent = new object();

        ~MenuController()
        {
            menuRenderer = null;
            menuMap = null;
            menuBuffer = null;
            menuMode = null;
            currentMenu = null;
            Buffer.OnSequencePressed.Remove(OnSequencePressed);  // On press sequence delegate
            Buffer.OnSequenceProgress.Remove(OnSequenceProgress);// On press part of sequence delegate
            Buffer.OnKeyPressed.Remove(OnKeyPressed);            // On press key delegate
            Buffer.OnPseudoPressed.Remove(OnPseudoPressed);      // On keymap was selecte
        }


        // Just render current menu if it is defined 
        // do not check the visibility -- render it
        void Redraw()
        {
            if (currentMenu != null)
                (menuRenderer as IMenuRender).RenderMenu(currentMenu);
        }

        void SetVisibility(bool vis)
        {
            if (vis && currentMenu == null)
                currentMenu = new MenuPanelRepresentation(null, DM.MenuBar);
            isVisible = vis;
            menuBuffer.SetActive(isVisible);
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
                    {
                        currentMenu = current;
                        return;
                    }
                    var parent = current.parent;
                    current.parent = null;
                    current = parent;
                }
            }
            currentMenu = new MenuPanelRepresentation(null, menu);
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

        void IMenuRender_OnGUI.OnGUI()
        {
            InputManager.OnGUI();
            if (isVisible)
                (menuRenderer as IMenuRender_OnGUI).OnGUI();
        }

        float readrawAt = 0;
        void IMenuRender_Update.Update()
        {
            if (Time.unscaledTime > readrawAt)
            {
                readrawAt = Time.unscaledTime + 0.5f;
                Redraw();
            }
        }
        void OnKeyPressed(Buffer buffer, KeyEvent evt)
        {
            Debug.Log(buffer.GetBufferHumanizedString());   // Just display current buffer content		
            buffer.Clear();
        }

        void OnSequenceProgress(Buffer buffer, DMKeyMapItem item)
        {
            // Ignore keymaps but open the menu map
            if (item.value is MenuMap)
            {
                buffer.Clear();
                Open(item.value as MenuMap);
                Redraw();
            }
        }

        void OnPseudoPressed(Buffer buffer, DMKeyMapItem item)
        {
            if (item.value is KeyMap)
            {
                buffer.Clear();
                Open(item.value as KeyMap);
                Redraw();
            }
            else
            {
                Debug.Log("{menu:" + item.value + "}");
            }
        }

        // The user typed the keystroke
        void OnSequencePressed(Buffer buffer, DMKeyMapItem item)
        {
            MenuEvent menuEvent = new MenuEvent();
            if (item.value is System.Action)
            {
                (item.value as System.Action).Invoke();
                Redraw();
            }
            else if (item.value is DMBool)
            {
                menuEvent.eventType = MenuLine.MenuEventType.Increment;
                menuEvent.keyEvent = KeyEvent.None;
                (item.value as DMBool).OnEvent(menuEvent);
                Redraw();
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
        #endregion
    }
}
