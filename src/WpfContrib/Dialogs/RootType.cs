namespace Avalon.Windows.Dialogs;

/// <summary>
///     Specifies the type of the root folder in a <see cref="FolderBrowserDialog" />.
/// </summary>
public enum RootType
{
    /// <summary>
    ///     Use <see cref="System.Environment.SpecialFolder" />.
    /// </summary>
    SpecialFolder,

    /// <summary>
    ///     Use a path.
    /// </summary>
    Path
}