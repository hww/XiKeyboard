/* Copyright (c) 2021 Valerya Pudova (hww) : For more information read the license file */

using System;
using UnityEngine;
using XiCore.StringTools;
using XiKeyboard.KeyMaps;
using XiKeyboard.Menu;

namespace XiKeyboard.Rendering
{
    /// <summary>
    /// This class renders a panel on the screen. 
    /// </summary>
    public class MenuRender : IMenuRender, IMenuRender_OnGUI
    {
        private readonly BetterStringBuilder stringBuilder = new BetterStringBuilder(80 * 40);
        private string menuText;
        private GUISkin menuSkin;

        public MenuRender()
        {
            menuSkin = Resources.Load<GUISkin>("XiKeyboard/Skins/Default Skin");
            menuSkin.box.normal.background = MakeTex(2, 2, new Color(0f, 0f, 0f, 0.7f));
            menuText = String.Empty;
        }

        ~MenuRender()
        {
            menuSkin = null;
            menuText = null;
        }
        private Texture2D MakeTex(int width, int height, Color col)
        {
            Color[] pix = new Color[width * height];
            for (int i = 0; i < pix.Length; ++i)
            {
                pix[i] = col;
            }
            Texture2D result = new Texture2D(width, height);
            result.SetPixels(pix);
            result.Apply();
            return result;
        }

        void IMenuRender.RenderMenu(MenuPanelRepresentation panel, MenuRenderOptions options = MenuRenderOptions.Default)
        {
            stringBuilder.Clear();

            // Calculate te width and read all texts from
            panel.Update(MenuConfig.Space.Length);
            var separatorWidth = panel.widthOfLine + MenuConfig.PrefixNormal.Length + MenuConfig.SuffixNormal.Length;

            // Prepair a formatting lines
            string itemFormat1 = $"<color={0}>{{1,-{separatorWidth}}}</color>";
            string itemFormat2 = $"<color={MenuColors.Cursor}>{{0}}</color><color={{1}}>{{2,-{panel.widthOfName}}}</color>{MenuConfig.Space}<color={{3}}>{{4,{panel.widthOfValue}}}</color><color={MenuColors.SuffixModified}>{{5}}</color>";

            string singleLine = string.Format(itemFormat1, MenuColors.HorizontalLine, new string(MenuConfig.NormalLineChar, separatorWidth));
            string spaceLine = null;
            string dashedLine = null;

            // Render a panel header with separator
            var header = string.Format(itemFormat1, MenuColors.MenuHeader, panel.title, MenuConfig.SuffixNormal);

            stringBuilder.AppendLine(header);
            stringBuilder.AppendLine(singleLine);

            var menuItems = panel.items;
            var count = panel.Count;
            // Render the mnu menuItems
            for (var i = 0; i < count; i++)
            {
                var panelItem = menuItems[i];
                

                if (panelItem.line is MenuLine)
                {
                    var keyItem = panelItem.line as MenuLine;
                    if (keyItem is MenuSeparator)
                    {
                        string lineText = string.Empty;
                        switch ((keyItem as MenuSeparator).type)
                        {
                            case MenuSeparator.Type.NoLine:
                                break;
                            case MenuSeparator.Type.Space:
                                if (spaceLine == null)
                                    spaceLine = string.Format(itemFormat1, MenuColors.HorizontalLine, new string(' ', separatorWidth));
                                lineText = spaceLine;
                                break;
                            case MenuSeparator.Type.SingleLine:
                                lineText = singleLine;
                                break;
                            case MenuSeparator.Type.DashedLine:
                                if (dashedLine == null)
                                    dashedLine = string.Format(itemFormat1, MenuColors.HorizontalLine, new string(MenuConfig.DashedLineChar, separatorWidth));
                                lineText = dashedLine;
                                break;
                            default:
                                throw new Exception();
                        }
                        if (i == (count - 1))
                            stringBuilder.Append(lineText);
                        else
                            stringBuilder.AppendLine(lineText);
                    }
                    else
                    {
                        if (keyItem.IsVisible)
                        {
                            var prefix = i == panel.selectedLine ? MenuConfig.PrefixCursor : MenuConfig.PrefixNormal;
                            var suffix = keyItem.IsDefault ? MenuConfig.SuffixNormal : MenuConfig.SuffixModified;

                            var titleColor = MenuColors.TitleDisabled;
                            var valueColor = MenuColors.ValueDisabled;

                            if (keyItem.IsEnabled)
                            {
                                if (keyItem.ButtonType != MenuLine.DMButtonType.NoButton)
                                {
                                    titleColor = keyItem.ButtonState ? MenuColors.ToggleTitleActive : MenuColors.ToggleTitleInactive;
                                    valueColor = MenuColors.ValueNormal;
                                }
                                else
                                {
                                    titleColor = MenuColors.TitleNormal;
                                    valueColor = MenuColors.ValueNormal;
                                }
                            }

                            string lineText = string.Format(itemFormat2,
                                prefix,
                                titleColor, panelItem.title,
                                valueColor, panelItem.value,
                                suffix);

                            if (i == (count - 1))
                                stringBuilder.Append(lineText);
                            else
                                stringBuilder.AppendLine(lineText);

                        }
                    }
                } 
                else if (panelItem.line is KeyMap)
                {
                    var prefix = i == panel.selectedLine ? MenuConfig.PrefixCursor : MenuConfig.PrefixNormal;
                    string lineText = string.Format(itemFormat2,
                               prefix,
                               MenuColors.TitleNormal, panelItem.title,
                               MenuColors.ValueNormal, panelItem.value,
                               MenuConfig.SuffixNormal);

                    if (i == (count - 1))
                        stringBuilder.Append(lineText);
                    else
                        stringBuilder.AppendLine(lineText);
                }

            }

            menuText = stringBuilder.ToString();
        }

        void IMenuRender_OnGUI.OnGUI()
        {
            GUI.skin = menuSkin;

            var textSize = GUI.skin.label.CalcSize(new GUIContent(menuText)) + new Vector2(10, 10);
            var position = new Vector2(20, 20);
            var rect = new Rect(position, textSize);

            GUI.Box(rect, GUIContent.none, GUI.skin.box);

            rect.x += 5f;
            rect.width -= 5f * 2f;
            rect.y += 5f;
            rect.height -= 5f * 2f;

            GUI.Label(rect, menuText);
        }
    }
}