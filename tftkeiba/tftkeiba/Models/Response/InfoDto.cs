using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace tftkeiba.Models.Response
{
    public class InfoDto
    {
        public long game_datetime { set; get; } // UNIX_timesamp
        public float game_length { set; get; } //sec
        public string game_variation { set; get; }
        public string game_version { set; get; }
        public List<ParticipantDto> participants { set; get; }
        public int queue_id { set; get; }
        public int tft_set_number { set; get; }
    }
}
