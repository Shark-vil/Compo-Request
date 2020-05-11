using Compo_Request.Windows.Editor.Windows;
using Compo_Shared_Data.Network.Models;
using Compo_Shared_Data.WPF.Models;
using Dragablz;
using System;
using System.Collections.Generic;
using System.Text;

namespace Compo_Request.Windows.Editor
{
    public static class BoundNewItem
    {
        public static Func<HeaderedItemViewModel> Factory
        {
            get
            {
                return
                    () =>
                    {
                        var TabItemView = new HeaderedItemViewModel()
                        {
                            Header = "Новый запрос",
                            Content = new EditorWebRequestControl()
                        };

                        ((EditorWebRequestControl)TabItemView.Content).Construct(TabItemView);

                        return TabItemView;
                    };
            }
        }

        public static HeaderedItemViewModel AddTab(string Header = "Новый запрос", MResponse ServerResponse = null)
        {
            var Content = new EditorWebRequestControl();

            var TabItemView = new HeaderedItemViewModel()
            {
                Header = Header,
                Content = Content
            };

            Content.Construct(TabItemView, ServerResponse);

            return TabItemView;
        }
    }
}
