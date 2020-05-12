using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Text;

namespace Compo_Request.Models
{
    public class DynamicModelEditorRequest : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        private ObservableCollection<string> Types = new ObservableCollection<string>
        {
            "GET",
            "POST",
            "PUT",
            "PATCH",
            "DELETE",
            "COPY",
            "HEAD",
            "OPTIONS",
            "LINK",
            "UNLINK",
            "PURGE",
            "LOCK",
            "UNLOCK",
            "PROPFIND",
            "VIEW"
        };
        private string Link = "";

        public DynamicModelEditorRequest() { }
        public DynamicModelEditorRequest(string Link)
        {
            this.Link = Link;
        }
        public DynamicModelEditorRequest(string Link, ObservableCollection<string> Types)
        {
            this.Link = Link;
            this.Types = Types;
        }

        public string RequestLink
        {
            get { return Link; }
            set
            {
                Link = value;
                OnPropertyChanged();
            }
        }

        public ObservableCollection<string> RequestTypes
        {
            get { return Types; }
            set
            {
                Types = value;
                OnPropertyChanged();
            }
        }

        protected void OnPropertyChanged([CallerMemberName] string name = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }
    }
}
