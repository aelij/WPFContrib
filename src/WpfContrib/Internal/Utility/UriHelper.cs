using System.Reflection;

namespace Avalon.Internal.Utility;

internal static class UriHelper
{
    public static Uri MakePackUri(string relativeFile)
    {
        string uriString = "pack://application:,,,/" + AssemblyShortName + ";component/" + relativeFile;
        return new Uri(uriString);
    }

    private static string s_assemblyShortName;

    private static string AssemblyShortName
    {
        get
        {
            if (s_assemblyShortName == null)
            {
                Assembly assembly = typeof(UriHelper).Assembly;

                // pull out the short name
                s_assemblyShortName = assembly.ToString().Split(',')[0];
            }

            return s_assemblyShortName;
        }
    }
}