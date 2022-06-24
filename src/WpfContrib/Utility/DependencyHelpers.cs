using System.Reflection;
using Avalon.Internal.Utility;

namespace Avalon.Windows.Utility;

/// <summary>
///     Encapsulates methods for dealing with dependency objects and properties.
/// </summary>
public static class DependencyHelpers
{
    /// <summary>
    ///     Gets the dependency property according to its name.
    /// </summary>
    /// <param name="type">The type.</param>
    /// <param name="propertyName">Name of the property.</param>
    /// <returns></returns>
    public static DependencyProperty GetDependencyProperty(Type type, string propertyName)
    {
        DependencyProperty prop = null;

        if (type != null)
        {
            FieldInfo fieldInfo = type.GetField(propertyName + "Property",
                BindingFlags.Static | BindingFlags.Public);

            if (fieldInfo != null)
            {
                prop = fieldInfo.GetValue(null) as DependencyProperty;
            }
        }

        return prop;
    }

    /// <summary>
    ///     Retrieves a <see cref="DependencyProperty" /> using reflection.
    /// </summary>
    /// <param name="o"></param>
    /// <param name="propertyName"></param>
    /// <returns></returns>
    public static DependencyProperty GetDependencyProperty(this DependencyObject o, string propertyName)
    {
        DependencyProperty prop = null;

        if (o != null)
        {
            prop = GetDependencyProperty(o.GetType(), propertyName);
        }

        return prop;
    }

    /// <summary>
    ///     Sets the value of the <paramref name="property" /> only if it hasn't been explicitely set.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="o">The object.</param>
    /// <param name="property">The property.</param>
    /// <param name="value">The value.</param>
    /// <returns></returns>
    public static bool SetIfDefault<T>(this DependencyObject o, DependencyProperty property, T value)
    {
        ArgumentValidator.NotNull(o, "o");
        ArgumentValidator.NotNull(property, "property");
        if (!property.PropertyType.IsAssignableFrom(typeof(T)))
        {
            throw new ArgumentException(SR.DependencyHelpers_IncompatibleDPType);
        }

        if (DependencyPropertyHelper.GetValueSource(o, property).BaseValueSource == BaseValueSource.Default)
        {
            o.SetValue(property, value);

            return true;
        }

        return false;
    }
}