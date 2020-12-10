using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace tftkeiba.Models.StaticData
{
    public class ChampionList: List<Champion>
    {
    }

    public class Champion
    {
        public string name { get; set; }
        public string championId { get; set; }
        public int cost { get; set; }
        public List<string> traits { get; set; }// Trait.Key
    }

}
