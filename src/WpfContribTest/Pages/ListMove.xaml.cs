using System.Collections.ObjectModel;

namespace WpfContribTest.Pages;

/// <summary>
/// Interaction logic for ListMove.xaml
/// </summary>
public partial class ListMove
{
    public ListMove()
    {
        InitializeComponent();

        TheList.ItemsSource = new ObservableCollection<int>(Enumerable.Range(1, 100));
    }
}
