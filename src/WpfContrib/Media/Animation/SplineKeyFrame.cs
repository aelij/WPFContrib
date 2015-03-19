using System;
using System.Windows;
using System.Windows.Media.Animation;
using Avalon.Internal.Utility;

namespace Avalon.Windows.Media.Animation
{
    /// <summary>
    ///     Animates from the <typeparamref name="T" /> value of the previous key frame to its own
    ///     <see cref="F:KeyFrame{T}.Value" /> using splined interpolation.
    /// </summary>
    public abstract class SplineKeyFrame<T> : KeyFrame<T>
        where T : struct
    {
        #region Dependency Properties

        #region KeySpline

        /// <summary>Identifies the <see cref="F:KeyFrame{T}.KeySpline" /> dependency property.</summary>
        /// <returns>The identifier for the <see cref="KeySpline" /> dependency property.</returns>
        public static readonly DependencyProperty KeySplineProperty = DependencyProperty.Register("KeySpline",
            typeof (KeySpline), typeof (SplineKeyFrame<T>), new PropertyMetadata(new KeySpline()));

        /// <summary>Gets or sets the two control points that define animation progress for this key frame.   </summary>
        /// <returns>The two control points that specify the cubic  Bezier curve which defines the progress of the key frame.</returns>
        public KeySpline KeySpline
        {
            get { return (KeySpline) GetValue(KeySplineProperty); }
            set
            {
                ArgumentValidator.NotNull(value, "value");

                SetValue(KeySplineProperty, value);
            }
        }

        #endregion

        #endregion

        #region Constructors

        /// <summary>Initializes a new instance of the <see cref="SplineKeyFrame{T}" /> class with the specified ending value.  </summary>
        protected SplineKeyFrame()
        {
            EnsureCalculator();
        }

        /// <summary>Initializes a new instance of the <see cref="SplineKeyFrame{T}" /> class with the specified ending value.  </summary>
        /// <param name="value">Ending value (also known as "target value") for the key frame.</param>
        protected SplineKeyFrame(T value)
            : base(value)
        {
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="SplineKeyFrame{T}" /> class with the specified ending value and
        ///     key time.
        /// </summary>
        /// <param name="value">Ending value (also known as "target value") for the key frame.</param>
        /// <param name="keyTime">
        ///     Key time for the key frame. The key time determines when the target value is reached which is
        ///     also when the key frame ends.
        /// </param>
        protected SplineKeyFrame(T value, KeyTime keyTime)
            : base(value, keyTime)
        {
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="SplineKeyFrame{T}" /> class with the specified ending value, key
        ///     time, and <see cref="KeySpline" />.
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
        protected SplineKeyFrame(T value, KeyTime keyTime, KeySpline keySpline)
            : this(value, keyTime)
        {
            if (keySpline == null)
            {
                throw new ArgumentNullException("keySpline");
            }
            KeySpline = keySpline;
        }

        #endregion

        #region Methods

        /// <summary>Creates a new instance of <see cref="SplineKeyFrame{T}" />.</summary>
        /// <returns>A new <see cref="SplineKeyFrame{T}" />.</returns>
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
            // ReSharper disable once CompareOfFloatsByEqualityOperator
            if (keyFrameProgress == 0)
            {
                return baseValue;
            }
            // ReSharper disable once CompareOfFloatsByEqualityOperator
            if (keyFrameProgress == 1)
            {
                return Value;
            }
            double splineProgress = KeySpline.GetSplineProgress(keyFrameProgress);
            return Calculator.Interpolate(baseValue, Value, splineProgress);
        }

        #endregion

        #region Calculator

        private static IAnimationCalculator<T> _calculator;

        /// <summary>
        ///     Gets the <see cref="IAnimationCalculator{T}" />.
        /// </summary>
        /// <value>The calculator.</value>
        protected IAnimationCalculator<T> Calculator
        {
            get
            {
                EnsureCalculator();
                return _calculator;
            }
        }

        /// <summary>
        ///     Creates the appropriate calculator for the animation type.
        /// </summary>
        /// <returns></returns>
        protected abstract IAnimationCalculator<T> CreateCalculator();

        private void EnsureCalculator()
        {
            if (_calculator == null)
            {
                _calculator = CreateCalculator();

                if (_calculator == null)
                {
                    throw new InvalidOperationException(SR.IAnimationCalculator_CreationFailed);
                }
            }
        }

        #endregion
    }
}