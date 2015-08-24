using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Messaging;
using Microsoft.WindowsAPICodePack.Dialogs;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Net;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Threading;
using VkMusicDiscoveryMVVM.Model;
using VkMusicDiscoveryMVVM.Model.Enum;
using VkMusicDiscoveryMVVM.ViewModel.Message;
using System.Linq;

namespace VkMusicDiscoveryMVVM.ViewModel
{
    public class MainViewModel : ViewModelBase
    {
        #region - Fields - 

        /// <summary>
        /// Обработанные данные с сервера вк.
        /// </summary>
        private readonly DataService _dataService;

        /// <summary>
        /// Воркер для ассинхронной загрузки файлов и отображения прогресс бара.
        /// </summary>
        private readonly BackgroundWorker _workerDownload;

        /// <summary>
        /// Папка выбранная для загрузки.
        /// </summary>
        private string _directoryToDownload;

        /// <summary>
        /// Список выделенных песен.
        /// </summary>
        List<Audio> SelectedItemsList = new List<Audio>();
        //Биндится к датагриду.
        private ObservableCollection<Audio> _recomendedAudios;
        public ObservableCollection<Audio> RecomendedAudios
        {
            get { return _recomendedAudios; }
            set
            {
                _recomendedAudios = value;
                RaisePropertyChanged();
            }
        }

        /// <summary>
        /// Список песен для загрузки, после наложения фильтров(Язык, блокировка исп/песн).
        /// </summary>
        private ObservableCollection<Audio> _fileteredRecomendedList
            = new ObservableCollection<Audio>
            { //Данные для визуального редактора.
                new Audio() { Artist = "Yanix", Title = "ban'ka-parilka" },
                new Audio() { Artist = "Pendelum", Title = "Watercolour" }
            };
        public ObservableCollection<Audio> FileteredRecomendedList
        {
            get { return _fileteredRecomendedList; }
            set
            {
                _fileteredRecomendedList = value;
                RaisePropertyChanged();
            }
        }

        /// <summary>
        /// The <see cref="Count" /> property's name.
        /// </summary>
        public const string CountPropertyName = "Count";

        private int _count = 50;
        /// <summary>
        /// TxbCount - количество песен для запроса. 
        /// </summary>
        public int Count
        {
            get
            {   return _count;}
                        set
            {
                if (_count == value)
                    return;

                _count = value;
                RaisePropertyChanged(CountPropertyName);
            }
        }

        /// <summary>
        /// The <see cref="Offset" /> property's name.
        /// </summary>
        public const string OffsetPropertyName = "Offset";

        private int _offset = 0;
        /// <summary>
        /// TxbOffset - смещение на X песен, для запроса.
        /// </summary>
        public int Offset
        {
            get
            {    return _offset;}
            set
            {
                if (_offset == value)
                    return;

                _offset = value;
                RaisePropertyChanged(OffsetPropertyName);
            }
        }

        /// <summary>
        /// The <see cref="Randomize" /> property's name.
        /// </summary>
        public const string RandomizePropertyName = "Randomize";

        private bool _ramdomize = false;

        /// <summary>
        /// CbxRandom - случайный порядок для запроса
        /// </summary>
        public bool Randomize
        {
            get
            {  return _ramdomize; }

            set
            {
                if (_ramdomize == value)
                    return;

                _ramdomize = value;
                RaisePropertyChanged(RandomizePropertyName);
            }
        }

        /// <summary>
        /// The <see cref="RuLang" /> property's name.
        /// </summary>
        public const string RuLangPropertyName = "RuLang";

        private bool _ruLang = false;
        /// <summary>
        /// RbtnLangRu - фильтр песен с символами кириллицы.
        /// </summary>
        public bool RuLang
        {
            get
            {   return _ruLang;  }

            set
            {
                if (_ruLang == value)
                    return;

                _ruLang = value;
                RaisePropertyChanged(RuLangPropertyName);
            }
        }

        /// <summary>
        /// The <see cref="EngLang" /> property's name.
        /// </summary>
        public const string EngLangPropertyName = "EngLang";

        private bool _engLang = false;
        /// <summary>
        /// RbtnLangEng - фильтр песен с символами НЕкириллицы.
        /// </summary>
        public bool EngLang
        {
            get
            { return _engLang; }

            set
            {
                if (_engLang == value)
                    return;

                _engLang = value;
                RaisePropertyChanged(EngLangPropertyName);
            }
        }

        /// <summary>
        /// The <see cref="AllLang" /> property's name.
        /// </summary>
        public const string AllLangPropertyName = "AllLang";

        private bool _allLang = true;
        /// <summary>
        /// RbtnLangAll - без фильтра по языку
        /// </summary>
        public bool AllLang
        {
            get
            { return _allLang; }

            set
            {
                if (_allLang == value)
                    return;

                _allLang = value;
                RaisePropertyChanged(AllLangPropertyName);
            }
        }

        /// <summary>
        /// The <see cref="BtnDownloadText" /> property's name.
        /// </summary>
        public const string BtnDownloadTextPropertyName = "BtnDownloadText";

        private string _btnDownloadText = "Download";
        /// <summary>
        /// BtnDownloadall - текст на кнопке скачать.
        /// </summary>
        public string BtnDownloadText
        {
            get
            {   return _btnDownloadText; }

            set
            {
                if (_btnDownloadText == value)
                    return;

                _btnDownloadText = value;
                RaisePropertyChanged(BtnDownloadTextPropertyName);
            }
        }

        /// <summary>
        /// The <see cref="ProgressBarMaximum" /> property's name.
        /// </summary>
        public const string ProgressBarMaximumPropertyName = "ProgressBarMaximum";

        private double _progressBarMaximum = 1; //По дефолту, чтобы не был весь заполенен.
        /// <summary>
        /// ProgressBarDownload - максимальное значение.
        /// </summary>
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
        /// <summary>
        /// ProgressBarDownload - текущее значение.
        /// </summary>
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
        /// The <see cref="ProgressBarText" /> property's name.
        /// </summary>
        public const string ProgressBarTextPropertyName = "ProgressBarText";

        private string _progressBarText = "";
        /// <summary>
        /// TblProgressBar - текст на прогресс баре.
        /// </summary>
        public string ProgressBarText
        {
            get
            { return _progressBarText; }

            set
            {
                if (_progressBarText == value)
                    return;

                _progressBarText = value;

                RaisePropertyChanged(ProgressBarTextPropertyName);
            }
        }

        /// <summary>
        /// The <see cref="CbxFindBest" /> property's name.
        /// </summary>
        public const string CbxFindBestPropertyName = "CbxFindBest";

        private bool _cbxFindBest = false;
        /// <summary>
        /// CbxBestBitrate - искать ли лучший битрейт для песен перед загрузкой.
        /// </summary>
        public bool CbxFindBest
        {
            get
            {   return _cbxFindBest; }

            set
            {
                if (_cbxFindBest == value)
                    return;

                _cbxFindBest = value;
                RaisePropertyChanged(CbxFindBestPropertyName);
            }
        }


        #endregion - Fields - 

        #region - Commands - 
        /// <summary>
        /// Команда для BtnRefresh.
        /// </summary>
        public RelayCommand BtnRefreshCommand { get; private set; }

        /// <summary>
        /// Команда для кнопок выбора языка.
        /// </summary>
        public RelayCommand<RoutedEventArgs> RbtnsLangCheckedCommand { get; private set; }

        /// <summary>
        /// Команда для BtnDownloadall.
        /// </summary>
        public RelayCommand BtnDownloadallCommand { get; private set; }

        /// <summary>
        /// Команда для DataGridAudio, при смене выделения.
        /// </summary>
        public RelayCommand<IList> AuidiosSelChangedCommand { get; private set; }

        #endregion - Commands - 

        public MainViewModel()
        {
            //if (IsInDesignMode)
            //{
            //    RecomendedAudios = audioDataService.GetRecommendations(Count);
            //}
            //else
            //{
            //    // Code runs "for real"
            //}
            Messenger.Default.Register<UserInfo>(this, userTokenReceived);
            Messenger.Default.Register<PlaySongRequest>(this, playSongRequestRecived);

            _dataService = new DataService();

            //Инициализация воркера
            _workerDownload = new BackgroundWorker();
            _workerDownload.WorkerSupportsCancellation = true; //Для возможности отмены.
            _workerDownload.DoWork += worker_DoWork;
            _workerDownload.RunWorkerCompleted += worker_RunWorkerCompleted;

            BtnRefreshCommand = new RelayCommand(BtnRefreshExecute);
            RbtnsLangCheckedCommand = new RelayCommand<RoutedEventArgs>(RbtnsLangCheckedExecute);
            BtnDownloadallCommand = new RelayCommand(BtnDownloadallExecute);
            AuidiosSelChangedCommand = new RelayCommand<IList>(AuidiosSelChangedExecute);
        }

        #region - Message receiver handlers -
        /// <summary>
        /// При получении токена, получить песни. 
        /// </summary>
        /// <param name="userInfo">Токен и ИД юзера</param>
        private void userTokenReceived(UserInfo userInfo)
        {
            _dataService.InitVkApi(userInfo);
            RecomendedAudios = _dataService.GetRecommendations(Count, Randomize, Offset);
            FileteredRecomendedList = RecomendedAudios;
            FilterSongs();
        }

        /// <summary>
        /// Отправить нужное Audio, при получении запроса.
        /// </summary>
        private void playSongRequestRecived(PlaySongRequest songRequest)
        {
            //Если нет песен в списке, то ничего не делать.
            if (_fileteredRecomendedList.Count == 0)
                return;
            
            //Если нет песни до/после которой надо искать.
            if (songRequest.PlayedSong == null)
            {
                //Если нет выделенных песен, то играть первую
                if (SelectedItemsList.Count == 0)
                    Messenger.Default.Send(new SendSongToPlay(_fileteredRecomendedList[0]));
                //Если есть, то играть первую из выделенных
                else
                    Messenger.Default.Send(new SendSongToPlay(SelectedItemsList[0]));
            }
            else 
            {
                var curIndex = _fileteredRecomendedList.IndexOf(songRequest.PlayedSong);
                if (curIndex == -1) //Такой песни нет в текущем листе
                {
                    Messenger.Default.Send(new SendSongToPlay(_fileteredRecomendedList[0]));
                    return;
                }
                //Выдать песню после/до проигранной.
                if (songRequest.NextSong)
                {
                    //Если последняя в списке песня, то начать заново.
                    if ((curIndex + 2) > _fileteredRecomendedList.Count)
                        Messenger.Default.Send(new SendSongToPlay(_fileteredRecomendedList[0]));
                    else
                        Messenger.Default.Send(new SendSongToPlay(_fileteredRecomendedList[curIndex + 1]));

                }
                else
                {
                    if (curIndex == 0)
                    { //Если первая песня
                        Messenger.Default.Send(new SendSongToPlay(
                            _fileteredRecomendedList[_fileteredRecomendedList.Count - 1]));
                    }
                    else
                        Messenger.Default.Send(new SendSongToPlay(_fileteredRecomendedList[curIndex - 1]));

                }
            }
        }

        #endregion - Message receiver handlers -

        #region - Command execute methods -

        private void AuidiosSelChangedExecute(IList audios)
        {
            SelectedItemsList = audios.Cast<Audio>().ToList();
        }
    
        private void BtnRefreshExecute()
        {
            RecomendedAudios = _dataService.GetRecommendations(Count, Randomize, Offset);
            FilterSongs();
        }

        private void RbtnsLangCheckedExecute(RoutedEventArgs e)
        {
            FilterSongs();
        }

        private void BtnDownloadallExecute()
        {
            if (_workerDownload.IsBusy)
            {
                _workerDownload.CancelAsync();
                return;
            }


            var dirDialog = new CommonOpenFileDialog { IsFolderPicker = true };
            if (dirDialog.ShowDialog() == CommonFileDialogResult.Ok)
            {
                _directoryToDownload = dirDialog.FileName;
                ProgressBarMaximum = _fileteredRecomendedList.Count;
                ProgressBarValue = 0;
                BtnDownloadText = "Cancel";
                _workerDownload.RunWorkerAsync();
            }
        }

        #endregion - Command execute methods -

        #region - Private methods -

        /// <summary>
        /// Фильтрация текущего списка по языку и списку блокировки.
        /// </summary>
        private void FilterSongs()
        {
            // FileteredRecomendedList.Clear();
            var filterSongs = new ObservableCollection<Audio>();

            foreach (var track in RecomendedAudios)
            {
                if ((AllLang != true) && (IsWrongLang(track)))
                    continue;

                /* if (IsContentInBlockArtists(track))
                     continue;

                 if (IsContentInBlockSongs(track))
                     continue;*/

                filterSongs.Add(track);
            }
            FileteredRecomendedList = filterSongs;
            ProgressBarText = "Count: " + FileteredRecomendedList.Count;
         /*   if (!_workerDownload.IsBusy) // Показывать количество, только если ничего не скачивается.
                TblProgressBar.Text = "Count: " + _fileteredRecomendedList.Count;*/
        }

        /// <summary>
        /// Проверка песню на текущий язык.
        /// </summary>
        /// <returns>True - неверный язык</returns>
        private bool IsWrongLang(Audio track)
        {
            if (RuLang == true)
            {
                if (!Utilities.IsStringRussian(track.Artist + track.Title))
               // if (!Regex.IsMatch((track.Artist + track.Title), "[А-Яа-я]")) Don't work, WTF??
                    return true;
            }
            else
            {
                if (Utilities.IsStringRussian(track.Artist + track.Title))
                    //if (Regex.IsMatch((track.Artist + track.Title), "[А-Яа-я]"))
                    return true;
            }
            return false;

        }

        /// <summary>
        /// Процесс асинхронной загрузки файлов.
        /// </summary>
        private void DownloadFiles()
        {
            double value = 0;
            var filesToDownloadList = new List<Audio>(_fileteredRecomendedList);
            foreach (var track in filesToDownloadList)
            {
                if (_workerDownload.CancellationPending)
                {
                    return;
                }
                string fileName = track.GetArtistDashTitle() + ".mp3"; //В вк пока только мр3.

                if (fileName.IndexOfAny(Path.GetInvalidFileNameChars()) != -1) //Если есть недопустимые символы, то удалить.
                    fileName = string.Concat(fileName.Split(Path.GetInvalidFileNameChars(), StringSplitOptions.RemoveEmptyEntries));

                if (fileName.Length > 250) //Если имя файла длинее 250 символов - обрезать.
                    fileName = fileName.Substring(0, 250);

                Audio replacedAudio = track;
                if (CbxFindBest)
                    replacedAudio = _dataService.ReplaceWithBetterQuality(track);

                new WebClient().DownloadFile(replacedAudio.Url, _directoryToDownload + '\\' + fileName);
                // if (isAddDownToBlock)
                //    BlockHeader(fileName, BlockTabType.Songs);
                ProgressBarValue = ++value;
                ProgressBarText = value + "/" + filesToDownloadList.Count;
            }
        }

        #endregion - Private methods -

        #region - Event handlers -

        /// <summary>
        /// Отобразить исход закачки.
        /// </summary>
        private void worker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            BtnDownloadText = "Download All";
            ProgressBarValue = 0;
            ProgressBarText = e.Cancelled ? "Canceled" : "Completed";
        }

        /// <summary>
        /// Скачивать файлы, с возможностью отмены.
        /// </summary>
        private void worker_DoWork(object sender, DoWorkEventArgs e)
        {
            DownloadFiles();
            if (_workerDownload.CancellationPending)
            {
                e.Cancel = true;
            }
        }

        #endregion - Event handlers -
    }
}