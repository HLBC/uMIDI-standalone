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
        private IMessage[] buffer;
        private IMessage[] lastBuffer;
        private int playHead;

        public MidiFile(MetaMidiStream stream, List<IMessage> messages,
            int bufferSize)
        {
            MetaStream = stream;
            BufferSize = bufferSize;
            EndOfFile = false;
            this.messages = messages;
            playHead = 0;
            buffer = new IMessage[BufferSize];
            lastBuffer = new IMessage[messages.Count % BufferSize];
        }

        public MidiFile(int bufferSize, int beatsPerMeasure, int subdivision,
            int bpm, List<IMessage> messages) : this(
                new MetaMidiStream(beatsPerMeasure, subdivision, bpm),
                messages, bufferSize)
        {

        }

        public void PushNextBuffer()
        {
            IMessage[] buffer;
            int range;
            if (playHead + BufferSize > messages.Count)
            {
                range = lastBuffer.Length;
                buffer = lastBuffer;
            }
            else
            {
                range = BufferSize;
                buffer = this.buffer;
            }

            for (int i = 0; i < range; i++)
            {
                buffer[i] = messages[playHead + i];
            }

            playHead += BufferSize;
            MetaStream.PushBuffer(buffer);
        }
    }
}
