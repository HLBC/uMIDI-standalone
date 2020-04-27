using System;
using System.Collections.Generic;
using uMIDI.Common;
using uMIDI.IO;

namespace uMIDI
{
    public class MidiFile
    {
        public MetaMidiStream MetaStream { get; }
        public int BufferSize { get; }
        public bool EndOfFile { get; }
        private List<AbstractMessage> _messages;
        private int _playHead;

        public MidiFile(MetaMidiStream stream, List<AbstractMessage> messages,
            int bufferSize)
        {
            MetaStream = stream;
            BufferSize = bufferSize;
            EndOfFile = false;
            _messages = new List<AbstractMessage>();
            _playHead = 0;
        }

        public MidiFile(int ticksPerBeat, int bufferSize, int beatsPerMeasure,
            int subdivision, int bpm, List<AbstractMessage> messages,
            KeySignature key) :
            this(new MetaMidiStream(ticksPerBeat, beatsPerMeasure, subdivision,
                    bpm, key), messages, bufferSize)
        {

        }

        public void AddMessage(AbstractMessage messageData)
        {
            _messages.Add(messageData);
        }

        public void AddMessage(List<AbstractMessage> messageData)
        {
            _messages.AddRange(messageData);
        }

        public void PushNextBuffer()
        {

        }
    }
}
