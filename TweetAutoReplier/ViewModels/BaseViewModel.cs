using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace TweetAutoReplier.ViewModels
{
    public abstract class BaseViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        public void RaisePropChanged([CallerMemberName] string propName = null)
        {
            var e = new PropertyChangedEventArgs(propName);

            PropertyChanged?.Invoke(this, e);
        }
    }
}
