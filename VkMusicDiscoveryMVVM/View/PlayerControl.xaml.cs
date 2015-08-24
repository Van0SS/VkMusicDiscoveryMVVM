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
using VkMusicDiscoveryMVVM.ViewModel.Message;

namespace VkMusicDiscoveryMVVM.View
{
    /// <summary>
    /// Interaction logic for PlayerControl.xaml
    /// </summary>
    public partial class PlayerControl : UserControl
    {
        public PlayerControl()
        {
            InitializeComponent();
        }

        //Да, тут не по MVVM, но даже на StackOverflow! (лол) сказали, что в данном случае лучше сделать так.
        /// <summary>
        /// Рассчитывание позиции клика на прогресс баре.
        /// </summary>
        private void ProgressBarPlayer_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            var x = e.GetPosition(ProgressBarPlayer).X;
            var ratio = x / ProgressBarPlayer.ActualWidth;
            Messenger.Default.Send(new ProgressBarClickParams(ratio));
        }
    }
}
