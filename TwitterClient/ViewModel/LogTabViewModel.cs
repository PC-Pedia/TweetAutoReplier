using System.Collections.ObjectModel;
using TwitterClient.Contracts;
using TwitterClient.Model;

namespace TwitterClient.ViewModel
{
    public class LogTabViewModel
    {
        #region Fields

        private IAppTabsViewModels _IAppTabsViewModels;

        #endregion

        public LogTabViewModel(IAppTabsViewModels IAppTabsViewModels)
        {
            _IAppTabsViewModels = IAppTabsViewModels;
        }

        #region Properties

        public ObservableCollection<Log> LogList { get; } = new ObservableCollection<Log>();

        #endregion

        #region Methods

        public void Log(string Text)
        {
            LogList.Add(new Log(Text));
        }

        public void SafeLog(string Text)
        {
            App.Current.Dispatcher.Invoke(() => Log(Text));
        }

        #endregion
    }
}
