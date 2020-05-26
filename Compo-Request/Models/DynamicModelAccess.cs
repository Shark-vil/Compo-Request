using Compo_Shared_Data.WPF.Models;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace Compo_Request.Models
{
    public class DynamicModelAccess : INotifyPropertyChanged
    {
        private WAccess Access;

        public DynamicModelAccess()
        {
            Access = new WAccess();
        }

        public DynamicModelAccess(WAccess Access)
        {
            this.Access = Access;
        }

        public WAccess GetWAccess()
        {
            return Access;
        }

        public string Key
        {
            get { return Access.Key; }
            set
            {
                Access.Key = value;
                OnPropertyChanged();
            }
        }

        public string Description
        {
            get { return Access.Description; }
            set
            {
                Access.Description = value;
                OnPropertyChanged();
            }
        }

        public bool IsSelected
        {
            get { return Access.IsSelected; }
            set
            {
                Access.IsSelected = value;
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
