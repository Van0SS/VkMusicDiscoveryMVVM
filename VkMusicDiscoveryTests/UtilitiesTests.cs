using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using VkMusicDiscoveryMVVM;

namespace VkMusicDiscoveryTests
{
    [TestClass]
    public class UtilitiesTests
    {
        [TestMethod]
        public void ToLowerButFirstUpTest()
        {
            string input = "CAPSNAME";
            string expected = "Capsname";
            
            var output = Utilities.ToLowerButFirstUp(input);
            Assert.AreEqual(expected, output);

            string input2 = "lowercasename";
            string expected2 = "Lowercasename";

            var output2 = Utilities.ToLowerButFirstUp(input2);
            Assert.AreEqual(expected2, output2);

            string input3 = "rAndOmcasenAme";
            string expected3 = "Randomcasename";

            var output3 = Utilities.ToLowerButFirstUp(input3);
            Assert.AreEqual(expected3, output3);
        }
    }
}
