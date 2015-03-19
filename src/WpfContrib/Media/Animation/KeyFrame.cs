using System;
using System.Windows;
using System.Windows.Media.Animation;

namespace Avalon.Windows.Media.Animation
{
    /// <summary>
    ///     Abstract class that, when implemented, defines an animation segment with its own target value and
    ///     interpolation method for a <see cref="T:KeyFramesAnimationBase{T}" />.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public abstract class KeyFrame<T> : Freezable, IKeyFrame
        where T : struct
    {
        #region Dependency Properties

        #region KeyTime

        /// <summary>Identifies the <see cref="KeyTime" /> dependency property.</summary>
        /// <returns>The identifier for the <see cref="KeyTime" /> dependency property.</returns>
        public static readonly DependencyProperty KeyTimeProperty = DependencyProperty.Register("KeyTime",
            typeof (KeyTime), typeof (KeyFrame<T>), new PropertyMetadata(KeyTime.Uniform));

        /// <summary>Gets or sets the key frame's target value.</summary>
        /// <returns>
        ///     The key frame's target value, which is the value of this key frame at its specified <see cref="KeyTime" />.
        ///     The default value is 0.
        /// </returns>
        public T Value
        {
            get { return (T) GetValue(ValueProperty); }
            set { SetValue(ValueProperty, value); }
        }

        #endregion

        #region Value

        /// <summary>Identifies the <see cref="Value" /> dependency property.</summary>
        /// <returns>The identifier for the <see cref="Value" /> dependency property.</returns>
        public static readonly DependencyProperty ValueProperty = DependencyProperty.Register("Value", typeof (T),
            typeof (KeyFrame<T>), new PropertyMetadata());

        /// <summary> Gets or sets the time at which the key frame's target <see cref="Value" /> should be reached. </summary>
        /// <returns>
        ///     The time at which the key frame's current value should be equal to its <see cref="Value" /> property. The
        ///     default value is <see cref="F:KeyTime.Uniform" />.
        /// </returns>
        public KeyTime KeyTime
        {
            get { return (KeyTime) GetValue(KeyTimeProperty); }
            set { SetValue(KeyTimeProperty, value); }
        }

        #endregion

        #endregion

        #region Constructors

        /// <summary>
        ///     Initializes a new instance of the <see cref="KeyFrame{T}" /> class that has the specified target
        ///     <see cref="Value" />.
        /// </summary>
        protected KeyFrame()
        {
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="KeyFrame{T}" /> class that has the specified target
        ///     <see cref="Value" />.
        /// </summary>
        /// <param name="value">The <see cref="Value" /> of the new <see cref="KeyFrame{T}" /> instance.</param>
        protected KeyFrame(T value)
        {
            Value = value;
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="KeyFrame{T}" /> class that has the specified target
        ///     <see cref="Value" /> and <see cref="KeyTime" />.
        /// </summary>
        /// <param name="value">The <see cref="Value" /> of the new <see cref="KeyFrame{T}" /> instance.</param>
        /// <param name="keyTime">The <see cref="KeyTime" /> of the new <see cref="KeyFrame{T}" /> instance.</param>
        protected KeyFrame(T value, KeyTime keyTime)
        {
            Value = value;
            KeyTime = keyTime;
        }

        #endregion

        #region Methods

        /// <summary>Returns the interpolated value of a specific key frame at the progress increment provided.</summary>
        /// <returns>The output value of this key frame given the specified base value and progress.</returns>
        /// <param name="baseValue">The value to animate from.</param>
        /// <param name="keyFrameProgress">
        ///     A value between 0.0 and 1.0, inclusive, that specifies the percentage of time that has
        ///     elapsed for this key frame.
        /// </param>
        /// <exception cref="ArgumentOutOfRangeException">Occurs if keyFrameProgress is not between 0.0 and 1.0, inclusive.</exception>
        public T InterpolateValue(T baseValue, double keyFrameProgress)
        {
            if ((keyFrameProgress < 0) || (keyFrameProgress > 1))
            {
                throw new ArgumentOutOfRangeException("keyFrameProgress");
            }
            return InterpolateValueCore(baseValue, keyFrameProgress);
        }

        /// <summary>Calculates the value of a key frame at the progress increment provided. </summary>
        /// <returns>The output value of this key frame given the specified base value and progress.</returns>
        /// <param name="baseValue">The value to animate from; typically the value of the previous key frame.</param>
        /// <param name="keyFrameProgress">
        ///     A value between 0.0 and 1.0, inclusive, that specifies the percentage of time that has
        ///     elapsed for this key frame.
        /// </param>
        protected abstract T InterpolateValueCore(T baseValue, double keyFrameProgress);

        #endregion

        #region IKeyFrame Implementation

        object IKeyFrame.Value
        {
            get { return Value; }
            set { Value = (T) value; }
        }

        #endregion
    }
}