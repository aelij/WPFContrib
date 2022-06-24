using Avalon.Windows.Controls;

namespace WpfContribTest.Pages;

/// <summary>
///     Interaction logic for NotifyIcon.xaml
/// </summary>
public partial class NotifyIconDemo : Page
{
    private readonly Storyboard _iconAnimation;

    public NotifyIconDemo()
    {
        InitializeComponent();

        _iconAnimation = Resources["IconAnimation"] as Storyboard;
    }

    private void Button_Click(object sender, RoutedEventArgs e)
    {
        notifyIcon.ShowBalloonTip(1000, "Balloon", "Balloon tip demo.", NotifyBalloonIcon.Info);
    }

    private void CheckBox_Checked(object sender, RoutedEventArgs e)
    {
        _iconAnimation.Begin(this, true);
    }

    private void CheckBox_Unchecked(object sender, RoutedEventArgs e)
    {
        _iconAnimation.Stop(this);
    }
}