using System;
using System.Collections.Generic;
using uMIDI.Common;
using uMIDI.IO;

namespace uMIDI.src
{
    public class MidiFile
    {
        public MetaMidiStream MetaStream { get; }
        private List<IMessage> messages;
        public MidiFile()
        {
        }
    }
}
