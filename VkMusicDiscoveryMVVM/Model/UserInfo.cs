using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VkMusicDiscoveryMVVM.Model
{
    /// <summary>
    /// Инфо о авторизованном пользователе.
    /// </summary>
    public class UserInfo
    {
        public int UserId { get; private set; }
        public string AccessToken { get; private set; }

        public UserInfo(int userId, string accesToken)
        {
            UserId = userId;
            AccessToken = accesToken;
        }
    }
}
