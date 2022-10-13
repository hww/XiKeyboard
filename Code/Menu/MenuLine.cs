/* Copyright (c) 2021 Valerya Pudova (hww) */

namespace XiKeyboard.Menu
{
    ///-------------------------------------------------------------------------------------------------
    /// <summary>   The base class for all menu items. </summary>
    ///
    ///-------------------------------------------------------------------------------------------------

    public class MenuLine
    {

        ///-------------------------------------------------------------------------------------------------
        /// <summary>   Values that represent menu button types. </summary>
        ///
        /// <remarks>   Valery, 10/12/2022. </remarks>
        ///-------------------------------------------------------------------------------------------------

        public enum DMButtonType
        {
            NoButton,
            Toggle,
            Radio
        }

        public virtual string Title => null;
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

        ///-------------------------------------------------------------------------------------------------
        /// <summary>
        /// Values that represent menu event types. The main purpose make the menu code abstracted from
        /// the keyboard.
        /// </summary>
        ///
        /// <remarks>   Valery, 10/12/2022. </remarks>
        ///-------------------------------------------------------------------------------------------------

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

    ///-------------------------------------------------------------------------------------------------
    /// <summary>   The very simple menu item. </summary>
    ///
    ///-------------------------------------------------------------------------------------------------

    public class MenuLineSimple : MenuLine
    { 
        protected string text;
        protected string help;
        protected string shortcut;
        public object binding;

        ///-------------------------------------------------------------------------------------------------
        /// <summary>   Constructor. </summary>
        ///
        ///
        /// <param name="text">     The title. </param>
        /// <param name="shortcut"> (Optional) The shortcut. </param>
        /// <param name="help">     (Optional) The help. </param>
        ///-------------------------------------------------------------------------------------------------

        public MenuLineSimple(string text, string shortcut = null, string help = null)
        {
            this.text = text;
            this.help = help;
            this.shortcut = shortcut;
        }

        ///-------------------------------------------------------------------------------------------------
        /// <summary>   New menu item with function binded too. </summary>
        ///
        ///
        /// <param name="text">     . </param>
        /// <param name="binding">  . </param>
        /// <param name="shortcut"> (Optional) </param>
        /// <param name="help">     (Optional) </param>
        ///-------------------------------------------------------------------------------------------------

        public MenuLineSimple(string text, object binding, string shortcut = null, string help = null) 
        {
            this.text = text;
            this.help = help;
            this.shortcut = shortcut;
            this.binding = binding;
        }

        #region IMenuLine

        public override string Title => text==null ? string.Empty : text;
        public override string Help => help == null ? string.Empty : help;
        public override string Shorcut => shortcut;
        public override string Value => shortcut == null ? string.Empty : shortcut;
        public override object Binding => binding;

        ///-------------------------------------------------------------------------------------------------
        /// <summary>   Executes the 'event' action. </summary>
        ///

        ///
        /// <param name="menuEvent">    The menu event. </param>
        ///-------------------------------------------------------------------------------------------------

        public override void OnEvent(MenuEvent menuEvent)
        {
            var evt = menuEvent.eventType;
            if (evt == MenuLine.MenuEventType.Increment && binding is System.Action)
                (binding as System.Action).Invoke();
        }

        #endregion
    }

    ///-------------------------------------------------------------------------------------------------
    /// <summary>   The complex menu item. </summary>
    ///
    ///-------------------------------------------------------------------------------------------------

    public class MenuLineComplex : MenuLine
    {
        public delegate bool Precondition(MenuLineComplex menuLine);
        public delegate MenuLineComplex Filter(MenuLineComplex menuLine);

        protected string title;
        protected string help;
        protected string shortcut;

        public readonly object binding;
        public readonly Filter filter;
        public readonly Precondition enable;
        public readonly Precondition visible;            
        public readonly DMButtonType buttonType;
        public readonly Precondition buttonState;

        #region IMenuLine

        public override string Title => title == null ? title : string.Empty;
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

        ///-------------------------------------------------------------------------------------------------
        /// <summary>   Non selectable string. </summary>
        ///
        ///
        /// <param name="text">     . </param>
        /// <param name="shortcut"> (Optional) </param>
        /// <param name="help">     (Optional) </param>
        ///-------------------------------------------------------------------------------------------------

        public MenuLineComplex(string text, string shortcut = null, string help = null) 
        {
            this.title = text;
            this.help = help;
            this.shortcut = shortcut;
        }

        ///-------------------------------------------------------------------------------------------------
        /// <summary>   Non selectable string. </summary>
        ///
        ///
        /// <param name="text">     The label of menu. </param>
        /// <param name="binding">  The binding of menu. </param>
        /// <param name="shortcut"> (Optional) A shorcuts sequence</param>
        /// <param name="help">     (Optional) The value of this property, help, specifies a help-echo string to display while the mouse is on that item. </param>
        ///-------------------------------------------------------------------------------------------------

        public MenuLineComplex(string text, object binding, string shortcut = null, string help = null) : this(text, shortcut, help)
        {
            this.binding = binding;
            this.enable = null;
            this.visible = null;
            this.filter = null;
        }

        ///-------------------------------------------------------------------------------------------------
        /// <summary>   Constructor for a complex menu item. </summary>
        ///
        ///
        /// <param name="text">     The label of menu. </param>
        /// <param name="binging">  The binding of menu. </param>
        /// <param name="enable">   (Optional) The result of evaluating form determines whether the item
        ///                         is enabled (non-nil means yes). If the item is not enabled, you can’t
        ///                         really click on it. </param>
        /// <param name="visible">  (Optional) The result of evaluating form determines whether the item
        ///                         should actually appear in the menu (non-nil means yes). If the item
        ///                         does not appear, then the menu is displayed as if this item were not
        ///                         defined at all. </param>
        /// <param name="filter">   (Optional) This property provides a way to compute the menu item
        ///                         dynamically. The property value filter-fn should be a function of one
        ///                         argument; when it is called, its argument will be real-binding. The
        ///                         function should return the binding to use instead. </param>
        /// <param name="shortcut"> (Optional) A shorcuts sequence. </param>
        /// <param name="help">     (Optional) The value of this property, help, specifies a help-echo
        ///                         string to display while the mouse is on that item. </param>
        ///-------------------------------------------------------------------------------------------------

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

        ///-------------------------------------------------------------------------------------------------
        /// <summary>
        /// Constructor for a complex menu item
        /// 
        /// When used the checkbox: The binding poin to the function to toggle the value The buttonState
        /// points to function to read the state the radio button: The binding poin to the function to
        /// set the option to active The buttonState points to function to read the option is active.
        /// </summary>
        ///
        ///
        /// <param name="text">         The label of menu. </param>
        /// <param name="binging">      The binding of menu. </param>
        /// <param name="enable">       (Optional) The result of evaluating form determines whether the
        ///                             item is enabled (non-nil means yes). If the item is not enabled,
        ///                             you can’t really click on it. </param>
        /// <param name="visible">      (Optional) The result of evaluating form determines whether the
        ///                             item should actually appear in the menu (non-nil means yes). If
        ///                             the item does not appear, then the menu is displayed as if this
        ///                             item were not defined at all. </param>
        /// <param name="filter">       (Optional) This property provides a way to compute the menu item
        ///                             dynamically. The property value filter-fn should be a function of
        ///                             one argument; when it is called, its argument will be real-
        ///                             binding. The function should return the binding to use instead. </param>
        /// <param name="buttonType">   (Optional) This property provides a way to define radio buttons
        ///                             and toggle buttons. </param>
        /// <param name="buttonState">  (Optional) The delegate will return the button state. </param>
        /// <param name="shortcut">     (Optional) A shorcuts sequence. </param>
        /// <param name="help">         (Optional) The value of this property, help, specifies a help-
        ///                             echo string to display while the mouse is on that item. </param>
        ///-------------------------------------------------------------------------------------------------

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

        ///-------------------------------------------------------------------------------------------------
        /// <summary>   Executes the 'event' action. </summary>
        ///
        ///
        /// <param name="menuEvent">    The menu event. </param>
        ///-------------------------------------------------------------------------------------------------

        public override void OnEvent(MenuEvent menuEvent)
        {
            var evt = menuEvent.eventType;
            if (evt == MenuEventType.Right && binding is System.Action)
                (binding as System.Action).Invoke();
        }
    }

    ///-------------------------------------------------------------------------------------------------
    /// <summary>   The menu separator class. </summary>
    ///
    ///-------------------------------------------------------------------------------------------------

    public class MenuSeparator : MenuLine
    {
        private static int randomId = 0;

        ///-------------------------------------------------------------------------------------------------
        /// <summary>   Values that represent types. </summary>
        ///
        ///-------------------------------------------------------------------------------------------------

        public enum Type { NoLine, Space, SingleLine, DashedLine }
        /// <summary>   The separator type. </summary>
        public Type type;

        private int id;

        ///-------------------------------------------------------------------------------------------------
        /// <summary>   The constructor of separator. </summary>
        ///
        ///
        /// <param name="separatorType">    Type of the separator. </param>
        ///-------------------------------------------------------------------------------------------------

        public MenuSeparator(Type separatorType) 
        {
            id = randomId++;
            type = separatorType;
        }

        public override string Title => $"--{id}";

    }

}