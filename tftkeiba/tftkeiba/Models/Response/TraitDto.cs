using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace tftkeiba.Models.Response
{
    public class TraitDto
    {
        public string name { set; get; }
        public int num_units { set; get; }
        // Current style for this trait. (0 = No style, 1 = Bronze, 2 = Silver, 3 = Gold, 4 = Chromatic)
        public int style { set; get; }
        public int tier_current { set; get; }
        public int tier_total { set; get; }
    }
}
