using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using tftkeiba.Models.Response;
using tftkeiba.Models.StaticData;

namespace tftkeiba.Manager
{
    public sealed class DataManager
    {
        private static DataManager _singleInstance = new DataManager() { 
            MasterDataLoadSucceeded = false 
        };

        public List<LeagueListDTO> Leagues { set; get; }
        public ChampionList Mst_Champions { set; get; }
        public ItemList Mst_Items { set; get; }
        public TraitList Mst_Traits { set; get; }
        public TierList Mst_Tiers { set; get; }
        public bool MasterDataLoadSucceeded { set; get; }

        public static DataManager GetInstance()
        {
            return _singleInstance;
        }
    }
}
