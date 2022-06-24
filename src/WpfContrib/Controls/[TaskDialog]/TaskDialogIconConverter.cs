using System.Globalization;
using System.Text.RegularExpressions;
using Avalon.Internal.Utility;

namespace Avalon.Windows.Controls;

/// <summary>
///     Converts between <see cref="TaskDialogIcon" /> and an <see cref="ImageSource" />.
/// </summary>
public class TaskDialogIconConverter : ImageSourceConverter
{
    private static readonly string s_iconUriScheme;
    private static readonly Regex s_iconRegex = new(@"component/Images/(\w+)\.ico", RegexOptions.IgnoreCase);

    static TaskDialogIconConverter()
    {
        string assemblyName = typeof(TaskDialog).Assembly.FullName;
        assemblyName = assemblyName[..assemblyName.IndexOf(',')];

        s_iconUriScheme = InvariantString.Format("pack://application:,,,/{0};component/Images/{{0}}.ico", assemblyName);
    }
    /// <summary>
    ///     Determines whether the converter can convert an object of the given type to an instance of
    ///     <see cref="T:System.Windows.Media.ImageSource" />.
    /// </summary>
    /// <param name="context">Type context information used to evaluate conversion.</param>
    /// <param name="sourceType">The type of the source that is being evaluated for conversion.</param>
    /// <returns>
    ///     true if the converter can convert the provided type to an instance of
    ///     <see cref="T:System.Windows.Media.ImageSource" />; otherwise, false.
    /// </returns>
    public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType)
    {
        return (sourceType == typeof(TaskDialogIcon)) ||
               base.CanConvertFrom(context, sourceType);
    }

    /// <summary>
    ///     Attempts to convert a specified object to an instance of <see cref="T:System.Windows.Media.ImageSource" />.
    /// </summary>
    /// <param name="context">Type context information used for conversion.</param>
    /// <param name="culture">Cultural information that is respected during conversion.</param>
    /// <param name="value">The object being converted.</param>
    /// <returns>
    ///     A new instance of <see cref="T:System.Windows.Media.ImageSource" />.
    /// </returns>
    /// <exception cref="T:System.NotSupportedException">
    ///     <paramref name="value" /> is null or is an invalid type.
    /// </exception>
    public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value)
    {
        if (value == null)
        {
            throw GetConvertFromException(null);
        }

        if (value is not TaskDialogIcon icon && !Enum.TryParse(value as string, true, out icon))
        {
            return null;
        }

        if (icon == TaskDialogIcon.None)
        {
            return null;
        }

        value = InvariantString.Format(s_iconUriScheme, value);
        return base.ConvertFrom(context, culture, value);
    }

    /// <summary>
    ///     Determines whether an instance of <see cref="T:System.Windows.Media.ImageSource" /> can be converted to a different
    ///     type.
    /// </summary>
    /// <param name="context">Type context information used to evaluate conversion.</param>
    /// <param name="destinationType">The desired type to evaluate the conversion to.</param>
    /// <returns>
    ///     true if the converter can convert this instance of <see cref="T:System.Windows.Media.ImageSource" />; otherwise,
    ///     false.
    /// </returns>
    /// <exception cref="T:System.ArgumentException">
    ///     <paramref name="context" /> instance is not an <see cref="T:System.Windows.Media.ImageSource" />.
    /// </exception>
    public override bool CanConvertTo(ITypeDescriptorContext context, Type destinationType)
    {
        return destinationType == typeof(TaskDialogIcon) ||
               base.CanConvertTo(context, destinationType);
    }

    /// <summary>
    ///     Attempts to convert an instance of <see cref="T:System.Windows.Media.ImageSource" /> to a specified type.
    /// </summary>
    /// <param name="context">Context information used for conversion.</param>
    /// <param name="culture">Cultural information that is respected during conversion.</param>
    /// <param name="value"><see cref="T:System.Windows.Media.ImageSource" /> to convert.</param>
    /// <param name="destinationType">Type being evaluated for conversion.</param>
    /// <returns>
    ///     A new instance of the <paramref name="destinationType" />.
    /// </returns>
    /// <exception cref="T:System.NotSupportedException">
    ///     <paramref name="value" /> is null or is not a valid type.-or-<paramref name="context" /> instance cannot serialize
    ///     to a string.
    /// </exception>
    public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value,
        Type destinationType)
    {
        if (value == null)
        {
            return null;
        }
        if (destinationType == typeof(TaskDialogIcon))
        {
            Match match = s_iconRegex.Match(value.ToString());
            if (match.Success)
            {
                if (Enum.TryParse(match.Groups[1].Value, out TaskDialogIcon icon))
                {
                    return icon;
                }
            }
            return null;
        }
        return base.ConvertTo(context, culture, value, destinationType);
    }
    /// <summary>
    ///     Converts a <see cref="TaskDialogIcon" /> to an <see cref="ImageSource" />.
    /// </summary>
    /// <param name="value">The value.</param>
    /// <returns></returns>
    public static ImageSource ConvertFrom(TaskDialogIcon value)
    {
        return (ImageSource)new TaskDialogIconConverter().ConvertFrom(null, null, value);
    }

    /// <summary>
    ///     Converts a <see cref="TaskDialogIcon" /> to an <see cref="ImageSource" />.
    /// </summary>
    /// <param name="value">The value.</param>
    /// <returns></returns>
    public static ImageSource ConvertFrom(string value)
    {
        return (ImageSource)new TaskDialogIconConverter().ConvertFrom(null, null, value);
    }

    /// <summary>
    ///     Converts an <see cref="ImageSource" /> to a <see cref="TaskDialogIcon" />.
    /// </summary>
    /// <param name="value">The value.</param>
    /// <returns></returns>
    public static TaskDialogIcon? ConvertTo(ImageSource value)
    {
        return
            new TaskDialogIconConverter().ConvertTo(null, null, value, typeof(TaskDialogIcon)) as TaskDialogIcon?;
    }
}
