namespace Avalon.Windows.Media.Animation
{
    /// <summary>
    ///     Provides methods for performing animation-related calculations.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public interface IAnimationCalculator<T>
        where T : struct
    {
        /// <summary>
        ///     Adds the specified values.
        /// </summary>
        /// <param name="value1">The value1.</param>
        /// <param name="value2">The value2.</param>
        /// <returns></returns>
        T Add(T value1, T value2);

        /// <summary>
        ///     Subtracts the specified values.
        /// </summary>
        /// <param name="value1">The value1.</param>
        /// <param name="value2">The value2.</param>
        /// <returns></returns>
        T Subtract(T value1, T value2);

        /// <summary>
        ///     Scales the specified value.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <param name="factor">The factor.</param>
        /// <returns></returns>
        T Scale(T value, double factor);

        /// <summary>
        ///     Interpolates the specified value for the given progress.
        /// </summary>
        /// <param name="from">From.</param>
        /// <param name="to">To.</param>
        /// <param name="progress">The progress.</param>
        /// <returns></returns>
        T Interpolate(T from, T to, double progress);

        /// <summary>
        ///     Gets the zero value for the given base value.
        /// </summary>
        /// <param name="baseValue">The base value.</param>
        /// <returns></returns>
        T GetZeroValue(T baseValue);

        /// <summary>
        ///     Gets the length of the segment.
        /// </summary>
        /// <param name="from">From.</param>
        /// <param name="to">To.</param>
        /// <returns></returns>
        double GetSegmentLength(T from, T to);

        /// <summary>
        ///     Determines whether the specified value is valid.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns>
        ///     <c>true</c> if specified value is valid; otherwise, <c>false</c>.
        /// </returns>
        bool IsValidAnimationValue(T value);
    }
}