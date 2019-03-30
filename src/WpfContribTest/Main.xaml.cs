using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Security;
using System.Security.Permissions;
using System.Windows;
using System.Xml;
using Avalon.Windows.Converters;
using Microsoft.Win32;

namespace WpfContribTest
{
    /// <summary>
    ///     Interaction logic for Window1.xaml
    /// </summary>
    public partial class Main
    {
        public Main()
        {
            InitializeComponent();

            Loaded += Main_Loaded;
        }

        private void Main_Loaded(object sender, RoutedEventArgs e)
        {
        }
    }

    #region Converters

    public class PageRequirementsConverter : ValueConverter
    {
        private static readonly bool _isFullTrust;
        private static readonly decimal _maxVersion;
        private static readonly int _sp;

        public bool AsText { get; set; }

        static PageRequirementsConverter()
        {
            try
            {
                new SecurityPermission(PermissionState.Unrestricted).Demand();
                _isFullTrust = true;
            }
            catch (SecurityException)
            {
            }

            try
            {
                using (RegistryKey key = Registry.LocalMachine.OpenSubKey(@"Software\Microsoft\NET Framework Setup\NDP")
                    )
                {
                    IEnumerable<string> versions = key.GetSubKeyNames().Where(s => s.Length > 3 && s.StartsWith("v"));
                    decimal max = 0;
                    string maxString = null;
                    foreach (string version in versions)
                    {
                        decimal v;
                        if (decimal.TryParse(version.Substring(1, 3), out v) && v > max)
                        {
                            max = v;
                            maxString = version;
                        }
                    }
                    if (max > 0)
                    {
                        _maxVersion = max;

                        using (RegistryKey versionKey = key.OpenSubKey(maxString))
                        {
                            _sp = (int) versionKey.GetValue("SP", 0);
                        }
                    }
                }
            }
            catch
            {
            }
        }

        public override object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            XmlElement elem = value as XmlElement;
            if (elem != null)
            {
                bool requiresFullTrust = false;
                bool.TryParse(elem.GetAttribute("RequiresFullTrust"), out requiresFullTrust);

                if (requiresFullTrust && !_isFullTrust)
                {
                    return AsText ? "Requires full trust" : (object) false;
                }

                decimal requiresVersion;
                if (decimal.TryParse(elem.GetAttribute("RequiresVersion"), out requiresVersion))
                {
                    int requiresSp;
                    int.TryParse(elem.GetAttribute("RequiresSp"), out requiresSp);

                    bool result = _maxVersion > requiresVersion || (_maxVersion == requiresVersion && _sp >= requiresSp);
                    if (!result && AsText)
                    {
                        string stringResult = "Requires .NET " + requiresVersion;
                        if (_maxVersion == requiresVersion && _sp < requiresSp)
                        {
                            stringResult += " SP" + requiresSp;
                        }
                        return stringResult;
                    }
                    return result;
                }
            }
            return AsText ? null : (object) true;
        }
    }

    public class NotNullConverter : ValueConverter
    {
        public override object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            string s = value as string;
            if (s != null)
            {
                return s.Length > 0;
            }
            return value != null;
        }
    }

    #endregion
}