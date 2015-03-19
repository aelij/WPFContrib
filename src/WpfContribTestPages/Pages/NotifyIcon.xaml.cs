using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Animation;
using Avalon.Windows.Controls;

namespace WpfContribTest.Pages
{
    /// <summary>
    ///     Interaction logic for NotifyIcon.xaml
    /// </summary>
    public partial class NotifyIconDemo : Page
    {
        private readonly Storyboard iconAnimation;

        public NotifyIconDemo()
        {
            InitializeComponent();

            iconAnimation = Resources["IconAnimation"] as Storyboard;
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            notifyIcon.ShowBalloonTip(1000, "Balloon", "Balloon tip demo.", NotifyBalloonIcon.Info);
        }

        private void CheckBox_Checked(object sender, RoutedEventArgs e)
        {
            iconAnimation.Begin(this, true);
        }

        private void CheckBox_Unchecked(object sender, RoutedEventArgs e)
        {
            iconAnimation.Stop(this);
        }
    }
}