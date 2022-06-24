using System.Globalization;
using System.Xml;
using Avalon.Windows.Converters;
using Microsoft.Win32;

namespace WpfContribTest;

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
public class PageRequirementsConverter : ValueConverter
{
    private static readonly decimal s_maxVersion;
    private static readonly int s_sp;

    public bool AsText { get; set; }

    static PageRequirementsConverter()
    {
        try
        {
            using RegistryKey key = Registry.LocalMachine.OpenSubKey(@"Software\Microsoft\NET Framework Setup\NDP");
            IEnumerable<string> versions = key.GetSubKeyNames().Where(s => s.Length > 3 && s.StartsWith("v"));
            decimal max = 0;
            string maxString = null;
            foreach (string version in versions)
            {
                if (decimal.TryParse(version.AsSpan(1, 3), out decimal v) && v > max)
                {
                    max = v;
                    maxString = version;
                }
            }
            if (max > 0)
            {
                s_maxVersion = max;

                using RegistryKey versionKey = key.OpenSubKey(maxString);
                s_sp = (int)versionKey.GetValue("SP", 0);
            }
        }
        catch
        {
        }
    }

    public override object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value is not XmlElement elem || !decimal.TryParse(elem.GetAttribute("RequiresVersion"), out decimal requiresVersion))
        {
            return AsText ? null : true;
        }

        _ = int.TryParse(elem.GetAttribute("RequiresSp"), out int requiresSp);

        bool result = s_maxVersion > requiresVersion || (s_maxVersion == requiresVersion && s_sp >= requiresSp);
        if (!result && AsText)
        {
            string stringResult = "Requires .NET " + requiresVersion;
            if (s_maxVersion == requiresVersion && s_sp < requiresSp)
            {
                stringResult += " SP" + requiresSp;
            }
            return stringResult;
        }
        return result;
    }
}

public class NotNullConverter : ValueConverter
{
    public override object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value is string s)
        {
            return s.Length > 0;
        }
        return value != null;
    }
}