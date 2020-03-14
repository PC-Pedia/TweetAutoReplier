using Microsoft.VisualBasic;
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
using TwitterClient.Common;
using TwitterClient.Models;

namespace TwitterClient.ViewModel
{
    public class MainTabViewModel : BaseNotify
    {
        private readonly IFilteredStream stream;
        private readonly IAppTabsViewModels tabs;
        public ObservableCollection<Follower> Followers { get; }

        public MainTabViewModel(IAppTabsViewModels tabs)
        {
            this.tabs = tabs;

            stream = Stream.CreateFilteredStream();
            stream.StreamStopped += StreamStopped;
            stream.StreamStarted += StreamStarted;
            stream.MatchingTweetReceived += TweetReceived;

            Followers = new ObservableCollection<Follower>();
            Followers.CollectionChanged += FollowersChanged;

            StartStreamCommand = new RelayCommand(StartStreamClicked);
            StopStreamCommand = new RelayCommand(StopStreamClicked);

            AddFollowerCommand = new RelayCommand(AddFollowerClicked);
            EditFollowerCommand = new RelayCommand(EditFollowerClicked);
            DeleteFollowerCommand = new RelayCommand(DeleteFollowerClicked);
            GetTweetFollowerCommand = new RelayCommand(GetTweetClicked);
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
                RaisePropChanged("ScreenName");
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
                RaisePropChanged("Filter");
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
                RaisePropChanged("DisplayTime");
            }
        }

        private bool StreamActive
        {
            get
            {
                return stream.StreamState == StreamState.Running;
            }
        }

        private void AddFollowerClicked(object obj)
        {
            if (StreamActive)
            {
                MessageBox.Show("Stop the stream before adding new followers.");
                return;
            }

            if (string.IsNullOrEmpty(ScreenName))
            {
                MessageBox.Show("Screen Name is null or empty.");
                return;
            }

            Follower f = Followers.SingleOrDefault(x => x.ScreenName.Equals(ScreenName));

            if (f == null)
            {
                long id;

                if ((id = GetUserId(ScreenName)) == 0)
                    return;

                f = new Follower
                {
                    ScreenName = ScreenName,
                    IdStr = id.ToString(),
                    Filter = Filter,
                    DisplayTime = DisplayTime.ToString(),
                    NoOfReplies = tabs.MessageTabViewModel.MessageList.Count.ToString(),
                    Messages = tabs.MessageTabViewModel.MessageList.ToList()
                };

                Followers.Add(f);
            }
            else
            {
                f.DisplayTime = DisplayTime.ToString();
                f.Filter = Filter;
                f.NoOfReplies = tabs.MessageTabViewModel.MessageList.Count.ToString();
                f.Messages = tabs.MessageTabViewModel.MessageList.ToList();
            }

            ScreenName = "";
            Filter = "";

            if (DisplayTime)
                DisplayTime = false;

            if (tabs.MessageTabViewModel.MessageList.Count > 0)
                tabs.MessageTabViewModel.MessageList.Clear();
        }

        private void DeleteFollowerClicked(object obj)
        {
            int index = (int)obj;
            Followers.RemoveAt(index);
        }

        private void EditFollowerClicked(object obj)
        {
            int index = (int)obj;
            Follower follower = Followers.ElementAt(index);

            ScreenName = follower.ScreenName;
            Filter = follower.Filter;
            DisplayTime = Convert.ToBoolean(follower.DisplayTime);

            if (tabs.MessageTabViewModel.MessageList.Count > 0)
                tabs.MessageTabViewModel.MessageList.Clear();

            follower.Messages.ForEach(x =>
            {
                tabs.MessageTabViewModel.MessageList.Add(x);
            }
            );
        }

        private void GetTweetClicked(object obj)
        {
            string choice = Interaction.InputBox(
                "How many tweets do you want to retrieve?",
                "Question",
                "1"
            );

            if (!string.IsNullOrEmpty(choice))
            {
                int count;

                if (!int.TryParse(choice, out count))
                {
                    MessageBox.Show("Input needs to be a number.", "Invalid input");
                    return;
                }

                if (count < 1 || count > 40)
                {
                    MessageBox.Show("Input needs to be between 0 to 41.", "Invalid input");
                    return;
                }

                Follower f = (Follower)obj;

                System.Windows.Forms.Cursor.Current = System.Windows.Forms.Cursors.WaitCursor;
                var tweets = Timeline.GetUserTimeline(f.ScreenName, count);
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
            if (stream.FollowingUserIds.Count == 0)
            {
                MessageBox.Show("No followers to follow.");
                return;
            }

            stream.StartStreamMatchingAllConditionsAsync();
        }

        private void StopStreamClicked(object obj)
        {
            if (StreamActive)
                stream.StopStream();
        }

        private long GetUserId(string name)
        {
            try
            {
                if (!string.IsNullOrEmpty(name))
                {
                    System.Windows.Forms.Cursor.Current = System.Windows.Forms.Cursors.WaitCursor;
                    IUser user = User.GetUserFromScreenName(name);
                    System.Windows.Forms.Cursor.Current = System.Windows.Forms.Cursors.Default;
                    return user.Id;
                }
            }
            catch
            {
                System.Windows.Forms.Cursor.Current = System.Windows.Forms.Cursors.Default;
                MessageBox.Show("User does not exist.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return 0;
            }

            return 0;
        }

        private void StreamStarted(object sender, EventArgs e)
        {
            // log event
        }

        private void StreamStopped(object sender, StreamExceptionEventArgs e)
        {
            // log event
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
                string text = $"{tweet.CreatedBy.ScreenName} tweeted {tweet.Text}";
                // log text

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

                    Tweet.PublishTweetInReplyTo(reply, tweet.Id);

                    string sent = $"Sent \"{reply}\" to {tweet.CreatedBy.ScreenName}";
                    // log sent
                }
            }
        }

        private void FollowersChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            switch (e.Action)
            {
                case NotifyCollectionChangedAction.Add:
                    stream.AddFollow(long.Parse(((Follower)e.NewItems[0]).IdStr));
                    break;
                case NotifyCollectionChangedAction.Remove:
                    stream.RemoveFollow(long.Parse(((Follower)e.OldItems[0]).IdStr));
                    break;
                case NotifyCollectionChangedAction.Reset:
                    if (stream.FollowingUserIds.Count > 0) stream.ClearFollows();
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
