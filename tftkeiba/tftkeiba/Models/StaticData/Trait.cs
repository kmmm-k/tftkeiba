using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace tftkeiba.Models.StaticData
{

    public class TraitList : List<Trait>
    {
    }

    public class Trait
    {
        public string key { get; set; } 
        public string name { get; set; }
        public string description { get; set; }
        public string type { get; set; }
        public List<Set> sets { get; set; }
        public string innate { get; set; }

        //https://developer.riotgames.com/apis#tft-match-v1/GET_getMatch
        //Current style for this trait. (0 = No style, 1 = Bronze, 2 = Silver, 3 = Gold, 4 = Chromatic)
        public enum Styles
        {
            None, Bronze, Silver, Gold, Chromatic
        };
        public static Dictionary<Styles, string> STYLES_DIC = new Dictionary<Styles, string>()
        {
            {Styles.Bronze, "bronze"},
            {Styles.Silver, "silver"},
            {Styles.Gold, "gold"},
            {Styles.Chromatic, "chromatic"},
        };
    }

    public class Set
    {
        public string style { get; set; }
        public int min { get; set; }
        public int max { get; set; }
    }

}
