using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace XiKeyboard.Assets.XiKeyboard.Code.Libs
{
    internal class TextureUtils
    {
        ///-------------------------------------------------------------------------------------------------
        /// <summary>   Makes a tex. </summary>
        ///
        ///
        /// <param name="width">    The width. </param>
        /// <param name="height">   The height. </param>
        /// <param name="color">    The color. </param>
        ///
        /// <returns>   A Texture2D. </returns>
        ///-------------------------------------------------------------------------------------------------

        public static Texture2D MakeTex(int width, int height, Color color)
        {
            Color[] pix = new Color[width * height];
            for (int i = 0; i < pix.Length; ++i)
            {
                pix[i] = color;
            }
            Texture2D result = new Texture2D(width, height);
            result.SetPixels(pix);
            result.Apply();
            return result;
        }
    }
}
