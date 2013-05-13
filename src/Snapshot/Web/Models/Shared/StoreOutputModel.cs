using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Web.Models.Shared
{
    public class StoreOutputModel<T>
    {
        public T[] Items { get; set; }
        public int TotalItems { get; set; }
    }
}