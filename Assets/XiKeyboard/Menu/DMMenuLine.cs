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

namespace XiKeyboard
{
    /// <summary>
    /// The base class for all menu items
    /// </summary>
    public class DMMenuLine
    {
        public virtual string Text => null;
        public virtual string Help => null;
        public virtual string Shorcut => null;
        public virtual object Binding => null;
        public virtual string Value => null;

        public enum EventTag
        {
            None,           
            Up,             
            Down,           
            Left,           
            Right,          
            Back,           
            Reset,
            Open,
            Close,
            Toggle
        }

        protected virtual void OnEvent(EventTag evt, bool shift) { }
    }
    /// <summary>
    /// The very simple menu item
    /// </summary>
    public class DMMenuLineSimple : DMMenuLine
    { 
        protected string text;
        protected string help;
        protected string shortcut;
        public object binding;

        public DMMenuLineSimple(string text, string shortcut = null, string help = null)
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
        protected DMMenuLineSimple(string text, object binding, string shortcut = null, string help = null) 
        {
            this.text = text;
            this.help = help;
            this.shortcut = shortcut;
            this.binding = binding;
        }

        #region IMenuLine

        public override string Text => text==null ? text : string.Empty;
        public override string Help => help == null ? help : string.Empty;
        public override string Shorcut => shortcut == null ? shortcut : string.Empty;
        public override object Binding => binding;
        public override string Value => string.Empty;

        protected override void OnEvent(EventTag evt, bool shift)
        {
            if (evt == EventTag.Right && binding is System.Action)
                (binding as System.Action).Invoke();
        }

        #endregion
    }
    /// <summary>
    /// The complex menu item
    /// </summary>
    public class DMMenuLineComplex : DMMenuLine
    {
        public delegate bool Precondition(DMMenuLineComplex menuLine);
        public delegate DMMenuLineComplex Filter(DMMenuLineComplex menuLine);

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

        public override string Text => text == null ? text : string.Empty;
        public override string Help => help == null ? help : string.Empty;
        public override string Shorcut => shortcut == null ? shortcut : string.Empty;
        public override object Binding => binding;
        public override string Value
        {
             get
             {
                switch (buttonType)
                {
                    case ButtonType.Toggle:
                        return buttonState.Invoke(this) ? "☑" : "☐";
                    case ButtonType.Radio:
                        return buttonState.Invoke(this) ? "🔘" : "⚪";
                }
                if (binding is KeyMap)
                    return "⯈";
                return string.Empty;
            }
        } 

        #endregion

        public bool IsEnabled => enable == null || enable(this);
        public bool IsVisible => visible == null || visible(this);
        public bool ButtonState => buttonState == null || buttonState(this);

        public DMMenuLineComplex GetFiltered()
        {
            return filter == null ? this : filter(this);
        }

        /// <summary>
        /// Non selectable string
        /// </summary>
        /// <param name="text"></param>
        /// <param name="shortcut"></param>
        /// <param name="help"></param>
        public DMMenuLineComplex(string text, string shortcut = null, string help = null) 
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
        public DMMenuLineComplex(string text, object binding, string shortcut = null, string help = null) : this(text, shortcut, help)
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
        public DMMenuLineComplex(string text, 
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


        /// <summary>
        /// Constructor for a complex menu item
        /// 
        /// When used the checkbox:
        /// The binding poin to the function to toggle the value
        /// The buttonState points to function to read the state
        /// the radio button:
        /// The binding poin to the function to set the option to active
        /// The buttonState points to function to read the option is active
        /// </summary>
        /// <param name="text"></param>
        /// <param name="binging"></param>
        /// <param name="enable"></param>
        /// <param name="visible"></param>
        /// <param name="filter"></param>
        /// <param name="buttonType"></param>
        /// <param name="buttonState"></param>
        /// <param name="shortcut"></param>
        /// <param name="help"></param>
        public DMMenuLineComplex(string text, 
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

        protected override void OnEvent(EventTag evt, bool shift)
        {
            if (evt == EventTag.Right && binding is System.Action)
                (binding as System.Action).Invoke();
        }
    }

    /// <summary>
    /// The menu separator class
    /// </summary>
    public class DMMenuSeparator : DMMenuLine
    {
        public enum Type { NoLine, Space, SingleLine, DashedLine }
        /// <summary>The separator type</summary>
        public Type type;
        /// <summary>The constructor of separator</summary>
        public DMMenuSeparator(Type separatorType) 
        {
            type = separatorType;
        }
    }

}