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
    /// Запрос на проигрывание песни.
    /// </summary>
    class PlaySongRequest : MessageBase
    {
        /// <summary>
        /// Текущая проигранная песня.
        /// </summary>
        public Audio PlayedSong { get; private set; }

        /// <summary>
        /// Надо следующую песню? Или предыдущую.
        /// </summary>
        public bool NextSong { get; private set; }

        public PlaySongRequest()
        {
        }

        public PlaySongRequest(Audio audio, bool nextSong)
        {
            PlayedSong = audio;
            NextSong = nextSong;
        }
    }
}
