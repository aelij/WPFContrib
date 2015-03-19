using System;
using System.Reflection;

// ReSharper disable once CheckNamespace
namespace Avalon.Internal.Utility
{
    internal static class UriHelper
    {
        public static Uri MakePackUri(string relativeFile)
        {
            string uriString = "pack://application:,,,/" + AssemblyShortName + ";component/" + relativeFile;
            return new Uri(uriString);
        }

        private static string _assemblyShortName;

        private static string AssemblyShortName
        {
            get
            {
                if (_assemblyShortName == null)
                {
                    Assembly assembly = typeof (UriHelper).Assembly;

                    // pull out the short name
                    _assemblyShortName = assembly.ToString().Split(',')[0];
                }

                return _assemblyShortName;
            }
        }
    }
}