using System.Collections.Generic;
using TwitterClient.ViewModels;

namespace TwitterClient.Models
{
    public class Follower : BaseViewModel
    {
        private string _screenName;
        public string ScreenName
        {
            get => _screenName;
            set { _screenName = value; RaisePropChanged(); }
        }


        private string _idStr;
        public string IdStr
        {
            get => _idStr;
            set { _idStr = value; RaisePropChanged(); }
        }


        private string _noOfReplies;
        public string NoOfReplies
        {
            get => _noOfReplies;
            set { _noOfReplies = value; RaisePropChanged(); }
        }


        private string _filter;
        public string Filter
        {
            get => _filter;
            set { _filter = value; RaisePropChanged(); }
        }


        private string _displayTime;
        public string DisplayTime
        {
            get => _displayTime;
            set { _displayTime = value; RaisePropChanged(); }
        }

        public List<string> Messages { get; set; }
        public Follower()
        {
            Messages = new List<string>();
        }
    }
}
