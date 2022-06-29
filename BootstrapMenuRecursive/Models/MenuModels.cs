using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BootstrapMenuRecursive.Models
{
    public class MenuModels
    {
        public class MenuList
        {
            public List<MenuItem> Menus { get; set; }
        }

        public class MenuItem
        {
            public string MenuName { get; set; }
            public string Url { get; set; }
            public List<MenuItem> Menus { get; set; }
        }

    }
}