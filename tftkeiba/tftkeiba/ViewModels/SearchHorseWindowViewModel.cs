using Prism.Commands;
using Prism.Mvvm;
using Prism.Services.Dialogs;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using tftkeiba.Manager;
using tftkeiba.Models;
using tftkeiba.Models.Response;
using tftkeiba.Views;

namespace tftkeiba.ViewModels
{
    public class SearchHorseWindowViewModel : BindableBase, IDialogAware
    {
        #region Binding
        private string horseName;
        public string HorseName { set => SetProperty(ref horseName, value); get { return horseName; } }
        private Horse selectedHorse;

        public event Action<IDialogResult> RequestClose;

        public Horse SelectedHorse { set => SetProperty(ref selectedHorse, value); get { return selectedHorse; } }
        public List<LeagueListDTO> Leagues
        {
            get
            {
                return DataManager.GetInstance().Leagues;
            }
        }
        public ObservableCollection<Horse> Horses { set; get; }
        #endregion
        public DelegateCommand SearchCommand { get; }
        public ICommand SelectCommand { get; }

        public string Title => "部分検索";

        public SearchHorseWindowViewModel()
        {
            Horses = new ObservableCollection<Horse>();
            SearchCommand = new DelegateCommand(ExecuteSearch);
            SelectCommand = new DelegateCommand(CloseDialog);
        }

        private void ExecuteSearch()
        {
            Horses.Clear();
            if (string.IsNullOrEmpty(HorseName)) return;
            try
            {
                var h = Leagues
                    .SelectMany(q => q.entries.Select(r => new Horse { Name = r.summonerName, Tier = q.tier, Rank = r.rank, LP = r.leaguePoints }))
                  .Where(q => q.Name.ToLower().Trim(' ').Contains(HorseName.ToLower().Trim(' ')))
                  .OrderBy(q => q.Name);
                Horses.AddRange(h);
            }
            catch
            {
                // エラーは無視
            }
            
        }

        protected virtual void CloseDialog()
        {
            if (SelectedHorse == null) return;
            var param = new DialogParameters() { { "HorseName", SelectedHorse.Name } };
            RaiseRequestClose(new DialogResult(ButtonResult.OK, param));
        }
        public virtual void RaiseRequestClose(IDialogResult dialogResult)
        {
            RequestClose?.Invoke(dialogResult);
        }

        public bool CanCloseDialog()
        {
            return true;
        }

        public void OnDialogClosed()
        {
            
        }

        public void OnDialogOpened(IDialogParameters parameters)
        {
            HorseName = parameters.GetValue<string>("HorseName");
            ExecuteSearch();
        }
    }
}
