using System.Globalization;
using Avalon.Windows;

namespace Avalon.Internal.Utility;

internal static class EnumHelper
{
    public static ulong ToUInt64(object value) => Convert.GetTypeCode(value) switch
    {
        TypeCode.SByte or TypeCode.Int16 or TypeCode.Int32 or TypeCode.Int64 => (ulong)Convert.ToInt64(value, CultureInfo.InvariantCulture),
        TypeCode.Byte or TypeCode.UInt16 or TypeCode.UInt32 or TypeCode.UInt64 => Convert.ToUInt64(value, CultureInfo.InvariantCulture),
        _ => throw new InvalidOperationException(SR.EnumHelper_InvalidObjectType),
    };
}