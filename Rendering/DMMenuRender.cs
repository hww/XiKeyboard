/* Copyright (c) 2021 Valerya Pudova (hww) : For more information read the license file */

using System;
using UnityEngine;
using XiCore.StringTools;

namespace XiKeyboard
{
    /// <summary>
    /// This class renders a panel on the screen. 
    /// </summary>
    public class DMMenuRender : IDMRender, IDMRender_OnGUI
    {
        private readonly BetterStringBuilder stringBuilder = new BetterStringBuilder(80 * 40);
        private string menuText;
        private GUISkin menuSkin;

        public DMMenuRender()
        {
            menuSkin = Resources.Load<GUISkin>("XiKeyboard/Skins/Default Skin");
            menuSkin.box.normal.background = MakeTex(2, 2, new Color(0f, 0f, 0f, 0.7f));
            menuText = String.Empty;
        }

        ~DMMenuRender()
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

        void IDMRender.RenderMenu(DMMenuPanelRepresentation panel, DMMenuOptions options = DMMenuOptions.Default)
        {
            stringBuilder.Clear();

            // Calculate te width and read all texts from
            panel.Update(DMConfig.Space.Length, DMConfig.SuffixModified.Length);
            var lineWidth = panel.widthOfLine + DMConfig.PrefixNormal.Length + DMConfig.SuffixNormal.Length;

            // Prepair a formatting lines
            string itemFormat1 = $"<color={0}>{{1,-{panel.widthOfLine}}}</color>";
            string itemFormat2 = $"<color={DMColors.Cursor}>{{0}}</color><color={{1}}>{{2,-{panel.widthOfName}}}</color>{DMConfig.Space}<color={{3}}>{{4,{panel.widthOfValue}}}</color><color={DMColors.SuffixModified}>{{5}}</color>";

            string singleLine = string.Format(itemFormat1, DMColors.HorizontalLine, new string(DMConfig.NormalLineChar, lineWidth));
            string spaceLine = null;
            string dashedLine = null;

            // Render a panel header with separator
            var header = string.Format(itemFormat1, DMColors.Header, panel.title, DMConfig.SuffixNormal);

            stringBuilder.AppendLine(header);

            var menuItems = panel.items;
            var count = panel.Count;
            // Render the mnu menuItems
            for (var i = 0; i < count; i++)
            {
                var panelItem = menuItems[i];
                var keyItem = panelItem.line;
               
                if (keyItem is DMMenuSeparator)
                {
                    switch ((keyItem as DMMenuSeparator).type)
                    {
                        case DMMenuSeparator.Type.NoLine:
                            break;
                        case DMMenuSeparator.Type.Space:
                            if (spaceLine == null)
                                spaceLine = string.Format(itemFormat1, DMColors.HorizontalLine, new string(' ', lineWidth));
                            stringBuilder.AppendLine(spaceLine);
                            break;
                        case DMMenuSeparator.Type.SingleLine:
                            stringBuilder.AppendLine(singleLine);
                            break;
                        case DMMenuSeparator.Type.DashedLine:
                            if (dashedLine == null)
                                dashedLine = string.Format(itemFormat1, DMColors.HorizontalLine, new string(DMConfig.DashedLineChar, lineWidth));
                            stringBuilder.AppendLine(dashedLine);
                            break;
                        default:
                            throw new Exception();
                    }
                } 
                else
                {

                    if (keyItem.IsVisible)
                    {
                        var prefix = i == panel.selectedLine ? DMConfig.PrefixCursor : DMConfig.PrefixNormal;
                        var suffix = keyItem.IsDefault ? DMConfig.SuffixNormal : DMConfig.SuffixModified;

                        var titleColor = DMColors.TitleDisabled;
                        var valueColor = DMColors.ValueDisabled;

                        if (keyItem.IsEnabled)
                        {
                            if (keyItem.ButtonType != DMMenuLine.DMButtonType.NoButton)
                            {
                                titleColor = keyItem.ButtonState ? DMColors.ToggleTitleActive : DMColors.ToggleTitleInactive;
                                valueColor = DMColors.ValueNormal;
                            }
                            else
                            {
                                titleColor = DMColors.TitleNormal;
                                valueColor = DMColors.ValueNormal;
                            }
                        }

                        string lineText = string.Format(itemFormat2,
                            prefix, 
                            titleColor, panelItem.title,
                            valueColor, panelItem.value,
                            suffix);
                        stringBuilder.AppendLine(lineText);

                    }
                }
            }

            menuText = stringBuilder.ToString();
        }

        void IDMRender_OnGUI.OnGUI()
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