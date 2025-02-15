using System.Diagnostics.CodeAnalysis;
using System.Runtime.InteropServices;
using System.Windows.Controls.Primitives;
using System.Windows.Interop;
using System.Windows.Markup;
using Avalon.Internal.Utility;
using Avalon.Internal.Win32;

namespace Avalon.Windows.Controls;

/// <summary>
///     Specifies a component that creates an icon in the notification area.
/// </summary>
[ContentProperty("Text")]
[DefaultEvent("MouseDoubleClick")]
public sealed class NotifyIcon : FrameworkElement, IDisposable, IAddChild
{
    private static readonly int s_taskbarCreatedWindowMessage;

    private static int s_nextId;

    private readonly object _syncObj = new();

    private NotifyIconHwndSource _hwndSource;
    private readonly int _id = s_nextId++;
    private bool _iconCreated;
    private bool _doubleClick;
    /// <summary>
    ///     Identifies the <see cref="BalloonTipClick" /> routed event.
    /// </summary>
    public static readonly RoutedEvent BalloonTipClickEvent = EventManager.RegisterRoutedEvent("BalloonTipClick",
        RoutingStrategy.Bubble, typeof(RoutedEventHandler), typeof(NotifyIcon));

    /// <summary>
    ///     Occurs when the balloon tip is clicked.
    /// </summary>
    public event RoutedEventHandler BalloonTipClick
    {
        add { AddHandler(BalloonTipClickEvent, value); }
        remove { RemoveHandler(BalloonTipClickEvent, value); }
    }

    /// <summary>
    ///     Identifies the <see cref="BalloonTipClosed" /> routed event.
    /// </summary>
    public static readonly RoutedEvent BalloonTipClosedEvent = EventManager.RegisterRoutedEvent("BalloonTipClosed",
        RoutingStrategy.Bubble, typeof(RoutedEventHandler), typeof(NotifyIcon));

    /// <summary>
    ///     Occurs when the balloon tip is closed by the user.
    /// </summary>
    public event RoutedEventHandler BalloonTipClosed
    {
        add { AddHandler(BalloonTipClosedEvent, value); }
        remove { RemoveHandler(BalloonTipClosedEvent, value); }
    }

    /// <summary>
    ///     Identifies the <see cref="BalloonTipShown" /> routed event.
    /// </summary>
    public static readonly RoutedEvent BalloonTipShownEvent = EventManager.RegisterRoutedEvent("BalloonTipShown",
        RoutingStrategy.Bubble, typeof(RoutedEventHandler), typeof(NotifyIcon));

    /// <summary>
    ///     Occurs when the balloon tip is displayed on the screen.
    /// </summary>
    public event RoutedEventHandler BalloonTipShown
    {
        add { AddHandler(BalloonTipShownEvent, value); }
        remove { RemoveHandler(BalloonTipShownEvent, value); }
    }

    /// <summary>
    ///     Identifies the <see cref="Click" /> routed event.
    /// </summary>
    public static readonly RoutedEvent ClickEvent = EventManager.RegisterRoutedEvent("Click", RoutingStrategy.Bubble,
        typeof(RoutedEventHandler), typeof(NotifyIcon));

    /// <summary>
    ///     Occurs when the user clicks the icon in the notification area.
    /// </summary>
    public event RoutedEventHandler Click
    {
        add { AddHandler(ClickEvent, value); }
        remove { RemoveHandler(ClickEvent, value); }
    }

    /// <summary>
    ///     Identifies the <see cref="DoubleClick" /> routed event.
    /// </summary>
    public static readonly RoutedEvent DoubleClickEvent = EventManager.RegisterRoutedEvent("DoubleClick",
        RoutingStrategy.Bubble, typeof(RoutedEventHandler), typeof(NotifyIcon));

    /// <summary>
    ///     Occurs when the user double-clicks the icon in the notification area of the taskbar.
    /// </summary>
    public event RoutedEventHandler DoubleClick
    {
        add { AddHandler(DoubleClickEvent, value); }
        remove { RemoveHandler(DoubleClickEvent, value); }
    }

    /// <summary>
    ///     Identifies the <see cref="MouseClick" /> routed event.
    /// </summary>
    public static readonly RoutedEvent MouseClickEvent = EventManager.RegisterRoutedEvent("MouseClick",
        RoutingStrategy.Bubble, typeof(MouseButtonEventHandler), typeof(NotifyIcon));

    /// <summary>
    ///     Occurs when the user clicks a <see cref="NotifyIcon" /> with the mouse.
    /// </summary>
    public event MouseButtonEventHandler MouseClick
    {
        add { AddHandler(MouseClickEvent, value); }
        remove { RemoveHandler(MouseClickEvent, value); }
    }

    /// <summary>
    ///     Identifies the <see cref="MouseDoubleClick" /> routed event.
    /// </summary>
    public static readonly RoutedEvent MouseDoubleClickEvent = EventManager.RegisterRoutedEvent("MouseDoubleClick",
        RoutingStrategy.Bubble, typeof(MouseButtonEventHandler), typeof(NotifyIcon));

    /// <summary>
    ///     Occurs when the user double-clicks the <see cref="NotifyIcon" /> with the mouse.
    /// </summary>
    public event MouseButtonEventHandler MouseDoubleClick
    {
        add { AddHandler(MouseDoubleClickEvent, value); }
        remove { RemoveHandler(MouseDoubleClickEvent, value); }
    }
    /// <summary>
    ///     Initializes the <see cref="NotifyIcon" /> class.
    /// </summary>
    static NotifyIcon()
    {
        s_taskbarCreatedWindowMessage = NativeMethods.RegisterWindowMessage("TaskbarCreated");

        VisibilityProperty.OverrideMetadata(typeof(NotifyIcon), new FrameworkPropertyMetadata(OnVisibilityChanged));
    }

    /// <summary>
    ///     Initializes a new instance of the <see cref="NotifyIcon" /> class.
    /// </summary>
    public NotifyIcon()
    {
        IsVisibleChanged += OnIsVisibleChanged;
    }

    /// <summary>
    ///     Releases unmanaged resources and performs other cleanup operations before the
    ///     <see cref="NotifyIcon" /> is reclaimed by garbage collection.
    /// </summary>
    ~NotifyIcon()
    {
        Dispose(false);
    }

    /// <summary>
    ///     Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
    /// </summary>
    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    private void Dispose(bool isDisposing)
    {
        if (isDisposing)
        {
            if (_hwndSource != null)
            {
                UpdateIcon(true);
                _hwndSource.Dispose();
            }
        }
        else if (_hwndSource != null)
        {
            _ = NativeMethods.PostMessage(new HandleRef(_hwndSource, _hwndSource.Handle),
                NativeMethods.WindowMessage.Close, IntPtr.Zero, IntPtr.Zero);
            _hwndSource.Dispose();
        }
    }
    /// <summary>
    ///     Displays a balloon tip in the taskbar for the specified time period.
    /// </summary>
    /// <param name="timeout">The time period, in milliseconds, the balloon tip should display.</param>
    public void ShowBalloonTip(int timeout)
    {
        ShowBalloonTip(timeout, BalloonTipTitle, BalloonTipText, BalloonTipIcon);
    }

    /// <summary>
    ///     Displays a balloon tip with the specified title, text, and icon in the taskbar for the specified time period.
    /// </summary>
    /// <param name="timeout">The time period, in milliseconds, the balloon tip should display.</param>
    /// <param name="tipTitle">The title to display on the balloon tip.</param>
    /// <param name="tipText">The text to display on the balloon tip.</param>
    /// <param name="tipIcon">One of the <see cref="NotifyBalloonIcon" /> values.</param>
    public void ShowBalloonTip(int timeout, string tipTitle, string tipText, NotifyBalloonIcon tipIcon)
    {
        if (timeout < 0)
        {
            throw new ArgumentOutOfRangeException(nameof(timeout), timeout, SR.NotifyIcon_NoNegativeTimeout);
        }
        ArgumentValidator.NotNullOrEmptyString(tipText, "tipText");
        ArgumentValidator.EnumValueIsDefined(typeof(NotifyBalloonIcon), tipIcon, "tipIcon");

        if (_iconCreated)
        {
            var pnid = new NativeMethods.NOTIFYICONDATA
            {
                hWnd = _hwndSource.Handle,
                uID = _id,
                uFlags = NativeMethods.NotifyIconFlags.Balloon,
                uTimeoutOrVersion = timeout,
                szInfoTitle = tipTitle,
                szInfo = tipText,
                dwInfoFlags = (int)tipIcon
            };
            _ = NativeMethods.Shell_NotifyIcon(1, pnid);
        }
    }
    private void ShowContextMenu()
    {
        if (ContextMenu != null)
        {
            _ = NativeMethods.SetForegroundWindow(new HandleRef(_hwndSource, _hwndSource.Handle));

            ContextMenuService.SetPlacement(ContextMenu, PlacementMode.MousePoint);
            ContextMenu.IsOpen = true;
        }
    }

    /// <summary>
    ///     Shows or hides the icon according to the <see cref="P:IsVisible" /> and <see cref="P:Visibility" /> properties.
    /// </summary>
    private void UpdateIconForVisibility()
    {
        if (IconVisibility == NotifyIconVisibility.UseControlVisibility)
        {
            UpdateIcon();
        }
    }

    private void UpdateIcon(bool forceDestroy = false)
    {
        if (DesignerProperties.GetIsInDesignMode(this))
            return;

        var iconVisibility = IconVisibility;
        bool showIconInTray = !forceDestroy &&
                              (iconVisibility == NotifyIconVisibility.Visible ||
                               (iconVisibility == NotifyIconVisibility.UseControlVisibility && IsVisible));

        lock (_syncObj)
        {
            IntPtr iconHandle = IntPtr.Zero;

            try
            {
                if (showIconInTray && _hwndSource == null)
                {
                    _hwndSource = new NotifyIconHwndSource(this);
                }

                if (_hwndSource != null)
                {
                    _hwndSource.LockReference(showIconInTray);

                    var pnid = new NativeMethods.NOTIFYICONDATA
                    {
                        uCallbackMessage = (int)NativeMethods.WindowMessage.TrayMouseMessage,
                        uFlags = NativeMethods.NotifyIconFlags.Message | NativeMethods.NotifyIconFlags.ToolTip,
                        hWnd = _hwndSource.Handle,
                        uID = _id,
                        szTip = Text
                    };
                    if (Icon != null)
                    {
                        iconHandle = NativeMethods.GetHIcon(Icon);

                        pnid.uFlags |= NativeMethods.NotifyIconFlags.Icon;
                        pnid.hIcon = iconHandle;
                    }

                    if (showIconInTray && iconHandle != IntPtr.Zero)
                    {
                        if (!_iconCreated)
                        {
                            _ = NativeMethods.Shell_NotifyIcon(0, pnid);
                            _iconCreated = true;
                        }
                        else
                        {
                            _ = NativeMethods.Shell_NotifyIcon(1, pnid);
                        }
                    }
                    else if (_iconCreated)
                    {
                        _ = NativeMethods.Shell_NotifyIcon(2, pnid);
                        _iconCreated = false;
                    }
                }
            }
            finally
            {
                if (iconHandle != IntPtr.Zero)
                {
                    _ = NativeMethods.DestroyIcon(iconHandle);
                }
            }
        }
    }

    private static void OnVisibilityChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
    {
        ((NotifyIcon)o).UpdateIconForVisibility();
    }

    private void OnIsVisibleChanged(object sender, DependencyPropertyChangedEventArgs e)
    {
        UpdateIconForVisibility();
    }
    private void WmMouseDown(MouseButton button, int clicks)
    {
        MouseButtonEventArgs args;

        if (clicks == 2)
        {
            RaiseEvent(new RoutedEventArgs(DoubleClickEvent));

            args = new MouseButtonEventArgs(InputManager.Current.PrimaryMouseDevice, 0, button)
            {
                RoutedEvent = MouseDoubleClickEvent
            };
            RaiseEvent(args);

            _doubleClick = true;
        }

        args = new MouseButtonEventArgs(InputManager.Current.PrimaryMouseDevice, 0, button)
        {
            RoutedEvent = MouseDownEvent
        };
        RaiseEvent(args);
    }

    private void WmMouseMove()
    {
        var args = new MouseEventArgs(InputManager.Current.PrimaryMouseDevice, 0) { RoutedEvent = MouseMoveEvent };
        RaiseEvent(args);
    }

    private void WmMouseUp(MouseButton button)
    {
        var args = new MouseButtonEventArgs(InputManager.Current.PrimaryMouseDevice, 0, button)
        {
            RoutedEvent = MouseUpEvent
        };
        RaiseEvent(args);

        if (!_doubleClick)
        {
            RaiseEvent(new RoutedEventArgs(ClickEvent));

            args = new MouseButtonEventArgs(InputManager.Current.PrimaryMouseDevice, 0, button)
            {
                RoutedEvent = MouseClickEvent
            };
            RaiseEvent(args);
        }

        _doubleClick = false;
    }

    private void WmTaskbarCreated()
    {
        _iconCreated = false;
        UpdateIcon();
    }

    private void WndProc(int message, IntPtr lParam, out bool handled)
    {
        handled = true;

        if (message <= (int)NativeMethods.WindowMessage.MeasureItem)
        {
            if (message == (int)NativeMethods.WindowMessage.Destroy)
            {
                UpdateIcon(true);
                return;
            }
        }
        else
        {
            if (message != (int)NativeMethods.WindowMessage.TrayMouseMessage)
            {
                if (message == s_taskbarCreatedWindowMessage)
                {
                    WmTaskbarCreated();
                }
                handled = false;
                return;
            }
            switch ((NativeMethods.WindowMessage)lParam)
            {
                case NativeMethods.WindowMessage.MouseMove:
                    WmMouseMove();
                    return;
                case NativeMethods.WindowMessage.MouseDown:
                    WmMouseDown(MouseButton.Left, 1);
                    return;
                case NativeMethods.WindowMessage.LButtonUp:
                    WmMouseUp(MouseButton.Left);
                    return;
                case NativeMethods.WindowMessage.LButtonDblClk:
                    WmMouseDown(MouseButton.Left, 2);
                    return;
                case NativeMethods.WindowMessage.RButtonDown:
                    WmMouseDown(MouseButton.Right, 1);
                    return;
                case NativeMethods.WindowMessage.RButtonUp:
                    ShowContextMenu();
                    WmMouseUp(MouseButton.Right);
                    return;
                case NativeMethods.WindowMessage.RButtonDblClk:
                    WmMouseDown(MouseButton.Right, 2);
                    return;
                case NativeMethods.WindowMessage.MButtonDown:
                    WmMouseDown(MouseButton.Middle, 1);
                    return;
                case NativeMethods.WindowMessage.MButtonUp:
                    WmMouseUp(MouseButton.Middle);
                    return;
                case NativeMethods.WindowMessage.MButtonDblClk:
                    WmMouseDown(MouseButton.Middle, 2);
                    return;
            }
            switch ((NativeMethods.NotifyIconMessage)lParam)
            {
                case NativeMethods.NotifyIconMessage.BalloonShow:
                    RaiseEvent(new RoutedEventArgs(BalloonTipShownEvent));
                    return;
                case NativeMethods.NotifyIconMessage.BalloonHide:
                case NativeMethods.NotifyIconMessage.BalloonTimeout:
                    RaiseEvent(new RoutedEventArgs(BalloonTipClosedEvent));
                    return;
                case NativeMethods.NotifyIconMessage.BalloonUserClick:
                    RaiseEvent(new RoutedEventArgs(BalloonTipClickEvent));
                    return;
            }
            return;
        }
        if (message == s_taskbarCreatedWindowMessage)
        {
            WmTaskbarCreated();
        }
        handled = false;
    }

    [SuppressMessage("Style", "IDE0060:Remove unused parameter", Justification = "Delegate")]
    private IntPtr WndProc(IntPtr hwnd, int msg, IntPtr wParam, IntPtr lParam, ref bool handled)
    {
        WndProc(msg, lParam, out handled);

        return IntPtr.Zero;
    }

    /// <summary>
    /// Identifies the <see cref="IconVisibility"/> dependency property.
    /// </summary>
    public static readonly DependencyProperty IconVisibilityProperty = DependencyProperty.Register(
        "IconVisibility", typeof(NotifyIconVisibility), typeof(NotifyIcon), new FrameworkPropertyMetadata(OnIconVisibilityChanged));

    private static void OnIconVisibilityChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        var notifyIcon = (NotifyIcon)d;
        notifyIcon.UpdateIcon();
    }

    /// <summary>
    /// Gets or sets the notify icon's visibility.
    /// </summary>
    public NotifyIconVisibility IconVisibility
    {
        get { return (NotifyIconVisibility)GetValue(IconVisibilityProperty); }
        set { SetValue(IconVisibilityProperty, value); }
    }
    /// <summary>
    ///     Gets or sets the icon to display on the balloon tip.
    /// </summary>
    /// <value>The balloon tip icon.</value>
    public NotifyBalloonIcon BalloonTipIcon
    {
        get { return (NotifyBalloonIcon)GetValue(BalloonTipIconProperty); }
        set { SetValue(BalloonTipIconProperty, value); }
    }

    /// <summary>
    ///     Identifies the <see cref="BalloonTipIcon" /> dependency property.
    /// </summary>
    public static readonly DependencyProperty BalloonTipIconProperty =
        DependencyProperty.Register("BalloonTipIcon", typeof(NotifyBalloonIcon), typeof(NotifyIcon),
            new FrameworkPropertyMetadata(NotifyBalloonIcon.None));
    /// <summary>
    ///     Gets or sets the text to display on the balloon tip.
    /// </summary>
    /// <value>The balloon tip text.</value>
    public string BalloonTipText
    {
        get { return (string)GetValue(BalloonTipTextProperty); }
        set { SetValue(BalloonTipTextProperty, value); }
    }

    /// <summary>
    ///     Identifies the <see cref="BalloonTipText" /> dependency property.
    /// </summary>
    public static readonly DependencyProperty BalloonTipTextProperty =
        DependencyProperty.Register("BalloonTipText", typeof(string), typeof(NotifyIcon),
            new FrameworkPropertyMetadata());
    /// <summary>
    ///     Gets or sets the title of the balloon tip.
    /// </summary>
    /// <value>The balloon tip title.</value>
    public string BalloonTipTitle
    {
        get { return (string)GetValue(BalloonTipTitleProperty); }
        set { SetValue(BalloonTipTitleProperty, value); }
    }

    /// <summary>
    ///     Identifies the <see cref="BalloonTipTitle" /> dependency property.
    /// </summary>
    public static readonly DependencyProperty BalloonTipTitleProperty =
        DependencyProperty.Register("BalloonTipTitle", typeof(string), typeof(NotifyIcon),
            new FrameworkPropertyMetadata());
    /// <summary>
    ///     Gets or sets the tooltip text displayed when the mouse pointer rests on a notification area icon.
    /// </summary>
    /// <value>The text.</value>
    public string Text
    {
        get { return (string)GetValue(TextProperty); }
        set { SetValue(TextProperty, value); }
    }

    /// <summary>
    ///     Identifies the <see cref="Text" /> dependency property.
    /// </summary>
    public static readonly DependencyProperty TextProperty =
        DependencyProperty.Register("Text", typeof(string), typeof(NotifyIcon),
            new FrameworkPropertyMetadata(OnTextPropertyChanged, OnCoerceTextProperty), ValidateTextProperty);

    private static bool ValidateTextProperty(object baseValue)
    {
        var value = (string)baseValue;

        return value == null || value.Length <= 0x3f;
    }

    private static void OnTextPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        var notifyIcon = (NotifyIcon)d;

        notifyIcon.UpdateIcon();
    }

    private static object OnCoerceTextProperty(DependencyObject d, object baseValue)
    {
        return (string)baseValue ?? string.Empty;
    }
    /// <summary>
    ///     Gets or sets the icon.
    /// </summary>
    /// <value>The icon.</value>
    public ImageSource Icon
    {
        get { return (ImageSource)GetValue(IconProperty); }
        set { SetValue(IconProperty, value); }
    }

    /// <summary>
    ///     Identifies the <see cref="Icon" /> dependency property.
    /// </summary>
    public static readonly DependencyProperty IconProperty =
        Window.IconProperty.AddOwner(typeof(NotifyIcon),
            new FrameworkPropertyMetadata(OnNotifyIconChanged) { Inherits = true });

    private static void OnNotifyIconChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
    {
        var notifyIcon = (NotifyIcon)o;

        notifyIcon.UpdateIcon();
    }
    void IAddChild.AddChild(object value)
    {
        throw new InvalidOperationException(SR.IAddChild_TextOnly);
    }

    void IAddChild.AddText(string text)
    {
        Text = text;
    }
    private class NotifyIconHwndSource : HwndSource
    {
        private readonly NotifyIcon _reference;
        private GCHandle _rootRef;

        internal NotifyIconHwndSource(NotifyIcon component)
            : base(0, 0, 0, 0, 0, null, IntPtr.Zero)
        {
            _reference = component;

            AddHook(_reference.WndProc);
        }

        ~NotifyIconHwndSource()
        {
            if (Handle != IntPtr.Zero)
            {
                _ = NativeMethods.PostMessage(new HandleRef(this, Handle), NativeMethods.WindowMessage.Close,
                    IntPtr.Zero, IntPtr.Zero);
            }
        }

        public void LockReference(bool locked)
        {
            if (locked)
            {
                if (!_rootRef.IsAllocated)
                {
                    _rootRef = GCHandle.Alloc(_reference, GCHandleType.Normal);
                }
            }
            else if (_rootRef.IsAllocated)
            {
                _rootRef.Free();
            }
        }
    }
}

/// <summary>
/// Defines the icon visibility modes of <see cref="NotifyIcon"/>.
/// </summary>
public enum NotifyIconVisibility
{
    /// <summary>
    /// The icon is not shown.
    /// </summary>
    Hidden,
    /// <summary>
    /// The icon is shown.
    /// </summary>
    Visible,
    /// <summary>
    /// The icon is shown according to <see cref="UIElement.IsVisible"/>.
    /// </summary>
    UseControlVisibility
}
