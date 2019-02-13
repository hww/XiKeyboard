// =============================================================================
// MIT License
//
// Copyright (c) [2018] [Valeriya Pudova]
//
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:
//
// The above copyright notice and this permission notice shall be included in all
// copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
// SOFTWARE.
// =============================================================================

namespace VARP.Keyboard
{
    /// <summary>
    /// The base class for all menu items
    /// </summary>
    public abstract class MenuLine
    {
        public virtual string Text => null;
        public virtual string Help => null;
        public virtual string Shorcut => null;
        public virtual object Binding => null;
        
        
        
    }

    public class MenuLineSimple : MenuLine
    { 
        protected string text;
        protected string help;
        protected string shortcut;
        public object binding;

        public MenuLineSimple(string text, string shortcut = null, string help = null)
        {
            this.text = text;
            this.help = help;
            this.shortcut = shortcut;
        }

        /// <summary>
        /// New menu item with function binded too
        /// </summary>
        /// <param name="text"></param>
        /// <param name="binding"></param>
        /// <param name="shortcut"></param>
        /// <param name="help"></param>
        public MenuLineSimple(string text, object binding, string shortcut = null, string help = null) 
        {
            this.text = text;
            this.help = help;
            this.shortcut = shortcut;
            this.binding = binding;
        }

        #region IMenuLine

        public override string Text => text;
        public override string Help => help;
        public override string Shorcut => shortcut;
        public override object Binding => binding;

        #endregion
    }

    public class MenuLineComplex : MenuLine
    {
        public delegate bool Precondition(MenuLineComplex menuLine);
        public delegate MenuLineComplex Filter(MenuLineComplex menuLine);

        public enum ButtonType
        {
            NoButton,
            Toggle,
            Radio
        }

        protected string text;
        protected string help;
        protected string shortcut;

        public readonly object binding;
        public readonly Filter filter;
        public readonly Precondition enable;
        public readonly Precondition visible;            
        public readonly ButtonType buttonType;
        public readonly Precondition buttonState;

        #region IMenuLine

        public override string Text => text;
        public override string Help => help;
        public override string Shorcut => shortcut;
        public override object Binding => binding;

        #endregion

        public bool IsEnabled => enable == null || enable(this);
        public bool IsVisible => visible == null || visible(this);
        public bool ButtonState => buttonState == null || buttonState(this);

        public MenuLineComplex GetFiltered()
        {
            return filter == null ? this : filter(this);
        }

        /// <summary>
        /// Non selectable string
        /// </summary>
        /// <param name="text"></param>
        /// <param name="shortcut"></param>
        /// <param name="help"></param>
        public MenuLineComplex(string text, string shortcut = null, string help = null) 
        {
            this.text = text;
            this.help = help;
            this.shortcut = shortcut;
        }

        /// <summary>Non selectable string</summary>
        /// <param name="text"></param>
        /// <param name="binding"></param>
        /// <param name="shortcut"></param>
        /// <param name="help"></param>
        public MenuLineComplex(string text, object binding, string shortcut = null, string help = null) : this(text, shortcut, help)
        {
            this.binding = binding;
            this.enable = null;
            this.visible = null;
            this.filter = null;
        }

        /// <summary>Constructor for a complex menu item</summary>
        /// <param name="text"></param>
        /// <param name="binging"></param>
        /// <param name="enable"></param>
        /// <param name="visible"></param>
        /// <param name="filter"></param>
        /// <param name="shortcut"></param>
        /// <param name="help"></param>
        public MenuLineComplex(string text, 
            object binging,
            Precondition enable = null, 
            Precondition visible = null,
            Filter filter = null,
            string shortcut = null,
            string help = null) : this(text, binging, shortcut, help)
        {
            this.enable = enable;
            this.visible = visible;
            this.filter = filter;
        }


        /// <summary>Constructor for a complex menu item</summary>
        /// <param name="text"></param>
        /// <param name="binging"></param>
        /// <param name="enable"></param>
        /// <param name="visible"></param>
        /// <param name="filter"></param>
        /// <param name="buttonType"></param>
        /// <param name="buttonState"></param>
        /// <param name="shortcut"></param>
        /// <param name="help"></param>
        public MenuLineComplex(string text, 
            object binging,
            Precondition enable = null,
            Precondition visible = null,
            Filter filter = null,
            ButtonType buttonType = ButtonType.NoButton,
            Precondition buttonState = null,
            string shortcut = null,
            string help = null) : this(text, binging, shortcut, help)
        {
            this.enable = enable;
            this.visible = visible;
            this.filter = filter;
            this.buttonType = buttonType;
            this.buttonState = buttonState;
        }
    }

    /// <summary>
    /// The menu separator class
    /// </summary>
    public class MenuSeparator : MenuLine
    {
        public enum Type { NoLine, Space, SingleLine, DashedLine }
        /// <summary>The separator type</summary>
        public Type type;
        /// <summary>The constructor of separator</summary>
        public MenuSeparator(Type separatorType) 
        {
            type = separatorType;
        }
    }

}