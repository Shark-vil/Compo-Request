using Compo_Request.Windows.Editor.Windows;
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

        public static HeaderedItemViewModel GetNewTabItem(string Header = "Новый запрос", object Content = null)
        {
            if (Content == null)
            {
                Content = new EditorWebRequestControl();

                var TabItemView = new HeaderedItemViewModel()
                {
                    Header = Header,
                    Content = Content
                };

                ((EditorWebRequestControl)TabItemView.Content).Construct(TabItemView);

                return TabItemView;
            }
            else
            {
                return new HeaderedItemViewModel()
                {
                    Header = Header,
                    Content = Content
                };
            }
        }
    }
}
