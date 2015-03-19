using System.Windows;
using System.Windows.Media.Animation;

namespace Avalon.Windows.Media.Animation
{
    /// <summary>
    ///     Animates from the <see cref="CornerRadius" /> value of the previous key frame to its own
    ///     <see cref="P:KeyFrame{T}.Value" /> using splined interpolation.
    /// </summary>
    public class SplineCornerRadiusKeyFrame : SplineKeyFrame<CornerRadius>
    {
        #region Constructors

        /// <summary>
        ///     Initializes a new instance of the <see cref="SplineCornerRadiusKeyFrame" /> class with the specified ending
        ///     value.
        /// </summary>
        public SplineCornerRadiusKeyFrame()
        {
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="SplineCornerRadiusKeyFrame" /> class with the specified ending
        ///     value.
        /// </summary>
        /// <param name="value">Ending value (also known as "target value") for the key frame.</param>
        public SplineCornerRadiusKeyFrame(CornerRadius value)
            : base(value)
        {
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="SplineCornerRadiusKeyFrame" /> class with the specified ending
        ///     value and key time.
        /// </summary>
        /// <param name="value">Ending value (also known as "target value") for the key frame.</param>
        /// <param name="keyTime">
        ///     Key time for the key frame. The key time determines when the target value is reached which is
        ///     also when the key frame ends.
        /// </param>
        public SplineCornerRadiusKeyFrame(CornerRadius value, KeyTime keyTime)
            : base(value, keyTime)
        {
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="SplineCornerRadiusKeyFrame" /> class with the specified ending
        ///     value, key time, and <see cref="KeySpline" />.
        /// </summary>
        /// <param name="keySpline">
        ///     <see cref="KeySpline" /> for the key frame. The <see cref="KeySpline" /> represents a Bezier
        ///     curve which defines animation progress of the key frame.
        /// </param>
        /// <param name="value">Ending value (also known as "target value") for the key frame.</param>
        /// <param name="keyTime">
        ///     Key time for the key frame. The key time determines when the target value is reached which is
        ///     also when the key frame ends.
        /// </param>
        public SplineCornerRadiusKeyFrame(CornerRadius value, KeyTime keyTime, KeySpline keySpline)
            : base(value, keyTime, keySpline)
        {
        }

        #endregion

        #region Freezable Methods

        /// <summary>
        ///     Creates a new instance of <see cref="SplineCornerRadiusKeyFrame" />.
        /// </summary>
        /// <returns>A new <see cref="SplineKeyFrame{T}" />.</returns>
        protected override Freezable CreateInstanceCore()
        {
            return new SplineCornerRadiusKeyFrame();
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