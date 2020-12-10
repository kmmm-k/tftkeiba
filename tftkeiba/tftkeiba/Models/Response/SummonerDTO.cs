using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace tftkeiba.Models.Response
{
    public class SummonerDto
    {
        public string accountId { set; get; }
        public int profileIconId { set; get; }
        public long revisionDate { set; get; }
        public string name { set; get; }
        public string id { set; get; }
        public string puuid { set; get; }
        public long summonerLevel { set; get; }
    }
}
