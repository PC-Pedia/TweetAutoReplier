using System;

namespace TwitterClient.Model
{
    public class Log : Contracts.NotifyPropertyChangedBase
    {
        public Log(string text) { LogData = $"[{DateTime.Now.ToLocalTime().ToString("dd/MM HH:mm")}] {text}"; }

        private string _logData;
        public string LogData
        {
            get
            {
                return _logData;
            }
            private set
            {
                _logData = value;
                OnPropertyChanged("LogData");
            }
        }
    }
}
