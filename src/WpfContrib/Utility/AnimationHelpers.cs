namespace Avalon.Windows.Utility;

/// <summary>
///     Encapsulates methods and properties for handling animations.
/// </summary>
public static class AnimationHelpers
{
    /// <summary>
    ///     Switches between the To and From properties of the each AnimationTimeline in the Storyboard.
    /// </summary>
    /// <param name="storyboard">The storyboard.</param>
    public static void ReverseStoryboard(this Storyboard storyboard)
    {
        foreach (var anim in storyboard.Children.OfType<AnimationTimeline>())
        {
            DependencyProperty from = anim.GetDependencyProperty("From");
            DependencyProperty to = anim.GetDependencyProperty("To");

            object fromValue = anim.GetValue(from);
            anim.SetValue(from, anim.GetValue(to));
            anim.SetValue(to, fromValue);
        }
    }

    /// <summary>
    ///     Returns a cloned Storyboard where the To and From properties of the AnimationTimeline have been switched.
    /// </summary>
    /// <param name="storyboard">The storyboard.</param>
    /// <returns></returns>
    public static Storyboard GetReversedStoryboard(this Storyboard storyboard)
    {
        Storyboard cloned = storyboard.Clone();

        ReverseStoryboard(cloned);

        return cloned;
    }

    /// <summary>
    ///     Creates and adds an AnimationTimeline to a Storyboard.
    /// </summary>
    /// <typeparam name="TAnimation">The type of the animation.</typeparam>
    /// <typeparam name="T"></typeparam>
    /// <param name="storyboard">The storyboard.</param>
    /// <param name="path">The path.</param>
    /// <param name="from">From.</param>
    /// <param name="to">To.</param>
    /// <param name="duration">The duration.</param>
    /// <returns></returns>
    public static TAnimation AddLinearAnimation<TAnimation, T>(this Storyboard storyboard, PropertyPath path,
        T? from, T? to, Duration duration)
        where TAnimation : AnimationTimeline, new()
        where T : struct
    {
        var timeline = new TAnimation();

        DependencyProperty fromProp = timeline.GetDependencyProperty("From");
        DependencyProperty toProp = timeline.GetDependencyProperty("To");

        timeline.Duration = duration;

        timeline.SetValue(fromProp, from);
        timeline.SetValue(toProp, to);

        Storyboard.SetTargetProperty(timeline, path);

        storyboard.Children.Add(timeline);

        return timeline;
    }

    /// <summary>
    ///     Adds the animation to the storyboard.
    /// </summary>
    /// <param name="storyboard">The storyboard.</param>
    /// <param name="timeline">The timeline.</param>
    /// <param name="target">The target.</param>
    /// <param name="property">The property.</param>
    public static void AddAnimation(this Storyboard storyboard, Timeline timeline, DependencyObject target,
        DependencyProperty property)
    {
        Storyboard.SetTarget(timeline, target);
        Storyboard.SetTargetProperty(timeline, new PropertyPath(property));
        storyboard.Children.Add(timeline);
    }

    /// <summary>
    ///     Adds the animation to the storyboard.
    /// </summary>
    /// <param name="storyboard">The storyboard.</param>
    /// <param name="timeline">The timeline.</param>
    /// <param name="targetName">Name of the target.</param>
    /// <param name="property">The property.</param>
    public static void AddAnimation(this Storyboard storyboard, Timeline timeline, string targetName,
        DependencyProperty property)
    {
        Storyboard.SetTargetName(timeline, targetName);
        Storyboard.SetTargetProperty(timeline, new PropertyPath(property));
        storyboard.Children.Add(timeline);
    }

    /// <summary>
    /// Attaches the specified event handler to the <see cref="E:Timeline.Completed"/> event.
    /// <remarks>
    /// Also ensures that the reference is released upon completion.
    /// </remarks>
    /// </summary>
    /// <param name="timeline">The timeline.</param>
    /// <param name="handler">The handler.</param>
    internal static void AttachCompletedEventHandler(this Timeline timeline, EventHandler handler)
    {
        var completionHandler = new AnimationCompletedHandler(timeline, handler);
        timeline.Completed += completionHandler.OnTimelineCompleted;
    }

    /// <summary>
    /// Provides a closure so that allows the <see cref="E:Timeline.Completed"/> event to be disconnected.
    /// </summary>
    internal class AnimationCompletedHandler
    {
        private readonly Timeline _timeline;
        private readonly EventHandler _handler;

        internal AnimationCompletedHandler(Timeline timeline, EventHandler handler)
        {
            _timeline = timeline;
            _handler = handler;
        }

        internal void OnTimelineCompleted(object sender, EventArgs e)
        {
            _timeline.Completed -= OnTimelineCompleted;
            _handler(_timeline, EventArgs.Empty);
        }
    }
    private static double? GetActualWidth(FrameworkElement obj)
    {
        return (double?)obj.GetValue(s_actualWidthProperty);
    }

    private static void SetActualWidth(FrameworkElement obj, double? value)
    {
        obj.SetValue(s_actualWidthPropertyKey, value);
    }

    private static void ClearActualWidth(FrameworkElement obj)
    {
        obj.ClearValue(s_actualWidthPropertyKey);
    }

    private static readonly DependencyPropertyKey s_actualWidthPropertyKey =
        DependencyProperty.RegisterAttachedReadOnly("ActualWidth", typeof(double?), typeof(AnimationHelpers),
            new FrameworkPropertyMetadata());

    private static readonly DependencyProperty s_actualWidthProperty = s_actualWidthPropertyKey.DependencyProperty;
    /// <summary>
    ///     Gets the width percentage.
    /// </summary>
    /// <param name="obj">The obj.</param>
    /// <returns></returns>
    public static int GetWidthPercentage(FrameworkElement obj)
    {
        return (int)obj.GetValue(WidthPercentageProperty);
    }

    /// <summary>
    ///     Sets the width percentage.
    /// </summary>
    /// <param name="obj">The obj.</param>
    /// <param name="value">The value.</param>
    public static void SetWidthPercentage(FrameworkElement obj, int value)
    {
        obj.SetValue(WidthPercentageProperty, value);
    }

    /// <summary>
    ///     Identifies the <c>WidthPercentage</c> attached property.
    /// </summary>
    public static readonly DependencyProperty WidthPercentageProperty =
        DependencyProperty.RegisterAttached("WidthPercentage", typeof(int), typeof(AnimationHelpers),
            new FrameworkPropertyMetadata(100, OnWidthPercentageChanged, CoercePercentage));

    private static void OnWidthPercentageChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
    {
        var element = (FrameworkElement)o;

        var percent = (int)e.NewValue;

        if (percent == 100)
        {
            element.ClearValue(FrameworkElement.WidthProperty);
            ClearActualWidth(element);
        }
        else
        {
            double? actualWidth = GetActualWidth(element);
            if (actualWidth == null)
            {
                if (element.IsArrangeValid)
                {
                    actualWidth = element.ActualWidth;
                    SetActualWidth(element, actualWidth);
                }
                else
                {
                    element.Loaded += DeferActualWidth;
                }
            }

            if (actualWidth != null)
            {
                SetWidth(element, percent, actualWidth.Value);
            }
        }
    }

    private static void DeferActualWidth(object sender, RoutedEventArgs e)
    {
        var fe = (FrameworkElement)sender;
        fe.Loaded -= DeferActualWidth;

        SetActualWidth(fe, fe.ActualWidth);
        SetWidth(fe, GetWidthPercentage(fe), fe.ActualWidth);
    }

    private static void SetWidth(FrameworkElement element, int percent, double actualWidth)
    {
        element.Width = percent / 100D * actualWidth;
    }
    private static double? GetActualHeight(FrameworkElement obj)
    {
        return (double?)obj.GetValue(s_actualHeightProperty);
    }

    private static void SetActualHeight(FrameworkElement obj, double? value)
    {
        obj.SetValue(s_actualHeightPropertyKey, value);
    }

    private static void ClearActualHeight(FrameworkElement obj)
    {
        obj.ClearValue(s_actualHeightPropertyKey);
    }

    private static readonly DependencyPropertyKey s_actualHeightPropertyKey =
        DependencyProperty.RegisterAttachedReadOnly("ActualHeight", typeof(double?), typeof(AnimationHelpers),
            new FrameworkPropertyMetadata());

    private static readonly DependencyProperty s_actualHeightProperty = s_actualHeightPropertyKey.DependencyProperty;
    /// <summary>
    ///     Gets the height percentage.
    /// </summary>
    /// <param name="obj">The obj.</param>
    /// <returns></returns>
    public static int GetHeightPercentage(FrameworkElement obj)
    {
        return (int)obj.GetValue(HeightPercentageProperty);
    }

    /// <summary>
    ///     Sets the height percentage.
    /// </summary>
    /// <param name="obj">The obj.</param>
    /// <param name="value">The value.</param>
    public static void SetHeightPercentage(FrameworkElement obj, int value)
    {
        obj.SetValue(HeightPercentageProperty, value);
    }

    /// <summary>
    ///     Identifies the <c>HeightPercentage</c> attached property.
    /// </summary>
    public static readonly DependencyProperty HeightPercentageProperty =
        DependencyProperty.RegisterAttached("HeightPercentage", typeof(int), typeof(AnimationHelpers),
            new FrameworkPropertyMetadata(100, OnHeightPercentageChanged, CoercePercentage));

    private static void OnHeightPercentageChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
    {
        var element = (FrameworkElement)o;

        var percent = (int)e.NewValue;

        if (percent == 100)
        {
            element.ClearValue(FrameworkElement.HeightProperty);
            ClearActualHeight(element);
        }
        else
        {
            double? actualHeight = GetActualHeight(element);
            if (actualHeight == null)
            {
                if (element.IsArrangeValid)
                {
                    actualHeight = element.ActualHeight;
                    SetActualHeight(element, actualHeight);
                }
                else
                {
                    element.Loaded += DeferActualHeight;
                }
            }

            if (actualHeight != null)
            {
                SetHeight(element, percent, actualHeight.Value);
            }
        }
    }

    private static void DeferActualHeight(object sender, RoutedEventArgs e)
    {
        var fe = (FrameworkElement)sender;
        fe.Loaded -= DeferActualHeight;

        SetActualHeight(fe, fe.ActualHeight);
        SetHeight(fe, GetHeightPercentage(fe), fe.ActualHeight);
    }

    private static void SetHeight(FrameworkElement element, int percent, double actualHeight)
    {
        element.Height = percent / 100D * actualHeight;
    }
    private static object CoercePercentage(DependencyObject o, object value)
    {
        var current = (int)value;

        if (current < 0)
        {
            current = 0;
        }

        return current;
    }
}
