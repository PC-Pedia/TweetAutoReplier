using System;
using System.Windows;
using System.Windows.Input;

using Tweetinvi;

using TwitterClient.Model;
using TwitterClient.Contracts;
using TwitterClient.Constants;

namespace TwitterClient.ViewModel
{
    public class MainWindowViewModel : NotifyPropertyChangedBase, IAppTabsViewModels
    {
        public MainWindowViewModel()
        {
            CheckConnectivity();

            SetTwitterCredentials();

            _mainWindowClosedCommand = new RelayCommand(new Action<object>(MainWindow_Closed));
            _mainWindowLoadedCommand = new RelayCommand(new Action<object>(MainWindow_Loaded));

            _importClickCommand = new RelayCommand(new Action<object>(ImportClicked));
            _exitApplicationCommand = new RelayCommand(new Action<object>(ExitApplicationClicked));

            MainTabViewModel = new MainTabViewModel(this);
            MessageTabViewModel = new MessageTabViewModel(this);
            LogTabViewModel = new LogTabViewModel(this);
        }

        #region Properties

        public MainTabViewModel MainTabViewModel { get; }
        public MessageTabViewModel MessageTabViewModel { get; }
        public LogTabViewModel LogTabViewModel { get; }

        public string AppTitle { get { return $"Tweet Auto Replier - {User.GetAuthenticatedUser().ScreenName}"; } }

        #endregion

        #region Methods
     
        private void ImportClicked(object arg0)
        {
            if (MessageBox.Show(
                "Do you want to clear current followers list?", 
                "Question",
                MessageBoxButton.YesNo, 
                MessageBoxImage.Question) == MessageBoxResult.Yes) MainTabViewModel.FollowersList.Clear();

            FollowersFile.OpenCollectionFromFile()?.ForEach(x => MainTabViewModel.FollowersList.Add(x));
        }

        private void ExitApplicationClicked(object arg0)
        {
            App.Current.Shutdown();
        }

        private void CheckConnectivity()
        {
            if (!Helpers.IPHelper.IsInternetAvailable)
            {
                MessageBox.Show("Sorry, no active internet connection has been found.", "Connection Failed");
                App.Current.Shutdown();
            }
        }

        private void SetTwitterCredentials()
        {
            Auth.SetUserCredentials(
                TwitterAuth.ConsumerKey,
                TwitterAuth.ConsumerSecret,
                TwitterAuth.AccessToken,
                TwitterAuth.AccessTokenSecret);
        }

        #endregion

        #region Event Handlers

        private void MainWindow_Closed(object arg0)
        {
            MainTabViewModel.StopClickCommand.Execute(null);

            FollowersFile.WriteCollectionToFile(MainTabViewModel.FollowersList);
        }

        private void MainWindow_Loaded(object arg0)
        {
            FollowersFile.LoadCollectionFromFile()?.ForEach(x => MainTabViewModel.FollowersList.Add(x));
        }

        #endregion

        #region Commands

        private ICommand _mainWindowClosedCommand;
        public ICommand MainWindowClosedCommand { get { return _mainWindowClosedCommand; } }

        private ICommand _mainWindowLoadedCommand;
        public ICommand MainWindowLoadedCommand { get { return _mainWindowLoadedCommand; } }

        private ICommand _importClickCommand;
        public ICommand ImportClickCommand { get { return _importClickCommand; } }

        private ICommand _exitApplicationCommand;
        public ICommand ExitApplicationCommand { get { return _exitApplicationCommand; } }

        #endregion
    }
}
