using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Messaging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Navigation;
using VkMusicDiscoveryMVVM.Model;
using VkMusicDiscoveryMVVM.Model.Enum;
using VkMusicDiscoveryMVVM.ViewModel.Message;

namespace VkMusicDiscoveryMVVM.ViewModel
{
    public class LoginViewModel : ViewModelBase
    {
        /// <summary>
        /// Команда для бразера, при завершении загрузки страницы.
        /// </summary>
        public RelayCommand<NavigationEventArgs> LoadCompletedCommand { get; private set; }

        public LoginViewModel()
        {
            LoadCompletedCommand = new RelayCommand<NavigationEventArgs>(LoadCompleted);
        }

        private void LoadCompleted(NavigationEventArgs e)
        {
            //Если url содержит токен, то забираем и выходим.
            if (e.Uri.ToString().IndexOf("access_token") != -1)
            {
                Regex myReg = new Regex(@"(?<name>[\w\d\x5f]+)=(?<value>[^\x26\s]+)",
                    RegexOptions.IgnoreCase | RegexOptions.Singleline);
                string token = "";
                int userId = 0;
                foreach (Match m in myReg.Matches(e.Uri.ToString()))
                {
                    if (m.Groups["name"].Value == "access_token")
                    {
                        token = m.Groups["value"].Value;
                    }
                    else if (m.Groups["name"].Value == "user_id")
                    {
                        userId = Convert.ToInt32(m.Groups["value"].Value);
                    }
                }

                //Отдаём токен и ИД главной форме.
                Messenger.Default.Send(new UserInfo(userId, token));

                //Закрываем окно логина.
                Messenger.Default.Send(new CloseLoginMessage());
                
            }
        }
    }
}
