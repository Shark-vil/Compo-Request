using Compo_Shared_Data.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace Compo_Shared_Data.Network.Models
{
    [Serializable]
    public class MBinding_WebRequest
    {
        public WebRequestItem Item;
        public WebRequestDir Directory;
        public WebRequestParamsItem[] Params;
    }
}
