namespace Avalon.Windows.Media.Animation;

/// <summary>
///     Animates from the <typeparamref name="T" /> value of the previous key frame to its own
///     <see cref="P:KeyFrame{T}.Value" /> using linear interpolation.
/// </summary>
/// <typeparam name="T"></typeparam>
public abstract class LinearKeyFrame<T> : KeyFrame<T>
    where T : struct
{
    /// <summary>Initializes a new instance of the <see cref="LinearKeyFrame{T}" /> class with the specified ending value. </summary>
    protected LinearKeyFrame()
    {
    }

    /// <summary>Initializes a new instance of the <see cref="LinearKeyFrame{T}" /> class with the specified ending value. </summary>
    /// <param name="value">Ending value (also known as "target value") for the key frame.</param>
    protected LinearKeyFrame(T value)
        : base(value)
    {
    }

    /// <summary>
    ///     Initializes a new instance of the <see cref="LinearKeyFrame{T}" /> class with the specified ending value and
    ///     key time.
    /// </summary>
    /// <param name="value">Ending value (also known as "target value") for the key frame.</param>
    /// <param name="keyTime">
    ///     Key time for the key frame. The key time determines when the target value is reached which is
    ///     also when the key frame ends.
    /// </param>
    protected LinearKeyFrame(T value, KeyTime keyTime)
        : base(value, keyTime)
    {
    }
    /// <summary>Creates a new instance of <see cref="LinearKeyFrame{T}" />.</summary>
    /// <returns>A new <see cref="LinearKeyFrame{T}" />.</returns>
    protected abstract override Freezable CreateInstanceCore();

    /// <summary>
    ///     Interpolates, in a linear fashion, between the previous key frame value and the value of the current key
    ///     frame, using the supplied progress increment.
    /// </summary>
    /// <returns>The output value of this key frame given the specified base value and progress.</returns>
    /// <param name="baseValue">The value to animate from.</param>
    /// <param name="keyFrameProgress">
    ///     A value between 0.0 and 1.0, inclusive, that specifies the percentage of time that has
    ///     elapsed for this key frame.
    /// </param>
    protected override sealed T InterpolateValueCore(T baseValue, double keyFrameProgress)
    {
        if (keyFrameProgress == 0)
        {
            return baseValue;
        }
        if (keyFrameProgress == 1)
        {
            return Value;
        }
        return Calculator.Interpolate(baseValue, Value, keyFrameProgress);
    }
    private static IAnimationCalculator<T> s_calculator;

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
