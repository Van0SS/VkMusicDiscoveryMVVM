using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VkMusicDiscoveryMVVM.Model.Enum;

namespace VkMusicDiscoveryMVVM.Model
{
    /// <summary>
    /// Класс песни в формате vk.
    /// </summary>
    public class Audio
    {
        /// <summary>
        /// ID песни.
        /// </summary>
        public uint Id { get; set; }
        /// <summary>
        /// Идентификатор владельца аудиозаписи.
        /// </summary>
        public int OwnerId { get; set; }

        private string _artist;
        /// <summary>
        /// Исполнитель.
        /// </summary>
        public string Artist
        {
            get { return _artist; }
            set
            {
                if (String.IsNullOrEmpty(value))
                {
                    _artist = "VA"; //Хороший исполнитель. +
                }
                else
                {
                    _artist = value;
                }
            }
        }

        private string _title;
        /// <summary>
        /// Название песни.
        /// </summary>
        public string Title
        {
            get { return _title; }
            set
            {
                if (String.IsNullOrEmpty(value))
                {
                    _title = "Track 1"; //Самый известный трек. +
                }
                else
                {
                    _title = value;
                }
            }
        }

        private int _kbps;
        /// <summary>
        /// Качество в kbps.
        /// </summary>
        public int Kbps
        {
            get { return _kbps; }
            set
            {
                if (value <= 0)
                {
                    throw new Exception("Value must be above 0");
                }
                else
                {
                    _kbps = value; //Может быть больше 320, т.к. это с картинкой!
                }
            }
        }

        /// <summary>
        /// Длительность.
        /// </summary>
        public uint Duration { get; set; }

        /// <summary>
        /// Ссылка на файл песни.
        /// </summary>
        public Uri Url { get; set; }

        /// <summary>
        /// Идентификатор текста аудиозаписи (если доступно).
        /// </summary>
        public uint? LyricsId { get; set; }

        /// <summary>
        /// Идентификатор жанра песни (если присвоен).
        /// </summary>
        public AudioGenre? GenreId { get; set; }

        /// <summary>
        /// Вернуть название песни в формате Artist - Title.
        /// </summary>
        /// <returns></returns>
        public string GetArtistDashTitle()
        {
            return Artist + " - " + Title;
        }
    }
}
