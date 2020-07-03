using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Input;
using TwitterClient.Common;
using TwitterClient.Constants;

namespace TwitterClient.ViewModels
{
    public class MessageTabViewModel : BaseViewModel
    {
        public ObservableCollection<string> MessageList { get; }

        public MessageTabViewModel()
        {
            MessageList = new ObservableCollection<string>();

            _charctersLeft = Convert.ToString(MaxCharLimit);

            AddMessageCommand = new RelayCommand(AddMessageClicked);
            ClearMessagesCommand = new RelayCommand(ClearMessagesClicked);
            UpdateMessageCommand = new RelayCommand(UpdateMessageClicked);
            DeleteMessageCommand = new GenericRelayCommand<int>(DeleteMessageClicked);
            EditMessageCommand = new GenericRelayCommand<int>(EditMessageClicked);
        }

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
                RaisePropChanged();
                CharactersLeft = (MaxCharLimit - value.Length).ToString();
            }
        }

        private string _charctersLeft;
        public string CharactersLeft
        {
            get
            {
                return $"{_charctersLeft} characters left.";
            }
            set
            {
                _charctersLeft = value;
                RaisePropChanged();
            }
        }

        public int MaxCharLimit => General.MaxMessageLength;

        private void AddMessageClicked(object obj)
        {
            MessageList.Add(MessageString);
            MessageString = "";
        }

        private int _idxMessage;

        private void UpdateMessageClicked(object obj)
        {
            MessageList[_idxMessage] = MessageString;
            MessageString = "";
        }

        private void EditMessageClicked(int index)
        {
            MessageString = MessageList.ElementAt(index);
            _idxMessage = index;
        }

        private void DeleteMessageClicked(int index)
        {
            MessageList.RemoveAt(index);
        }

        private void ClearMessagesClicked(object obj)
        {
            if (MessageList.Count > 0)
                MessageList.Clear();
        }

        public ICommand AddMessageCommand { get; set; }
        public ICommand EditMessageCommand { get; set; }
        public ICommand UpdateMessageCommand { get; set; }
        public ICommand DeleteMessageCommand { get; set; }
        public ICommand ClearMessagesCommand { get; set; }
    }
}
