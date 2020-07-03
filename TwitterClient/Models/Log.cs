using System;
using TwitterClient.ViewModels;

namespace TwitterClient.Models
{
    public class Log : BaseViewModel
    {
        public Log(string text)
        {
            string nowStr = DateTime.Now.ToString("dd/MM HH:mm");
            Text = $"[{nowStr}] {text}";
        }

        private string _text;
        public string Text
        {
            get
            {
                return _text;
            }
            private set
            {
                _text = value;
                RaisePropChanged("LogData");
            }
        }
    }
}
