using Newtonsoft.Json;
using Prism.Commands;
using Prism.Mvvm;
using Prism.Services.Dialogs;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using tftkeiba.Manager;
using tftkeiba.Models.StaticData;

namespace tftkeiba.ViewModels
{
    public class SettingsWindowViewModel : BindableBase, IDialogAware
    {

        #region Binding
        public bool IsGeneratingTanpyo { 
            set
            {
                Properties.Settings.Default.IsGeneratingTanpyo = value;
                RaisePropertyChanged(nameof(IsGeneratingTanpyo));
                Properties.Settings.Default.Save();
            }
            get
            {
                return Properties.Settings.Default.IsGeneratingTanpyo;
            }
        }
        
        public bool ShouldGetAllSummonerData
        {
            set
            {
                Properties.Settings.Default.ShouldGetAllSummonerData = value;
                RaisePropertyChanged(nameof(ShouldGetAllSummonerData));
                Properties.Settings.Default.Save();
            }
            get
            {
                return Properties.Settings.Default.ShouldGetAllSummonerData;
            }
        }

        public ObservableCollection<Tier> TierList { set; get; }

        public string Title => "設定";
        #endregion

        public SettingsWindowViewModel()
        {
            TierList = new ObservableCollection<Tier>(DataManager.GetInstance().Mst_Tiers);
        }
        #region IDialogAware
        public event Action<IDialogResult> RequestClose;

        protected virtual void CloseDialog()
        {

            var param = new DialogParameters() { { "", "" } };
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
            // 取得リーグ保存
            Properties.Settings.Default.LeagueList = JsonConvert.SerializeObject(TierList);
            Properties.Settings.Default.Save();
        }

        public void OnDialogOpened(IDialogParameters parameters)
        {
            
        }
        #endregion
    }
}
