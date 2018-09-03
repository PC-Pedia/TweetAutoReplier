using System;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using System.Windows.Input;

using Tweetinvi;
using Tweetinvi.Events;
using Tweetinvi.Models;
using Tweetinvi.Streaming;
using TwitterClient.Contracts;
using TwitterClient.Helpers;
using TwitterClient.Model;

namespace TwitterClient.ViewModel
{
    public class MainTabViewModel : Contracts.NotifyPropertyChangedBase
    {
        #region Fields

        private IAppTabsViewModels _IAppTabsViewModels;
        private IFilteredStream _filteredStream = Stream.CreateFilteredStream();

        #endregion

        public MainTabViewModel(IAppTabsViewModels IAppTabsViewModels)
        {
            _IAppTabsViewModels = IAppTabsViewModels;

            FollowersList.CollectionChanged += FollowersList_CollectionChanged;

            _filteredStream.MatchingTweetReceived += TweetReceived;
            _filteredStream.StreamStopped += StreamStopped;
            _filteredStream.StreamStarted += StreamStarted;

            _startClickCommand = new RelayCommand(new Action<object>(StartClicked));
            _stopClickCommand = new RelayCommand(new Action<object>(StopClicked));

            _addFollowerClickCommand = new RelayCommand(new Action<object>(AddFollowerClicked));
            _editFollowerClickCommand = new RelayCommand(new Action<object>(EditFollowerClicked));
            _deleteFollowerClickCommand = new RelayCommand(new Action<object>(DeleteFollowerClicked));
            _getTweetClickCommand = new RelayCommand(new Action<object>(GetTweetClicked));
        }

        #region Properties

        public ObservableCollection<Follower> FollowersList { get; } = new ObservableCollection<Follower>();

        private string _screenNameString;
        public string ScreenNameString
        {
            get
            {
                return _screenNameString;
            }
            set
            {
                _screenNameString = value;
                OnPropertyChanged("ScreenNameString");
            }
        }

        private string _filterString;
        public string FilterString
        {
            get
            {
                return _filterString;
            }
            set
            {
                _filterString = value;
                OnPropertyChanged("FilterString");
            }
        }

        private bool _IsDisplayTime;
        public bool IsDisplayTime
        {
            get
            {
                return _IsDisplayTime;
            }
            set
            {
                _IsDisplayTime = value;
                OnPropertyChanged("IsDisplayTime");
            }
        }

        private bool isStreamActive { get { return _filteredStream.StreamState == StreamState.Running; } }

        private MessageTabViewModel _messageTabViewModel { get { return _IAppTabsViewModels.MessageTabViewModel; } }

        private LogTabViewModel _logTabViewModel { get { return _IAppTabsViewModels.LogTabViewModel; } }

        #endregion

        #region Methods

        private void AddFollowerClicked(object arg0)
        {
            if (isStreamActive)
            {
                MessageBox.Show("Stream is active!, please stop it before adding new name(s).");
                return;
            }

            if (String.IsNullOrEmpty(ScreenNameString))
            {
                MessageBox.Show("Screen Name is null or empty.");
                return;
            }

            var follower = FollowersList.SingleOrDefault(x => x.ScreenName.Equals(ScreenNameString));

            if (follower == null)
            {
                long id;

                if ((id = getFollowerId(ScreenNameString)) == 0) return;

                FollowersList.Add(new Follower
                {
                    ScreenName = ScreenNameString,
                    IdStr = id.ToString(),
                    Filter = FilterString,
                    DisplayTime = IsDisplayTime.ToString(),
                    NoOfReplies = _messageTabViewModel.MessageList.Count.ToString(),
                    Messages = _messageTabViewModel.MessageList.ToList()
                });
            }
            else
            {
                follower.DisplayTime = IsDisplayTime.ToString();
                follower.Filter = FilterString;
                follower.NoOfReplies = _messageTabViewModel.MessageList.Count.ToString();
                follower.Messages = _messageTabViewModel.MessageList.ToList();
            }

            // reset elements to default values
            ScreenNameString = String.Empty;
            FilterString = String.Empty;
            if (IsDisplayTime) IsDisplayTime = false;
            if (_messageTabViewModel.MessageList.Count > 0) _messageTabViewModel.MessageList.Clear();
            //
        }

        private void DeleteFollowerClicked(object arg0)
        {
            FollowersList.RemoveAt((int)arg0);
        }

        private void EditFollowerClicked(object arg0)
        {
            var follower = FollowersList.ElementAt((int)arg0);

            ScreenNameString = follower.ScreenName;
            FilterString = follower.Filter;
            IsDisplayTime = Convert.ToBoolean(follower.DisplayTime);

            if (_messageTabViewModel.MessageList.Count > 0) _messageTabViewModel.MessageList.Clear();
            follower.Messages.ForEach(x => _messageTabViewModel.MessageList.Add(x));
        }

        private void GetTweetClicked(object arg0)
        {
            var userInput = Microsoft.VisualBasic.Interaction.InputBox("How many tweets do you want to retrieve?\nMaximum tweets can be received is 40.", "Question", "1");

            if (!String.IsNullOrEmpty(userInput))
            {
                int result;

                if (!int.TryParse(userInput, out result))
                {
                    MessageBox.Show("Input needs to be a number.", "Invalid input");
                    return;
                }

                if (result < 1 || result > 40)
                {
                    MessageBox.Show("Input needs to be between 0 to 41.", "Invalid input");
                    return;
                }

                CursorHelper.ShowWaitCursor();
                foreach (ITweet tweet in Timeline.GetUserTimeline(((Follower)arg0).ScreenName, result))
                {
                    MessageBox.Show($"Created at: {tweet.CreatedAt.ToLocalTime()}\n{tweet.Text}", tweet.CreatedBy.ScreenName);
                }
                CursorHelper.ShowDefaultCursor();
            }
        }

        private void StartClicked(object arg0)
        {
            if (_filteredStream.FollowingUserIds.Count == 0)
            {
                MessageBox.Show("No followers.");
                return;
            }

            _filteredStream.StartStreamMatchingAllConditionsAsync();
        }

        private void StopClicked(object arg0)
        {
            if (isStreamActive) _filteredStream.StopStream();
        }

        private long getFollowerId(string name)
        {
            long id = 0;

            try
            {
                if (!String.IsNullOrEmpty(name))
                {
                    CursorHelper.ShowWaitCursor();
                    id = User.GetUserFromScreenName(name).Id;
                    CursorHelper.ShowDefaultCursor();
                }
            }
            catch
            {
                CursorHelper.ShowDefaultCursor();
                MessageBox.Show("User does not exist.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

            return id;
        }

        #endregion

        #region Event Handlers

        private void StreamStarted(object sender, EventArgs e)
        {
            _logTabViewModel.SafeLog("Stream started.");
        }

        private void StreamStopped(object sender, StreamExceptionEventArgs e)
        {
            _logTabViewModel.SafeLog("Stream stopped.");

            if (e.Exception != null)
            {
                _logTabViewModel.SafeLog(e.Exception.Message);

                if (Regex.IsMatch(e.Exception.Message, "exceeded connection limit", RegexOptions.IgnoreCase)) return;

                ExceptionsFile.AppendExceptionToFile(e.Exception);

                for (int x = 5; x-- > 0;)
                {
                    _logTabViewModel.SafeLog($"Restarting in: {x} seconds.");
                    System.Threading.Thread.Sleep(1000);
                }

                StartClicked(null);
            }
        }

        private void TweetReceived(object sender, MatchedTweetReceivedEventArgs e)
        {
            var tweet = e.Tweet;

           // _logTabViewModel.SafeLog($"InReplyToStatusIdStr: {tweet.InReplyToStatusIdStr}, inReplyToScreenName: {tweet.InReplyToScreenName}");

            if (!tweet.IsRetweet && (tweet.InReplyToScreenName == null && tweet.InReplyToStatusId == null))
            {
                _logTabViewModel.SafeLog($"{tweet.CreatedBy.ScreenName} tweeted {tweet.Text}");

                var follower = FollowersList.SingleOrDefault(x => x.IdStr.Equals(tweet.CreatedBy.IdStr));

                if (follower == null) return;

                if (!String.IsNullOrEmpty(follower.Filter) && !Regex.IsMatch(tweet.Text, follower.Filter, RegexOptions.IgnoreCase)) return;

                if (follower.Messages.Count > 0)
                {
                    var textToPublish = $"@{tweet.CreatedBy.ScreenName} {follower.Messages.ElementAt(new Random().Next(follower.Messages.Count))}";

                    if (Convert.ToBoolean(follower.DisplayTime))
                        textToPublish += $" // {DateTime.Now.ToLocalTime().ToString("HH:mm:ss")}";

                    Tweet.PublishTweetInReplyTo(textToPublish, tweet.Id);

                    _logTabViewModel.SafeLog($"Replied '{textToPublish}' to '{tweet.CreatedBy.ScreenName}'");
                }
            }
        }

        private void FollowersList_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            switch (e.Action)
            {
                case NotifyCollectionChangedAction.Add:
                    _filteredStream.AddFollow(long.Parse(((Follower)e.NewItems[0]).IdStr));
                    break;
                case NotifyCollectionChangedAction.Remove:
                    _filteredStream.RemoveFollow(long.Parse(((Follower)e.OldItems[0]).IdStr));
                    break;
                case NotifyCollectionChangedAction.Reset:
                    if (_filteredStream.FollowingUserIds.Count > 0) _filteredStream.ClearFollows();
                    break;
            }
        }

        #endregion

        #region Commands

        private ICommand _startClickCommand;
        public ICommand StartClickCommand { get { return _startClickCommand; } }


        private ICommand _stopClickCommand;
        public ICommand StopClickCommand { get { return _stopClickCommand; } }


        private ICommand _addFollowerClickCommand;
        public ICommand AddFollowerClickCommand { get { return _addFollowerClickCommand; } }


        private ICommand _deleteFollowerClickCommand;
        public ICommand DeleteFollowerClickCommand { get { return _deleteFollowerClickCommand; } }


        private ICommand _editFollowerClickCommand;
        public ICommand EditFollowerClickCommand { get { return _editFollowerClickCommand; } }


        private ICommand _getTweetClickCommand;
        public ICommand GetTweetClickCommand { get { return _getTweetClickCommand; } }

        #endregion
    }
}
