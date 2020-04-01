using uMIDI.Common;

namespace uMIDI
{
    public interface IMidiInstrument
    {
        public void ProcessMidi(MidiMessage[] messages);
    }
}
