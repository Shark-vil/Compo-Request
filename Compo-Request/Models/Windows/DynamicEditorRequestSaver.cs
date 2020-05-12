using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Text;

namespace Compo_Request.Models.Windows
{
    public class DynamicEditorRequestSaver : INotifyPropertyChanged
    {
        private string _RequestName;
        private string _RequestDirectoryName;

        public DynamicEditorRequestSaver() { }
        public DynamicEditorRequestSaver(string RequestName, string RequestDirectoryName)
        {
            _RequestName = RequestName;
            _RequestDirectoryName = RequestDirectoryName;
        }

        public string RequestName
        {
            get { return _RequestName; }
            set
            {
                _RequestName = value;
                OnPropertyChanged();
            }
        }

        public string RequestDirectoryName
        {
            get { return _RequestDirectoryName; }
            set
            {
                _RequestDirectoryName = value;
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
