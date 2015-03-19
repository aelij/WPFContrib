using System.Windows;
using System.Windows.Controls;

namespace WpfContribTest.Pages
{
    /// <summary>
    ///     Interaction logic for FolderBrowserDialog.xaml
    /// </summary>
    public partial class FolderBrowserDialog : Page
    {
        public FolderBrowserDialog()
        {
            InitializeComponent();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            Avalon.Windows.Dialogs.FolderBrowserDialog dialog = new Avalon.Windows.Dialogs.FolderBrowserDialog
            {
                ShowEditBox = ShowEditBox.IsChecked == true,
                BrowseShares = BrowseShares.IsChecked == true
            };
            if (dialog.ShowDialog() == true)
            {
                folder.Text = dialog.SelectedPath;
            }
        }
    }
}