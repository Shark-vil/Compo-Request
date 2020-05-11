using Compo_Request.Network.Utilities;
using Compo_Shared_Data.Debugging;
using Dragablz;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;

namespace Compo_Request.Windows.Editor
{
    public class BoundModel
    {
        private readonly ObservableCollection<HeaderedItemViewModel> _items;

        public BoundModel()
        {
            _items = new ObservableCollection<HeaderedItemViewModel>();
        }

        public BoundModel(params HeaderedItemViewModel[] items)
        {
            _items = new ObservableCollection<HeaderedItemViewModel>(items);
        }

        public ObservableCollection<HeaderedItemViewModel> Items
        {
            get { return _items; }
        }


        public ItemActionCallback ClosingTabItemHandler
        {
            get { return ClosingTabItemHandlerImpl; }
        }

        /// <summary>
        /// Callback to handle tab closing.
        /// </summary>        
        private static void ClosingTabItemHandlerImpl(ItemActionCallbackArgs<TabablzControl> args)
        {
            var viewModel = args.DragablzItem.DataContext as HeaderedItemViewModel;

            Debug.Log(viewModel);
        }
    }
}
