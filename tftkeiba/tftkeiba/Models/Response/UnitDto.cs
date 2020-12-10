using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace tftkeiba.Models.Response
{
    public class UnitDto
    {
        public List<int> items { set; get; }
        public string character_id { set; get; }
        // If a unit is chosen as part of the Fates set mechanic, the chosen trait will be indicated by this field. Otherwise this field is excluded from the response.
        public string chosen { set; get; }
        //	Unit name. This field is often left blank.
        public string name { set; get; }
        public int rarity { set; get; }
        public int tier { set; get; }

    }
}
