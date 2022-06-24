namespace Avalon.Windows.Media.Animation;

internal sealed class CornerRadiusAnimationCalculator : IAnimationCalculator<CornerRadius>
{
    private readonly DoubleAnimationCalculator _calc = new();

    public CornerRadius Add(CornerRadius value1, CornerRadius value2)
    {
        return new CornerRadius(_calc.Add(value1.TopLeft, value2.TopLeft), _calc.Add(value1.TopRight, value2.TopRight),
            _calc.Add(value1.BottomRight, value2.BottomRight), _calc.Add(value1.BottomLeft, value2.BottomLeft));
    }

    public CornerRadius Subtract(CornerRadius value1, CornerRadius value2)
    {
        return new CornerRadius(value1.TopLeft - value2.TopLeft, value1.TopRight - value2.TopRight,
            value1.BottomRight - value2.BottomRight, value1.BottomLeft - value2.BottomLeft);
    }

    public CornerRadius Scale(CornerRadius value, double factor)
    {
        return new CornerRadius(_calc.Scale(value.TopLeft, factor), _calc.Scale(value.TopRight, factor),
            _calc.Scale(value.BottomRight, factor), _calc.Scale(value.BottomLeft, factor));
    }

    public CornerRadius Interpolate(CornerRadius from, CornerRadius to, double progress)
    {
        return new CornerRadius(_calc.Interpolate(from.TopLeft, to.TopLeft, progress),
            _calc.Interpolate(from.TopRight, to.TopRight, progress),
            _calc.Interpolate(from.BottomRight, to.BottomRight, progress),
            _calc.Interpolate(from.BottomLeft, to.BottomLeft, progress));
    }

    public CornerRadius GetZeroValue(CornerRadius baseValue)
    {
        return new CornerRadius(_calc.GetZeroValue(baseValue.TopLeft), _calc.GetZeroValue(baseValue.TopRight),
            _calc.GetZeroValue(baseValue.BottomRight), _calc.GetZeroValue(baseValue.BottomLeft));
    }

    public double GetSegmentLength(CornerRadius from, CornerRadius to)
    {
        double result = Math.Pow(_calc.GetSegmentLength(from.TopLeft, to.TopLeft), 2) +
                          Math.Pow(_calc.GetSegmentLength(from.TopRight, to.TopRight), 2) +
                         Math.Pow(_calc.GetSegmentLength(from.BottomRight, to.BottomRight), 2) +
                        Math.Pow(_calc.GetSegmentLength(from.BottomLeft, to.BottomLeft), 2);
        return Math.Sqrt(result);
    }

    public bool IsValidAnimationValue(CornerRadius value)
    {
        return _calc.IsValidAnimationValue(value.TopLeft) ||
               _calc.IsValidAnimationValue(value.TopRight) ||
               _calc.IsValidAnimationValue(value.BottomRight) ||
               _calc.IsValidAnimationValue(value.BottomLeft);
    }
}
