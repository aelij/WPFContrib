using System;

namespace Avalon.Windows.Media.Animation
{
    /// <summary>
    ///     Provides an implementation of the <see cref="IAnimationCalculator{T}" /> interface
    ///     for the <see cref="System.Double" /> type.
    /// </summary>
    public class DoubleAnimationCalculator : IAnimationCalculator<double>
    {
        /// <summary>
        ///     Adds the specified values.
        /// </summary>
        /// <param name="value1">The value1.</param>
        /// <param name="value2">The value2.</param>
        /// <returns></returns>
        public double Add(double value1, double value2)
        {
            return value1 + value2;
        }

        /// <summary>
        ///     Subtracts the specified values.
        /// </summary>
        /// <param name="value1">The value1.</param>
        /// <param name="value2">The value2.</param>
        /// <returns></returns>
        public double Subtract(double value1, double value2)
        {
            return value1 - value2;
        }

        /// <summary>
        ///     Scales the specified value.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <param name="factor">The factor.</param>
        /// <returns></returns>
        public double Scale(double value, double factor)
        {
            return (value*factor);
        }

        /// <summary>
        ///     Interpolates the specified value for the given progress.
        /// </summary>
        /// <param name="from">From.</param>
        /// <param name="to">To.</param>
        /// <param name="progress">The progress.</param>
        /// <returns></returns>
        public double Interpolate(double from, double to, double progress)
        {
            return (from + ((to - from)*progress));
        }

        /// <summary>
        ///     Gets the length of the segment.
        /// </summary>
        /// <param name="from">From.</param>
        /// <param name="to">To.</param>
        /// <returns></returns>
        public double GetSegmentLength(double from, double to)
        {
            return Math.Abs(to - @from);
        }

        /// <summary>
        ///     Gets the zero value for the given base value.
        /// </summary>
        /// <param name="baseValue">The base value.</param>
        /// <returns></returns>
        public double GetZeroValue(double baseValue)
        {
            return 0;
        }

        /// <summary>
        ///     Determines whether the specified value is valid.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns>
        ///     <c>true</c> if specified value is valid; otherwise, <c>false</c>.
        /// </returns>
        public bool IsValidAnimationValue(double value)
        {
            return !double.IsInfinity(value) && !double.IsNaN(value);
        }
    }
}