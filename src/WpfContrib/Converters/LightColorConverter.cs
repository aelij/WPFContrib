using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;
using Avalon.Windows.Utility;

namespace Avalon.Windows.Converters
{
    /// <summary>
    ///     Converts a <see cref="Color" /> to a <see cref="Color" /> with a higher brightness.
    /// </summary>
    public class LightColorConverter : ValueConverter
    {
        private float _factor = 1.0F;

        /// <summary>
        ///     Gets or sets the factor.
        /// </summary>
        /// <value>The factor.</value>
        public float Factor
        {
            get { return _factor; }
            set { _factor = value; }
        }

        /// <summary>
        ///     Converts a value.
        /// </summary>
        /// <param name="value">The value produced by the GTMT#binding source.</param>
        /// <param name="targetType">The type of the GTMT#binding target property.</param>
        /// <param name="parameter">The converter parameter to use.</param>
        /// <param name="culture">The culture to use in the converter.</param>
        /// <returns>
        ///     A converted value. If the method returns null, the valid null value is used.
        /// </returns>
        public override object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            Color? color = value as Color? ?? ColorConverter.ConvertFromString(value as string) as Color?;
            return color != null ? color.Value.Lighten(_factor) : Binding.DoNothing;
        }

        /// <summary>
        ///     Converts a value.
        ///     <remarks>Not implemented.</remarks>
        /// </summary>
        /// <param name="value">The value that is produced by the GTMT#binding target.</param>
        /// <param name="targetType">The type to convert to.</param>
        /// <param name="parameter">The converter parameter to use.</param>
        /// <param name="culture">The culture to use in the converter.</param>
        /// <returns>
        ///     A converted value. If the method returns null, the valid null value is used.
        /// </returns>
        public override object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return Binding.DoNothing;
        }
    }
}