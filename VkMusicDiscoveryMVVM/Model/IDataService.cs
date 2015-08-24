using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VkMusicDiscoveryMVVM.Model
{
    public interface IDataService
    {
        ObservableCollection<Audio> GetRecommendations(int count, bool random = false, int offset = 0);
        void InitVkApi(UserInfo userInfo);
    }
}
