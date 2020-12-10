using Newtonsoft.Json;
using Prism.Commands;
using Prism.Mvvm;
using Prism.Services.Dialogs;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using tftkeiba.Manager;
using tftkeiba.Models;
using tftkeiba.Models.Response;
using tftkeiba.Models.StaticData;
using tftkeiba.Utils;
using tftkeiba.Views;

namespace tftkeiba.ViewModels
{
    /// <summary>
    /// 
    /// </summary>
    public class MainWindowViewModel : BindableBase
    {
        #region Binding
        private string _title = String.Format("{0} {1}", Application.Current.MainWindow.GetType().Assembly.GetName().Name, Application.Current.MainWindow.GetType().Assembly.GetName().Version);
        public string Title
        {
            get { return _title; }
            set { SetProperty(ref _title, value); }
        }
        public Dictionary<string, string> WideRegionCbx
        {
            get
            {
                return Consts.API_WIDE_REGIONAL_DIC;
            }
        }
        public Dictionary<string, string> RegionCbx
        {
            get
            {
                return Consts.API_REGIONAL_DIC;
            }
        }
        public ObservableCollection<Horse> Horses { set; get; }
        public List<LeagueListDTO> Leagues
        {
            set
            {
                DataManager.GetInstance().Leagues = value;
            }
            get
            {
                return DataManager.GetInstance().Leagues;
            }
        }
        // TODO:Region, WideRegionはSettingに保存してもいいかも
        // TODO:WideRegionはRegionから自動で判別できるなら非表示でいいかも
        //private string region;
        public string Region 
        { 
            set
            {
                Properties.Settings.Default.Region = value;
                RaisePropertyChanged(nameof(Region));
                Properties.Settings.Default.Save();
            }
            get 
            {
                return Properties.Settings.Default.Region;
            } 
        }
        //private string wideRegion;
        public string WideRegion
        {
            set
            {
                Properties.Settings.Default.WideRegion = value;
                RaisePropertyChanged(nameof(WideRegion));
                Properties.Settings.Default.Save();
            }
            get
            {
                return Properties.Settings.Default.WideRegion;
            }
        }
        public bool ShouldGetAllSummonerData
        {
            get
            {
                return Properties.Settings.Default.ShouldGetAllSummonerData;
            }
        }
        private string message = "";
        public string Message { set => SetProperty(ref message, value); get { return message; } }
        private bool showApiKey = false;
        public bool ShowApiKey { set => SetProperty(ref showApiKey, value); get { return showApiKey; } }
        private bool hasLeague = false;
        public bool HasLeague { set => SetProperty(ref hasLeague, value); get { return hasLeague; } }
        private Horse selectedHorse;
        public Horse SelectedHorse { set => SetProperty(ref selectedHorse, value); get { return selectedHorse; } }
        public string ApiKey
        {
            set
            {
                Properties.Settings.Default.APIKey = value;
                RaisePropertyChanged(nameof(ApiKey));
                Properties.Settings.Default.Save();
            }
            get
            {
                return Properties.Settings.Default.APIKey;
            }
        }
        #endregion

        #region Commands
        public DelegateCommand SearchInfoCommand { get; }
        public DelegateCommand GetLeagueCommand { get; }
        public DelegateCommand GetPlacementsCommand { get; }
        public DelegateCommand ShowApiKeyCommand { get; }
        public DelegateCommand SearchFromPartCommand { get; }// MEMO:部分検索はダブルクリックに割り当てるのもアリかな？
        public ICommand ClearCommand { get; }
        public ICommand SaveImageCommand { get; }
        #endregion

        private int RequestMatchCount = 4; // サモナー一人につき履歴をいくつ取得するか
        private int ApiCount = 0;
        private int ApiThreatholdPerSec = 20; // Developerのキーでは1秒に20回以上リクエストを送れない https://developer.riotgames.com/docs/portal
        private IDialogService DialogService { get; }
        public MainWindowViewModel(IDialogService dialogService)
        {

            DialogService = dialogService;
            Horses = CreateNewHorseList();
            GetLeagueCommand = new DelegateCommand(async () => { await ExecuteGetLeagueCommand(); });
            GetPlacementsCommand = new DelegateCommand(async () => { await ExecuteGetPlacementsCommand(); });
            SearchInfoCommand = new DelegateCommand(async () => { await ExecuteSearchInfoCommand(); });
            ShowApiKeyCommand = new DelegateCommand(() => { ShowApiKey = !ShowApiKey; });
            SaveImageCommand = new DelegateCommand<DataGrid>(ExecuteSaveImageCommand);
            ClearCommand = new DelegateCommand<DataGrid>(ExecuteClearCommand);
            SearchFromPartCommand = new DelegateCommand(ExecuteSearchFromPartCommand);

            Properties.Settings.Default.PropertyChanged += DefaultPropertyChanged; // 設定変更時にViewへ反映させたいので

            LoadMasterData();
        }

        private void DefaultPropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            RaisePropertyChanged(e.PropertyName);
        }

        private void LoadMasterData()
        {
           Message = "マスターデータ読込中...";
            // 取得リーグリストの生成
            try
            {
                DataManager.GetInstance().Mst_Tiers = JsonUtils.LoadJson<TierList>(Consts.STATIC_DATA_FOLDER_PATH + Properties.Settings.Default.TiersJsonName);
                var tmp = JsonConvert.DeserializeObject<TierList>(Properties.Settings.Default.LeagueList);
                if (tmp == null || tmp.Count == 0)
                {
                    tmp = DataManager.GetInstance().Mst_Tiers;
                    try
                    {
                        // デフォルトはチャレ・グラマス・マスターの３つ
                        tmp.First(q => q.key == "Challenger").IsChecked = true;
                        tmp.First(q => q.key == "Grandmaster").IsChecked = true;
                        tmp.First(q => q.key == "Master").IsChecked = true;
                        Properties.Settings.Default.Save();
                    }
                    catch
                    {
                        // 構造が変わってchallengerなどが無くなった場合　無視
                    }
                }
                // マスターデータの方に保存データをロード（構成が変わった際に対応できるよう）
                // FIXME:効率のいいやり方あれば
                foreach(var tier in DataManager.GetInstance().Mst_Tiers)
                {
                    var t = tmp.FirstOrDefault(q => q.key == tier.key);
                    if (t != null) tier.IsChecked = t.IsChecked;
                    // JonConverterでDeseriarize時、コンストラクタなどでdivisionsが生成されているとsetterが呼ばれずにOnCheckChangedイベントを設定できないのでここで生成
                    // FIXME:いい方法があれば
                    if (tier.divisions == null) tier.divisions = new ObservableCollection<Division>();
                    foreach(var division in tier.divisions)
                    {
                        var d = t.divisions.FirstOrDefault(q => q.key == division.key);
                        if (d != null) division.IsChecked = d.IsChecked;
                    }
                }                

                DataManager.GetInstance().Mst_Champions = JsonUtils.LoadJson<ChampionList>(Consts.STATIC_DATA_FOLDER_PATH + Properties.Settings.Default.ChampionsJsonName);
                DataManager.GetInstance().Mst_Items = JsonUtils.LoadJson<ItemList>(Consts.STATIC_DATA_FOLDER_PATH + Properties.Settings.Default.ItemsJsonName);
                DataManager.GetInstance().Mst_Traits = JsonUtils.LoadJson<TraitList>(Consts.STATIC_DATA_FOLDER_PATH + Properties.Settings.Default.TraitsJsonName);
                DataManager.GetInstance().MasterDataLoadSucceeded = true;
                Message = "マスターデータ読込完了";
            }
            catch (Exception ex)
            {
                Message = "マスターデータ読込失敗 " + ex.Message;
            }
        }

        private void ExecuteSaveImageCommand(DataGrid grid)
        {
            CopyUIElementToClipboard(grid);
            Message = "表をクリップボードにコピーしました。";
        }

        /// <summary>
        /// Copies a UI element to the clipboard as an image, and as text.
        /// まるコピ　https://brianlagunas.com/wpfcopy-uielement-to-clipboard-as-multiple-formats/
        /// </summary>
        /// <param name="element">The element to copy.</param>
        public static void CopyUIElementToClipboard(FrameworkElement element)
        {
            //data object to hold our different formats representing the element
            DataObject dataObject = new DataObject();
            //lets start with the text representation
            //to make is easy we will just assume the object set as the DataContext has the ToString method overrideen and we use that as the text
            dataObject.SetData(DataFormats.Text, element.DataContext.ToString(), true);

            //now lets do the image representation
            double width = element.ActualWidth;
            double height = element.ActualHeight;
            RenderTargetBitmap bmpCopied = new RenderTargetBitmap((int)Math.Round(width), (int)Math.Round(height), 96, 96, PixelFormats.Default);
            DrawingVisual dv = new DrawingVisual();
            using (DrawingContext dc = dv.RenderOpen())
            {
                VisualBrush vb = new VisualBrush(element);
                dc.DrawRectangle(vb, null, new Rect(new Point(), new Size(width, height)));
            }
            bmpCopied.Render(dv);
            dataObject.SetData(DataFormats.Bitmap, bmpCopied, true);

            //now place our object in the clipboard
            Clipboard.SetDataObject(dataObject, true);
        }

        private async Task ExecuteGetPlacementsCommand()
        {
            ApiCount = 0;
            Message = "戦績を取得中...";

            foreach (var uma in Horses)
            {
                if (string.IsNullOrEmpty(uma.Puuid) == true) continue;
                var matchIds = await CallAPI.SendRiotAsync<MatchList>(string.Format("https://{0}{1}{2}{3}{4}", Consts.API_WIDE_REGIONAL_DIC[WideRegion], Consts.EP_TFT_MATCH_IDS, uma.Puuid, Consts.EP_TFT_MATCH_IDS_QUERY, RequestMatchCount), HttpMethod.Get);
                await addApiCount();
                uma.RecentMatches.Clear();
                foreach (var matchId in matchIds)
                {
                    var matchRes = await CallAPI.SendRiotAsync<MatchDto>(string.Format("https://{0}{1}{2}", Consts.API_WIDE_REGIONAL_DIC[WideRegion], Consts.EP_TFT_MATCH, matchId), HttpMethod.Get);
                    var match = matchRes.info.participants.Where(q => q.puuid == uma.Puuid).FirstOrDefault();
                    match.matchNumber = matchIds.IndexOf(matchId) + 1;
                    uma.RecentMatches.Add(match);
                    await addApiCount();
                }
            }

            // Avgの良い/悪い色付け
            foreach (var uma in Horses.Where(q => q.Average <= 4.0 && q.Average == Horses.Min(r => r.Average)))
            {

                uma.AverageColor = Brushes.Red;
            }
            foreach (var uma in Horses.Where(q => q.Average >= 4.0 && q.Average == Horses.Max(r => r.Average)))
            {
                uma.AverageColor = Brushes.Blue;
            }

            // 短評生成
            if (DataManager.GetInstance().MasterDataLoadSucceeded == true && Properties.Settings.Default.IsGeneratingTanpyo == true)
            {
                foreach (var uma in Horses)
                {
                    uma.Tanpyo = "";

                    //--- Champに関する傾向
                    var matches = uma.RecentMatches.SelectMany(q => q.units, (q, r) => new { match = q, unit = r })
                        .Join(DataManager.GetInstance().Mst_Champions, q => q.unit.character_id, r => r.championId, (q, r) => new { match = q.match, unit = q.unit, champ = r });

                    // cost1～3 かつ tier3
                    var champs13 = matches.Where(q => q.champ.cost <= 3 && q.unit.tier == 3)
                        .OrderByDescending(q => q.champ.cost)
                        .GroupBy(q => q.champ.championId)
                        .OrderByDescending(q => q.Count());

                    foreach (var c in champs13)
                    {
                        if (c.Count() == RequestMatchCount)
                        {
                            uma.Tanpyo += c.First().champ.name + "に愛された馬。";
                        }
                        else if (c.Count() >= RequestMatchCount * 0.75)
                        {
                            uma.Tanpyo += c.First().champ.name + "OTP。";
                        }
                        else if (c.Count() >= RequestMatchCount * 0.5)
                        {
                            uma.Tanpyo += c.First().champ.name + "好き。";
                        }
                    }

                    // 5コス構成（定義：5コス5体以上かつ、5コス★2が1体以上いる）
                    // MEMO:Lv9は見なくていい？
                    var cost5 = uma.RecentMatches
                        .Where(q => q.units
                            .Join(DataManager.GetInstance().Mst_Champions, r => r.character_id, s => s.championId, (r, s) => new { unit = r, champ = s })
                            .Where(r => r.champ.cost == 5).Count() >= 5);
                    cost5 = cost5.Where(q => q.units.Where(r => r.tier >= 2).Count() >= 1);
                    if (cost5.Count() >= RequestMatchCount * 0.5)
                    {
                        uma.Tanpyo += string.Format("5コス構成傾向アリ ({0}/{1})。", cost5.Count(), RequestMatchCount);
                    }

                    //--- Traitに関する傾向
                    var matches_t = uma.RecentMatches.SelectMany(q => q.traits, (q, r) => new { match = q, trait = r })
                       .Join(DataManager.GetInstance().Mst_Traits, q => q.trait.name, r => r.key, (q, r) => new { match = q.match, trait = q.trait, trait_m = r });

                    // StyleがGold以上(かつset区分が3以上ある)構成を擦っている
                    // MEMO:編成難度的に適切か？
                    var trait6 = matches_t
                        .Where(q => q.trait_m.sets.Count() >= 3 &&
                            q.trait_m.sets.Where(r => r.style == Trait.STYLES_DIC[Trait.Styles.Gold]).Count() > 0 &&
                            (q.trait.style == (int)Trait.Styles.Gold || q.trait.style == (int)Trait.Styles.Chromatic))
                        .GroupBy(q => q.trait_m.key)
                        .OrderByDescending(q => q.Count());
                    foreach (var t in trait6)
                    {
                        if (t.Count() == RequestMatchCount)
                        {
                            uma.Tanpyo += t.First().trait_m.name + "真・OTP。";
                        }
                        else if (t.Count() >= RequestMatchCount * 0.75)
                        {
                            uma.Tanpyo += t.First().trait_m.name + "OTP。";
                        }
                        else if (t.Count() >= RequestMatchCount * 0.5)
                        {
                            uma.Tanpyo += t.First().trait_m.name + "寄せ。";
                        }
                    }

                    // TODO:フォーチュンかどうかを判断するのにアイテム数という切り口があるが...

                    //--- レア編成
                    // cost4以上のtier3を出した
                    var champs45 = matches.Where(q => q.champ.cost >= 4 && q.unit.tier == 3)
                        .OrderByDescending(q => q.champ.cost)
                        .OrderBy(q => q.match.placement);

                    foreach (var c in champs45)
                    {
                        uma.Tanpyo += string.Format("{0}では{1}{2}の★★★を作", c.match.matchNumberString, c.champ.cost == 5 ? "運命を味方に付け" : "", c.champ.name);
                        if (c.match.placement <= 2) uma.Tanpyo += string.Format("り、見事{0}位を獲得。", c.match.placement);
                        else if (c.match.placement <= 4) uma.Tanpyo += string.Format("り、{0}位入賞。", c.match.placement);
                        else uma.Tanpyo += string.Format("ったが、結果は{0}位。", c.match.placement);
                        break; // 複数あると長くなるのでとりあえず1つだけで
                    }

                    // Trait9/9の構成を出した
                    var trait9 = matches_t.Where(q => q.trait_m.sets.Where(r => r.min >= 9).Count() > 0 && q.trait.num_units >= 9)
                        .OrderBy(q => q.match.placement);
                    foreach (var t in trait9)
                    {
                        uma.Tanpyo += string.Format("{0}では{1}[{2}]を出し", t.match.matchNumberString, t.trait_m.name, t.trait_m.sets.Select(q => q.min).OrderByDescending(q => q).First());
                        if (t.match.placement <= 2) uma.Tanpyo += string.Format("、見事{0}位を獲得。", t.match.placement);
                        else if (t.match.placement <= 4) uma.Tanpyo += string.Format("、{0}位入賞。", t.match.placement);
                        else uma.Tanpyo += string.Format("たが、結果は{0}位。", t.match.placement);
                        break;// 複数あると長くなるのでとりあえず1つだけで
                    }

                    // ここまでなにもない場合
                    if (string.IsNullOrEmpty(uma.Tanpyo) == true)
                    {
                        if (uma.Average <= 2.0) uma.Tanpyo = "変幻自在";
                        else if (uma.Average <= 4.0) uma.Tanpyo = "オールラウンダー";
                        else if (uma.Average <= 6.0) uma.Tanpyo = "特筆事項なし";
                        else uma.Tanpyo = "迷走中";
                    }
                }
            }
            Message = "戦績取得完了";
        }

        private async Task ExecuteGetLeagueCommand()
        {
            Message = "リーグ情報を取得中...";
            try
            {
                // Challenger,Grandmaster,Master
                Leagues = new List<LeagueListDTO>();
                var s_lgs = DataManager.GetInstance().Mst_Tiers
                    .Where(q => q.IsChecked == true && q.IsSpecial == true);
                if (s_lgs != null)
                {
                    foreach (var league in s_lgs)
                    {
                        var endpoint = string.Format("https://{0}{1}{2}", Consts.API_REGIONAL_DIC[Region],Consts.EP_TFT_LEAGUE,league.api);
                        var data = await CallAPI.SendRiotAsync<LeagueListDTO>(endpoint, HttpMethod.Get);
                        Leagues.Add(data);
                    }
                }

                // 一般リーグ
                // MEMO:DIA以下のリーグ情報を都度取得するのは困難なので凍結（例えばDIAMOND Iで件数205のページが150件近くある）
            /*    var lgs = DataManager.GetInstance().Mst_Tiers
                    .SelectMany(q => q.divisions, (q, r) => new { tier = q, division = r })
                    .Where(q => q.division.IsChecked == true && q.tier.IsSpecial == false);
                if (lgs != null)
                {
                    foreach (var league in lgs)
                    {
                        var endpoint = string.Format("https://{0}{1}{2}/{3}", Consts.API_REGIONAL_DIC[Region], Consts.EP_TFT_LEAGUE, league.tier.api, league.division.api);
                        // MEMO:本来一般リーグの取得APIはLeagueEntryDTOを返すがデータを統合したいのでLeagueItemDTOを使用 https://developer.riotgames.com/apis#tft-league-v1
                        var data = await CallAPI.SendRiotAsync<LeagueItemDTOList>(endpoint, HttpMethod.Get);
                        var ll = new LeagueListDTO()
                        {
                            entries = data,
                            tier = league.tier.key + " " + league.division.key
                        };
                        Leagues.Add(ll);
                    }
                }
*/
                Message = "リーグ情報取得完了";
                HasLeague = true;
            }
            catch (Exception ex)
            {
                // APIキーが期限切れ?
                Message = ex.Message + " APIキーの期限切れの可能性有";
            }
        }

        private async Task ExecuteSearchInfoCommand()
        {
            Message = "サモナー情報検索中...";
            foreach (var horse in Horses)
            {
                if (string.IsNullOrEmpty(horse.Name))
                {
                    horse.Clear();
                    continue;
                }

                try
                {
                    // ローカルに取得したデータから検索
                    var h = Leagues.SelectMany(q => q.entries.Select(r => new { item = r, tier = q.tier }))
                    .Where(q => q.item.summonerName.ToLower().Trim(' ') == horse.Name.ToLower().Trim(' ')) // サモナー名末尾にスペース入れ奴
                    .FirstOrDefault();

                    // TODO:ローカルに無い場合、APIを使って全ランクからデータ検索を行う
                    if (h == null && ShouldGetAllSummonerData == true)
                    {
                        SummonerDto sum = null;
                        try
                        {
                            sum = await CallAPI.SendRiotAsync<SummonerDto>(string.Format("https://{0}{1}{2}", Consts.API_REGIONAL_DIC[Region], Consts.EP_TFT_SUMMONER_BY_NAME, horse.Name), HttpMethod.Get);
                        }
                        catch
                        {
                            // 見つからなかった場合、404例外
                        }
                        if (sum != null)
                        {
                            var entryList = await CallAPI.SendRiotAsync<LeagueItemDTOList>(string.Format("https://{0}{1}{2}", Consts.API_REGIONAL_DIC[Region], Consts.EP_TFT_LEAGUE_ENTRY_BY_ID, sum.id), HttpMethod.Get);
                            var entry = entryList.FirstOrDefault();
                            if (entry != null)
                            {
                                h = new { item = entry, tier = entry.tier + " " + entry.rank };
                                // 余計な呼び出しをなくすためリーグ再取得まではローカルから取得する
                                Leagues.Add(new LeagueListDTO()
                                {
                                    entries = entryList,
                                    tier = entry.tier + " " + entry.rank
                                });
                            }
                        }
                    }

                    if (h == null)
                    {
                        horse.Clear();
                        continue;
                    }


                    // サモナー名が変わった場合, 戦績とPuuidはリセットする
                    if (horse.SummonerID != h.item.summonerId)
                    {
                        horse.RecentMatches.Clear();
                        horse.Puuid = null;
                    }
                    // サモナー名が変わらなくてもリーグ情報が更新されているのでTier,LPなどは更新する
                    horse.SummonerID = h.item.summonerId;
                    horse.Tier = h.tier;
                    horse.Rank = h.item.rank;
                    horse.LP = h.item.leaguePoints;

                    // puuidが無い場合は取得
                    if (string.IsNullOrEmpty(horse.Puuid) == true)
                    {
                        var sum = await CallAPI.SendRiotAsync<SummonerDto>(string.Format("https://{0}{1}{2}", Consts.API_REGIONAL_DIC[Region], Consts.EP_TFT_SUMMONER_BY_ID, horse.SummonerID), HttpMethod.Get);
                        horse.Puuid = sum.puuid;
                    }

                }
                catch
                {
                    // 例外無視
                }
               
            }
            Message = "サモナー情報検索完了";
        }
        private void ExecuteClearCommand(DataGrid grid)
        {
            var res = MessageBox.Show(Application.Current.MainWindow, "表をクリアします", "確認", MessageBoxButton.OKCancel);
            if (res == MessageBoxResult.Cancel) return;
            foreach (var horse in Horses)
            {
                horse.Clear(true);
            }
            // ソート解除
            foreach (DataGridColumn column in grid.Columns)
            {
                column.SortDirection = null;
            }
            Message = "表をクリアしました";
        }

        private void ExecuteSearchFromPartCommand()
        {
            if (SelectedHorse == null || SelectedHorse.Name == null)
            {
                Message = "サモナーを選択してください";
                return;
            }

            DialogService.ShowDialog(nameof(SearchHorseWindow), new DialogParameters
            {
                {"HorseName", selectedHorse.Name }
            },
            async (IDialogResult ret) => {
                var name = ret.Parameters.GetValue<string>("HorseName");
                if (string.IsNullOrEmpty(name) == false)
                {
                    selectedHorse.Name = name;
                    await ExecuteSearchInfoCommand();
                }
            });

        }

        private ObservableCollection<Horse> CreateNewHorseList()
        {
            var horses = new ObservableCollection<Horse>();
            for (int i = 0; i <= 7; i++)
            {
                horses.Add(new Horse());
            }
            return horses;
        }

        private async Task addApiCount()
        {
            var m = Message;
            ApiCount++;
            if (ApiCount >= ApiThreatholdPerSec)
            {
                Message = "API連続実行制限回避のため休止(1000ms)";
                await Task.Delay(1000);
                ApiCount = 0;
                Message = m;
            }
        }
    }
}
