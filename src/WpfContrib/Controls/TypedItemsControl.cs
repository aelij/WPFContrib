namespace Avalon.Windows.Controls;

/// <summary>
///     Represents a control that can be used to present a collection of items,
///     wrapped in the specified container type.
/// </summary>
public class TypedItemsControl : ItemsControl
{
    /// <summary>
    ///     Gets or sets the type of the item container.
    /// </summary>
    /// <value>The type of the item container.</value>
    public Type ItemContainerType
    {
        get { return (Type)GetValue(ItemContainerTypeProperty); }
        set { SetValue(ItemContainerTypeProperty, value); }
    }

    /// <summary>
    ///     Identifies the <see cref="ItemContainerType" /> dependency property.
    /// </summary>
    public static readonly DependencyProperty ItemContainerTypeProperty =
        DependencyProperty.Register("ItemContainerType", typeof(Type), typeof(TypedItemsControl),
            new FrameworkPropertyMetadata(), ValidateItemContainerType);

    /// <summary>
    ///     Validates the type of the item container.
    /// </summary>
    /// <param name="o">The object to validate.</param>
    /// <returns></returns>
    private static bool ValidateItemContainerType(object o)
    {
        var type = (Type)o;

        return type == null ||
               (type.GetConstructor(Type.EmptyTypes) != null &&
                typeof(DependencyObject).IsAssignableFrom(type));
    }
    /// <summary>
    ///     Creates or identifies the element that is used to display the given item.
    /// </summary>
    /// <returns>
    ///     The element that is used to display the given item.
    /// </returns>
    protected override DependencyObject GetContainerForItemOverride()
    {
        Type type = ItemContainerType;
        if (type == null)
        {
            return base.GetContainerForItemOverride();
        }

        return (DependencyObject)Activator.CreateInstance(type);
    }

    /// <summary>
    ///     Determines if the specified item is (or is eligible to be) its own container.
    /// </summary>
    /// <param name="item">The item to check.</param>
    /// <returns>
    ///     true if the item is (or is eligible to be) its own container; otherwise, false.
    /// </returns>
    protected override bool IsItemItsOwnContainerOverride(object item)
    {
        Type type = ItemContainerType;
        if (type == null || item == null)
        {
            return base.IsItemItsOwnContainerOverride(item);
        }

        return type.IsInstanceOfType(item);
    }
}
