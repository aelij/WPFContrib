namespace Avalon.Windows.Controls;

/// <summary>
/// Represents a delegate used for routed events that support cancellation.
/// </summary>
/// <param name="sender"></param>
/// <param name="e"></param>
public delegate void CancelRoutedEventHandler(object sender, CancelRoutedEventArgs e);

/// <summary>
/// Represents routed events arguments that support cancellation.
/// </summary>
public class CancelRoutedEventArgs : RoutedEventArgs
{
    /// <summary>
    /// Initializes a new instance of <see cref="CancelRoutedEventArgs"/>.
    /// </summary>
    public CancelRoutedEventArgs()
    {
    }

    /// <summary>
    /// Initializes a new instance of <see cref="CancelRoutedEventArgs"/>.
    /// </summary>
    public CancelRoutedEventArgs(RoutedEvent routedEvent)
        : base(routedEvent)
    {
    }

    /// <summary>
    /// Initializes a new instance of <see cref="CancelRoutedEventArgs"/>.
    /// </summary>
    public CancelRoutedEventArgs(RoutedEvent routedEvent, object source)
        : base(routedEvent, source)
    {
    }

    /// <summary>
    /// Gets or sets a value indicating whether to cancel the event.
    /// </summary>
    public bool Cancel { get; set; }
}
