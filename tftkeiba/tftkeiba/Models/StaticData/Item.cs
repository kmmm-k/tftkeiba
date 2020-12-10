using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace tftkeiba.Models.StaticData
{
    public class ItemList : List<Item>
    {
    }

    public class Item
    {
        public int id { get; set; }
        public string name { get; set; }
        public string description { get; set; }
    }

}
