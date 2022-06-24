using Avalon.Windows.Utility;

namespace Avalon.Windows.Internal.Utility;

internal static class Animator
{
    public static void AnimatePropertyFromTo(DependencyObject target, DependencyProperty property, double? fromValue, double? targetValue, int duration)
    {
        AnimatePropertyFromTo(target, property, fromValue, targetValue, duration, new DoubleAnimation(), null, 0);
    }

    public static void AnimatePropertyFromTo(DependencyObject target, DependencyProperty property, double? fromValue, double? targetValue, int duration, EventHandler animationDone)
    {
        AnimatePropertyFromTo(target, property, fromValue, targetValue, duration, new DoubleAnimation(), animationDone, 0);
    }

    public static void AnimatePropertyFromTo(DependencyObject target, DependencyProperty property, double? fromValue, double? targetValue, int duration, DoubleAnimation animation, EventHandler animationDone, int startTime)
    {
        if (property.PropertyType != typeof(double))
        {
            throw new ArgumentException(SR.Animator_PropertyMustBeDouble, nameof(property));
        }
        if (target is not IAnimatable)
        {
            throw new ArgumentException(SR.Animator_TargetMustBeIAnimatable, nameof(target));
        }
        animation.From = fromValue;
        animation.To = targetValue;
        animation.BeginTime = TimeSpan.FromMilliseconds(startTime);
        animation.Duration = TimeSpan.FromMilliseconds(duration);
        if (animationDone != null)
        {
            animation.AttachCompletedEventHandler(animationDone);
        }
        AnimateProperty(target, property, animation, animationDone);
    }
    public static bool IsAnimationEnabled
    {
        get { return RenderCapability.Tier >= 0x20000; }
    }
    private static void AnimateProperty(DependencyObject target, DependencyProperty property, DoubleAnimation animation, EventHandler animationDone)
    {
        if (IsAnimationEnabled)
        {
            ((IAnimatable)target).BeginAnimation(property, animation);
        }
        else
        {
            ((IAnimatable)target).BeginAnimation(property, null);
            if (animation.To != null)
            {
                target.SetValue(property, animation.To);
            }
            else
            {
                target.ClearValue(property);
            }

            animationDone?.Invoke(target, EventArgs.Empty);
        }
    }
}
