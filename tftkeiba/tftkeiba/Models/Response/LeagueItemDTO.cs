using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace tftkeiba.Models.Response
{
    public class LeagueItemDTO
    {
        public bool freshBlood { set; get; }
        public int wins { set; get; }
        public string summonerName { set; get; }
        //public MiniSeriesDTO miniSeries { set; get; }
        public bool inactive { set; get; }
        public bool veteran { set; get; }
        public bool hotStreak { set; get; }
        public string rank { set; get; }
        public int leaguePoints { set; get; }
        public int losses { set; get; }
        public string summonerId { set; get; }

        #region LegueEntryDTOと統合
        public string tier { set; get; }

        #endregion

    }

    public class LeagueItemDTOList : List<LeagueItemDTO>
    {

    }
}
