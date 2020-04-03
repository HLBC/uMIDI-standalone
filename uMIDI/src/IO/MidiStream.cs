using System.Collections.Generic;
using uMIDI.Common;

namespace uMIDI.IO
{
    public class MidiStream
    {
        public MidiStreamState State { get; }
        private ISet<IMidiInstrument> instruments;

        public MidiStream()
        {
            State = new MidiStreamState();
            instruments = new HashSet<IMidiInstrument>();
        }

        public void AddInstrument(IMidiInstrument instrument)
        {
            //TODO
        }

        // Formerly SendToStream()
        public void Push(IMessage message)
        {
            //TODO
        }
    }
}
