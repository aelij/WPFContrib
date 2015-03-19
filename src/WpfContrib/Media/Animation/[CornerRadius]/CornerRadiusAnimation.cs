using System.Windows;
using System.Windows.Media.Animation;

namespace Avalon.Windows.Media.Animation
{
    /// <summary>
    ///     Animates the value of a CornerRadius property between two target values using linear interpolation over a
    ///     specified Duration.
    /// </summary>
    public class CornerRadiusAnimation : LinearAnimationBase<CornerRadius>
    {
        #region Constructors

        /// <summary> Initializes a new instance of the CornerRadiusAnimation class.</summary>
        public CornerRadiusAnimation()
        {
        }

        /// <summary>
        ///     Initializes a new instance of the CornerRadiusAnimation class that animates to the specified value over the
        ///     specified duration. The starting value for the animation is the base value of the property being animated or the
        ///     output from another animation.
        /// </summary>
        /// <param name="toValue">The destination value of the animation. </param>
        /// <param name="duration">
        ///     The length of time the animation takes to play from start to finish, once. See the Duration
        ///     property for more information.
        /// </param>
        public CornerRadiusAnimation(CornerRadius toValue, Duration duration)
            : base(toValue, duration)
        {
        }

        /// <summary>
        ///     Initializes a new instance of the CornerRadiusAnimation class that animates to the specified value over the
        ///     specified duration and has the specified fill behavior. The starting value for the animation is the base value of
        ///     the property being animated or the output from another animation.
        /// </summary>
        /// <param name="fillBehavior">Specifies how the animation behaves when it is not active.</param>
        /// <param name="toValue">The destination value of the animation. </param>
        /// <param name="duration">
        ///     The length of time the animation takes to play from start to finish, once. See the Duration
        ///     property for more information.
        /// </param>
        public CornerRadiusAnimation(CornerRadius toValue, Duration duration, FillBehavior fillBehavior)
            : base(toValue, duration, fillBehavior)
        {
        }

        /// <summary>
        ///     Initializes a new instance of the CornerRadiusAnimation class that animates from the specified starting value
        ///     to the specified destination value over the specified duration.
        /// </summary>
        /// <param name="toValue">The destination value of the animation.</param>
        /// <param name="duration">
        ///     The length of time the animation takes to play from start to finish, once. See the Duration
        ///     property for more information.
        /// </param>
        /// <param name="fromValue">The starting value of the animation.</param>
        public CornerRadiusAnimation(CornerRadius fromValue, CornerRadius toValue, Duration duration)
            : base(fromValue, toValue, duration)
        {
        }

        /// <summary>
        ///     Initializes a new instance of the CornerRadiusAnimation class that animates from the specified starting value
        ///     to the specified destination value over the specified duration and has the specified fill behavior.
        /// </summary>
        /// <param name="fillBehavior">Specifies how the animation behaves when it is not active.</param>
        /// <param name="toValue">The destination value of the animation. </param>
        /// <param name="duration">
        ///     The length of time the animation takes to play from start to finish, once. See the Duration
        ///     property for more information.
        /// </param>
        /// <param name="fromValue">The starting value of the animation.</param>
        public CornerRadiusAnimation(CornerRadius fromValue, CornerRadius toValue, Duration duration,
            FillBehavior fillBehavior)
            : base(fromValue, toValue, duration, fillBehavior)
        {
        }

        #endregion

        #region Freezable Method

        /// <summary>Creates a new instance of the <see cref="CornerRadiusAnimation" />.</summary>
        /// <returns>The new instance.</returns>
        protected override Freezable CreateInstanceCore()
        {
            return new CornerRadiusAnimation();
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