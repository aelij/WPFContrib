using System.Diagnostics;
using Avalon.Windows.Internal.Utility;
using Avalon.Windows.Utility;

namespace Avalon.Windows.Controls;

/// <summary>
/// Provides a modal dialog that uses inline <see cref="UIElement"/>s
/// instead of a top-level window.
/// </summary>
public class InlineModalDialog : HeaderedContentControl
{
    private bool _isOpen;
    private DispatcherFrame _dispatcherFrame;
    /// <summary>
    /// Initializes the <see cref="InlineModalDialog"/> class.
    /// </summary>
    static InlineModalDialog()
    {
        DefaultStyleKeyProperty.OverrideMetadata(typeof(InlineModalDialog), new FrameworkPropertyMetadata(typeof(InlineModalDialog)));
        IsTabStopProperty.OverrideMetadata(typeof(InlineModalDialog), new FrameworkPropertyMetadata(false.Box()));

        CommandManager.RegisterClassCommandBinding(typeof(InlineModalDialog),
            new CommandBinding(CloseCommand, (sender, args) => ((InlineModalDialog)sender).HandleCloseCommand(args)));
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="InlineModalDialog"/> class.
    /// </summary>
    public InlineModalDialog()
    {
        KeyboardNavigation.SetTabNavigation(this, KeyboardNavigationMode.Local);
        KeyboardNavigation.SetDirectionalNavigation(this, KeyboardNavigationMode.Local);

        Loaded += OnLoaded;
    }

    private void OnLoaded(object sender, RoutedEventArgs e)
    {
        Loaded -= OnLoaded;

        _ = Focus();
    }
    /// <summary>
    /// Identifies the <see cref="DialogIntroAnimation"/> dependency property.
    /// </summary>
    public static readonly DependencyProperty DialogIntroAnimationProperty = DependencyProperty.Register("DialogIntroAnimation", typeof(Storyboard), typeof(InlineModalDialog));

    /// <summary>
    /// Gets or sets the animation that runs when this dialog is shown.
    /// </summary>
    public Storyboard DialogIntroAnimation
    {
        get { return (Storyboard)GetValue(DialogIntroAnimationProperty); }
        set { SetValue(DialogIntroAnimationProperty, value); }
    }

    /// <summary>
    /// Identifies the <see cref="DialogOutroAnimation"/> dependency property.
    /// </summary>
    public static readonly DependencyProperty DialogOutroAnimationProperty = DependencyProperty.Register("DialogOutroAnimation", typeof(Storyboard), typeof(InlineModalDialog));

    /// <summary>
    /// Gets or sets the animation that runs when this dialog is closed.
    /// </summary>
    public Storyboard DialogOutroAnimation
    {
        get { return (Storyboard)GetValue(DialogOutroAnimationProperty); }
        set { SetValue(DialogOutroAnimationProperty, value); }
    }

    /// <summary>
    /// Gets or sets a value indicating whether to show the blurrer.
    /// </summary>
    /// <value><c>true</c> if the blurrer is shown; otherwise, <c>false</c>.</value>
    public bool ShowBlurrer
    {
        get { return (bool)GetValue(ShowBlurrerProperty); }
        set { SetValue(ShowBlurrerProperty, value); }
    }

    /// <summary>
    /// Identifies the <see cref="ShowBlurrer"/> dependency property.
    /// </summary>
    public static readonly DependencyProperty ShowBlurrerProperty =
        DependencyProperty.Register("ShowBlurrer", typeof(bool), typeof(InlineModalDialog), new UIPropertyMetadata(true));

    /// <summary>
    /// Gets or sets the owner.
    /// <remarks>
    /// This value is used to retrieve the corresponding <see cref="InlineModalDecorator"/>.
    /// </remarks>
    /// </summary>
    /// <value>The owner.</value>
    public DependencyObject Owner
    {
        get { return (DependencyObject)GetValue(OwnerProperty); }
        set { SetValue(OwnerProperty, value); }
    }

    /// <summary>
    /// Identifies the <see cref="Owner"/> dependency property.
    /// </summary>
    public static readonly DependencyProperty OwnerProperty =
        DependencyProperty.Register("Owner", typeof(DependencyObject), typeof(InlineModalDialog), new FrameworkPropertyMetadata());

    private static InlineModalDecorator GetModalDecorator(DependencyObject obj)
    {
        return (InlineModalDecorator)obj.GetValue(s_modalDecoratorProperty);
    }

    internal static void SetModalDecorator(DependencyObject obj, InlineModalDecorator value)
    {
        obj.SetValue(s_modalDecoratorProperty, value);
    }

    internal static void ClearModalDecorator(DependencyObject obj)
    {
        obj.ClearValue(s_modalDecoratorProperty);
    }

    private static readonly DependencyProperty s_modalDecoratorProperty =
        DependencyProperty.RegisterAttached("ModalDecorator", typeof(InlineModalDecorator), typeof(InlineModalDialog), new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.Inherits | FrameworkPropertyMetadataOptions.OverridesInheritanceBehavior));

    /// <summary>
    /// Gets or sets the dialog result.
    /// </summary>
    /// <value>The dialog result.</value>
    public bool? DialogResult { get; set; }
    /// <summary>
    /// Identifies the <c>Close</c> routed command, which can be used to close the dialog.
    /// </summary>
    public static readonly RoutedCommand CloseCommand = new("Close", typeof(InlineModalDialog));
    /// <summary>
    /// Identifies the <see cref="E:Opened"/> routed event.
    /// </summary>
    public static readonly RoutedEvent OpenedEvent = EventManager.RegisterRoutedEvent("Opened", RoutingStrategy.Bubble, typeof(RoutedEventHandler), typeof(InlineModalDialog));

    /// <summary>
    /// Fires when the dialog is opened.
    /// </summary>
    public event RoutedEventHandler Opened
    {
        add { AddHandler(OpenedEvent, value); }
        remove { RemoveHandler(OpenedEvent, value); }
    }

    /// <summary>
    /// Raises the <see cref="Opened"/> routed event.
    /// </summary>
    protected virtual void OnOpened()
    {
        RaiseEvent(new RoutedEventArgs(OpenedEvent));
    }

    /// <summary>
    /// Identifies the <see cref="E:Closed"/> routed event.
    /// </summary>
    public static readonly RoutedEvent ClosedEvent = EventManager.RegisterRoutedEvent("Closed", RoutingStrategy.Bubble, typeof(RoutedEventHandler), typeof(InlineModalDialog));

    /// <summary>
    /// Fires when the dialog closes.
    /// </summary>
    public event RoutedEventHandler Closed
    {
        add { AddHandler(ClosedEvent, value); }
        remove { RemoveHandler(ClosedEvent, value); }
    }

    /// <summary>
    /// Raises the <see cref="Closed"/> routed event.
    /// </summary>
    protected virtual void OnClosed()
    {
        RaiseEvent(new RoutedEventArgs(ClosedEvent));
    }

    /// <summary>
    /// Identifies the <see cref="E:Closing"/> routed event.
    /// </summary>
    public static readonly RoutedEvent ClosingEvent = EventManager.RegisterRoutedEvent("Closing", RoutingStrategy.Bubble, typeof(CancelRoutedEventHandler), typeof(InlineModalDialog));

    /// <summary>
    /// Fires when the dialog is about to close.
    /// </summary>
    public event CancelRoutedEventHandler Closing
    {
        add { AddHandler(ClosingEvent, value); }
        remove { RemoveHandler(ClosingEvent, value); }
    }

    /// <summary>
    /// Raises the <see cref="Closing"/> routed event.
    /// </summary>
    /// <param name="args"></param>
    protected virtual void OnClosing(CancelRoutedEventArgs args)
    {
        RaiseEvent(args);
    }
    /// <summary>
    /// Shows the modal dialog.
    /// </summary>
    public void Show()
    {
        ValidateOwner();

        InlineModalDecorator panel = GetModalDecorator(Owner);

        if (panel == null)
            return;
        panel.AddModal(this, ShowBlurrer);

        if (Animator.IsAnimationEnabled)
        {
            Storyboard dialogAnim = DialogIntroAnimation;
            if (dialogAnim != null)
            {
                if (dialogAnim.IsFrozen)
                {
                    dialogAnim = dialogAnim.Clone();
                }

                dialogAnim.AttachCompletedEventHandler(OnOpenAnimationCompleted);
                dialogAnim.Begin(this);
            }
        }
        else
        {
            OpenCompleted();
        }

        _dispatcherFrame = new DispatcherFrame();
        Dispatcher.PushFrame(_dispatcherFrame);

        _isOpen = false;
    }

    /// <summary>
    /// Closes the modal dialog.
    /// </summary>
    public void Close()
    {
        if (!_isOpen)
            return;

        ValidateOwner();

        InlineModalDecorator panel = GetModalDecorator(Owner);

        if (panel == null)
            return;

        var cancelArgs = new CancelRoutedEventArgs(ClosingEvent);
        OnClosing(cancelArgs);
        if (cancelArgs.Cancel)
            return;

        if (Animator.IsAnimationEnabled)
        {
            Storyboard dialogAnim = DialogOutroAnimation;
            if (dialogAnim != null)
            {
                if (dialogAnim.IsFrozen)
                {
                    dialogAnim = dialogAnim.Clone();
                }

                // Add a handler so we know when the dialog can be closed.
                dialogAnim.AttachCompletedEventHandler(OnCloseAnimationCompleted);
                dialogAnim.Begin(this);
            }
        }
        else
        {
            CloseDialog(panel);
        }
    }

    /// <summary>
    /// Attempts to close the front-most dialog of the provided owner.
    /// </summary>
    /// <param name="owner"></param>
    public static void CloseCurrent(DependencyObject owner)
    {
        InlineModalDecorator panel = GetModalDecorator(owner);
        if (panel == null)
            return;

        if (panel.TopmostModal is InlineModalDialog current)
        {
            current.Close();
        }
    }
    private void ValidateOwner()
    {
        if (Owner == null)
        {
            throw new InvalidOperationException("Owner must be set.");
        }
    }

    /// <summary>
    /// Handler for the close command.
    /// </summary>
    /// <param name="e"></param>
    private void HandleCloseCommand(ExecutedRoutedEventArgs e)
    {
        if (DialogResult == null)
        {
            if (e.OriginalSource is Button source)
            {
                if (source.IsDefault)
                {
                    DialogResult = true;
                }
                else if (source.IsCancel)
                {
                    DialogResult = false;
                }
            }
        }
        e.Handled = true;
        Close();
    }

    private void OnOpenAnimationCompleted(object sender, EventArgs e)
    {
        OpenCompleted();
    }

    private void OpenCompleted()
    {
        _isOpen = true;

        OnOpened();
    }

    /// <summary>
    /// Handler for the window close animation completion, this delays
    /// actually closing the window until all outro animations have completed.
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void OnCloseAnimationCompleted(object sender, EventArgs e)
    {
        InlineModalDecorator panel = GetModalDecorator(Owner);
        Debug.Assert(panel != null);
        CloseDialog(panel);
    }

    private void CloseDialog(InlineModalDecorator panel)
    {
        panel.RemoveModal(this, ShowBlurrer);

        OnClosed();

        var topMost = panel.TopmostModal;
        if (topMost != null)
        {
            _ = topMost.Focus();
        }

        if (_dispatcherFrame != null)
        {
            _dispatcherFrame.Continue = false;
            _dispatcherFrame = null;
        }

        _isOpen = false;
    }
}
