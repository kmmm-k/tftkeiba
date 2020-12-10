using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using tftkeiba.Models.Response;

namespace tftkeiba.Models
{
    public class Horse : BindableBase
    {
        // 調子スコア算出時順位に対する重みづけ
        private static Dictionary<int, int> placementWeightDic = new Dictionary<int, int>()
        {
            {1, 8},
            {2, 6},
            {3, 4},
            {4, 2},
            {5, -2},
            {6, -4},
            {7, -6},
            {8, -8},
        };
        // 調子スコア算出時試合に対する重みづけ（直近の方を重く見る）
        // 2020.12.14 直近がより強くなるよう重みづけを変更 before:1.2 1.1 0.9 0.8
        private static Dictionary<int, double> matchWeightDic = new Dictionary<int, double>()
        {
            {1, 1.4},
            {2, 1.2},
            {3, 0.8},
            {4, 0.6},
        };
        #region Binding
        private string name;
        public string Name { set => SetProperty(ref name, value); get { return name; } }
        private string tier;
        public string Tier { set => SetProperty(ref tier, value); get { return tier; } }
        private string rank;
        public string Rank { set => SetProperty(ref rank, value); get { return rank; } }
        private int? lp;
        public int? LP { set => SetProperty(ref lp, value); get { return lp; } }
        private Brush averageColor = Brushes.Black;
        public Brush AverageColor { set => SetProperty(ref averageColor, value); get { return averageColor; } }


        private ObservableCollection<ParticipantDto> recentMatches;
        public ObservableCollection<ParticipantDto> RecentMatches
        {
            set
            {
                SetProperty(ref recentMatches, value);
                recentMatches.CollectionChanged += (object sender, NotifyCollectionChangedEventArgs e) =>
                {
                    RaisePropertyChanged(nameof(RecentPlacesString));
                    RaisePropertyChanged(nameof(Average));
                    RaisePropertyChanged(nameof(Condition));
                    RaisePropertyChanged(nameof(ConditionColor));
                    //RaisePropertyChanged(nameof(ConditionScore));
                };
            }
            get
            {
                return recentMatches;
            }
        }

    /*    private ObservableCollection<int> recentPlaces;
        public ObservableCollection<int> RecentPlaces { 
            set 
            {
                SetProperty(ref recentPlaces, value);
                recentPlaces.CollectionChanged += (object sender, NotifyCollectionChangedEventArgs e) =>
                {
                    RaisePropertyChanged(nameof(RecentPlacesString));
                    RaisePropertyChanged(nameof(Average));
                    RaisePropertyChanged(nameof(Condition));
                    RaisePropertyChanged(nameof(ConditionColor));
                    //RaisePropertyChanged(nameof(ConditionScore));
                };
            } get { return recentPlaces; } }*/
        public string RecentPlacesString 
        { 
            get
            {
                string s = null;
                foreach(var match in recentMatches)
                {
                    if (string.IsNullOrEmpty(s) != true) s = " > " + s;
                    s = match.placement.ToString() + s;
                }
                return s;
            } 
        }
        public double? Average
        {
            get
            {
                if (recentMatches.Count == 0) return null;
                return recentMatches.Average(q => q.placement);
            }
        }
        public double ConditionScore { 
            get
            {
                double score = 0.0;
                int match = 1;
                int matchCount = recentMatches.Count() > 4 ? 4 : recentMatches.Count();
                foreach (var m in recentMatches)
                {
                    if (match > 4) break;
                    score += matchWeightDic[match] * (double)placementWeightDic[m.placement];
                    match++;
                }
                return score / (double)matchCount;
            } 
        }
        /// <summary>
        /// conditionScoreに基づいた文字列を返す
        /// </summary>
        public string Condition 
        { 
            get 
            {
                if (ConditionScore >= 6.25) return "↑↑↑";
                if (ConditionScore >= 4) return "↑↑";
                if (ConditionScore >= 2) return "↑";
                if (ConditionScore <= -6.25) return "↓↓↓";
                if (ConditionScore <= -4) return "↓↓";
                if (ConditionScore <= -2) return "↓";
                return "-";
            } 
        }
        public Brush ConditionColor
        {
            get
            {
                if (ConditionScore >= 2) return Brushes.Red;
                if (ConditionScore <= -2) return Brushes.Blue;
                return Brushes.Black;
            }
        }
        private string tanpyo;
        public string Tanpyo { set => SetProperty(ref tanpyo, value); get { return tanpyo; } }
        private string remarks;
        public string Remarks { set => SetProperty(ref remarks, value); get { return remarks; } }
        private string puuid;
        public string Puuid { set => SetProperty(ref puuid, value); get { return puuid; } }
        #endregion

        public string SummonerID { set; get; }

        public Horse()
        {
            RecentMatches = new ObservableCollection<ParticipantDto>();
        }

        public void Clear(bool includeName = false)
        {
            if (includeName) Name = null;
            LP = null;
            Rank = null;
            Tier = null;
            Puuid = null;
            SummonerID = null;
            Tanpyo = null;
            Remarks = null;
            averageColor = Brushes.Black;
            RecentMatches.Clear();
        }
    }
}
