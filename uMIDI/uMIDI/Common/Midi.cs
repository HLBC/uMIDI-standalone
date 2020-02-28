namespace uMIDI.Common
{
    public struct MidiMessage
    {
        public byte Status;
        public byte Byte1;
        public byte Byte2;
    }

    public interface IMessage
    {
        MidiMessage Message { get; };
    }

    public interface INote
    {
        INote ShiftByInterval(int interval);
    }

    public class MSystemReset : IMessage
    {
        public override MidiMessage Message
        {
            get { return new MidiMessage(Status = 0xFF, Byte1 = 0, Byte2 = 0); }
        }
    }
}