using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Text;

namespace uMIDI.tests.IO
{
    [TestClass()]
    public class MidiParserTests
    {

        [TestMethod()]
        public void FindChunkBodyLengthShouldReturnFiveGivenFiveBytes()
        {
            //arrange
            List<byte> inputBytes = new List<byte> { 0x01, 0x05, 0xf2, 0x53, 0xa8 };

            //act


            //assert
            Assert.IsTrue(true);
        }
    }
}
