using System.Windows.Controls;

namespace TweetAutoReplier.Views
{
    /// <summary>
    /// Interaction logic for MainTabView.xaml
    /// </summary>
    public partial class MainTabView : UserControl
    {
        public MainTabView()
        {
            InitializeComponent();
        }

        private void lvFollowers_ContextMenuOpening(object sender, ContextMenuEventArgs e)
        {
            e.Handled = lvFollowers.SelectedIndex == -1;
        }
    }
}
