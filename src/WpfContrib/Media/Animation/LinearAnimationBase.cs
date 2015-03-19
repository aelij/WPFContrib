using System;
using System.Windows;
using System.Windows.Media.Animation;

namespace Avalon.Windows.Media.Animation
{
    /// <summary>
    ///     Animates the value of a <typeparamref name="T" /> property between two target values using linear interpolation
    ///     over a specified <see cref="T:Duration" />.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public abstract class LinearAnimationBase<T> : AnimationBase<T>
        where T : struct
    {
        #region Fields

        private AnimationType _animationType;
        private bool _isAnimationFunctionValid;
        private T[] _keyValues;

        #endregion

        #region Dependency Properties

        #region By

        /// <summary> Identifies the By dependency property. </summary>
        /// <returns>The identifier for the By dependency property.</returns>
        public static readonly DependencyProperty ByProperty = DependencyProperty.Register("By", typeof (T?),
            typeof (LinearAnimationBase<T>), new PropertyMetadata(new T?(), OnAnimationFunctionChanged),
            ValidateFromToOrByValue);

        /// <summary>Gets or sets the total amount by which the animation changes its starting value.</summary>
        /// <returns>The total amount by which the animation changes its starting value. The default value is null.</returns>
        public T? By
        {
            get { return (T?) GetValue(ByProperty); }
            set { SetValue(ByProperty, value); }
        }

        #endregion

        #region From

        /// <summary> Identifies the From dependency property. </summary>
        /// <returns>The identifier for the From dependency property.</returns>
        public static readonly DependencyProperty FromProperty = DependencyProperty.Register("From", typeof (T?),
            typeof (LinearAnimationBase<T>), new PropertyMetadata(new T?(), OnAnimationFunctionChanged),
            ValidateFromToOrByValue);

        /// <summary>Gets or sets the animation's starting value.</summary>
        /// <returns>The starting value of the animation. The default value is null.</returns>
        public T? From
        {
            get { return (T?) GetValue(FromProperty); }
            set { SetValue(FromProperty, value); }
        }

        #endregion

        #region To

        /// <summary> Identifies the To dependency property. </summary>
        /// <returns>The identifier for the To dependency property.</returns>
        public static readonly DependencyProperty ToProperty = DependencyProperty.Register("To", typeof (T?),
            typeof (LinearAnimationBase<T>), new PropertyMetadata(new T?(), OnAnimationFunctionChanged),
            ValidateFromToOrByValue);

        /// <summary> Gets or sets the animation's ending value.</summary>
        /// <returns>The ending value of the animation. The default value is null.</returns>
        public T? To
        {
            get { return (T?) GetValue(ToProperty); }
            set { SetValue(ToProperty, value); }
        }

        #endregion

        #endregion

        #region Constructors

        /// <summary>Initializes a new instance of the <see cref="LinearAnimationBase{T}" /> class.</summary>
        protected LinearAnimationBase()
        {
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="LinearAnimationBase{T}" /> class that animates to the specified
        ///     value over the specified duration. The starting value for the animation is the base value of the property being
        ///     animated or the output from another animation.
        /// </summary>
        /// <param name="toValue">The destination value of the animation. </param>
        /// <param name="duration">
        ///     The length of time the animation takes to play from start to finish, once. See the Duration
        ///     property for more information.
        /// </param>
        protected LinearAnimationBase(T toValue, Duration duration)
            : this()
        {
            To = toValue;
            Duration = duration;
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="LinearAnimationBase{T}" /> class that animates to the specified
        ///     value over the specified duration and has the specified fill behavior. The starting value for the animation is the
        ///     base value of the property being animated or the output from another animation.
        /// </summary>
        /// <param name="fillBehavior">Specifies how the animation behaves when it is not active.</param>
        /// <param name="toValue">The destination value of the animation. </param>
        /// <param name="duration">
        ///     The length of time the animation takes to play from start to finish, once. See the Duration
        ///     property for more information.
        /// </param>
        protected LinearAnimationBase(T toValue, Duration duration, FillBehavior fillBehavior)
            : this(toValue, duration)
        {
            FillBehavior = fillBehavior;
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="LinearAnimationBase{T}" /> class that animates from the specified
        ///     starting value to the specified destination value over the specified duration.
        /// </summary>
        /// <param name="toValue">The destination value of the animation.</param>
        /// <param name="duration">
        ///     The length of time the animation takes to play from start to finish, once. See the Duration
        ///     property for more information.
        /// </param>
        /// <param name="fromValue">The starting value of the animation.</param>
        protected LinearAnimationBase(T fromValue, T toValue, Duration duration)
            : this()
        {
            From = fromValue;
            To = toValue;
            Duration = duration;
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="LinearAnimationBase{T}" /> class that animates from the specified
        ///     starting value to the specified destination value over the specified duration and has the specified fill behavior.
        /// </summary>
        /// <param name="fillBehavior">Specifies how the animation behaves when it is not active.</param>
        /// <param name="toValue">The destination value of the animation. </param>
        /// <param name="duration">
        ///     The length of time the animation takes to play from start to finish, once. See the Duration
        ///     property for more information.
        /// </param>
        /// <param name="fromValue">The starting value of the animation.</param>
        protected LinearAnimationBase(T fromValue, T toValue, Duration duration, FillBehavior fillBehavior)
            : this(fromValue, toValue, duration)
        {
            FillBehavior = fillBehavior;
        }

        #endregion

        #region Clone/Freeze Method

        /// <summary>
        ///     Creates a modifiable clone of this <see cref="LinearAnimationBase{T}" />, making deep copies of this object's
        ///     values. When copying dependency properties, this method copies resource references and data bindings (but they
        ///     might no longer resolve) but not animations or their current values.
        /// </summary>
        /// <returns>
        ///     A modifiable clone of the current object. The cloned object's
        ///     <see cref="P:System.Windows.Freezable.IsFrozen" /> property will be false even if the source's
        ///     <see cref="P:System.Windows.Freezable.IsFrozen" /> property was true.
        /// </returns>
        public new LinearAnimationBase<T> Clone()
        {
            return (LinearAnimationBase<T>) base.Clone();
        }

        /// <summary>
        ///     When implemented in a derived class, creates a new instance of the <see cref="T:System.Windows.Freezable" />
        ///     derived class.
        /// </summary>
        /// <returns>The new instance.</returns>
        protected abstract override Freezable CreateInstanceCore();

        #endregion

        #region Animation Methods

        private static void OnAnimationFunctionChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            LinearAnimationBase<T> anim = (LinearAnimationBase<T>) d;
            anim._isAnimationFunctionValid = false;
            anim.InvalidateProperty(e.Property);
        }

        /// <summary>
        ///     Calculates a value that represents the current value of the property being animated, as determined by the
        ///     <see cref="T:LinearAnimationBase{T}" />.
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
        ///     An <see cref="T:AnimationClock" /> that generates the
        ///     <see cref="P:System.Windows.Media.Animation.Clock.CurrentTime" /> or
        ///     <see cref="P:System.Windows.Media.Animation.Clock.CurrentProgress" /> used by the animation.
        /// </param>
        protected override T GetCurrentValueCore(T defaultOriginValue, T defaultDestinationValue,
            AnimationClock animationClock)
        {
            if (!_isAnimationFunctionValid)
            {
                ValidateAnimationFunction();
            }
            // ReSharper disable once PossibleInvalidOperationException
            double progress = animationClock.CurrentProgress.Value;
            T origin = new T();
            T destination = new T();
            T cumulative = new T();
            T additive = new T();
            bool isFrom = false;
            bool isTo = false;
            switch (_animationType)
            {
                case AnimationType.Automatic:
                    origin = defaultOriginValue;
                    destination = defaultDestinationValue;
                    isFrom = true;
                    isTo = true;
                    break;

                case AnimationType.From:
                    origin = _keyValues[0];
                    destination = defaultDestinationValue;
                    isTo = true;
                    break;

                case AnimationType.To:
                    origin = defaultOriginValue;
                    destination = _keyValues[0];
                    isFrom = true;
                    break;

                case AnimationType.By:
                    destination = _keyValues[0];
                    additive = defaultOriginValue;
                    isFrom = true;
                    break;

                case AnimationType.FromTo:
                    origin = _keyValues[0];
                    destination = _keyValues[1];
                    if (IsAdditive)
                    {
                        additive = defaultOriginValue;
                        isFrom = true;
                    }
                    break;

                case AnimationType.FromBy:
                    origin = _keyValues[0];
                    destination = Calculator.Add(_keyValues[0], _keyValues[1]);
                    if (IsAdditive)
                    {
                        additive = defaultOriginValue;
                        isFrom = true;
                    }
                    break;
            }
            if (isFrom && !Calculator.IsValidAnimationValue(defaultOriginValue))
            {
                throw new ArgumentException(SR.LinearAnimationBase_InvalidDefaultValue, "defaultOriginValue");
            }
            if (isTo && !Calculator.IsValidAnimationValue(defaultDestinationValue))
            {
                throw new ArgumentException(SR.LinearAnimationBase_InvalidDefaultValue, "defaultDestinationValue");
            }
            if (IsCumulative)
            {
                int? iteration = animationClock.CurrentIteration;
                iteration = iteration - 1;
                double iterationDouble = iteration.GetValueOrDefault();
                if (iterationDouble > 0)
                {
                    cumulative = Calculator.Scale(Calculator.Subtract(destination, origin), iterationDouble);
                }
            }
            return Calculator.Add(additive,
                Calculator.Add(cumulative, Calculator.Interpolate(origin, destination, progress)));
        }

        private void ValidateAnimationFunction()
        {
            _animationType = AnimationType.Automatic;
            _keyValues = null;
            if (From != null)
            {
                if (To != null)
                {
                    _animationType = AnimationType.FromTo;
                    _keyValues = new[] {From.Value, To.Value};
                }
                else if (By != null)
                {
                    _animationType = AnimationType.FromBy;
                    _keyValues = new[] {From.Value, By.Value};
                }
                else
                {
                    _animationType = AnimationType.From;
                    _keyValues = new[] {From.Value};
                }
            }
            else if (To != null)
            {
                _animationType = AnimationType.To;
                _keyValues = new[] {To.Value};
            }
            else if (By != null)
            {
                _animationType = AnimationType.By;
                _keyValues = new[] {By.Value};
            }
            _isAnimationFunctionValid = true;
        }

        private static bool ValidateFromToOrByValue(object value)
        {
            T? val = (T?) value;
            if (val != null && SafeCalculator != null)
            {
                return SafeCalculator.IsValidAnimationValue(val.Value);
            }
            return true;
        }

        #endregion
    }
}