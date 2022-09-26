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
        public delegate void Method();
        private Buffer menuBuffer;
        private Mode menuMode;
        private KeyMap menuMap;
        private MenuRender menuRenderer;
        private MenuPanelRepresentation currentMenu;


        private bool isVisible;

        public MenuController()
        {
            menuRenderer = new MenuRender();
            menuMap = new KeyMap("menu-map", "The menu key map");
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
            Buffer.OnKeyPressed.Add(OnKeyPressed);            // On press key delegate
            Buffer.OnPseudoPressed.Add(OnPseudoPressed);      // On keymap was selected
        }

        ~MenuController()
        {
            menuRenderer = null;
            menuMap = null;
            menuBuffer = null;
            menuMode = null;
            currentMenu = null;
            Buffer.OnSequencePressed.Remove(OnSequencePressed);  // On press sequence delegate
            Buffer.OnKeyPressed.Remove(OnKeyPressed);            // On press key delegate
            Buffer.OnPseudoPressed.Remove(OnPseudoPressed);      // On keymap was selecte
        }

        public bool IsVisible => isVisible;

        public MenuPanelRepresentation Current => currentMenu;

        public MenuEvent controllerEvent;

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
                currentMenu = new MenuPanelRepresentation(null, DM.GlobalKeymap);
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
                currentMenu = new MenuPanelRepresentation(Current, menu);
            SetVisibility(true);
            controllerEvent.eventType = MenuLine.MenuEventType.Open;
            currentMenu.OnEvent(controllerEvent);
            (menuRenderer as IMenuRender).RenderMenu(currentMenu);
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

        void IMenuRender_OnGUI.OnGUI()
        {
            InputManager.OnGUI();
            if (isVisible)
                (menuRenderer as IMenuRender_OnGUI).OnGUI();
        }

        float readrawAt = 0;
        void IMenuRender_Update.Update()
        {
            if (Time.unscaledTime>readrawAt)
            {
                readrawAt = Time.unscaledTime + 0.5f;
                Redraw();
            }
        }

        // The user typed the keystroke
        void OnSequencePressed(Buffer buffer, DMKeyMapItem item)
        {
            MenuEvent menuEvent;
            if (item.value is System.Action)
            {
                (item.value as System.Action).Invoke();
                Redraw();
            }
            if (item.value is DMBool)
            {
                menuEvent.eventType = MenuLine.MenuEventType.Right;
                menuEvent.keyEvent = KeyEvent.None;
                (item.value as DMBool).OnEvent(controllerEvent);
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

        void OnKeyPressed(Buffer buffer, KeyEvent evt)
        {
            Debug.Log(buffer.GetBufferHumanizedString());   // Just display current buffer content		
        }

        void OnPseudoPressed(Buffer buffer, DMKeyMapItem item)
        {
            Debug.Log("{menu:" + item.value + "}");
            // (this as IMenuRender).RenderMenu(item.value as KeyMap, 0);
        }

        readonly object menuControlEvent = new object();
    }
}