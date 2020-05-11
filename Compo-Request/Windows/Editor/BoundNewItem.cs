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
                        var dateTime = DateTime.Now;

                        return new HeaderedItemViewModel()
                        {
                            Header = dateTime.ToLongTimeString(),
                            Content = dateTime.ToString("R")
                        };
                    };
            }
        }
    }
}
