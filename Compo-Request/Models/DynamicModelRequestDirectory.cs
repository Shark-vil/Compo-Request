using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Text;

namespace Compo_Request.Models
{
    public class DynamicModelRequestDirectory : INotifyPropertyChanged
    {
        private int _CollectionIndex { get; set; }
        private string _RequestDir { get; set; }
        private string _WebRequest { get; set; }
        private string _RequestMethod { get; set; }

        public DynamicModelRequestDirectory() { }
        public DynamicModelRequestDirectory(int CollectionIndex, string RequestDir, string WebRequest, string RequestMethod)
        {
            _CollectionIndex = CollectionIndex;
            _RequestDir = RequestDir;
            _WebRequest = WebRequest;
            _RequestMethod = RequestMethod;
        }

        public int CollectionIndex
        {
            get { return _CollectionIndex; }
            set
            {
                _CollectionIndex = value;
                OnPropertyChanged();
            }
        }

        public string RequestDir
        {
            get { return _RequestDir; }
            set
            {
                _RequestDir = value;
                OnPropertyChanged();
            }
        }

        public string WebRequest
        {
            get { return _WebRequest; }
            set
            {
                _WebRequest = value;
                OnPropertyChanged();
            }
        }

        public string RequestMethod
        {
            get { return _RequestMethod; }
            set
            {
                _RequestMethod = value;
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
