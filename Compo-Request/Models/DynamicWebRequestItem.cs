using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Text;

namespace Compo_Request.Models
{
    public class DynamicWebRequestItem : INotifyPropertyChanged
    {
        private int _Id { get; set; }
        private string _Link { get; set; }
        private string _Key { get; set; }
        private string _Value { get; set; }
        private string _Description { get; set; }
        private bool _IsDatabase { get; set; }

        public DynamicWebRequestItem() { }
        public DynamicWebRequestItem(int Id, string Link, string Key, string Value, string Description, bool IsDatabase)
        {
            _Id = Id;
            _Link = Link;
            _Key = Key;
            _Value = Value;
            _Description = Description;
            _IsDatabase = IsDatabase;
        }

        public int Id
        {
            get { return _Id; }
            set
            {
                _Id = value;
                OnPropertyChanged();
            }
        }

        public string Link
        {
            get { return _Link; }
            set
            {
                _Link = value;
                OnPropertyChanged();
            }
        }

        public string Key
        {
            get { return _Key; }
            set
            {
                _Key = value;
                OnPropertyChanged();
            }
        }

        public string Value
        {
            get { return _Value; }
            set
            {
                _Value = value;
                OnPropertyChanged();
            }
        }

        public string Description
        {
            get { return _Description; }
            set
            {
                _Description = value;
                OnPropertyChanged();
            }
        }

        public bool IsDatabase
        {
            get { return _IsDatabase; }
            set
            {
                _IsDatabase = value;
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
