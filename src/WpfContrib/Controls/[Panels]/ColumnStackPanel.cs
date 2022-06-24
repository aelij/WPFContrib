namespace Avalon.Windows.Controls;

/// <summary>
///     Stacks elements in columns.
/// </summary>
public class ColumnStackPanel : Panel
{
    /// <summary>
    ///     Gets or sets the column count.
    /// </summary>
    /// <value>The column count.</value>
    public int ColumnCount
    {
        get { return (int)GetValue(ColumnCountProperty); }
        set { SetValue(ColumnCountProperty, value); }
    }

    /// <summary>
    ///     Identifies the <see cref="ColumnCount" /> dependency property.
    /// </summary>
    public static readonly DependencyProperty ColumnCountProperty =
        DependencyProperty.Register("ColumnCount", typeof(int), typeof(ColumnStackPanel),
            new FrameworkPropertyMetadata(1, FrameworkPropertyMetadataOptions.AffectsMeasure, null,
                CoerceColumnCount));

    private static object CoerceColumnCount(DependencyObject o, object baseValue)
    {
        var value = (int)baseValue;

        if (value < 1)
        {
            value = 1;
        }

        return value;
    }
    /// <summary>
    ///     Gets or sets a value that indicates the dimension by which child elements are stacked.
    /// </summary>
    /// <value>The orientation of child elements.</value>
    public Orientation Orientation
    {
        get { return (Orientation)GetValue(OrientationProperty); }
        set { SetValue(OrientationProperty, value); }
    }

    /// <summary>
    ///     Identifies the <see cref="Orientation" /> dependency property.
    /// </summary>
    public static readonly DependencyProperty OrientationProperty =
        StackPanel.OrientationProperty.AddOwner(typeof(ColumnStackPanel));
    /// <summary>
    ///     Gets the start column.
    /// </summary>
    /// <param name="obj">The obj.</param>
    /// <returns></returns>
    public static int GetStartColumn(DependencyObject obj)
    {
        return (int)obj.GetValue(StartColumnProperty);
    }

    /// <summary>
    ///     Sets the start column.
    /// </summary>
    /// <param name="obj">The object.</param>
    /// <param name="value">The value.</param>
    public static void SetStartColumn(DependencyObject obj, int value)
    {
        obj.SetValue(StartColumnProperty, value);
    }

    /// <summary>
    ///     Identifies the <c>StartColumn</c> dependency property.
    /// </summary>
    public static readonly DependencyProperty StartColumnProperty =
        DependencyProperty.RegisterAttached("StartColumn", typeof(int), typeof(ColumnStackPanel),
            new FrameworkPropertyMetadata(0,
                FrameworkPropertyMetadataOptions.AffectsMeasure |
                FrameworkPropertyMetadataOptions.AffectsParentMeasure, OnStartColumnChanged, CoerceStartColumn));

    private static void OnStartColumnChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
    {
        o.CoerceValue(EndColumnProperty);
    }

    private static object CoerceStartColumn(DependencyObject o, object baseValue)
    {
        var value = (int)baseValue;

        if (VisualTreeHelper.GetParent(o) is ColumnStackPanel panel)
        {
            int lastColumn = panel.ColumnCount - 1;

            if (value < 0)
            {
                value = 0;
            }
            else if (value > lastColumn)
            {
                value = lastColumn;
            }
        }

        return value;
    }
    /// <summary>
    ///     Gets the end column.
    /// </summary>
    /// <param name="obj">The obj.</param>
    /// <returns></returns>
    public static int GetEndColumn(DependencyObject obj)
    {
        return (int)obj.GetValue(EndColumnProperty);
    }

    /// <summary>
    ///     Sets the end column.
    /// </summary>
    /// <param name="obj">The obj.</param>
    /// <param name="value">The value.</param>
    public static void SetEndColumn(DependencyObject obj, int value)
    {
        obj.SetValue(EndColumnProperty, value);
    }

    /// <summary>
    ///     Identifies the <c>EndColumn</c> dependency property.
    /// </summary>
    public static readonly DependencyProperty EndColumnProperty =
        DependencyProperty.RegisterAttached("EndColumn", typeof(int), typeof(ColumnStackPanel),
            new FrameworkPropertyMetadata(0,
                FrameworkPropertyMetadataOptions.AffectsMeasure |
                FrameworkPropertyMetadataOptions.AffectsParentMeasure, null, CoerceEndColumn));

    private static object CoerceEndColumn(DependencyObject o, object baseValue)
    {
        var value = (int)baseValue;

        if (VisualTreeHelper.GetParent(o) is ColumnStackPanel panel)
        {
            int lastColumn = panel.ColumnCount - 1;
            int startColumn = GetStartColumn(o);

            if (value < startColumn)
            {
                value = startColumn;
            }
            else if (value > lastColumn)
            {
                value = lastColumn;
            }
        }

        return value;
    }
    /// <summary>
    ///     Measures the size in layout required for child elements.
    /// </summary>
    /// <param name="availableSize"></param>
    /// <returns></returns>
    protected override Size MeasureOverride(Size availableSize)
    {
        bool isVertical = Orientation == Orientation.Vertical;

        var childConstraint = new Size(double.PositiveInfinity, double.PositiveInfinity);

        int columnCount = ColumnCount;
        double columnSize = (isVertical ? availableSize.Width : availableSize.Height) / columnCount;

        var widths = new double[columnCount];
        var heights = new double[columnCount];

        foreach (UIElement child in InternalChildren)
        {
            if (child != null)
            {
                int startColumn = GetStartColumn(child);
                int endColumn = GetEndColumn(child);
                int span = endColumn - startColumn + 1;

                if (isVertical)
                {
                    childConstraint.Width = columnSize * span;
                }
                else
                {
                    childConstraint.Height = columnSize * span;
                }
                child.Measure(childConstraint);
                Size desiredChildSize = child.DesiredSize;

                for (int column = startColumn; column <= endColumn; ++column)
                {
                    double desiredWidth = isVertical ? desiredChildSize.Width : desiredChildSize.Height;
                    if (widths[column] < desiredWidth)
                    {
                        widths[column] = desiredWidth;
                    }

                    heights[column] += isVertical ? desiredChildSize.Height : desiredChildSize.Width;
                }
            }
        }

        double maxWidth = widths.Sum();
        double maxHeight = heights.Max();
        return isVertical ? new Size(maxWidth, maxHeight) : new Size(maxHeight, maxWidth);
    }

    /// <summary>
    ///     Positions child elements and determines a size.
    /// </summary>
    /// <param name="finalSize"></param>
    /// <returns></returns>
    protected override Size ArrangeOverride(Size finalSize)
    {
        bool isVertical = Orientation == Orientation.Vertical;

        int columnCount = ColumnCount;
        double columnSize = (isVertical ? finalSize.Width : finalSize.Height) / columnCount;

        var heights = new double[columnCount];

        foreach (UIElement child in InternalChildren)
        {
            if (child != null)
            {
                int startColumn = GetStartColumn(child);
                int endColumn = GetEndColumn(child);
                int span = endColumn - startColumn + 1;

                double size = isVertical ? child.DesiredSize.Height : child.DesiredSize.Width;

                double y = heights.Skip(startColumn).Take(span).Max();

                var rect = isVertical
                    ? new Rect(columnSize * startColumn, y, columnSize * span, size)
                    : new Rect(y, columnSize * startColumn, size, columnSize * span);

                child.Arrange(rect);

                for (int column = startColumn; column <= endColumn; ++column)
                {
                    heights[column] = size + y;
                }
            }
        }
        return finalSize;
    }
}
