using Compo_Shared_Data.WPF.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Text;

namespace Compo_Request.Models
{
    public class DynamicModelChat : INotifyPropertyChanged
    {
        private ModelChatMessage _ModelChatMessage = new ModelChatMessage();

        public DynamicModelChat() { }
        public DynamicModelChat(ModelChatMessage _ModelChatMessage)
        {
            this._ModelChatMessage = _ModelChatMessage;
        }

        public int Id
        {
            get { return _ModelChatMessage.Id; }
            set
            {
                _ModelChatMessage.Id = value;
                OnPropertyChanged();
            }
        }

        public string FullName
        {
            get { return _ModelChatMessage.FullName; }
            set
            {
                _ModelChatMessage.FullName = value;
                OnPropertyChanged();
            }
        }

        public string Email
        {
            get { return _ModelChatMessage.Email; }
            set
            {
                _ModelChatMessage.Email = value;
                OnPropertyChanged();
            }
        }

        public string Message
        {
            get { return _ModelChatMessage.Message; }
            set
            {
                _ModelChatMessage.Message = value;
                OnPropertyChanged();
            }
        }

        public DateTime Date
        {
            get { return _ModelChatMessage.Date; }
            set
            {
                _ModelChatMessage.Date = value;
                OnPropertyChanged();
            }
        }

        public int ProjectId
        {
            get { return _ModelChatMessage.ProjectId; }
            set
            {
                _ModelChatMessage.ProjectId = value;
                OnPropertyChanged();
            }
        }

        public int UserId
        {
            get { return _ModelChatMessage.UserId; }
            set
            {
                _ModelChatMessage.Id = value;
                OnPropertyChanged();
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string name = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }
    }
}
