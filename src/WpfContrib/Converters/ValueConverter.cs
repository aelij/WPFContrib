using System.Globalization;
using System.Windows.Data;
using System.Windows.Markup;

namespace Avalon.Windows.Converters;

/// <summary>
///     Provides a base class for <see cref="IValueConverter" />s and <see cref="IMultiValueConverter" />s.
/// </summary>
public abstract class ValueConverter : MarkupExtension, IValueConverter, IMultiValueConverter
{
    object IValueConverter.Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        return Convert(value, targetType, parameter, culture);
    }

    object IValueConverter.ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        return ConvertBack(value, targetType, parameter, culture);
    }
    object IMultiValueConverter.Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
    {
        return MultiConvert(values, targetType, parameter, culture);
    }

    object[] IMultiValueConverter.ConvertBack(object value, Type[] targetTypes, object parameter,
        CultureInfo culture)
    {
        return MultiConvertBack(value, targetTypes, parameter, culture);
    }
    /// <summary>
    ///     Converts a value.
    /// </summary>
    /// <param name="value">The value produced by the <see cref="Binding" /> source.</param>
    /// <param name="targetType">The type of the <see cref="Binding" /> target property.</param>
    /// <param name="parameter">The converter parameter to use.</param>
    /// <param name="culture">The culture to use in the converter.</param>
    /// <returns>
    ///     A converted value. If the method returns null, the valid null value is used.
    /// </returns>
    public virtual object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }

    /// <summary>
    ///     Converts a value.
    /// </summary>
    /// <param name="value">The value that is produced by the <see cref="Binding" /> target.</param>
    /// <param name="targetType">The type to convert to.</param>
    /// <param name="parameter">The converter parameter to use.</param>
    /// <param name="culture">The culture to use in the converter.</param>
    /// <returns>
    ///     A converted value. If the method returns null, the valid null value is used.
    /// </returns>
    public virtual object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }

    /// <summary>
    ///     Converts source values to a value for the <see cref="Binding" /> target. The data binding engine calls this method
    ///     when it propagates the values from source bindings to the <see cref="Binding" /> target.
    /// </summary>
    /// <param name="values">
    ///     The array of values that the source bindings in the
    ///     <see cref="T:System.Windows.Data.MultiBinding" /> produces. The value
    ///     <see cref="F:System.Windows.DependencyProperty.UnsetValue" /> indicates that the source binding has no value to
    ///     provide for conversion.
    /// </param>
    /// <param name="targetType">The type of the <see cref="Binding" /> target property.</param>
    /// <param name="parameter">The converter parameter to use.</param>
    /// <param name="culture">The culture to use in the converter.</param>
    /// <returns>
    ///     A converted value.If the method returns null, the valid null value is used.A return value of
    ///     <see cref="T:System.Windows.DependencyProperty" />.<see cref="F:System.Windows.DependencyProperty.UnsetValue" />
    ///     indicates that the converter did not produce a value, and that the binding will use the
    ///     <see cref="P:System.Windows.Data.BindingBase.FallbackValue" /> if it is available, or else will use the default
    ///     value.A return value of <see cref="T:System.Windows.Data.Binding" />.
    ///     <see cref="F:System.Windows.Data.Binding.DoNothing" /> indicates that the binding does not transfer the value or
    ///     use the <see cref="P:System.Windows.Data.BindingBase.FallbackValue" /> or the default value.
    /// </returns>
    public virtual object MultiConvert(object[] values, Type targetType, object parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }

    /// <summary>
    ///     Converts a <see cref="Binding" /> target value to the source binding values.
    /// </summary>
    /// <param name="value">The value that the <see cref="Binding" /> target produces.</param>
    /// <param name="targetTypes">
    ///     The array of types to convert to. The array length indicates the number and types of values
    ///     that are suggested for the method to return.
    /// </param>
    /// <param name="parameter">The converter parameter to use.</param>
    /// <param name="culture">The culture to use in the converter.</param>
    /// <returns>
    ///     An array of values that have been converted from the target value back to the source values.
    /// </returns>
    public virtual object[] MultiConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
    /// <summary>
    ///     Converts a value.
    /// </summary>
    /// <param name="value">The value.</param>
    /// <returns></returns>
    public object Convert(object value)
    {
        return Convert(value, null);
    }

    /// <summary>
    ///     Converts a value.
    /// </summary>
    /// <param name="value">The value.</param>
    /// <param name="parameter">The parameter.</param>
    /// <returns></returns>
    public object Convert(object value, object parameter)
    {
        return Convert(value, null, parameter, null);
    }

    /// <summary>
    ///     Converts a value.
    /// </summary>
    /// <param name="value">The value.</param>
    /// <returns></returns>
    public object ConvertBack(object value)
    {
        return ConvertBack(value, null);
    }

    /// <summary>
    ///     Converts a value.
    /// </summary>
    /// <param name="value">The value.</param>
    /// <param name="parameter">The parameter.</param>
    /// <returns></returns>
    public object ConvertBack(object value, object parameter)
    {
        return ConvertBack(value, null, parameter, null);
    }

    /// <summary>
    ///     Converts a set of values to a single value.
    /// </summary>
    /// <param name="values">The values.</param>
    /// <returns></returns>
    public object MultiConvert(object[] values)
    {
        return MultiConvert(values, null);
    }

    /// <summary>
    ///     Converts a set of values to a single value.
    /// </summary>
    /// <param name="values">The values.</param>
    /// <param name="parameter">The parameter.</param>
    /// <returns></returns>
    public object MultiConvert(object[] values, object parameter)
    {
        return MultiConvert(values, null, parameter, null);
    }

    /// <summary>
    ///     Converts a set of values to a single value.
    /// </summary>
    /// <param name="value">The value.</param>
    /// <returns></returns>
    public object[] MultiConvertBack(object value)
    {
        return MultiConvertBack(value, null);
    }

    /// <summary>
    ///     Converts a set of values to a single value.
    /// </summary>
    /// <param name="value">The value.</param>
    /// <param name="parameter">The parameter.</param>
    /// <returns></returns>
    public object[] MultiConvertBack(object value, object parameter)
    {
        return MultiConvertBack(value, null, parameter, null);
    }

    /// <summary>
    ///     Returns this instance.
    /// </summary>
    /// <param name="serviceProvider">Object that can provide services for the markup extension.</param>
    /// <returns>
    ///     This instance.
    /// </returns>
    public override sealed object ProvideValue(IServiceProvider serviceProvider)
    {
        return this;
    }
}
