using Compo_Request.Windows.Editor.Pages;
using Compo_Request.Windows.Editor.Windows;
using Compo_Shared_Data.Debugging;
using Compo_Shared_Data.Models;
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

                        ((EditorWebRequestControl)TabItemView.Content)
                            .Construct(TabItemView);

                        return TabItemView;
                    };
            }
        }

        public static HeaderedItemViewModel AddTab(string Header = "Новый запрос", ModelRequestDirectory RequestDirectory = null)
        {
            var Content = new EditorWebRequestControl(RequestDirectory);

            var TabItemView = new HeaderedItemViewModel()
            {
                Header = Header,
                Content = Content
            };

            //Content.Construct(TabItemView, RequestDirectory);

            ((EditorWebRequestControl)TabItemView.Content)
                .Construct(TabItemView);

            return TabItemView;
        }

        public static HeaderedItemViewModel AddHistoryTab(string Header, WebRequestHistory HistoryItem)
        {
            var Content = new EditorWebRequestControl();

            var TabItemView = new HeaderedItemViewModel()
            {
                Header = Header,
                Content = Content
            };

            //Content.Construct(TabItemView, RequestDirectory);

            ((EditorWebRequestControl)TabItemView.Content)
                .SetHistory(HistoryItem);

            ((EditorWebRequestControl)TabItemView.Content)
                .Construct(TabItemView);

            return TabItemView;
        }
    }
}
