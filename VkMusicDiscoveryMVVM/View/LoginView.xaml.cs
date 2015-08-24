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
using System.Windows.Shapes;
using VkMusicDiscoveryMVVM.Model;
using VkMusicDiscoveryMVVM.ViewModel.Message;

namespace VkMusicDiscoveryMVVM.View
{
    /// <summary>
    /// Interaction logic for WindowLogin.xaml
    /// </summary>
    public partial class LoginView : Window
    {
        public LoginView()
        {
            InitializeComponent();

            //Закрываем форму при поступлении команды.
            Messenger.Default.Register<CloseLoginMessage>(this, _ => Close());
            Autorize();
        }

        public void Autorize() //Авторизация клиентских приложений.
        {
            WebBrowserLogin.Navigate(String.Format("http://api.vk.com/oauth/authorize?client_id={0}&scope={1}&display=popup&response_type=token",
                Constants.AppId, Constants.Scope));
        }
    }
}
