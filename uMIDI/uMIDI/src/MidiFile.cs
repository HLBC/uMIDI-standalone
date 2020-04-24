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
        private List<IMessage> messages;
        private int playHead;

        public MidiFile(MetaMidiStream stream, List<IMessage> messages,
            int bufferSize)
        {
            MetaStream = stream;
            BufferSize = bufferSize;
            EndOfFile = false;
            this.messages = messages;
            playHead = 0;
        }

        public MidiFile(int bufferSize, int beatsPerMeasure, int subdivision,
            int bpm, List<IMessage> messages) : this(
                new MetaMidiStream(beatsPerMeasure, subdivision, bpm),
                messages, bufferSize)
        {

        }

        public void PushNextBuffer()
        {

        }
    }
}
