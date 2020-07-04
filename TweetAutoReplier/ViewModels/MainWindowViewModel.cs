using System;
using System.Windows;
using System.Windows.Input;
using TweetAutoReplier.Common;
using TweetAutoReplier.Constants;
using TweetAutoReplier.FileHandlers;
using TweetAutoReplier.Views;

namespace TweetAutoReplier.ViewModels
{
    public class MainWindowViewModel : BaseViewModel, IAppTabsViewModels
    {
        public Tweetinvi.TwitterClient Client { get; }
        public MainTabViewModel MainTabViewModel { get; }
        public MessageTabViewModel MessageTabViewModel { get; }
        public LogTabViewModel LogTabViewModel { get; }

        public MainWindowViewModel(MainWindow view)
        {
            Client = new Tweetinvi.TwitterClient(
                TwitterAuth.ConsumerKey,
                TwitterAuth.ConsumerSecret,
                TwitterAuth.AccessToken,
                TwitterAuth.AccessTokenSecret
            );

            view.Loaded += viewLoaded;
            view.Closed += viewClosed;

            ImportClickCommand = new RelayCommand<object>(ImportClicked);
            ExitEventCommand = new RelayCommand<object>(ExitClicked);

            MainTabViewModel = new MainTabViewModel(this);
            MessageTabViewModel = new MessageTabViewModel();
            LogTabViewModel = new LogTabViewModel();
        }

        private string title = "Tweet Auto Reply - ";
        public string Title
        {
            get => title;
            set
            {
                title = value;
                RaisePropChanged();
            }
        }

        private async void viewLoaded(object sender, RoutedEventArgs e)
        {
            var me = await Client.Users.GetAuthenticatedUserAsync();

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

        public ICommand ImportClickCommand { get; set; }
        public ICommand ExitEventCommand { get; set; }
    }
}
