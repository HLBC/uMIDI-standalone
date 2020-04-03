using uMIDI.Common;

namespace uMIDI
{
    public interface IMidiInstrument
    {
        void ProcessMidi(MidiMessage[] messages);
    }
}
