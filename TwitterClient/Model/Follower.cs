using System.Collections.Generic;

namespace TwitterClient.Model
{
    public class Follower : Contracts.NotifyPropertyChangedBase
    {
        private string _screenName;
        public string ScreenName
        {
            get { return _screenName; }
            set { _screenName = value; OnPropertyChanged("ScreenName"); }
        }


        private string _idStr;
        public string IdStr
        {
            get { return _idStr; }
            set { _idStr = value; OnPropertyChanged("IdStr"); }
        }


        private string _noOfReplies;
        public string NoOfReplies
        {
            get { return _noOfReplies; }
            set { _noOfReplies = value; OnPropertyChanged("NoOfReplies"); }
        }


        private string _filter;
        public string Filter
        {
            get { return _filter; }
            set { _filter = value; OnPropertyChanged("Filter"); }
        }


        private string _displayTime;
        public string DisplayTime
        {
            get { return _displayTime; }
            set { _displayTime = value; OnPropertyChanged("DisplayTime"); }
        }

        public List<string> Messages { get; set; } = new List<string>();
    }
}
