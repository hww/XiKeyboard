/* Copyright (c) 2021 Valerya Pudova (hww) */

using System;
using System.Collections.Generic;
using System.Text;
using JetBrains.Annotations;

namespace XiKeyboard.KeyMaps
{
    ///-------------------------------------------------------------------------------------------------
    /// <summary>
    /// This class allow to convert string expression to the keycode sequence. The sequence is the
    /// array of integers. Each character is the 32 bits word.
    /// </summary>
    ///
    ///-------------------------------------------------------------------------------------------------

    public static class KBD
    {
        ///-------------------------------------------------------------------------------------------------
        /// <summary>   Convert the keycode sequence to the string. </summary>
        ///

        ///
        /// <param name="sequence">     . </param>
        /// <param name="separator">    (Optional) </param>
        ///
        /// <returns>   The given data converted to a string. </returns>
        ///-------------------------------------------------------------------------------------------------

        public static string ConvertToString(KeyEvent[] sequence, string separator = null)
        {
            UnityEngine.Debug.Assert(sequence != null);
            return ConvertToString(sequence, 0, sequence.Length, separator);
        }

        ///-------------------------------------------------------------------------------------------------
        /// <summary>   Convert the keycode sequence to the string. </summary>
        ///

        ///
        /// <param name="sequence">     . </param>
        /// <param name="starts">       . </param>
        /// <param name="quantity">     . </param>
        /// <param name="separator">    (Optional) </param>
        ///
        /// <returns>   The given data converted to a string. </returns>
        ///-------------------------------------------------------------------------------------------------

        public static string ConvertToString(KeyEvent[] sequence, int starts, int quantity, string separator = null)
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

        ///-------------------------------------------------------------------------------------------------
        /// <summary>   Convert the keycode sequence to the string. </summary>
        ///

        ///
        /// <param name="sequence"> . </param>
        ///
        /// <returns>   The given data converted to a string list. </returns>
        ///-------------------------------------------------------------------------------------------------

        public static string[] ConvertToStringList(KeyEvent[] sequence)
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

        ///-------------------------------------------------------------------------------------------------
        /// <summary>
        /// EMACS keycode sequence parser For converting from expression like: "C-x C-v" to convert to
        /// the KeyEvent[] Supports multiple or single tokens When multiple tokens each one separated
        /// with ' ' space.
        /// </summary>
        ///

        ///
        /// <exception cref="ArgumentNullException">    Thrown when one or more required arguments are
        ///                                             null. </exception>
        /// <exception cref="ArgumentException">        Thrown when one or more arguments have
        ///                                             unsupported or illegal values. </exception>
        ///
        /// <param name="expression">   . </param>
        ///
        /// <returns>   A KeyEvent[]. </returns>
        ///-------------------------------------------------------------------------------------------------

        public static KeyEvent[] ParseSequence([NotNull] string expression)
        {
            if (expression == null) throw new ArgumentNullException(nameof(expression));
            if (expression == string.Empty) throw new ArgumentException(nameof(expression));

            var sequence = new List<KeyEvent>();
            var tags = expression.Split(' ');

            foreach (var s in tags)
            {
                if (string.IsNullOrEmpty(s))
                    continue;

                var evt = KeyEvent.ParseExpression(s);

                if (evt >= 0)
                {
                    sequence.Add(evt);
                }
                else
                {
                    // This case will be translated as string
                    // "abcd" => "abcd"
                    foreach (var c in s)
                    {
                        sequence.Add(c);
                    }
                }
            }
            return sequence.ToArray();
        }

        ///-------------------------------------------------------------------------------------------------
        /// <summary>   Convert list of strings to list of events. </summary>
        ///

        ///
        /// <param name="sequence"> Array of strings. </param>
        ///
        /// <returns>   Array of events. </returns>
        ///-------------------------------------------------------------------------------------------------

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