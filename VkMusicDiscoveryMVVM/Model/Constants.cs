using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VkMusicDiscoveryMVVM.Model.Enum;

namespace VkMusicDiscoveryMVVM.Model
{
    public static class Constants
    {
        //Id приложения в ВК.
        public const int AppId = 4533969;
        //Права доступа только к аудио.
        public const int Scope = (int)(ScopeType.Audio);
    }
}
