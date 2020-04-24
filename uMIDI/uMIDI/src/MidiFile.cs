using System;
using System.Collections.Generic;
using uMIDI.Common;
using uMIDI.IO;

namespace uMIDI.src
{
    public class MidiFile
    {
        public MetaMidiStream MetaStream { get; }
        public int BufferSize { get; }
        public bool EndOfFile { get; }
        private LinkedList<IMessage> messages;
        private int playHead;

        public MidiFile(int bufferSize)
        {
            MetaStream = new MetaMidiStream();
            BufferSize = bufferSize;
            EndOfFile = false;
            messages = new LinkedList<IMessage>();
            playHead = 0;
        }

        public void PushNextBuffer()
        {

        }
    }
}
