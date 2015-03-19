// ReSharper disable once CheckNamespace
namespace Avalon.Windows.Internal.Utility
{
    internal static class BooleanBoxes
    {
        internal static readonly object FalseBox = false;
        internal static readonly object TrueBox = true;

        internal static object Box(this bool value)
        {
            if (value)
            {
                return TrueBox;
            }
            return FalseBox;
        }
    }
}
