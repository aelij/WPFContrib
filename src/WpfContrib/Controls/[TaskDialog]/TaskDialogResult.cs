namespace Avalon.Windows.Controls;

/// <summary>
///     Represents a result received from a <see cref="TaskDialog" />.
/// </summary>
public sealed class TaskDialogResult
{
    /// <summary>
    ///     Gets or sets the selected button.
    /// </summary>
    /// <value>The button.</value>
    public object Button { get; internal set; }

    /// <summary>
    ///     Gets or sets the selected radio button.
    /// </summary>
    /// <value>The radio button.</value>
    public object RadioButton { get; internal set; }

    /// <summary>
    ///     Gets or sets the selected standard button.
    /// </summary>
    /// <value>The standard button.</value>
    public TaskDialogButtons StandardButton
    {
        get { return (ButtonData?.Button) ?? TaskDialogButtons.None; }
    }

    /// <summary>
    ///     Gets or sets the selected button data.
    /// </summary>
    /// <value>The button data.</value>
    public TaskDialogButtonData ButtonData { get; internal set; }

    /// <summary>
    ///     Gets or sets a value indicating whether the verification was checked.
    /// </summary>
    /// <value>
    ///     <c>true</c> if the verification was checked; otherwise, <c>false</c>.
    /// </value>
    public bool IsVerified { get; internal set; }
}