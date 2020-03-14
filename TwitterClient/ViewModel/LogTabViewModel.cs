using System.Collections.ObjectModel;
using System.Windows;
using TwitterClient.Models;

namespace TwitterClient.ViewModel
{
    public class LogTabViewModel
    {
        public ObservableCollection<Log> Logs { get; }

        public LogTabViewModel()
        {
            Logs = new ObservableCollection<Log>();
        }

        public void Log(string text)
        {
            Logs.Add(new Log(text));
        }

        public void LogOnGuiThread(string Text)
        {
            Application.Current.Dispatcher.Invoke(() => Log(Text));
        }
    }
}
