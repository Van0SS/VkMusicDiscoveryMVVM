using GalaSoft.MvvmLight.Messaging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VkMusicDiscoveryMVVM.ViewModel.Message
{
    /// <summary>
    /// Параметры клика по прогресс бару
    /// </summary>
    class ProgressBarClickParams : MessageBase
    {
        //Отношение клика от всей длины. (0..1)
        public double ValueRatioClick { get; private set; }

        public ProgressBarClickParams(double value)
        {
            ValueRatioClick = value;
        }
    }
}
