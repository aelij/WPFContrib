namespace WpfContribTest.Pages;

/// <summary>
///     Interaction logic for Converters.xaml
/// </summary>
public partial class Converters : Page
{
    public Converters()
    {
        InitializeComponent();

        DataContext = new DataClass { Color = "Red" };
    }

    public class DataClass
    {
        public string Color { get; set; }
    }
}