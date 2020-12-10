using tftkeiba.Views;
using Prism.Ioc;
using Prism.Modularity;
using System.Windows;
using tftkeiba.ViewModels;

namespace tftkeiba
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App
    {
        protected override Window CreateShell()
        {
            // アップグレードを行った際の設定情報引継ぎ
            if (!tftkeiba.Properties.Settings.Default.IsUpgrated)
            {
                tftkeiba.Properties.Settings.Default.Upgrade();
                tftkeiba.Properties.Settings.Default.IsUpgrated = true;
                tftkeiba.Properties.Settings.Default.Save();
            }
            return Container.Resolve<MainWindow>();
        }

        protected override void RegisterTypes(IContainerRegistry containerRegistry)
        {
            containerRegistry.RegisterDialog<SearchHorseWindow, SearchHorseWindowViewModel>();
            containerRegistry.RegisterDialog<SettingsWindow, SettingsWindowViewModel>();
        }
    }
}
