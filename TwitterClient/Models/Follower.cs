using System.Collections.Generic;
using TwitterClient.ViewModels;

namespace TwitterClient.Models
{
    public class Follower : BaseViewModel
    {
        private string _screenName;
        public string ScreenName
        {
            get { return _screenName; }
            set { _screenName = value; RaisePropChanged(); }
        }


        private string _idStr;
        public string IdStr
        {
            get { return _idStr; }
            set { _idStr = value; RaisePropChanged(); }
        }


        private string _noOfReplies;
        public string NoOfReplies
        {
            get { return _noOfReplies; }
            set { _noOfReplies = value; RaisePropChanged(); }
        }


        private string _filter;
        public string Filter
        {
            get { return _filter; }
            set { _filter = value; RaisePropChanged(); }
        }


        private string _displayTime;
        public string DisplayTime
        {
            get { return _displayTime; }
            set { _displayTime = value; RaisePropChanged(); }
        }

        public List<string> Messages { get; set; } = new List<string>();
    }
}
