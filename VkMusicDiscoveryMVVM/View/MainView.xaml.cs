using GalaSoft.MvvmLight.Messaging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using VkMusicDiscoveryMVVM.Model.Enum;
using VkMusicDiscoveryMVVM.View;
using VkMusicDiscoveryMVVM.ViewModel;

namespace VkMusicDiscoveryMVVM
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainView : Window
    {
        public MainView()
        {
            InitializeComponent();
            //В сообщения
            var login = new LoginView();
            login.ShowDialog();
            //
        }
    }
}
