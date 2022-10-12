/* Copyright (c) 2021 Valerya Pudova (hww) */

using System;
using System.Collections.Generic;
using System.Text;
using JetBrains.Annotations;

namespace XiKeyboard.KeyMaps
{
    /// <summary>
    /// This class allow to convert string expression to the 
    /// keycode sequence. The sequence is the array of integers.
    /// Each character is the 32 bits word.
    /// </summary>
    public static class KBD
    {
        /// <summary>
        /// Convert the keycode sequence to the string
        /// </summary>
        /// <param name="sequence"></param>
        /// <param name="separator"></param>
        /// <returns></returns>
        public static string ConvertToString(int[] sequence, string separator = null)
        {
            UnityEngine.Debug.Assert(sequence != null);
            return ConvertToString(sequence, 0, sequence.Length, separator);
        }
        /// <summary>
        /// Convert the keycode sequence to the string
        /// </summary>
        /// <param name="sequence"></param>
        /// <param name="starts"></param>
        /// <param name="quantity"></param>
        /// <param name="separator"></param>
        /// <returns></returns>
        public static string ConvertToString(int[] sequence, int starts, int quantity, string separator = null)
        {
            UnityEngine.Debug.Assert(sequence != null);
            UnityEngine.Debug.Assert(starts < sequence.Length);
            UnityEngine.Debug.Assert(starts + quantity - 1 < sequence.Length);

            if (sequence.Length == 0)
                return string.Empty;

            var idx = starts;
            var cnt = 0;

            var sb = new StringBuilder();
            if (separator != null)
            {
                var addSeparator = false;
                while (cnt < quantity)
                {
                    if (addSeparator) sb.Append(separator);
                    sb.Append(KeyEvent.GetName(sequence[idx++]));
                    addSeparator = true;
                    cnt++;
                }
            }
            else
            {
                while (cnt < quantity)
                {
                    sb.Append(KeyEvent.GetName(sequence[idx++]));
                    cnt++;
                }
            }
            return sb.ToString();
        }
        /// <summary>
        /// Convert the keycode sequence to the string
        /// </summary>
        /// <param name="sequence"></param>
        /// <returns></returns>
        public static string[] ConvertToStringList(int[] sequence)
        {
            UnityEngine.Debug.Assert(sequence != null);

            var result = new string[sequence.Length];
            if (sequence.Length == 0)
                return result;
            var i = 0;
            foreach (var code in sequence)
                result[i++] = KeyEvent.GetName(code);
            return result;
        }

 
        /// <summary>
        /// Convert list of strings to list of events
        /// </summary>
        /// <param name="sequence">Array of strings</param>
        /// <returns>Array of events</returns>
        public static KeyEvent[] ParsePseudo(string[] sequence)
        {
            var idx = 0;
            var result = new KeyEvent[sequence.Length];
            foreach (var s in sequence)
                result[idx++] = KeyEvent.GetPseudoCode(s);
            return result;
        }
    }
}