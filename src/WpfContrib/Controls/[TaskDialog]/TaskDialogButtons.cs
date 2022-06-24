using Avalon.Windows.Converters;

namespace Avalon.Windows.Controls;

/// <summary>
///     Specifies standard buttons for the <see cref="TaskDialog" />.
/// </summary>
/// <seealso cref="TaskDialogButtonData.IsSingleButton"/>
[Flags, TypeConverter(typeof(EnumResourceTypeConverter<SR>))]
public enum TaskDialogButtons
{
    /// <summary>
    ///     No buttons; an empty selection.
    /// </summary>
    None = 0,

    /// <summary>
    ///     OK button.
    /// </summary>
    OK = 1,

    /// <summary>
    ///     Cancel button.
    /// </summary>
    Cancel = 2,

    /// <summary>
    ///     Yes button.
    /// </summary>
    Yes = 4,

    /// <summary>
    ///     No button.
    /// </summary>
    No = 8,

    /// <summary>
    ///     Retry button.
    /// </summary>
    Retry = 16,

    /// <summary>
    ///     Close button.
    /// </summary>
    Close = 32
}