using Microsoft.VisualBasic;
using System;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Media.Animation;
using TweetAutoReplier.Common;
using TweetAutoReplier.Models;
using Tweetinvi.Client;
using Tweetinvi.Core.Models;
using Tweetinvi.Events;
using Tweetinvi.Models;
using Tweetinvi.Parameters;
using Tweetinvi.Streaming;

namespace TweetAutoReplier.ViewModels
{
    public class MainTabViewModel : BaseViewModel
    {
        private readonly IFilteredStream _stream;
        private readonly IAppTabsViewModels _tabs;

        public ObservableCollection<Follower> Followers { get; }

        public MainTabViewModel(IAppTabsViewModels tabs)
        {
            _tabs = tabs;

            _stream = _tabs.Client.Streams.CreateFilteredStream();
            _stream.StreamStopped += StreamOnStreamStopped;
            _stream.StreamStarted += StreamStarted;
            _stream.MatchingTweetReceived += TweetReceived;

            Followers = new ObservableCollection<Follower>();
            Followers.CollectionChanged += FollowersChanged;

            StartStreamCommand = new RelayCommand<object>(StartStreamClicked);
            StopStreamCommand = new RelayCommand<object>(StopStreamClicked);

            AddFollowerCommand = new RelayCommand<object>(AddFollowerClicked);
            EditFollowerCommand = new RelayCommand<Follower>(EditFollowerClicked);
            DeleteFollowerCommand = new RelayCommand<Follower>(DeleteFollowerClicked);
            GetTweetFollowerCommand = new RelayCommand<Follower>(GetTweetClicked);
        }

        private string _screenName;
        public string ScreenName
        {
            get
            {
                return _screenName;
            }
            set
            {
                _screenName = value;
                RaisePropChanged();
            }
        }

        private string _filter;
        public string Filter
        {
            get
            {
                return _filter;
            }
            set
            {
                _filter = value;
                RaisePropChanged();
            }
        }

        private bool _displayTime;
        public bool DisplayTime
        {
            get
            {
                return _displayTime;
            }
            set
            {
                _displayTime = value;
                RaisePropChanged();
            }
        }

        private bool StreamActive => _stream.StreamState == StreamState.Running;

        private async void AddFollowerClicked(object obj)
        {
            if (StreamActive)
            {
                MessageBox.Show("Stop the stream before adding new followers.");
                return;
            }

            if (string.IsNullOrWhiteSpace(ScreenName))
            {
                MessageBox.Show("Screen name is null or empty.");
                return;
            }

            Follower follower = Followers.SingleOrDefault(x => x.ScreenName.Equals(ScreenName));

            if (follower == null)
            {
                long id = await GetUserId(ScreenName);

                if (id == 0)
                    return;

                follower = new Follower
                {
                    ScreenName = ScreenName,
                    IdStr = id.ToString(),
                    Filter = Filter,
                    DisplayTime = DisplayTime.ToString(),
                    NoOfReplies = _tabs.MessageTabViewModel.MessageList.Count.ToString(),
                    Messages = _tabs.MessageTabViewModel.MessageList.ToList()
                };

                Followers.Add(follower);
            }
            else
            {
                follower.DisplayTime = DisplayTime.ToString();
                follower.Filter = Filter;
                follower.NoOfReplies = _tabs.MessageTabViewModel.MessageList.Count.ToString();
                follower.Messages = _tabs.MessageTabViewModel.MessageList.ToList();
            }

            ScreenName = "";
            Filter = "";

            if (DisplayTime)
                DisplayTime = false;

            if (_tabs.MessageTabViewModel.MessageList.Count > 0)
                _tabs.MessageTabViewModel.MessageList.Clear();
        }

        private void DeleteFollowerClicked(Follower follower)
        {
            Followers.Remove(follower);
        }

        private void EditFollowerClicked(Follower follower)
        {
            ScreenName = follower.ScreenName;
            Filter = follower.Filter;
            DisplayTime = Convert.ToBoolean(follower.DisplayTime);

            if (_tabs.MessageTabViewModel.MessageList.Count > 0)
                _tabs.MessageTabViewModel.MessageList.Clear();

            follower.Messages.ForEach(x =>
            {
                _tabs.MessageTabViewModel.MessageList.Add(x);
            });
        }

        private async void GetTweetClicked(Follower follower)
        {
            string choice = Interaction.InputBox(
                "How many tweets do you want to retrieve?",
                "Question",
                "1"
            );

            if (!string.IsNullOrWhiteSpace(choice))
            {
                if (!int.TryParse(choice, out int count))
                {
                    MessageBox.Show("Input needs to be a number.", "Invalid input");
                    return;
                }

                if (count < 1 || count > 40)
                {
                    MessageBox.Show("Input needs to be between 0 and 41.", "Invalid input");
                    return;
                }

                GetUserTimelineParameters getUserTimelineParameters = new GetUserTimelineParameters(follower.ScreenName)
                {
                    PageSize = count
                };

                System.Windows.Forms.Cursor.Current = System.Windows.Forms.Cursors.WaitCursor;
                var tweets = await _tabs.Client.Timelines.GetUserTimelineAsync(getUserTimelineParameters);
                System.Windows.Forms.Cursor.Current = System.Windows.Forms.Cursors.Default;

                foreach (ITweet tweet in tweets)
                {
                    string createdBy = tweet.CreatedBy.ScreenName;
                    string text = $"Created at: {tweet.CreatedAt.ToLocalTime()}\n{tweet.Text}";
                    MessageBox.Show(text, createdBy);
                }
            }
        }

        private void StartStreamClicked(object obj)
        {
            if (_stream.FollowingUserIds.Count == 0)
            {
                MessageBox.Show("No followers to follow.");
                return;
            }

            if (StreamActive)
            {
                MessageBox.Show("Stream already running.");
                return;
            }

            _stream.StartMatchingAllConditionsAsync();
        }

        private void StopStreamClicked(object obj)
        {
            if (StreamActive)
                _stream.Stop();
        }

        private async Task<long> GetUserId(string name)
        {
            if (string.IsNullOrWhiteSpace(name))
                return 0; 

            try
            {
                System.Windows.Forms.Cursor.Current = System.Windows.Forms.Cursors.WaitCursor;
                IUser user = await _tabs.Client.Users.GetUserAsync(name);
                System.Windows.Forms.Cursor.Current = System.Windows.Forms.Cursors.Default;
                return user.Id;
            }
            catch
            {
                System.Windows.Forms.Cursor.Current = System.Windows.Forms.Cursors.Default;
                MessageBox.Show("User does not exist.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return 0;
            }
        }

        private void StreamStarted(object sender, EventArgs e)
        {
            _tabs.LogTabViewModel.GuiLog("Stream started.");
        }

        private void StreamOnStreamStopped(object sender, StreamStoppedEventArgs e)
        {
            _tabs.LogTabViewModel.GuiLog("Stream stopped.");
        }

        private void TweetReceived(object sender, MatchedTweetReceivedEventArgs e)
        {
            bool IsNotReply(ITweet t)
            {
                return (t.InReplyToScreenName == null && t.InReplyToStatusId == null);
            }

            var tweet = e.Tweet;

            if (!tweet.IsRetweet && IsNotReply(tweet))
            {
                string tweeted = $"{tweet.CreatedBy.ScreenName} tweeted {tweet.Text}";
                _tabs.LogTabViewModel.GuiLog(tweeted);

                var follower = Followers.SingleOrDefault(x => x.IdStr.Equals(tweet.CreatedBy.IdStr));

                if (follower == null)
                    return;

                if (!string.IsNullOrEmpty(follower.Filter) &&
                    !Regex.IsMatch(tweet.Text, follower.Filter, RegexOptions.IgnoreCase))
                    return;

                if (follower.Messages.Count > 0)
                {
                    string reply = $"@{tweet.CreatedBy.ScreenName} {follower.Messages.ElementAt(new Random().Next(follower.Messages.Count))}";

                    if (Convert.ToBoolean(follower.DisplayTime))
                    {
                        string time = DateTime.Now.ToLocalTime().ToString("HH:mm:ss");
                        reply += $" // {time}";
                    }

                    PublishTweetParameters publishTweetParameters = new PublishTweetParameters()
                    {
                        InReplyToTweetId = tweet.Id,
                        Text = reply
                    };

                    _tabs.Client.Tweets.PublishTweetAsync(publishTweetParameters);

                    string sent = $"Sent \"{reply}\" to {tweet.CreatedBy.ScreenName}";
                    _tabs.LogTabViewModel.GuiLog(sent);
                }
            }
        }

        private void FollowersChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            switch (e.Action)
            {
                case NotifyCollectionChangedAction.Add:
                    _stream.AddFollow(long.Parse(((Follower)e.NewItems[0]).IdStr));
                    break;
                case NotifyCollectionChangedAction.Remove:
                    _stream.RemoveFollow(long.Parse(((Follower)e.OldItems[0]).IdStr));
                    break;
                case NotifyCollectionChangedAction.Reset:
                    if (_stream.FollowingUserIds.Count > 0)
                        _stream.ClearFollows();
                    break;
            }
        }

        public ICommand StartStreamCommand { get; set; }
        public ICommand StopStreamCommand { get; set; }

        public ICommand AddFollowerCommand { get; set; }
        public ICommand DeleteFollowerCommand { get; set; }
        public ICommand EditFollowerCommand { get; set; }
        public ICommand GetTweetFollowerCommand { get; set; }
    }
}
