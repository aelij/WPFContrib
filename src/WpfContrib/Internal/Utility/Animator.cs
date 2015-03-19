using System;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Animation;
using Avalon.Windows.Utility;

// ReSharper disable once CheckNamespace
namespace Avalon.Windows.Internal.Utility
{
    internal static class Animator
    {
        #region Public Methods

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
                throw new ArgumentException(SR.Animator_PropertyMustBeDouble, "property");
            }
            if (!(target is IAnimatable))
            {
                throw new ArgumentException(SR.Animator_TargetMustBeIAnimatable, "target");
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

        #endregion

        #region Public Properties

        public static bool IsAnimationEnabled
        {
            get { return RenderCapability.Tier >= 0x20000; }
        }

        #endregion

        #region Private Methods

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

                if (animationDone != null)
                {
                    animationDone(target, EventArgs.Empty);
                }
            }
        }

        #endregion
    }
}
