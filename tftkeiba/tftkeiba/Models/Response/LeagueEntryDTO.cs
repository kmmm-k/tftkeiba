using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace tftkeiba.Models.Response
{
    public class LeagueEntryDTO
    {
        public string leagueId { set; get; }
        public string summonerId { set; get; }
        public string summonerName { set; get; }
        public string queueType { set; get; }
        public string tier { set; get; }
        public string rank { set; get; }
        public int leaguePoints { set; get; }
        public int wins { set; get; }
        public int losses { set; get; }
        public bool boolean { set; get; }
        public bool veteran { set; get; }
        public bool freshBlood { set; get; }
        public bool inactive { set; get; }
        //public MiniSeriesDTO miniSeries {set;get;}


    }

 /*   public class MiniSeriesDTO
    {
        public int losses { set; get; }
        public string progress { set; get; }
        public int target { set; get; }
        public int wins { set; get; }
    }*/
}
