using GalaSoft.MvvmLight.Threading;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using VkMusicDiscoveryMVVM.Model;
using VkMusicDiscoveryMVVM.ViewModel;

namespace VkMusicDiscoveryMVVM
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        static App()
        {
            MainViewModel mainViewModel = new MainViewModel();
           // mainViewModel.DataService = new DataService();
            MainView mainView = new MainView();
            mainView.Show();
            mainView.Activate();
        }
    }
}
