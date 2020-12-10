using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace tftkeiba
{
    public static class Consts
    {
        public static Dictionary<string, string> API_REGIONAL_DIC = new Dictionary<string, string>()
        {
            { "BR1", "br1.api.riotgames.com" },
            { "EUN1", "eun1.api.riotgames.com" },
            { "EUW1", "euw1.api.riotgames.com" },
            { "JP1", "jp1.api.riotgames.com" },
            { "KR", "kr.api.riotgames.com" },
            { "LA1", "la1.api.riotgames.com" },
            { "LA2", "la2.api.riotgames.com" },
            { "NA1", "na1.api.riotgames.com" },
            { "OC1", "oc1.api.riotgames.com" },
            { "TR1", "tr1.api.riotgames.com" },
            { "RU", "ru.api.riotgames.com" }
        };

        public static Dictionary<string, string> API_WIDE_REGIONAL_DIC = new Dictionary<string, string>()
        {
            {"AMERICAS","americas.api.riotgames.com" },
            {"ASIA","asia.api.riotgames.com" },
            {"EUROPE","europe.api.riotgames.com" },
        };
        
        public const string EP_TFT_SUMMONER_BY_NAME = "/tft/summoner/v1/summoners/by-name/"; // GET /tft/summoner/v1/summoners/by-name/{summonerName}
        public const string EP_TFT_MATCH_IDS = "/tft/match/v1/matches/by-puuid/"; // GET /tft/match/v1/matches/by-puuid/{puuid}/ids
        public const string EP_TFT_MATCH_IDS_QUERY = "/ids?count=";
        public const string EP_TFT_MATCH = "/tft/match/v1/matches/"; //GET /tft/match/v1/matches/{matchId}
        public const string EP_TFT_SUMMONER_BY_ID = "/tft/summoner/v1/summoners/"; //GET /tft/summoner/v1/summoners/{encryptedSummonerId}

        public const string EP_TFT_LEAGUE = "/tft/league/v1/";
        public const string EP_TFT_LEAGUE_ENTRY_BY_ID = "/tft/league/v1/entries/by-summoner/"; //GET /tft/league/v1/entries/by-summoner/{encryptedSummonerId}

        public const string STATIC_DATA_FOLDER_PATH = "./StaticDatas/";
    }
}
