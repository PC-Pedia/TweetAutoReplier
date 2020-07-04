using System.Windows.Controls;

namespace TwitterClient.Views
{
    /// <summary>
    /// Interaction logic for MessageTabView.xaml
    /// </summary>
    public partial class MessageTabView : UserControl
    {
        public MessageTabView()
        {
            InitializeComponent();
        }

        private void lvMessages_ContextMenuOpening(object sender, ContextMenuEventArgs e)
        {
            e.Handled = lvMessages.SelectedIndex == -1;
        }
    }
}
