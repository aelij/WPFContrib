using Avalon.Internal.Utility;

namespace Avalon.Windows.Media.Animation;

/// <summary>
/// Base class for generic animations
/// </summary>
/// <typeparam name="T"></typeparam>
public abstract class AnimationBase<T> : AnimationTimeline
    where T : struct
{
    /// <summary>
    ///     Initializes a new instance of the <see cref="AnimationBase{T}" /> class.
    /// </summary>
    protected AnimationBase()
    {
        EnsureCalculator();
    }
    /// <summary>
    ///     Creates a modifiable clone of this <see cref="T:AnimationBase{T}" />, making deep copies of this object's
    ///     values. When copying dependency properties, this method copies resource references and data bindings (but they
    ///     might no longer resolve) but not animations or their current values.
    /// </summary>
    /// <returns>
    ///     A modifiable clone of the current object. The cloned object's
    ///     <see cref="P:System.Windows.Freezable.IsFrozen" /> property will be false even if the source's
    ///     <see cref="P:System.Windows.Freezable.IsFrozen" /> property was true.
    /// </returns>
    public new AnimationBase<T> Clone()
    {
        return (AnimationBase<T>)base.Clone();
    }
    /// <summary>
    ///     Gets a value that specifies whether the animation's output value is added to the base value of the property
    ///     being animated.
    /// </summary>
    /// <returns>
    ///     true if the animation adds its output value to the base value of the property being animated instead of
    ///     replacing it; otherwise, false. The default value is false.
    /// </returns>
    public bool IsAdditive
    {
        get { return (bool)GetValue(IsAdditiveProperty); }
        set { SetValue(IsAdditiveProperty, value); }
    }

    /// <summary>Gets or sets a value that specifies whether the animation's value accumulates when it repeats.</summary>
    /// <returns>
    ///     true if the animation accumulates its values when its
    ///     <see cref="System.Windows.Media.Animation.Timeline.RepeatBehavior" /> property causes it to repeat its simple
    ///     duration; otherwise, false. The default value is false.
    /// </returns>
    public bool IsCumulative
    {
        get { return (bool)GetValue(IsCumulativeProperty); }
        set { SetValue(IsCumulativeProperty, value); }
    }
    /// <summary>Gets the current value of the animation.</summary>
    /// <returns>The current value of the animation.</returns>
    /// <param name="defaultOriginValue">
    ///     The origin value provided to the animation if the animation does not have its own
    ///     start value. If this animation is the first in a composition chain it will be the base value of the property being
    ///     animated; otherwise it will be the value returned by the previous animation in the chain.
    /// </param>
    /// <param name="defaultDestinationValue">
    ///     The destination value provided to the animation if the animation does not have
    ///     its own destination value.
    /// </param>
    /// <param name="animationClock">
    ///     The <see cref="System.Windows.Media.Animation.AnimationClock" /> which can generate the
    ///     <see cref="P:System.Windows.Media.Animation.Clock.CurrentTime" /> or
    ///     <see cref="P:System.Windows.Media.Animation.Clock.CurrentProgress" /> value to be used by the animation to generate
    ///     its output value.
    /// </param>
    public override sealed object GetCurrentValue(object defaultOriginValue, object defaultDestinationValue,
        AnimationClock animationClock)
    {
        ArgumentValidator.NotNull(defaultOriginValue, "defaultOriginValue");
        ArgumentValidator.NotNull(defaultDestinationValue, "defaultDestinationValue");

        return GetCurrentValue((T)defaultOriginValue, (T)defaultDestinationValue, animationClock);
    }

    /// <summary>Gets the current value of the animation.</summary>
    /// <param name="defaultOriginValue">
    ///     The origin value provided to the animation if the animation does not have its own
    ///     start value. If this animation is the first in a composition chain it will be the base value of the property being
    ///     animated; otherwise it will be the value returned by the previous animation in the chain.
    /// </param>
    /// <param name="defaultDestinationValue">
    ///     The destination value provided to the animation if the animation does not have
    ///     its own destination value.
    /// </param>
    /// <param name="animationClock">
    ///     The <see cref="System.Windows.Media.Animation.AnimationClock" /> which can generate the
    ///     <see cref="P:System.Windows.Media.Animation.Clock.CurrentTime" /> or
    ///     <see cref="P:System.Windows.Media.Animation.Clock.CurrentProgress" /> value to be used by the animation to generate
    ///     its output value.
    /// </param>
    public T GetCurrentValue(T defaultOriginValue, T defaultDestinationValue, AnimationClock animationClock)
    {
        ArgumentValidator.NotNull(animationClock, "animationClock");

        ReadPreamble();
        if (animationClock.CurrentState == ClockState.Stopped)
        {
            return defaultDestinationValue;
        }
        return GetCurrentValueCore(defaultOriginValue, defaultDestinationValue, animationClock);
    }

    /// <summary>
    ///     Calculates a value that represents the current value of the property being animated, as determined by the host
    ///     animation.
    /// </summary>
    /// <returns>The calculated value of the property, as determined by the current animation.</returns>
    /// <param name="defaultOriginValue">
    ///     The suggested origin value, used if the animation does not have its own explicitly set
    ///     start value.
    /// </param>
    /// <param name="defaultDestinationValue">
    ///     The suggested destination value, used if the animation does not have its own
    ///     explicitly set end value.
    /// </param>
    /// <param name="animationClock">
    ///     An <see cref="System.Windows.Media.Animation.AnimationClock" /> that generates the
    ///     <see cref="P:System.Windows.Media.Animation.Clock.CurrentTime" /> or
    ///     <see cref="P:System.Windows.Media.Animation.Clock.CurrentProgress" /> used by the host animation.
    /// </param>
    protected abstract T GetCurrentValueCore(T defaultOriginValue, T defaultDestinationValue,
        AnimationClock animationClock);

    /// <summary>Gets the type of the target property associated with the animation.</summary>
    /// <returns>The type of the target property associated with the animation.</returns>
    public override sealed Type TargetPropertyType
    {
        get
        {
            ReadPreamble();
            return typeof(T);
        }
    }
    /// <summary>
    ///     The <see cref="IAnimationCalculator{T}" /> instance for this type.
    /// </summary>
    private static IAnimationCalculator<T> s_calculator;

    /// <summary>
    ///     Gets the static instance of the calculator, without ensuring it exists.
    ///     Should be called only when the object is sure to be initialized.
    /// </summary>
    /// <value>The calculator.</value>
    internal static IAnimationCalculator<T> SafeCalculator
    {
        get { return s_calculator; }
    }

    /// <summary>
    ///     Gets the <see cref="IAnimationCalculator{T}" />.
    /// </summary>
    /// <value>The calculator.</value>
    protected IAnimationCalculator<T> Calculator
    {
        get
        {
            EnsureCalculator();
            return s_calculator;
        }
    }

    /// <summary>
    ///     Creates the appropriate calculator for the animation type.
    /// </summary>
    /// <returns></returns>
    protected abstract IAnimationCalculator<T> CreateCalculator();

    private void EnsureCalculator()
    {
        if (s_calculator == null)
        {
            s_calculator = CreateCalculator();

            if (s_calculator == null)
            {
                throw new InvalidOperationException(SR.IAnimationCalculator_CreationFailed);
            }
        }
    }
}
