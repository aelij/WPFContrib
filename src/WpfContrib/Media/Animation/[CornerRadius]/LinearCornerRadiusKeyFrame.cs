using System.Windows;
using System.Windows.Media.Animation;

namespace Avalon.Windows.Media.Animation
{
    /// <summary>
    ///     Animates from the <see cref="LinearCornerRadiusKeyFrame" /> value of the previous key frame to its own
    ///     <see cref="P:KeyFrame{T}.Value" /> using linear interpolation.
    /// </summary>
    public class LinearCornerRadiusKeyFrame : LinearKeyFrame<CornerRadius>
    {
        #region Constructors

        /// <summary>
        ///     Initializes a new instance of the <see cref="LinearCornerRadiusKeyFrame" /> class with the specified ending
        ///     value.
        /// </summary>
        public LinearCornerRadiusKeyFrame()
        {
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="LinearCornerRadiusKeyFrame" /> class with the specified ending
        ///     value.
        /// </summary>
        /// <param name="value">Ending value (also known as "target value") for the key frame.</param>
        public LinearCornerRadiusKeyFrame(CornerRadius value)
            : base(value)
        {
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="LinearCornerRadiusKeyFrame" /> class with the specified ending
        ///     value and key time.
        /// </summary>
        /// <param name="value">Ending value (also known as "target value") for the key frame.</param>
        /// <param name="keyTime">
        ///     Key time for the key frame. The key time determines when the target value is reached which is
        ///     also when the key frame ends.
        /// </param>
        public LinearCornerRadiusKeyFrame(CornerRadius value, KeyTime keyTime)
            : base(value, keyTime)
        {
        }

        #endregion

        #region Freezable Methods

        /// <summary>
        ///     Creates a new instance of <see cref="LinearCornerRadiusKeyFrame" />.
        /// </summary>
        /// <returns>A new <see cref="LinearCornerRadiusKeyFrame" />.</returns>
        protected override Freezable CreateInstanceCore()
        {
            return new LinearCornerRadiusKeyFrame();
        }

        #endregion

        #region Calculator Methods

        /// <summary>
        ///     Creates the appropriate calculator for the animation type.
        /// </summary>
        /// <returns></returns>
        protected override IAnimationCalculator<CornerRadius> CreateCalculator()
        {
            return new CornerRadiusAnimationCalculator();
        }

        #endregion
    }
}