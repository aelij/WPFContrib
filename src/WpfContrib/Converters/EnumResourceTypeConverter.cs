using System.Globalization;
using System.Reflection;
using System.Resources;
using Avalon.Internal.Utility;

namespace Avalon.Windows.Converters;

/// <summary>
///     Provides a <see cref="TypeConverter" /> that converts enum values to and from resource strings.
/// </summary>
/// <typeparam name="TResourceManager">
///     The type of the static class that contains a reference to a
///     <see cref="ResourceManager" />.
/// </typeparam>
public class EnumResourceTypeConverter<TResourceManager> : EnumConverter
{
    private static readonly ResourceManager s_resourceManager;
    /// <summary>
    ///     Initializes the <see cref="EnumResourceTypeConverter{TResourceManager}" /> class.
    /// </summary>
    static EnumResourceTypeConverter()
    {
        PropertyInfo property = typeof(TResourceManager).GetProperty("ResourceManager",
            BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static, null,
            typeof(ResourceManager), Type.EmptyTypes, null);

        if (property != null)
        {
            s_resourceManager = (ResourceManager)property.GetValue(null, null);
        }

        if (s_resourceManager == null)
        {
            throw new InvalidOperationException(SR.EnumResourceTypeConverter_InvalidResourceManager);
        }
    }

    /// <summary>
    ///     Initializes a new instance of the <see cref="EnumResourceTypeConverter{TResourceManager}" /> class.
    /// </summary>
    /// <param name="type">
    ///     A <see cref="T:System.Type" /> that represents the type of enumeration to associate with this
    ///     enumeration converter.
    /// </param>
    public EnumResourceTypeConverter(Type type)
        : base(type)
    {
    }
    /// <summary>
    ///     Converts the given value object to the specified destination type.
    /// </summary>
    /// <param name="context">An <see cref="T:System.ComponentModel.ITypeDescriptorContext" /> that provides a format context.</param>
    /// <param name="culture">
    ///     An optional <see cref="T:System.Globalization.CultureInfo" />. If not supplied, the current
    ///     culture is assumed.
    /// </param>
    /// <param name="value">The <see cref="T:System.Object" /> to convert.</param>
    /// <param name="destinationType">The <see cref="T:System.Type" /> to convert the value to.</param>
    /// <returns>
    ///     An <see cref="T:System.Object" /> that represents the converted <paramref name="value" />.
    /// </returns>
    /// <exception cref="T:System.ArgumentNullException">
    ///     <paramref name="destinationType" /> is null.
    /// </exception>
    /// <exception cref="T:System.ArgumentException">
    ///     <paramref name="value" /> is not a valid value for the enumeration.
    /// </exception>
    /// <exception cref="T:System.NotSupportedException">The conversion cannot be performed. </exception>
    public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value,
        Type destinationType)
    {
        if (value != null && destinationType == typeof(string))
        {
            return string.Join(", ",
                // split using commas
                value.ToString().Split(new[] { ',', ' ' }, StringSplitOptions.RemoveEmptyEntries)
                    // get resource string
                    .Select(s => s_resourceManager.GetString(string.Concat(EnumType.Name, '_', s), culture) ?? s)
                    .ToArray());
        }
        return base.ConvertTo(context, culture, value, destinationType);
    }

    /// <summary>
    ///     Converts the specified value object to an enumeration object.
    /// </summary>
    /// <param name="context">An <see cref="T:System.ComponentModel.ITypeDescriptorContext" /> that provides a format context.</param>
    /// <param name="culture">
    ///     An optional <see cref="T:System.Globalization.CultureInfo" />. If not supplied, the current
    ///     culture is assumed.
    /// </param>
    /// <param name="value">The <see cref="T:System.Object" /> to convert.</param>
    /// <returns>
    ///     An <see cref="T:System.Object" /> that represents the converted <paramref name="value" />.
    /// </returns>
    /// <exception cref="T:System.FormatException">
    ///     <paramref name="value" /> is not a valid value for the target type.
    /// </exception>
    /// <exception cref="T:System.NotSupportedException">The conversion cannot be performed. </exception>
    public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value)
    {
        if (value is string stringValue)
        {
            // we split and rejoin to trim any spaces between entries
            stringValue = string.Concat(',',
                string.Join(",", stringValue.Split(new[] { ',', ' ' }, StringSplitOptions.RemoveEmptyEntries)), ',');

            // get values
            ulong[] values = Enum.GetValues(EnumType).Cast<object>().Select(EnumHelper.ToUInt64).ToArray();

            ulong enumValue = Enum.GetNames(EnumType)
                // try to get string from resource manager
                .Select(
                    (s, i) =>
                        new KeyValuePair<int, string>(i,
                            s_resourceManager.GetString(string.Concat(EnumType.Name, '_', s), culture) ?? s))
                // filter to those strings who appear in the value parameter
                .Where(si => stringValue.Contains(string.Concat(',', si.Value.Trim(), ',')))
                // aggregate values using bit 'and' operation
                .Aggregate(0UL, (i, si) => i & values[si.Key]);

            return Convert.ChangeType(enumValue, Enum.GetUnderlyingType(EnumType), culture);
        }
        return base.ConvertFrom(context, culture, value);
    }
}
