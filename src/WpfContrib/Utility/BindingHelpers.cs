using System.Collections.ObjectModel;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Markup;
using Avalon.Internal.Utility;
using Avalon.Windows.Converters;

namespace Avalon.Windows.Utility;

/// <summary>
/// Encapsulates methods relating to WPF bindings.
/// </summary>
public static class BindingHelpers
{
    private static readonly List<DependencyProperty> s_defaultProperties;
    /// <summary>
    ///     Initializes the <see cref="BindingHelpers" /> class.
    /// </summary>
    static BindingHelpers()
    {
        s_defaultProperties = new List<DependencyProperty> { TextBox.TextProperty, Selector.SelectedValueProperty };

        EventManager.RegisterClassHandler(typeof(PasswordBox), PasswordBox.PasswordChangedEvent,
            new RoutedEventHandler(OnPasswordBoxChanged), true);
    }
    /// <summary>
    ///     Updates the source according to the default property.
    /// </summary>
    /// <param name="o">The source.</param>
    public static void UpdateSourceDefaultProperty(this DependencyObject o)
    {
        ArgumentValidator.NotNull(o, "o");

        Type type = o.GetType();
        DependencyProperty prop = GetDefaultDependencyProperty(type);
        BindingExpression exp = BindingOperations.GetBindingExpression(o, prop);
        exp?.UpdateSource();
    }

    /// <summary>
    ///     Updates the source according to the specified property.
    /// </summary>
    /// <param name="o">The source.</param>
    /// <param name="prop">The property.</param>
    public static void UpdateSourceProperty(this DependencyObject o, DependencyProperty prop)
    {
        ArgumentValidator.NotNull(o, "o");
        ArgumentValidator.NotNull(prop, "prop");

        BindingExpressionBase exp = BindingOperations.GetBindingExpressionBase(o, prop);
        exp?.UpdateSource();
    }

    /// <summary>
    ///     Updates the target of the specified bound property.
    /// </summary>
    /// <param name="o">The o.</param>
    /// <param name="prop">The prop.</param>
    public static void UpdateTarget(this DependencyObject o, DependencyProperty prop)
    {
        ArgumentValidator.NotNull(o, "o");
        ArgumentValidator.NotNull(prop, "prop");

        BindingExpressionBase exp = BindingOperations.GetBindingExpressionBase(o, prop);
        exp?.UpdateTarget();
    }

    /// <summary>
    ///     Determines whether the specified <see cref="DependencyObject" /> has an error.
    /// </summary>
    /// <param name="o">The source.</param>
    /// <returns>
    ///     <c>true</c> if the specified o has error; otherwise, <c>false</c>.
    /// </returns>
    public static bool HasError(this DependencyObject o)
    {
        ArgumentValidator.NotNull(o, "o");

        Type type = o.GetType();
        DependencyProperty prop = GetDefaultDependencyProperty(type);

        return HasError(o, prop);
    }

    /// <summary>
    ///     Determines whether the specified <see cref="DependencyObject" /> has an error
    ///     for the specified <see cref="DependencyProperty" />.
    /// </summary>
    /// <param name="o">The source.</param>
    /// <param name="p">The property.</param>
    /// <returns>
    ///     <c>true</c> if the specified o has error; otherwise, <c>false</c>.
    /// </returns>
    public static bool HasError(this DependencyObject o, DependencyProperty p)
    {
        var exp = BindingOperations.GetBindingExpression(o, p);
        return exp?.HasError == true;
    }

    /// <summary>
    ///     Gets the all the data validation errors under a given root element.
    /// </summary>
    /// <param name="root">The root.</param>
    /// <param name="markInvalid">If set to <c>true</c> ensures the error template appears.</param>
    /// <returns></returns>
    public static ReadOnlyCollection<ValidationError> GetErrors(this DependencyObject root, bool markInvalid)
    {
        ArgumentValidator.NotNull(root, "root");

        List<ValidationError> errors = new();

        // this will traverse the entire descendant tree since the predicate always returns false
        _ = root.FindVisualDescendant((DependencyObject o) =>
        {
            errors.AddRange(Validation.GetErrors(o));
            return false;
        });

        if (markInvalid)
        {
            foreach (ValidationError error in errors)
            {
                // causes the Validation.ErrorTemplate to appear
                Validation.MarkInvalid(
                    (BindingExpressionBase)error.BindingInError, error);
            }
        }

        return errors.AsReadOnly();
    }

    /// <summary>
    ///     Gets the default <see cref="DependencyProperty" />.
    /// </summary>
    /// <param name="type">The type.</param>
    /// <returns></returns>
    public static DependencyProperty GetDefaultDependencyProperty(Type type)
    {
        ArgumentValidator.NotNull(type, "type");

        var prop = s_defaultProperties.Find(defaultProp => defaultProp.OwnerType.IsAssignableFrom(type));

        if (prop == null)
        {
            string propertyName = GetDefaultPropertyName(type);
            if (propertyName != null)
            {
                prop = DependencyHelpers.GetDependencyProperty(type, GetDefaultPropertyName(type));
            }
        }

        return prop;
    }

    /// <summary>
    ///     Gets the default property name.
    /// </summary>
    /// <param name="type">The type.</param>
    /// <returns>
    ///     The property name if it exists; otherwise <c>null</c>.
    /// </returns>
    public static string GetDefaultPropertyName(Type type)
    {
        ArgumentValidator.NotNull(type, "type");

        object[] attrs = type.GetCustomAttributes(false);

        foreach (object attr in attrs)
        {
            if (attr is ContentPropertyAttribute contentAttribute)
            {
                return contentAttribute.Name;
            }

            if (attr is DefaultPropertyAttribute defaultAttribute)
            {
                return defaultAttribute.Name;
            }
        }

        return null;
    }

    /// <summary>
    ///     Evaluates the property path for the specified object and returns its value.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="source">The source.</param>
    /// <param name="path">The path.</param>
    /// <returns></returns>
    public static T Eval<T>(object source, string path)
    {
        ArgumentValidator.NotNull(path, "path");

        if (source == null)
        {
            return default;
        }

        var binding = new Binding(path) { Source = source };

        return Eval<T>(binding);
    }

    /// <summary>
    ///     Evaluates the binding returns a value.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="binding">The binding.</param>
    /// <returns></returns>
    public static T Eval<T>(this BindingBase binding)
    {
        ArgumentValidator.NotNull(binding, "binding");

        var helper = new EvalHelper();

        _ = BindingOperations.SetBinding(helper, EvalHelper.ValueProperty, binding);

        T result = (T)helper.GetValue(EvalHelper.ValueProperty);

        BindingOperations.ClearBinding(helper, EvalHelper.ValueProperty);

        return result;
    }
    // Use these properties to circumvent places properties that were not defined as Dependency Properties,
    // and thus are not data-bindable.
    private static readonly HashSet<DependencyObject> s_passwordLocks = new();

    /// <summary>
    ///     Gets the password.
    /// </summary>
    /// <param name="obj"></param>
    /// <returns></returns>
    public static string GetPassword(DependencyObject obj)
    {
        return (string)obj.GetValue(PasswordProperty);
    }

    /// <summary>
    ///     Sets the password.
    ///     <remarks>
    ///         Using this workaround compromises the security implemented in <see cref="PasswordBox" />.
    ///     </remarks>
    /// </summary>
    /// <param name="obj"></param>
    /// <param name="value"></param>
    public static void SetPassword(DependencyObject obj, string value)
    {
        obj.SetValue(PasswordProperty, value);
    }

    /// <summary>
    ///     Identifies the <c>Password</c> attached dependency property.
    /// </summary>
    public static readonly DependencyProperty PasswordProperty =
        DependencyProperty.RegisterAttached("Password", typeof(string), typeof(BindingHelpers),
            new FrameworkPropertyMetadata(string.Empty,
                FrameworkPropertyMetadataOptions.BindsTwoWayByDefault | FrameworkPropertyMetadataOptions.Journal,
                OnPasswordChanged));

    private static void OnPasswordChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
    {
        if (o is PasswordBox passwordBox)
        {
            if (s_passwordLocks.Contains(passwordBox))
                return;

            _ = s_passwordLocks.Add(passwordBox);
            passwordBox.Password = (string)e.NewValue;
            _ = s_passwordLocks.Remove(passwordBox);
        }
    }

    private static void OnPasswordBoxChanged(object sender, RoutedEventArgs e)
    {
        if (sender is PasswordBox passwordBox)
        {
            if (s_passwordLocks.Contains(passwordBox))
                return;

            _ = s_passwordLocks.Add(passwordBox);
            SetPassword(passwordBox, passwordBox.Password);
            _ = s_passwordLocks.Remove(passwordBox);
        }
    }
    /// <summary>
    ///     Gets the inlines.
    ///     <remarks>
    ///         Important: if the inlines in the TextBlock are changed after this property is set,
    ///         the get will not reflect these changes.
    ///     </remarks>
    /// </summary>
    /// <param name="obj"></param>
    /// <returns></returns>
    [TypeConverter(typeof(FormattedTextTypeConverter))]
    public static IEnumerable<Inline> GetInlines(DependencyObject obj)
    {
        return (IEnumerable<Inline>)obj.GetValue(InlinesProperty);
    }

    /// <summary>
    ///     Sets the inlines.
    /// </summary>
    /// <param name="obj"></param>
    /// <param name="value"></param>
    [TypeConverter(typeof(FormattedTextTypeConverter))]
    public static void SetInlines(DependencyObject obj, IEnumerable<Inline> value)
    {
        obj.SetValue(InlinesProperty, value);
    }

    /// <summary>
    ///     Identifies the <c>Inlines</c> attached dependency property.
    /// </summary>
    public static readonly DependencyProperty InlinesProperty =
        DependencyProperty.RegisterAttached("Inlines", typeof(IEnumerable<Inline>), typeof(BindingHelpers),
            new FrameworkPropertyMetadata(OnInlinesChanged));

    private static void OnInlinesChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
    {
        if (e.NewValue != null)
        {
            if (o is TextBlock textBlock)
            {
                textBlock.Inlines.AddRange((IEnumerable<Inline>)e.NewValue);
            }
        }
    }
    /// <summary>
    /// Identifies the <c>WindowStartupLocation</c> attached dependency property.
    /// </summary>
    public static readonly DependencyProperty WindowStartupLocationProperty = DependencyProperty.RegisterAttached(
        "WindowStartupLocation", typeof(WindowStartupLocation), typeof(BindingHelpers), new FrameworkPropertyMetadata(OnWindowStartupLocationChanged));

    private static void OnWindowStartupLocationChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        ((Window)d).WindowStartupLocation = (WindowStartupLocation)e.NewValue;
    }

    /// <summary>
    /// Sets the window startup location.
    /// </summary>
    /// <param name="window">The window.</param>
    /// <param name="value">The value.</param>
    public static void SetWindowStartupLocation(Window window, WindowStartupLocation value)
    {
        window.SetValue(WindowStartupLocationProperty, value);
    }

    /// <summary>
    /// Gets the window startup location.
    /// </summary>
    /// <param name="window">The window.</param>
    /// <returns></returns>
    public static WindowStartupLocation GetWindowStartupLocation(Window window)
    {
        return (WindowStartupLocation)window.GetValue(WindowStartupLocationProperty);
    }
    /// <summary>
    ///     Used internally in the Eval method.
    /// </summary>
    private class EvalHelper : DependencyObject
    {
        public static readonly DependencyProperty ValueProperty =
            DependencyProperty.Register("Value", typeof(object), typeof(EvalHelper));
    }
}
