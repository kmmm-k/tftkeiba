using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace tftkeiba.Models.Response
{
    public class LeagueListDTO
    {
        public string leagueId { set; get; }
        public List<LeagueItemDTO> entries { set; get; }
        public string tier { set; get; }
        public string name { set; get; }
        public string queue { set; get; }
    }
}
