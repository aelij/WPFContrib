using System.Globalization;
using System.Windows.Data;

namespace Avalon.Windows.Converters;

/// <summary>
///     Converts an enum to a string using its <see cref="TypeConverter" />.
/// </summary>
public class EnumToStringConverter : ValueConverter
{
    /// <summary>
    ///     Converts a value.
    ///     <remarks>
    ///         If the value is an enum, the converter uses the enum's <see cref="TypeConverter" /> to retrieve its text value.
    ///         Otherwise, the original value is returned.
    ///     </remarks>
    /// </summary>
    /// <param name="value">The value produced by the <see cref="Binding" /> source.</param>
    /// <param name="targetType">The type of the <see cref="Binding" /> target property.</param>
    /// <param name="parameter">The converter parameter to use.</param>
    /// <param name="culture">The culture to use in the converter.</param>
    /// <returns>
    ///     A converted value. If the method returns null, the valid null value is used.
    /// </returns>
    public override object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value is Enum)
        {
            return TypeDescriptor.GetConverter(value).ConvertTo(value, typeof(string));
        }
        return value;
    }
}