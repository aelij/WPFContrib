﻿using Avalon.Internal.Utility;
using Avalon.Windows.Converters;

namespace Avalon.Windows.Controls;

/// <summary>
///     Represents metadata for creating <see cref="TaskDialog" /> buttons or <see cref="CommandLink" />s.
/// </summary>
public class TaskDialogButtonData : INotifyPropertyChanged
{
    private TaskDialogButtonData(object header, object content, bool isDefault)
    {
        Header = header;
        Content = content;
        IsDefault = isDefault;
    }

    /// <summary>
    ///     Initializes a new instance of the <see cref="TaskDialogButtonData" /> class.
    /// </summary>
    /// <param name="value">The value.</param>
    /// <param name="header">The header.</param>
    /// <param name="content">The content.</param>
    /// <param name="isDefault">if set to <c>true</c>, the button is defaulted.</param>
    public TaskDialogButtonData(int value, object header, object content, bool isDefault)
        : this(header, content, isDefault)
    {
        Value = value;
    }

    /// <summary>
    ///     Initializes a new instance of the <see cref="TaskDialogButtonData" /> class.
    /// </summary>
    /// <param name="value">The value.</param>
    /// <param name="header">The header.</param>
    /// <param name="content">The content.</param>
    public TaskDialogButtonData(int value, object header, object content)
        : this(value, header, content, false)
    {
    }

    /// <summary>
    ///     Initializes a new instance of the <see cref="TaskDialogButtonData" /> class.
    /// </summary>
    /// <param name="button">The button.</param>
    /// <param name="isDefault">if set to <c>true</c>, the button is defaulted.</param>
    public TaskDialogButtonData(TaskDialogButtons button, bool isDefault)
        : this((int)button, null, null, isDefault)
    {
        if (!IsSingleButton(button))
        {
            throw new ArgumentOutOfRangeException(nameof(button), SR.TaskDialogButtonData_ButtonSingleValue);
        }

        Button = button;
        _header = TypeDescriptor.GetConverter(button).ConvertToString(button);
    }

    private static bool IsSingleButton(TaskDialogButtons button) => button switch
    {
        TaskDialogButtons.None or TaskDialogButtons.OK or TaskDialogButtons.Cancel or TaskDialogButtons.Yes or TaskDialogButtons.No or TaskDialogButtons.Retry or TaskDialogButtons.Close => true,
        _ => false,
    };

    /// <summary>
    ///     Initializes a new instance of the <see cref="TaskDialogButtonData" /> class.
    /// </summary>
    /// <param name="button">The button.</param>
    public TaskDialogButtonData(TaskDialogButtons button)
        : this(button, false)
    {
    }
    private object _header;

    /// <summary>
    ///     Gets or sets the header.
    /// </summary>
    /// <value>The header.</value>
    public object Header
    {
        get { return _header; }
        set
        {
            ValidateNonStandardButton();
            _header = value;
            OnPropertyChanged(nameof(Header));
        }
    }

    private object _content;

    /// <summary>
    ///     Gets or sets the content.
    /// </summary>
    /// <value>The content.</value>
    public object Content
    {
        get { return _content; }
        set
        {
            ValidateNonStandardButton();
            _content = value;
            OnPropertyChanged(nameof(Content));
        }
    }

    private int _value;

    /// <summary>
    ///     Gets or sets the value.
    /// </summary>
    /// <value>The value.</value>
    public int Value
    {
        get { return _value; }
        set
        {
            ValidateNonStandardButton();
            _value = value;
            OnPropertyChanged(nameof(Value));
        }
    }

    private bool _isDefault;

    /// <summary>
    ///     Gets or sets a value indicating whether the button is defaulted.
    /// </summary>
    /// <value>
    ///     <c>true</c> if the button is defaulted; otherwise, <c>false</c>.
    /// </value>
    public bool IsDefault
    {
        get { return _isDefault; }
        set
        {
            _isDefault = value;
            OnPropertyChanged(nameof(IsDefault));
        }
    }

    /// <summary>
    ///     Gets or sets the button.
    /// </summary>
    /// <value>The button.</value>
    public TaskDialogButtons Button { get; }

    private object _tag;

    /// <summary>
    ///     Gets or sets an arbitrary object value that can be used to store custom information about this object.
    /// </summary>
    /// <value>The intended value.</value>
    public object Tag
    {
        get { return _tag; }
        set
        {
            _tag = value;
            OnPropertyChanged(nameof(Tag));
        }
    }
    /// <summary>
    ///     Ensures that the current instance is a custom button.
    /// </summary>
    private void ValidateNonStandardButton()
    {
        if (Button != TaskDialogButtons.None)
        {
            throw new InvalidOperationException(SR.TaskDialogButtonData_SetStandardButton);
        }
    }

    /// <summary>
    ///     Returns a <see cref="T:System.String" /> that represents the current <see cref="T:System.Object" />.
    /// </summary>
    /// <returns>
    ///     A <see cref="T:System.String" /> that represents the current <see cref="T:System.Object" />.
    /// </returns>
    public override string ToString()
    {
        return InvariantString.Format("Header={0}, Content={1}", Header, Content);
    }

    /// <summary>
    ///     Converts a flag-valued <see cref="TaskDialogButtons" /> to <see cref="TaskDialogButtonData" /> objects.
    /// </summary>
    /// <param name="buttons">The buttons.</param>
    /// <returns></returns>
    public static IEnumerable<TaskDialogButtonData> FromStandardButtons(TaskDialogButtons buttons)
    {
        return
            ((IEnumerable<object>)new EnumFlagsConverter().Convert(buttons)).Cast<TaskDialogButtons>()
                .Select(b => new TaskDialogButtonData(b));
    }
    /// <summary>
    ///     Occurs when a property value changes.
    /// </summary>
    public event PropertyChangedEventHandler PropertyChanged;

    private void OnPropertyChanged(string propertyName)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}
