namespace Avalon.Windows.Internal.Utility;

internal static class BooleanBoxes
{
    public static readonly object FalseBox = false;
    public static readonly object TrueBox = true;

    internal static object Box(this bool value)
    {
        if (value)
        {
            return TrueBox;
        }
        return FalseBox;
    }
}
