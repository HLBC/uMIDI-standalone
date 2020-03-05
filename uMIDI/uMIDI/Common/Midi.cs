namespace uMIDI.Common
{
    public struct MidiMessage
    {
        public byte Status;
        public byte MsgSize;
        public byte[] Data;
    }

    public interface IMessage
    {
        MidiMessage Message { get; };
    }
}