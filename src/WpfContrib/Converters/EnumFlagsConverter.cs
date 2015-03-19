using System;
using System.Collections.Generic;
using System.Globalization;
using Avalon.Internal.Utility;

namespace Avalon.Windows.Converters
{
    /// <summary>
    ///     Converts an enumeration value flags to an array of separate enumeration values.
    /// </summary>
    public class EnumFlagsConverter : ValueConverter
    {
        /// <summary>
        ///     Converts a value.
        /// </summary>
        /// <param name="value">The value produced by the binding source.</param>
        /// <param name="targetType">The type of the binding target property.</param>
        /// <param name="parameter">The converter parameter to use.</param>
        /// <param name="culture">The culture to use in the converter.</param>
        /// <returns>
        ///     A converted value. If the method returns null, the valid null value is used.
        /// </returns>
        public override object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            List<object> values = new List<object>();

            Array enumValues = Enum.GetValues(value.GetType());

            ulong intValue = EnumHelper.ToUInt64(value);

            foreach (object enumValue in enumValues)
            {
                ulong intEnumValue = EnumHelper.ToUInt64(enumValue);

                if (intEnumValue != 0 && (intEnumValue & intValue) == intEnumValue)
                {
                    values.Add(enumValue);
                }
            }

            return values.ToArray();
        }
    }
}