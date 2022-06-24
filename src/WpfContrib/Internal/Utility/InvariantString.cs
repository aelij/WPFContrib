using System.Globalization;

namespace Avalon.Internal.Utility;

internal static class InvariantString
{
    public static string Format(string format, params object[] args)
    {
        return string.Format(CultureInfo.InvariantCulture, format, args);
    }
}