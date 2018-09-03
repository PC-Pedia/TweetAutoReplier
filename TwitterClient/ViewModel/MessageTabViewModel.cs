using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Input;
using TwitterClient.Contracts;
using TwitterClient.Constants;

namespace TwitterClient.ViewModel
{
    public class MessageTabViewModel : Contracts.NotifyPropertyChangedBase
    {
        #region Fields

        private IAppTabsViewModels _IAppTabsViewModels;

        #endregion

        public MessageTabViewModel(IAppTabsViewModels IAppTabsViewModels)
        {
            _IAppTabsViewModels = IAppTabsViewModels;

            _addMessageClickCommand = new RelayCommand(new Action<object>(AddMessageClicked));
            _editMessageClickCommand = new RelayCommand(new Action<object>(EditMessageClicked));
            _updateMessageClickCommand = new RelayCommand(new Action<object>(UpdateMessageClicked));
            _deleteMessageClickCommand = new RelayCommand(new Action<object>(DeleteMessageClicked));
            _clearMessagesClickCommand = new RelayCommand(new Action<object>(ClearMessagesClicked));
        }

        #region Properties

        private string _message;
        public string MessageString
        {
            get
            {
                return _message;
            }
            set
            {
                _message = value;
                OnPropertyChanged("MessageString");
                CharactersLeft = (General.MaxMessageLength - value.Length).ToString();
            }
        }

        private string _charctersLeft = General.MaxMessageLength.ToString();
        public string CharactersLeft
        {
            get
            {
                return $"{_charctersLeft} characters left.";
            }
            set
            {
                _charctersLeft = value;
                OnPropertyChanged("CharactersLeft");
            }
        }

        public ObservableCollection<string> MessageList { get; } = new ObservableCollection<string>();

        #endregion

        #region Methods

        private void AddMessageClicked(object arg0)
        {
            MessageList.Add(MessageString);
            MessageString = String.Empty;
        }

        private int _idxMessage;

        private void UpdateMessageClicked(object arg0)
        {
            MessageList[_idxMessage] = MessageString;
            MessageString = String.Empty;
        }

        private void EditMessageClicked(object arg0)
        {
            MessageString = MessageList.ElementAt((int)arg0);
            _idxMessage = (int)arg0;
        }

        private void DeleteMessageClicked(object arg0)
        {
            MessageList.RemoveAt((int)arg0);
        }

        private void ClearMessagesClicked(object arg0)
        {
            if (MessageList.Count > 0) MessageList.Clear();
        }

        #endregion

        #region Commands

        private ICommand _addMessageClickCommand;
        public ICommand AddMessageClickCommand { get { return _addMessageClickCommand; } }


        private ICommand _editMessageClickCommand;
        public ICommand EditMessageClickCommand { get { return _editMessageClickCommand; } }


        private ICommand _updateMessageClickCommand;
        public ICommand UpdateMessageClickCommand { get { return _updateMessageClickCommand; } }


        private ICommand _deleteMessageClickCommand;
        public ICommand DeleteMessageClickCommand { get { return _deleteMessageClickCommand; } }


        private ICommand _clearMessagesClickCommand;
        public ICommand ClearMessagesClickCommand { get { return _clearMessagesClickCommand; } }

        #endregion
    }
}
