namespace Avalon.Windows.Controls;

/// <summary>
///     Contains a list of selectable items, wrapped in the specified container type.
/// </summary>
public class TypedListBox : ListBox
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
        TypedItemsControl.ItemContainerTypeProperty.AddOwner(typeof(TypedListBox));
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
