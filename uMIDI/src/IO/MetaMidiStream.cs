using System;
namespace uMIDI.IO
{
    public class MetaMidiStream
    {
        public MetaState MetaState { get; }
        public MidiStream MidiStream { get; }

        public MetaMidiStream()
        {
            MetaState = new MetaState();
            MidiStream = new MidiStream();
        }
    }
}
