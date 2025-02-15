using System.Collections;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Reflection;
using System.Windows.Media.Effects;
using System.Windows.Shapes;
using Avalon.Windows.Utility;

namespace Avalon.Windows.Controls;

/// <summary>
/// Represents a behavior that allows moving items in an <see cref="ItemsControl" /> by dragging them.
/// <remarks>For this behavior to work, the <see cref="P:ItemsControl.ItemsPanel" /> must not be a <see cref="VirtualizingPanel" />,
/// and the <see cref="P:ItemsControl.ItemsSource" /> must be an <see cref="ObservableCollection{T}" />.</remarks>
/// </summary>
public class ListItemMoveBehavior : IDisposable
{
    private static readonly DependencyPropertyDescriptor s_itemsSourcePropertyDescriptor = DependencyPropertyDescriptor.FromProperty(ItemsControl.ItemsSourceProperty, typeof(ItemsControl));
    private static readonly DependencyPropertyDescriptor s_itemsPanelPropertyDescriptor = DependencyPropertyDescriptor.FromProperty(ItemsControl.ItemsPanelProperty, typeof(ItemsControl));

    private readonly ItemsControl _itemsControl;

    private FrameworkElement _element;
    private Panel _itemsPanel;
    private ScrollViewer _scrollViewer;
    private IList _collection;
    private MethodInfo _moveMethod;
    private bool _isEnabled;
    private Point _positionDelta;
    private PreviewAdorner _previewAdorner;
    private DateTime _lastMove;
    private int _startIndex;
    private int _lastIndex; private ListItemMoveBehavior(ItemsControl itemsControl)
    {
        _itemsControl = itemsControl;
        s_itemsSourcePropertyDescriptor.AddValueChanged(_itemsControl, OnItemsSourceChanged);
        s_itemsPanelPropertyDescriptor.AddValueChanged(_itemsControl, OnItemsPanelChanged);
        InitializeCollection();
        _itemsControl.PreviewMouseDown += ElementOnPreviewMouseDown;
    }

    private void OnItemsPanelChanged(object sender, EventArgs e)
    {
        _itemsPanel = null;
    }

    private ScrollViewer ScrollViewer
    {
        get
        {
            return _scrollViewer ??= _itemsControl.FindVisualDescendantByType<ScrollViewer>();
        }
    }

    private bool IsEnabled
    {
        get { return _isEnabled && ItemsPanel != null && ItemsPanel is not VirtualizingPanel; }
    }

    private Panel ItemsPanel
    {
        get
        {
            return _itemsPanel ??= _itemsControl.FindVisualDescendant(t => t is Panel p && p.IsItemsHost) as Panel;
        }
    }

    private void OnItemsSourceChanged(object sender, EventArgs e)
    {
        InitializeCollection();
    }

    private void InitializeCollection()
    {
        _collection = _itemsControl.ItemsSource as IList;
        if (_collection == null)
        {
            if (_itemsControl.ItemsSource is ICollectionView collectionView)
            {
                _collection = collectionView.SourceCollection as IList;
            }
        }
        if (_collection != null && IsObservableCollection(_collection))
        {
            _moveMethod = _collection.GetType().GetMethod("Move", new[] { typeof(int), typeof(int) });
            _isEnabled = true;
        }
        else
        {
            _isEnabled = true;
        }
        if (_collection == null)
            _isEnabled = false;
    }

    /// <summary>
    /// Performs event clean-up.
    /// </summary>
    public void Dispose()
    {
        s_itemsSourcePropertyDescriptor.RemoveValueChanged(_itemsControl, OnItemsSourceChanged);
        s_itemsPanelPropertyDescriptor.RemoveValueChanged(_itemsControl, OnItemsPanelChanged);
        _element.PreviewMouseDown -= ElementOnPreviewMouseDown;
    }
    private void ElementOnPreviewMouseDown(object sender, MouseEventArgs e)
    {
        if (!IsEnabled)
            return;
        _element = null;
        if (e.OriginalSource is UIElement element)
        {
            if (!GetIsElementDraggable(element))
                return;
            _element = _itemsControl.ContainerFromElement(element) as FrameworkElement;
        }
        if (_element == null)
            return;

        var isCaptured = _element.CaptureMouse();
        if (isCaptured)
        {
            e.Handled = true;

            _element.MouseMove += ElementOnMouseMove;
            _element.MouseUp += ElementOnMouseUp;
            _element.LostMouseCapture += ElementOnLostMouseCapture;

            var sourceItem = _element.DataContext;
            _startIndex = _collection.IndexOf(sourceItem);
            _lastIndex = _startIndex;

            var adornerLayer = AdornerLayer.GetAdornerLayer(_itemsControl);
            if (adornerLayer != null)
            {
                _element.SetCurrentValue(UIElement.OpacityProperty, 0.0);
                var firstChild = (FrameworkElement)VisualTreeHelper.GetChild(_element, 0);
                var top = e.GetPosition(_itemsControl).Y;
                _positionDelta = e.GetPosition(_element);
                _previewAdorner = new PreviewAdorner(_itemsControl, firstChild)
                {
                    Top = top - _positionDelta.Y
                };
                adornerLayer.Add(_previewAdorner);
            }
        }
    }

    private void ElementOnMouseMove(object sender, MouseEventArgs e)
    {
        var now = DateTime.Now;
        var sourceItem = _element.DataContext;
        var sourceIndex = _collection.IndexOf(sourceItem);
        if (sourceIndex < 0)
            return;

        var position = e.GetPosition(_itemsControl);

        if (_previewAdorner != null)
        {
            _previewAdorner.Top = position.Y - _positionDelta.Y;
            Debug.WriteLine(_previewAdorner.Top);
        }

        if (now - _lastMove > GetAnimationDuration(_itemsControl))
        {
            if (_itemsControl.InputHitTest(new Point(_positionDelta.X, position.Y)) is FrameworkElement hitTestElement)
            {
                if (_itemsControl.ContainerFromElement(hitTestElement) is FrameworkElement element)
                {
                    var item = element.DataContext;
                    _lastIndex = _collection.IndexOf(item);
                    if (sourceIndex != _lastIndex)
                    {
                        if (_moveMethod != null)
                        {
                            _ = _moveMethod.Invoke(_collection, new object[] { sourceIndex, _lastIndex });
                        }
                        else
                        {
                            var oldItem = _collection[sourceIndex];
                            _collection.RemoveAt(sourceIndex);
                            _collection.Insert(_lastIndex, oldItem);
                        }
                        _lastMove = now;
                    }
                }
            }

            Scroll(position);
        }
    }

    private void ElementOnMouseUp(object sender, MouseButtonEventArgs e)
    {
        _element.ReleaseMouseCapture();
    }

    private void ElementOnLostMouseCapture(object sender, MouseEventArgs mouseEventArgs)
    {
        _element.MouseMove -= ElementOnMouseMove;
        _element.MouseUp -= ElementOnMouseUp;
        _element.LostMouseCapture -= ElementOnLostMouseCapture;

        if (_previewAdorner != null)
        {
            var relativePoint = _element.TranslatePoint(new Point(), _itemsControl);

            var topAnimation = new DoubleAnimation { To = relativePoint.Y, Duration = GetAnimationDuration(_itemsControl), EasingFunction = new PowerEase() };
            topAnimation.Completed +=
                (o, args) =>
                {
                    AdornerLayer.GetAdornerLayer(_itemsControl).Remove(_previewAdorner);
                    _previewAdorner = null;
                    _element.InvalidateProperty(UIElement.OpacityProperty);
                    _element = null;
                };
            _previewAdorner.BeginAnimation(PreviewAdorner.TopProperty, topAnimation);
        }

        if (_startIndex != _lastIndex)
        {
            _itemsControl.RaiseEvent(new RoutedPropertyChangedEventArgs<int>(_startIndex, _lastIndex, ReorderCompletedEvent));
        }
    }
    private static bool IsObservableCollection(object collection)
    {
        var type = collection.GetType();
        return type.IsGenericType && type.GetGenericTypeDefinition() == typeof(ObservableCollection<>);
    }

    private void Scroll(Point position)
    {
        if (ScrollViewer != null)
        {
            double scrollMargin = Math.Min(ScrollViewer.FontSize * 2.0, ScrollViewer.ActualHeight / 2.0);
            if ((position.X >= (ScrollViewer.ActualWidth - scrollMargin)) &&
                (ScrollViewer.HorizontalOffset < (ScrollViewer.ExtentWidth - ScrollViewer.ViewportWidth)))
            {
                ScrollViewer.LineRight();
            }
            else if ((position.X < scrollMargin) && (ScrollViewer.HorizontalOffset > 0.0))
            {
                ScrollViewer.LineLeft();
            }
            else if ((position.Y >= (ScrollViewer.ActualHeight - scrollMargin)) &&
                     (ScrollViewer.VerticalOffset < (ScrollViewer.ExtentHeight - ScrollViewer.ViewportHeight)))
            {
                ScrollViewer.LineDown();
            }
            else if ((position.Y < scrollMargin) && (ScrollViewer.VerticalOffset > 0.0))
            {
                ScrollViewer.LineUp();
            }
        }
    }
    /// <summary>
    /// Gets a value indicating whether the behavior is enabled.
    /// </summary>
    /// <param name="obj">The object.</param>
    /// <returns></returns>
    public static bool GetIsEnabled(ItemsControl obj)
    {
        return (bool)obj.GetValue(IsEnabledProperty);
    }

    /// <summary>
    /// Sets a value indicating whether the behavior is enabled.
    /// </summary>
    /// <param name="obj">The object.</param>
    /// <param name="value">if set to <c>true</c>, enables the behavior.</param>
    public static void SetIsEnabled(ItemsControl obj, bool value)
    {
        obj.SetValue(IsEnabledProperty, value);
    }

    /// <summary>
    /// Identifies the <c>IsEnabled</c> attached dependency property.
    /// </summary>
    public static readonly DependencyProperty IsEnabledProperty =
        DependencyProperty.RegisterAttached("IsEnabled", typeof(bool), typeof(ListItemMoveBehavior),
                                            new FrameworkPropertyMetadata(OnIsEnabledChanged));

    private static void OnIsEnabledChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        if ((bool)e.NewValue)
        {
            var element = (ItemsControl)d;
            var behavior = new ListItemMoveBehavior(element);
            SetBehavior(element, behavior);
        }
        else
        {
            GetBehavior(d).Dispose();
            d.ClearValue(s_behaviorProperty);
        }
    }

    /// <summary>
    /// Gets a value indicating whether the element can be used for dragging.
    /// </summary>
    /// <param name="obj">The object.</param>
    /// <returns></returns>
    public static bool GetIsElementDraggable(UIElement obj)
    {
        return (bool)obj.GetValue(IsElementDraggableProperty);
    }

    /// <summary>
    /// Sets a value indicating whether the element can be used for dragging.
    /// </summary>
    /// <param name="obj">The object.</param>
    /// <param name="value">if set to <c>true</c>, enables the behavior.</param>
    public static void SetIsElementDraggable(UIElement obj, bool value)
    {
        obj.SetValue(IsElementDraggableProperty, value);
    }

    /// <summary>
    /// Identifies the <c>IsElementDraggable</c> attached dependency property.
    /// </summary>
    public static readonly DependencyProperty IsElementDraggableProperty =
        DependencyProperty.RegisterAttached("IsElementDraggable", typeof(bool), typeof(ListItemMoveBehavior),
                                            new FrameworkPropertyMetadata(false, FrameworkPropertyMetadataOptions.Inherits));

    private static ListItemMoveBehavior GetBehavior(DependencyObject obj)
    {
        return (ListItemMoveBehavior)obj.GetValue(s_behaviorProperty);
    }

    private static void SetBehavior(DependencyObject obj, ListItemMoveBehavior value)
    {
        obj.SetValue(s_behaviorProperty, value);
    }

    private static readonly DependencyProperty s_behaviorProperty =
        DependencyProperty.RegisterAttached("Behavior", typeof(ListItemMoveBehavior), typeof(ListItemMoveBehavior),
                                            new FrameworkPropertyMetadata());

    /// <summary>
    /// Identifies the <c>AnimationDuration</c> attached dependency property.
    /// </summary>
    public static readonly DependencyProperty AnimationDurationProperty = DependencyProperty.RegisterAttached(
        "AnimationDuration", typeof(TimeSpan), typeof(ListItemMoveBehavior), new FrameworkPropertyMetadata(TimeSpan.FromSeconds(0.1)));

    /// <summary>
    /// Gets the duration of the animation.
    /// </summary>
    /// <param name="itemsControl">The items control.</param>
    /// <returns></returns>
    public static TimeSpan GetAnimationDuration(ItemsControl itemsControl)
    {
        return (TimeSpan)itemsControl.GetValue(AnimationDurationProperty);
    }

    /// <summary>
    /// Sets the duration of the animation.
    /// </summary>
    /// <param name="itemsControl">The items control.</param>
    /// <param name="value">The value.</param>
    public static void SetAnimationDuration(ItemsControl itemsControl, TimeSpan value)
    {
        itemsControl.SetValue(AnimationDurationProperty, value);
    }
    /// <summary>
    /// Identifies the <c>ReorderCompleted</c> attached event.
    /// </summary>
    public static readonly RoutedEvent ReorderCompletedEvent = EventManager.RegisterRoutedEvent("ReorderCompleted", RoutingStrategy.Bubble, typeof(RoutedPropertyChangedEventHandler<int>), typeof(ListItemMoveBehavior));

    /// <summary>
    /// Adds the reorder completed handler.
    /// </summary>
    /// <param name="element">The element.</param>
    /// <param name="handler">The handler.</param>
    public static void AddReorderCompletedHandler(FrameworkElement element,
                                                  RoutedPropertyChangedEventHandler<int> handler)
    {
        element.AddHandler(ReorderCompletedEvent, handler);
    }

    /// <summary>
    /// Removes the reorder completed handler.
    /// </summary>
    /// <param name="element">The element.</param>
    /// <param name="handler">The handler.</param>
    public static void RemoveReorderCompletedHandler(FrameworkElement element,
                                                     RoutedPropertyChangedEventHandler<int> handler)
    {
        element.RemoveHandler(ReorderCompletedEvent, handler);
    }
    private class PreviewAdorner : Adorner
    {
        private readonly UIElement _itemElement;
        private readonly Rectangle _rectangle;

        public PreviewAdorner(UIElement adornedElement, UIElement itemElement)
            : base(adornedElement)
        {
            IsHitTestVisible = true;
            Opacity = 0.75;
            _itemElement = itemElement;
            _rectangle = new Rectangle
            {
                Fill = new VisualBrush(itemElement),
                Effect = new DropShadowEffect { ShadowDepth = 0, BlurRadius = 20 }
            };

            AddVisualChild(_rectangle);
            AddLogicalChild(_rectangle);
        }

        public static readonly DependencyProperty TopProperty =
            DependencyProperty.Register("Top", typeof(double), typeof(PreviewAdorner), new FrameworkPropertyMetadata(0.0, FrameworkPropertyMetadataOptions.AffectsArrange));

        public double Top
        {
            get { return (double)GetValue(TopProperty); }
            set { SetValue(TopProperty, value); }
        }

        protected override IEnumerator LogicalChildren
        {
            get { yield return _rectangle; }
        }

        protected override Visual GetVisualChild(int index)
        {
            if (index == 0)
            {
                return _rectangle;
            }
            throw new ArgumentOutOfRangeException(nameof(index));
        }

        protected override int VisualChildrenCount
        {
            get { return 1; }
        }

        protected override Size MeasureOverride(Size constraint)
        {
            _rectangle.Measure(_itemElement.RenderSize);
            return constraint;
        }

        protected override Size ArrangeOverride(Size finalSize)
        {
            _rectangle.Arrange(new Rect(new Point(0,
                                                  Math.Max(0, Math.Min(
                                                               AdornedElement.RenderSize.Height -
                                                               _itemElement.RenderSize.Height, Top))),
                                        _itemElement.RenderSize));
            return finalSize;
        }
    }
}
