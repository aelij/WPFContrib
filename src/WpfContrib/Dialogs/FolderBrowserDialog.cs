using System.Runtime.InteropServices;
using Avalon.Internal.Win32;
using Microsoft.Win32;

namespace Avalon.Windows.Dialogs;

/// <summary>
///     Prompts the user to select a folder.
/// </summary>
public sealed class FolderBrowserDialog : CommonDialog
{
    private NativeMethods.FolderBrowserOptions _dialogOptions;
    /// <summary>
    ///     Initializes a new instance of the <see cref="FolderBrowserDialog" /> class.
    /// </summary>
    public FolderBrowserDialog()
    {
        Initialize();
    }
    /// <summary>
    ///     Resets the properties of a common dialog to their default values.
    /// </summary>
    public override void Reset()
    {
        Initialize();
    }

    /// <summary>
    ///     Displays the folder browser dialog.
    /// </summary>
    /// <param name="hwndOwner">Handle to the window that owns the dialog box.</param>
    /// <returns>
    ///     If the user clicks the OK button of the dialog that is displayed, true is returned; otherwise, false.
    /// </returns>
    protected override bool RunDialog(IntPtr hwndOwner)
    {
        bool result = false;

        IntPtr pidlRoot = IntPtr.Zero,
            pszPath = IntPtr.Zero,
            pidlSelected = IntPtr.Zero;

        SelectedPath = string.Empty;

        try
        {
            if (RootType == RootType.SpecialFolder)
            {
                _ = NativeMethods.SHGetFolderLocation(hwndOwner, (int)RootSpecialFolder, IntPtr.Zero, 0, out pidlRoot);
            }
            else // RootType == Path
            {
                _ = NativeMethods.SHParseDisplayName(RootPath, IntPtr.Zero, out pidlRoot, 0, out uint iAttribute);
            }

            var browseInfo = new NativeMethods.BROWSEINFO
            {
                HwndOwner = hwndOwner,
                Root = pidlRoot,
                DisplayName = new string(' ', 256),
                Title = Title,
                Flags = (uint)_dialogOptions,
                LParam = 0,
                Callback = HookProc
            };

            // Show dialog
            pidlSelected = NativeMethods.SHBrowseForFolder(ref browseInfo);

            if (pidlSelected != IntPtr.Zero)
            {
                result = true;

                pszPath = Marshal.AllocHGlobal(260 * Marshal.SystemDefaultCharSize);
                _ = NativeMethods.SHGetPathFromIDList(pidlSelected, pszPath);

                SelectedPath = Marshal.PtrToStringAuto(pszPath);
            }
        }
        finally // release all unmanaged resources
        {
            NativeMethods.IMalloc malloc = NativeMethods.GetSHMalloc();

            if (pidlRoot != IntPtr.Zero)
            {
                malloc.Free(pidlRoot);
            }

            if (pidlSelected != IntPtr.Zero)
            {
                malloc.Free(pidlSelected);
            }

            if (pszPath != IntPtr.Zero)
            {
                Marshal.FreeHGlobal(pszPath);
            }

            _ = Marshal.ReleaseComObject(malloc);
        }

        return result;
    }

    private void Initialize()
    {
        RootType = RootType.SpecialFolder;
        RootSpecialFolder = Environment.SpecialFolder.Desktop;
        RootPath = string.Empty;
        Title = string.Empty;

        // default options
        _dialogOptions = NativeMethods.FolderBrowserOptions.BrowseFiles
                         | NativeMethods.FolderBrowserOptions.ShowEditBox
                         | NativeMethods.FolderBrowserOptions.UseNewStyle
                         | NativeMethods.FolderBrowserOptions.BrowseShares
                         | NativeMethods.FolderBrowserOptions.ShowStatusText
                         | NativeMethods.FolderBrowserOptions.ValidateResult;
    }

    private bool GetOption(NativeMethods.FolderBrowserOptions option)
    {
        return (_dialogOptions & option) != NativeMethods.FolderBrowserOptions.None;
    }

    private void SetOption(NativeMethods.FolderBrowserOptions option, bool value)
    {
        if (value)
        {
            _dialogOptions |= option;
        }
        else
        {
            _dialogOptions &= ~option;
        }
    }
    /// <summary>
    ///     Gets or sets the type of the root.
    /// </summary>
    /// <value>The type of the root.</value>
    public RootType RootType { get; set; }

    /// <summary>
    ///     Gets or sets the root path.
    ///     <remarks>Valid only if RootType is set to Path.</remarks>
    /// </summary>
    /// <value>The root path.</value>
    public string RootPath { get; set; }

    /// <summary>
    ///     Gets or sets the root special folder.
    ///     <remarks>Valid only if RootType is set to SpecialFolder.</remarks>
    /// </summary>
    /// <value>The root special folder.</value>
    public Environment.SpecialFolder RootSpecialFolder { get; set; }

    /// <summary>
    ///     Gets or sets the display name.
    /// </summary>
    /// <value>The display name.</value>
    public string SelectedPath { get; set; }

    /// <summary>
    ///     Gets or sets the title.
    /// </summary>
    /// <value>The title.</value>
    public string Title { get; set; }

    /// <summary>
    ///     Gets or sets a value indicating whether browsing files is allowed.
    /// </summary>
    /// <value></value>
    public bool BrowseFiles
    {
        get { return GetOption(NativeMethods.FolderBrowserOptions.BrowseFiles); }
        set { SetOption(NativeMethods.FolderBrowserOptions.BrowseFiles, value); }
    }

    /// <summary>
    ///     Gets or sets a value indicating whether to show an edit box.
    /// </summary>
    /// <value></value>
    public bool ShowEditBox
    {
        get { return GetOption(NativeMethods.FolderBrowserOptions.ShowEditBox); }
        set { SetOption(NativeMethods.FolderBrowserOptions.ShowEditBox, value); }
    }

    /// <summary>
    ///     Gets or sets a value indicating whether browsing shares is allowed.
    /// </summary>
    /// <value></value>
    public bool BrowseShares
    {
        get { return GetOption(NativeMethods.FolderBrowserOptions.BrowseShares); }
        set { SetOption(NativeMethods.FolderBrowserOptions.BrowseShares, value); }
    }

    /// <summary>
    ///     Gets or sets a value indicating whether to show status text.
    /// </summary>
    /// <value></value>
    public bool ShowStatusText
    {
        get { return GetOption(NativeMethods.FolderBrowserOptions.ShowStatusText); }
        set { SetOption(NativeMethods.FolderBrowserOptions.ShowStatusText, value); }
    }

    /// <summary>
    ///     Gets or sets a value indicating whether to validate the result.
    /// </summary>
    /// <value></value>
    public bool ValidateResult
    {
        get { return GetOption(NativeMethods.FolderBrowserOptions.ValidateResult); }
        set { SetOption(NativeMethods.FolderBrowserOptions.ValidateResult, value); }
    }
}
