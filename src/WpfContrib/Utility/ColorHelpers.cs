namespace Avalon.Windows.Utility;

/// <summary>
///     Encapsulates methods relating to colors.
/// </summary>
public static class ColorHelpers
{
    /// <summary>
    ///     Changes a GDI color object to an MIL one.
    /// </summary>
    /// <param name="color">The color.</param>
    /// <returns></returns>
    public static Color GdiToWindows(this System.Drawing.Color color)
    {
        return Color.FromArgb(color.A, color.R, color.G, color.B);
    }

    /// <summary>
    ///     Changes an MIL color object to a GDI one.
    /// </summary>
    /// <param name="color">The color.</param>
    /// <returns></returns>
    public static System.Drawing.Color WindowsToGdi(this Color color)
    {
        return System.Drawing.Color.FromArgb(color.A, color.R, color.G, color.B);
    }
    /// <summary>
    ///     Returns the given color with a darker luminosity.
    /// </summary>
    /// <param name="color">The color.</param>
    /// <param name="factor">The factor.</param>
    /// <returns></returns>
    public static Color Darken(this Color color, float factor)
    {
        CalculateHls(color, out int hue, out int luminosity, out int saturation);

        int luma = NewLuma(luminosity, ShadowAdj, true);
        return GetColorFromHls(hue, luma - ((int)(luma * factor)), saturation);
    }

    /// <summary>
    ///     Returns the given color with a lighter luminosity.
    /// </summary>
    /// <param name="color">The color.</param>
    /// <param name="factor">The factor.</param>
    /// <returns></returns>
    public static Color Lighten(this Color color, float factor)
    {
        CalculateHls(color, out int hue, out int luminosity, out int saturation);

        int luma = NewLuma(luminosity, HilightAdj, true);
        return GetColorFromHls(hue, luminosity + ((int)((luma - luminosity) * factor)), saturation);
    }
    private const int ShadowAdj = -333;
    private const int HilightAdj = 500;
    private const int HlsMax = 240;
    private const int RgbMax = 0xff;
    private const int Undefined = 160; private static void CalculateHls(Color color, out int hue, out int luminosity, out int saturation)
    {
        int r = color.R;
        int g = color.G;
        int b = color.B;
        int max = Math.Max(Math.Max(r, g), b);
        int min = Math.Min(Math.Min(r, g), b);
        int maxPlusMin = max + min;
        luminosity = ((maxPlusMin * HlsMax) + RgbMax) / 510;
        int maxMinusMin = max - min;
        if (maxMinusMin == 0)
        {
            saturation = 0;
            hue = Undefined;
        }
        else
        {
            if (luminosity <= 120)
            {
                saturation = ((maxMinusMin * HlsMax) + (maxPlusMin / 2)) / maxPlusMin;
            }
            else
            {
                saturation = ((maxMinusMin * HlsMax) + ((510 - maxPlusMin) / 2)) / (510 - maxPlusMin);
            }
            int rMax = (((max - r) * 40) + (maxMinusMin / 2)) / maxMinusMin;
            int gMax = (((max - g) * 40) + (maxMinusMin / 2)) / maxMinusMin;
            int bMax = (((max - b) * 40) + (maxMinusMin / 2)) / maxMinusMin;
            if (r == max)
            {
                hue = bMax - gMax;
            }
            else if (g == max)
            {
                hue = 80 + rMax - bMax;
            }
            else
            {
                hue = Undefined + gMax - rMax;
            }
            if (hue < 0)
            {
                hue += HlsMax;
            }
            if (hue > HlsMax)
            {
                hue -= HlsMax;
            }
        }
    }

    private static Color GetColorFromHls(int hue, int luminosity, int saturation)
    {
        byte r;
        byte g;
        byte b;
        if (saturation == 0)
        {
            r = g = b = (byte)(luminosity * 0xff / HlsMax);
        }
        else
        {
            int n2;
            if (luminosity <= 120)
            {
                n2 = ((luminosity * (HlsMax + saturation)) + 120) / HlsMax;
            }
            else
            {
                n2 = luminosity + saturation - (((luminosity * saturation) + 120) / HlsMax);
            }
            int n1 = (2 * luminosity) - n2;
            r = (byte)(((HueToRgb(n1, n2, hue + 80) * 0xff) + 120) / HlsMax);
            g = (byte)(((HueToRgb(n1, n2, hue) * 0xff) + 120) / HlsMax);
            b = (byte)(((HueToRgb(n1, n2, hue - 80) * 0xff) + 120) / HlsMax);
        }
        return Color.FromRgb(r, g, b);
    }

    private static int NewLuma(int luminosity, int n, bool scale)
    {
        if (n == 0)
        {
            return luminosity;
        }
        if (scale)
        {
            if (n > 0)
            {
                return (int)(((luminosity * (0x3e8 - n)) + (0xf1 * n)) / ((long)0x3e8));
            }
            return luminosity * (n + 0x3e8) / 0x3e8;
        }
        int result = luminosity;
        result += (int)(n * HlsMax / ((long)0x3e8));
        if (result < 0)
        {
            result = 0;
        }
        if (result > HlsMax)
        {
            result = HlsMax;
        }
        return result;
    }

    private static int HueToRgb(int n1, int n2, int hue)
    {
        if (hue < 0)
        {
            hue += HlsMax;
        }
        if (hue > HlsMax)
        {
            hue -= HlsMax;
        }
        if (hue < 40)
        {
            return n1 + ((((n2 - n1) * hue) + 20) / 40);
        }
        if (hue < 120)
        {
            return n2;
        }
        if (hue < 160)
        {
            return n1 + ((((n2 - n1) * (160 - hue)) + 20) / 40);
        }
        return n1;
    }
}
