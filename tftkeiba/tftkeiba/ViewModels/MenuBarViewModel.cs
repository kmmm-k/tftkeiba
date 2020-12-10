using Prism.Commands;
using Prism.Mvvm;
using Prism.Services.Dialogs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using tftkeiba.Views;

namespace tftkeiba.ViewModels
{
    public class MenuBarViewModel : BindableBase
    {
        public DelegateCommand SettingCommand { set; get; }
        private IDialogService DialogService { get; }

        public MenuBarViewModel(IDialogService dialogService)
        {
            DialogService = dialogService;
            SettingCommand = new DelegateCommand(ExecuteSettingCommand);
        }

        private void ExecuteSettingCommand()
        {
            DialogService.ShowDialog(nameof(SettingsWindow), new DialogParameters
            {
                {"", ""}
            },
            (IDialogResult ret) => {
           });
        }
    }
}
