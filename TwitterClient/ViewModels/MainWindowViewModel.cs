using System;
using System.Windows;
using System.Windows.Input;
using Tweetinvi;
using TwitterClient.Common;
using TwitterClient.Constants;
using TwitterClient.FileHandlers;
using TwitterClient.Views;

namespace TwitterClient.ViewModels
{
    public class MainWindowViewModel : BaseViewModel, IAppTabsViewModels
    {
        public MainTabViewModel MainTabViewModel { get; }
        public MessageTabViewModel MessageTabViewModel { get; }
        public LogTabViewModel LogTabViewModel { get; }

        public MainWindowViewModel(MainWindow view)
        {
            view.Closed += viewClosed;
            view.Loaded += viewLoaded;

            ImportClickCommand = new RelayCommand<object>(ImportClicked);
            ExitEventCommand = new RelayCommand<object>(ExitClicked);

            MainTabViewModel = new MainTabViewModel(this);
            MessageTabViewModel = new MessageTabViewModel();
            LogTabViewModel = new LogTabViewModel();
        }

        private string title = "Tweet Auto Reply - ";
        public string Title
        {
            get
            {
                return title;
            }
            set
            {
                title = value;
                RaisePropChanged();
            }
        }

        private void viewLoaded(object sender, RoutedEventArgs e)
        {
            SetCredentials();

            var me = User.GetAuthenticatedUser();

            if (me != null)
                Title += me.ScreenName;

            FollowersFile.LoadCollectionFromFile()?.ForEach(x => MainTabViewModel.Followers.Add(x));
        }

        private void viewClosed(object sender, EventArgs e)
        {
            MainTabViewModel.StopStreamCommand.Execute(null);

            FollowersFile.WriteCollectionToFile(MainTabViewModel.Followers);
        }

        private void ImportClicked(object obj)
        {
            if (MessageBox.Show(
                "Do you want to clear current followers list?",
                "Question",
                MessageBoxButton.YesNo,
                MessageBoxImage.Question) == MessageBoxResult.Yes)
                MainTabViewModel.Followers.Clear();

            FollowersFile.OpenCollectionFromFile()?.ForEach(x => MainTabViewModel.Followers.Add(x));
        }

        private void ExitClicked(object obj)
        {
            Application.Current.Shutdown();
        }

        private void SetCredentials()
        {
            Auth.SetUserCredentials(
                TwitterAuth.ConsumerKey,
                TwitterAuth.ConsumerSecret,
                TwitterAuth.AccessToken,
                TwitterAuth.AccessTokenSecret
            );
        }

        public ICommand ImportClickCommand { get; set; }
        public ICommand ExitEventCommand { get; set; }
    }
}
