using GalaSoft.MvvmLight.Messaging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VkMusicDiscoveryMVVM.Model;

namespace VkMusicDiscoveryMVVM.ViewModel.Message
{
    /// <summary>
    /// Инфа для прогрывания.
    /// </summary>
    public class SendSongToPlay : MessageBase
    {
        public Audio Song { get; private set; }

        public SendSongToPlay(Audio audio)
        {
            Song = audio;
        }
    }
}
