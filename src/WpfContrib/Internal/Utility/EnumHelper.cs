using System;
using System.Globalization;
using Avalon.Windows;

// ReSharper disable once CheckNamespace
namespace Avalon.Internal.Utility
{
    internal static class EnumHelper
    {
        public static ulong ToUInt64(object value)
        {
            switch (Convert.GetTypeCode(value))
            {
                case TypeCode.SByte:
                case TypeCode.Int16:
                case TypeCode.Int32:
                case TypeCode.Int64:
                    return (ulong) Convert.ToInt64(value, CultureInfo.InvariantCulture);

                case TypeCode.Byte:
                case TypeCode.UInt16:
                case TypeCode.UInt32:
                case TypeCode.UInt64:
                    return Convert.ToUInt64(value, CultureInfo.InvariantCulture);
            }

            throw new InvalidOperationException(SR.EnumHelper_InvalidObjectType);
        }
    }
}