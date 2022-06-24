using System.Collections;
using System.Windows.Data;
using Avalon.Windows.Internal.Utility;

namespace Avalon.Windows.Controls;

/// <summary>
/// Provides a host for <see cref="InlineModalDialog"/>s.
/// </summary>
[StyleTypedProperty(Property = "BlurrerStyle", StyleTargetType = typeof(Border))]
public class InlineModalDecorator : Decorator
{
    private const int DefaultChildIndex = 1;
    private const int BlurrerAnimationDurationMs = 500;

    private readonly Grid _panel;
    private readonly Border _blurrer;

    private bool _hasChild;
    /// <summary>
    /// Initializes a new instance of the <see cref="InlineModalDecorator"/> class.
    /// </summary>
    public InlineModalDecorator()
    {
        _panel = new Grid();
        AddVisualChild(_panel);
        AddLogicalChild(_panel);

        _blurrer = new Border { Visibility = Visibility.Collapsed };
        _ = _blurrer.SetBinding(StyleProperty, new Binding { Source = this, Path = new PropertyPath(BlurrerStyleProperty) });

        _ = _panel.Children.Add(_blurrer);
    }
    /// <summary>
    /// Identifies the <see cref="Target"/> dependency property.
    /// </summary>
    public static readonly DependencyProperty TargetProperty = DependencyProperty.Register(
        "Target", typeof(UIElement), typeof(InlineModalDecorator), new FrameworkPropertyMetadata(OnTargetChanged));

    private static void OnTargetChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        var oldValue = (UIElement)e.OldValue;
        if (oldValue != null)
        {
            InlineModalDialog.ClearModalDecorator(oldValue);
        }
        var newValue = (UIElement)e.NewValue;
        if (newValue != null)
        {
            InlineModalDialog.SetModalDecorator(newValue, (InlineModalDecorator)d);
        }
    }

    /// <summary>
    /// Gets or sets the dialog decorator target.
    /// This element will be marked as the root element under which inline dialogs can be used.
    /// </summary>
    public UIElement Target
    {
        get { return (UIElement)GetValue(TargetProperty); }
        set { SetValue(TargetProperty, value); }
    }

    /// <summary>
    /// Gets the current modal count.
    /// </summary>
    /// <value>The modal count.</value>
    public int ModalCount
    {
        get { return (int)GetValue(ModalCountProperty); }
        private set { SetValue(s_modalCountPropertyKey, value); }
    }

    private static readonly DependencyPropertyKey s_modalCountPropertyKey =
        DependencyProperty.RegisterReadOnly("ModalCount", typeof(int), typeof(InlineModalDecorator), new FrameworkPropertyMetadata(0));

    /// <summary>
    /// Identifies the <see cref="ModalCount"/> dependency property.
    /// </summary>
    public static readonly DependencyProperty ModalCountProperty = s_modalCountPropertyKey.DependencyProperty;

    /// <summary>
    /// Gets or sets the blurrer style.
    /// <remarks>
    /// The blurrer is a <see cref="Border"/>.
    /// </remarks>
    /// </summary>
    /// <value>The blurrer style.</value>
    public Style BlurrerStyle
    {
        get { return (Style)GetValue(BlurrerStyleProperty); }
        set { SetValue(BlurrerStyleProperty, value); }
    }

    /// <summary>
    /// Identifies the <see cref="BlurrerStyle"/> dependency property.
    /// </summary>
    public static readonly DependencyProperty BlurrerStyleProperty =
        DependencyProperty.Register("BlurrerStyle", typeof(Style), typeof(InlineModalDecorator), new FrameworkPropertyMetadata(GetDefaultBlurrerStyle()));

    private static Style GetDefaultBlurrerStyle()
    {
        var style = new Style(typeof(Border));
        style.Setters.Add(new Setter(Border.BackgroundProperty, Brushes.Black));
        style.Setters.Add(new Setter(OpacityProperty, 0.2D));
        style.Seal();
        return style;
    }
    /// <summary>
    /// Gets or sets the child element.
    /// </summary>
    public override UIElement Child
    {
        get
        {
            return _hasChild ? _panel.Children[DefaultChildIndex] : null;
        }
        set
        {
            if (_hasChild)
            {
                _panel.Children.RemoveAt(DefaultChildIndex);
            }

            if (value != null)
            {
                Panel.SetZIndex(value, -1);
                _panel.Children.Insert(DefaultChildIndex, value);
                _hasChild = true;
            }
            else
            {
                _hasChild = false;
            }
        }
    }

    /// <summary>
    /// Gets the logical children.
    /// </summary>
    protected override IEnumerator LogicalChildren
    {
        get { yield return _panel; }
    }

    /// <summary>
    /// Gets the visual children count.
    /// </summary>
    protected override int VisualChildrenCount
    {
        get { return 1; }
    }

    /// <summary>
    /// Gets the visual child at the specified index.
    /// </summary>
    /// <param name="index"></param>
    /// <returns></returns>
    protected override Visual GetVisualChild(int index)
    {
        if (index > 0)
        {
            throw new ArgumentOutOfRangeException(nameof(index));
        }
        return _panel;
    }

    /// <summary>
    /// Measures the element.
    /// </summary>
    /// <param name="constraint"></param>
    /// <returns></returns>
    protected override Size MeasureOverride(Size constraint)
    {
        _panel.Measure(constraint);
        return _panel.DesiredSize;
    }

    /// <summary>
    /// Arranges the element.
    /// </summary>
    /// <param name="arrangeSize"></param>
    /// <returns></returns>
    protected override Size ArrangeOverride(Size arrangeSize)
    {
        _panel.Arrange(new Rect(new Point(), arrangeSize));
        return _panel.RenderSize;
    }
    internal void AddModal(UIElement newElement)
    {
        AddModal(newElement, true);
    }

    internal void AddModal(UIElement newElement, bool showBlurrer)
    {
        int childCount = _panel.Children.Count;

        if (childCount > 1)
        {
            UIElement child = PeekChild();
            child.IsHitTestVisible = false;
        }

        if (childCount <= 2 && showBlurrer)
        {
            _blurrer.InvalidateArrange();
            Animator.AnimatePropertyFromTo(_blurrer, OpacityProperty, 0, null, BlurrerAnimationDurationMs);
            _blurrer.Visibility = Visibility.Visible;
        }

        _ = _panel.Children.Add(CreateDecorator(newElement));
        UpdateModalCount();
    }

    internal void RemoveModal(UIElement closingElement)
    {
        RemoveModal(closingElement, true);
    }

    internal void RemoveModal(UIElement closingElement, bool hideBlurrer)
    {
        if (_panel.Children.Count <= 1)
            return;

        if (PeekChild() is not Decorator decorator)
            return;

        var element = decorator.Child;
        if (!ReferenceEquals(element, closingElement))
            return;

        _panel.Children.Remove(decorator);
        int childCount = _panel.Children.Count;
        if (childCount > 0)
        {
            var child = PeekChild();
            child.IsHitTestVisible = true;

            if (childCount <= 2 && hideBlurrer)
            {
                Animator.AnimatePropertyFromTo(_blurrer, OpacityProperty, null, 0, BlurrerAnimationDurationMs, HideBlurrer);
            }
        }
        UpdateModalCount();
    }

    internal UIElement TopmostModal
    {
        get
        {
            return PeekChild() is Decorator decorator ? decorator.Child : null;
        }
    }
    private static Decorator CreateDecorator(UIElement element)
    {
        var decorator = new Decorator
        {
            Child = element
        };

        KeyboardNavigation.SetTabNavigation(decorator, KeyboardNavigationMode.Cycle);
        KeyboardNavigation.SetControlTabNavigation(decorator, KeyboardNavigationMode.Cycle);
        KeyboardNavigation.SetDirectionalNavigation(decorator, KeyboardNavigationMode.Cycle);

        return decorator;
    }

    private UIElement PeekChild()
    {
        return _panel.Children[^1];
    }

    private void UpdateModalCount()
    {
        ModalCount = _panel.Children.Count - 2;
    }

    private void HideBlurrer(object sender, EventArgs e)
    {
        _blurrer.Visibility = Visibility.Collapsed;
    }
}
