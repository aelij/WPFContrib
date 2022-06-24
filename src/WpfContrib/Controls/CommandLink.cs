﻿namespace Avalon.Windows.Controls;

/// <summary>
///     Represents a command link button.
/// </summary>
public class CommandLink : Button
{
    static CommandLink()
    {
        DefaultStyleKeyProperty.OverrideMetadata(typeof(CommandLink),
            new FrameworkPropertyMetadata(typeof(CommandLink)));
    }
    /// <summary>
    ///     Gets or sets a value indicating whether the icon is visible.
    /// </summary>
    /// <value>
    ///     <c>true</c> if the icon is visible; otherwise, <c>false</c>.
    /// </value>
    public bool IsIconVisible
    {
        get { return (bool)GetValue(IsIconVisibleProperty); }
        set { SetValue(IsIconVisibleProperty, value); }
    }

    /// <summary>
    ///     Identifies the <see cref="IsIconVisible" /> dependency property.
    /// </summary>
    public static readonly DependencyProperty IsIconVisibleProperty =
        DependencyProperty.Register("IsIconVisible", typeof(bool), typeof(CommandLink),
            new FrameworkPropertyMetadata(true));
}
