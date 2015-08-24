using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VkMusicDiscoveryMVVM.Model;

namespace VkMusicDiscoveryTests.TestData
{
    public class TestAudios
    {
        public Audio RussianSong { get; private set; }
        public Audio EnglishSong { get; private set; }

        public TestAudios()
        {
            RussianSong = new Audio() { Artist = "Yanix", Title = "Банька-Парилка" };
            EnglishSong = new Audio() { Artist = "Pendelum", Title = "Watercolour" };
        }
    }
}
