using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Messaging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Threading;
using VkMusicDiscoveryMVVM.Model;
using VkMusicDiscoveryMVVM.ViewModel.Message;

namespace VkMusicDiscoveryMVVM.ViewModel
{
    public class PlayerViewModel : ViewModelBase
    {
        private MediaPlayer _mediaPlayer = new MediaPlayer();

        /// <summary>
        /// Будет ли плеер играть одну песню постоянно.
        /// </summary>
        private bool _playerRepeatSong;

        /// <summary>
        /// Будет ли плеер играть случайную песню.
        /// </summary>
        private bool _playerShuffle;

        /// <summary>
        /// Таймер обновляющий прогресс и оставшееся время.
        /// </summary>
        private DispatcherTimer _timer;

        /// <summary>
        /// Текущая песня
        /// </summary>
        private Audio _curSong;

        readonly Random _rnd = new Random();


        /// <summary>
        /// The <see cref="ProgressBarMaximum" /> property's name.
        /// </summary>
        public const string ProgressBarMaximumPropertyName = "ProgressBarMaximum";

        private double _progressBarMaximum = 1;
        public double ProgressBarMaximum
        {
            get
            { return _progressBarMaximum; }

            set
            {
                if (_progressBarMaximum == value)
                    return;

                _progressBarMaximum = value;

                RaisePropertyChanged(ProgressBarMaximumPropertyName);
            }
        }

        /// <summary>
        /// The <see cref="ProgressBarValue" /> property's name.
        /// </summary>
        public const string ProgressBarValuePropertyName = "ProgressBarValue";

        private double _progressBarValue = 0;
        public double ProgressBarValue
        {
            get
            { return _progressBarValue; }

            set
            {
                if (_progressBarValue == value)
                    return;

                _progressBarValue = value;

                RaisePropertyChanged(ProgressBarValuePropertyName);
            }
        }

        /// <summary>
        /// The <see cref="TbTitleBar" /> property's name.
        /// </summary>
        public const string TbTitleBarPropertyName = "TbTitleBar";

        private string _tbTitleBar = "Select song and click open icon";
        /// <summary>
        /// TbPlayerSong - Название текущей песни.
        /// </summary>
        public string TbTitleBar
        {
            get
            {    return _tbTitleBar;  }

            set
            {
                if (_tbTitleBar == value)
                    return;

                _tbTitleBar = value;
                RaisePropertyChanged(TbTitleBarPropertyName);
            }
        }

        /// <summary>
        /// The <see cref="TbPlayerTime" /> property's name.
        /// </summary>
        public const string TbPlayerTimePropertyName = "TbPlayerTime";

        private string _tbPlayerTime = "0:00";
        /// <summary>
        /// TbPlayerTime - Текущее время проигрывания.
        /// </summary>
        public string TbPlayerTime
        {
            get
            { return _tbPlayerTime; }

            set
            {
                if (_tbPlayerTime == value)
                    return;

                _tbPlayerTime = value;
                RaisePropertyChanged(TbPlayerTimePropertyName);
            }
        }

        /// <summary>
        /// The <see cref="SldVolume" /> property's name.
        /// </summary>
        public const string SldVolumePropertyName = "SldVolume";

        private double _sldVolume = 0.2d;
        /// <summary>
        /// SldVolume - громкость, от 0 до 1;
        /// </summary>
        public double SldVolume
        {
            get
            { return _sldVolume; }

            set
            {
                if (_sldVolume == value)
                    return;

                _sldVolume = value;
                _mediaPlayer.Volume = SldVolume;
                RaisePropertyChanged(SldVolumePropertyName);
            }
        }

        public const string PlayerStatePropertyName = "PlayerState";

        private MediaState _playerState;
        /// <summary>
        /// Текущее состояние плеера(сам он не поддерживает).
        /// </summary>
        private MediaState PlayerState
        {
            get { return _playerState; }
            set
            {
                _playerState = value;
             //   ChangePlayButtonState();
                RaisePropertyChanged(PlayerStatePropertyName);
            }
        }

        /// <summary>
        /// Команда для кнопки Плей/Пауза.
        /// </summary>
        public RelayCommand BtnPlayerPlayPauseCommand { get; private set; }

        /// <summary>
        /// Команда для кнопки Открыть.
        /// </summary>
        public RelayCommand BtnPlayerOpenCommand { get; private set; }

        /// <summary>
        /// Команда для кнопки предыдущая песня.
        /// </summary>
        public RelayCommand BtnPlayerPrevCommand { get; private set; }

        /// <summary>
        /// Команда для кнопки следующая песня.
        /// </summary>
        public RelayCommand BtnPlayerNextCommand { get; private set; }

        public PlayerViewModel()
        {
            _mediaPlayer.MediaEnded += MediaPlayerEnded;
            _timer = new DispatcherTimer();
            _timer.Interval = TimeSpan.FromSeconds(1); //Обновлять каждую секунду.
            _timer.Tick += timer_Tick;

            BtnPlayerPlayPauseCommand = new RelayCommand(BtnPlayerPlayPauseExecute);
            BtnPlayerOpenCommand = new RelayCommand(BtnPlayerOpenExecute);
            BtnPlayerPrevCommand = new RelayCommand(BtnPlayerPrevExecute);
            BtnPlayerNextCommand = new RelayCommand(BtnPlayerNextExecute);
            //ClickProgressBarPlayerCommand = new RelayCommand<MouseButtonEventArgs>();

            _mediaPlayer.Volume = SldVolume;
            Messenger.Default.Register<SendSongToPlay>(this, SongToPlayReceive);
            Messenger.Default.Register<ProgressBarClickParams>(this, ClickProgressBarReceive);
        }

        #region - Register messages -

        private void SongToPlayReceive(SendSongToPlay message)
        {
            var song = message.Song;
            _mediaPlayer.Open(song.Url);
            _curSong = song;
            MediaPlayerPlay();
            TbTitleBar = song.GetArtistDashTitle();

            //TbPlayerSong.ToolTip = TbPlayerSong.Text;

            //Песня ещё не загрузилась, поэтому длительность лучше взять из объекта Audio т.е. то что сервер говорит.
            ProgressBarMaximum = song.Duration;
            var time = new TimeSpan(0, 0, (int)song.Duration);
            String emptyZero = "";
            if (time.Seconds < 10)
                emptyZero = "0";
            TbPlayerTime = "-" + time.Minutes + ":" + emptyZero + time.Seconds;
        }

        private void ClickProgressBarReceive(ProgressBarClickParams parameters)
        {
            _mediaPlayer.Position = new TimeSpan(0, 0, (int)(parameters.ValueRatioClick * ProgressBarMaximum));
            UpdatePlayerProgressNTime();
        }

        #endregion - Register messages -

        #region - Execute methods -

        /// <summary>
        /// Запросить предыдущую песню.
        /// </summary>
        private void BtnPlayerPrevExecute()
        {
            sendPlayedSongAndRequest(false);
        }

        /// <summary>
        /// Запросить следующую песню.
        /// </summary>
        private void BtnPlayerNextExecute()
        {
            sendPlayedSongAndRequest(true);
        }

        private void BtnPlayerOpenExecute()
        {
            openSongNPlay();
        }

        private void BtnPlayerPlayPauseExecute()
        {
            switch (PlayerState)
            {
                case MediaState.Play:
                    MediaPlayerPause();
                    break;
                case MediaState.Pause:
                    MediaPlayerPlay();
                    break;
                default: //При остальных состояниях повторять действия кнопки Open.
                    openSongNPlay();
                    break;
            }
        }

        #endregion - Execute methods -


        #region - Private methods -

        /// <summary>
        /// Запросить песню без параметров.
        /// </summary>
        private void openSongNPlay()
        {
            Messenger.Default.Send(new PlaySongRequest());
        }

        /// <summary>
        /// Запросить песню, на основе проигранной.
        /// </summary>
        /// <param name="requestNext"></param>
        private void sendPlayedSongAndRequest(bool requestNext)
        {
             Messenger.Default.Send(new PlaySongRequest(_curSong, requestNext));
        }

        /// <summary>
        /// Обновить время длительности песни и прогресс бар.
        /// </summary>
        private void UpdatePlayerProgressNTime()
        {
            ProgressBarValue = _mediaPlayer.Position.TotalSeconds;
            if (!_mediaPlayer.NaturalDuration.HasTimeSpan)
                return;
            var elapsedTime = _mediaPlayer.NaturalDuration.TimeSpan - _mediaPlayer.Position;
            String emptyZero = "";
            if (elapsedTime.Seconds < 10) //Чтобы не было: 3:4 а было: 3:04
                emptyZero = "0";
            TbPlayerTime = "-" + elapsedTime.Minutes + ":" + emptyZero + elapsedTime.Seconds;
        }

        private void MediaPlayerPlay()
        {
            _mediaPlayer.Play();
            PlayerState = MediaState.Play;
            _timer.Start();
        }

        private void MediaPlayerPause()
        {
            _mediaPlayer.Pause();
            PlayerState = MediaState.Pause;
            _timer.Stop();
        }

        /*
        private void ChangePlayButtonState()
        {
            BtnPlayerPlayPause.Content =
                FindResource(_playerState == MediaState.Play ? "PausePic" : "PlayPic");
        }

        private void PlayRandomSong()
        {
            int rndIndex = _rnd.Next(_fileteredRecomendedList.Count);
            OpenAndPlayByIndex(rndIndex);
        }
        */
#endregion - Private methods -
        //---------------------------------------------------------------------------------------------
        #region - Event Handlers -
        
        /// <summary>
        /// Каждую секунду.
        /// </summary>
        private void timer_Tick(object sender, EventArgs e)
        {
            UpdatePlayerProgressNTime();
        }

        /// <summary>
        /// Когда текущая песня проиграла.
        /// </summary>
        private void MediaPlayerEnded(object sender, EventArgs eventArgs)
        {
            if (_playerRepeatSong) //Если стоит повтор то перемотать на начало песни.
                _mediaPlayer.Position = new TimeSpan(0);
            else if (!_playerShuffle)
            {
                sendPlayedSongAndRequest(true);
            }
            else
            {
                //PlayRandomSong();
            }
        }

        /* private void BtnPlayerRepeat_OnClick(object sender, RoutedEventArgs e)
         {
             _playerRepeatSong = !_playerRepeatSong; //Сменить состояние кнопки.
             Brush brushFill = _playerRepeatSong ? Brushes.Black : null; //Если включено то залить чёрным.
             Brush brushStroke = _playerRepeatSong
                 ? Brushes.WhiteSmoke //И светлая рамка.
                 : (SolidColorBrush)(new BrushConverter().ConvertFrom("#333333"));//Иначё тёмная.
             foreach (Polygon polygon in PanelBtnRepeat.Children)
             {
                 polygon.Fill = brushFill;
                 polygon.Stroke = brushStroke;
             }
         }

         private void BtnPlayerShuffle_OnClick(object sender, RoutedEventArgs e)
         {
             _playerShuffle = !_playerShuffle;
             Brush brushFill = _playerShuffle ? Brushes.Black : null;
             Brush brushStroke = _playerShuffle
                 ? Brushes.WhiteSmoke
                 : (SolidColorBrush)(new BrushConverter().ConvertFrom("#333333"));
             foreach (Polygon polygon in GridButtonShuffle.Children)
             {
                 polygon.Fill = brushFill;
                 polygon.Stroke = brushStroke;
             }
         }*/
        /*

        */
        #endregion - Event Handlers -
    }
}
