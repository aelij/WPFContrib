using System.Windows;
using System.Windows.Media.Animation;

namespace Avalon.Windows.Media.Animation
{
    /// <summary>
    ///     Animates from the <see cref="CornerRadius" /> value of the previous key frame to its own
    ///     <see cref="P:KeyFrame{T}.Value" /> using discrete interpolation.
    /// </summary>
    public class DiscreteCornerRadiusKeyFrame : DiscreteKeyFrame<CornerRadius>
    {
        #region Constructors

        /// <summary>
        ///     Initializes a new instance of the <see cref="DiscreteCornerRadiusKeyFrame" /> class with the specified ending
        ///     value.
        /// </summary>
        public DiscreteCornerRadiusKeyFrame()
        {
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="DiscreteCornerRadiusKeyFrame" /> class with the specified ending
        ///     value.
        /// </summary>
        /// <param name="value">Ending value (also known as "target value") for the key frame.</param>
        public DiscreteCornerRadiusKeyFrame(CornerRadius value)
            : base(value)
        {
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="DiscreteCornerRadiusKeyFrame" /> class with the specified ending
        ///     value and key time.
        /// </summary>
        /// <param name="value">Ending value (also known as "target value") for the key frame.</param>
        /// <param name="keyTime">
        ///     Key time for the key frame. The key time determines when the target value is reached which is
        ///     also when the key frame ends.
        /// </param>
        public DiscreteCornerRadiusKeyFrame(CornerRadius value, KeyTime keyTime)
            : base(value, keyTime)
        {
        }

        #endregion

        #region Freezable Methods

        /// <summary>
        ///     Creates a new instance of <see cref="DiscreteCornerRadiusKeyFrame" />.
        /// </summary>
        /// <returns>
        ///     A new <see cref="DiscreteCornerRadiusKeyFrame" />.
        /// </returns>
        protected override Freezable CreateInstanceCore()
        {
            return new DiscreteCornerRadiusKeyFrame();
        }

        #endregion
    }
}