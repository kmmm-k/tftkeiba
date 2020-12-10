using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace tftkeiba.Models.Response
{
    public class ParticipantDto
    {
        // public CompanionDto companion { set; get; } 作ってない
        public int gold_left { set; get; }
        public int last_round { set; get; }
        public int level { set; get; }
        public int placement { set; get; }
        public int players_eliminated { set; get; }
        public string puuid { set; get; }
        public float time_eliminated { set; get; }
        public int total_damage_to_players { set; get; }
        public List<TraitDto> traits { set; get; }
        public List<UnitDto> units { set; get; }
        #region Local
        public int matchNumber { set; get; }
        public string matchNumberString { get
            {
                if (matchNumber == 1) return "前走";
                else if (matchNumber == 2) return "前々走";
                if (matchNumber >= 3) return string.Format("{0}走前", matchNumber);
                return "";
            } 
        }
        #endregion
    }
}
