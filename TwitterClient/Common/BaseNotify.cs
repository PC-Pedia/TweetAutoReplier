using System.ComponentModel;

namespace TwitterClient.Common
{
    abstract public class BaseNotify : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        public void RaisePropChanged(string propName)
        {
            var e = new PropertyChangedEventArgs(propName);

            PropertyChanged?.Invoke(this, e);
        }
    }
}
