using System.Windows;
using TweetAutoReplier.ViewModels;

namespace TweetAutoReplier.Views
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();

            DataContext = new MainWindowViewModel(this);
        }
    }
}
