namespace Avalon.Windows.Media.Animation;

/// <summary>
///     Animates the value of a <see cref="CornerRadius" /> property along a set of
///     <see cref="P:KeyFramesAnimationBase{T}.KeyFrames" />.
/// </summary>
public class CornerRadiusAnimationUsingKeyFrames : KeyFramesAnimationBase<CornerRadius>
{
    /// <summary>
    ///     Creates a new instance of <see cref="CornerRadiusAnimationUsingKeyFrames" />.
    /// </summary>
    /// <returns>
    ///     A new instance of <see cref="CornerRadiusAnimationUsingKeyFrames" />.
    /// </returns>
    protected override Freezable CreateInstanceCore()
    {
        return new CornerRadiusAnimationUsingKeyFrames();
    }
    /// <summary>
    ///     Creates the appropriate calculator for the animation type.
    /// </summary>
    /// <returns></returns>
    protected override IAnimationCalculator<CornerRadius> CreateCalculator()
    {
        return new CornerRadiusAnimationCalculator();
    }
}
