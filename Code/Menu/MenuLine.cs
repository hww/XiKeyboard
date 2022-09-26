/* Copyright (c) 2021 Valerya Pudova (hww) */

namespace XiKeyboard.Menu
{
    /// <summary>
    /// The base class for all menu items
    /// </summary>
    public class MenuLine
    {
        public enum DMButtonType
        {
            NoButton,
            Toggle,
            Radio
        }

        public virtual string Text => null;
        public virtual string Help => null;
        public virtual string Shorcut => null;
        public virtual string Value => null;
        public virtual int Count => 1;
        public virtual string GetValue(int idx) => Value;
        public virtual object Binding => null;
        public virtual bool IsDefault => true;
        public virtual bool IsVisible => true;
        public virtual bool IsEnabled => true;
        public virtual bool ButtonState => false;
        public virtual DMButtonType ButtonType => DMButtonType.NoButton;

        public enum MenuEventType
        {
            None,           
            Up,             
            Down,           
            Left,           
            Right,          
            Back, 
            Increment,
            Decrement,
            Reset,
            Open,
            Close,
            Toggle
        }

        public virtual void OnEvent(MenuEvent menuEvent) { }
    }
    /// <summary>
    /// The very simple menu item
    /// </summary>
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

        public override string Text => text==null ? string.Empty : text;
        public override string Help => help == null ? string.Empty : help;
        public override string Shorcut => shortcut;
        public override string Value => shortcut == null ? string.Empty : shortcut;
        public override object Binding => binding;

        public override void OnEvent(MenuEvent menuEvent)
        {
            var evt = menuEvent.eventType;
            if (evt == MenuEventType.Right && binding is System.Action)
                (binding as System.Action).Invoke();
        }

        #endregion
    }
    /// <summary>
    /// The complex menu item
    /// </summary>
    public class MenuLineComplex : MenuLine
    {
        public delegate bool Precondition(MenuLineComplex menuLine);
        public delegate MenuLineComplex Filter(MenuLineComplex menuLine);

        protected string text;
        protected string help;
        protected string shortcut;

        public readonly object binding;
        public readonly Filter filter;
        public readonly Precondition enable;
        public readonly Precondition visible;            
        public readonly DMButtonType buttonType;
        public readonly Precondition buttonState;

        #region IMenuLine

        public override string Text => text == null ? text : string.Empty;
        public override string Help => help == null ? help : string.Empty;
        public override string Shorcut => shortcut == null ? shortcut : string.Empty;
        public override object Binding => binding;

        #endregion

        public override bool IsEnabled => enable == null || enable(this);
        public override bool IsVisible => visible == null || visible(this);
        public override bool ButtonState => buttonState == null ? false : buttonState(this);
        public override DMButtonType ButtonType => buttonType;

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
        public MenuLineComplex(string text, 
            object binging,
            Precondition enable = null,
            Precondition visible = null,
            Filter filter = null,
            DMButtonType buttonType = DMButtonType.NoButton,
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

        public override void OnEvent(MenuEvent menuEvent)
        {
            var evt = menuEvent.eventType;
            if (evt == MenuEventType.Right && binding is System.Action)
                (binding as System.Action).Invoke();
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