namespace WpfContribTest.Pages;

/// <summary>
///     Interaction logic for Introduction.xaml
/// </summary>
public partial class Introduction
{
    public Introduction()
    {
        InitializeComponent();
        Version.Text = typeof(Avalon.Windows.Controls.TaskDialog).Assembly.GetName().Version.ToString();
    }
}