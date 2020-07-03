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
            EditMessageCommand = new RelayCommand(EditMessageClicked);
            UpdateMessageCommand = new RelayCommand(UpdateMessageClicked);
            DeleteMessageCommand = new RelayCommand(DeleteMessageClicked);
            ClearMessagesCommand = new RelayCommand(ClearMessagesClicked);
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
                RaisePropChanged("MessageString");
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
                RaisePropChanged("CharactersLeft");
            }
        }

        public int MaxCharLimit
        {
            get
            {
                return General.MaxMessageLength;
            }
        }

        private void AddMessageClicked(object obj)
        {
            MessageList.Add(MessageString);
            MessageString = String.Empty;
        }

        private int _idxMessage;

        private void UpdateMessageClicked(object obj)
        {
            MessageList[_idxMessage] = MessageString;
            MessageString = "";
        }

        private void EditMessageClicked(object obj)
        {
            MessageString = MessageList.ElementAt((int)obj);
            _idxMessage = (int)obj;
        }

        private void DeleteMessageClicked(object obj)
        {
            MessageList.RemoveAt((int)obj);
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
