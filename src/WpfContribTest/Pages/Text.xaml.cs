using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Animation;
using Avalon.Windows.Controls;

namespace WpfContribTest.Pages
{
    /// <summary>
    ///     Interaction logic for Text.xaml
    /// </summary>
    public partial class Text
    {
        public Text()
        {
            InitializeComponent();
        }

        private void CheckBox_Checked(object sender, RoutedEventArgs e)
        {
            if (AnimatedTextBlock != null)
            {
                if (((CheckBox) sender).IsChecked == true)
                {
                    AnimatedTextBlock.RepeatBehavior = RepeatBehavior.Forever;
                }
                else
                {
                    AnimatedTextBlock.ClearValue(AnimatedTextBlock.RepeatBehaviorProperty);
                }
            }
        }
    }
}
