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

    public class NoteOnMessage : IMessage, Note
    {
        MidiMessage Message
        {
            get
            {
                byte status = 0x90 + Channel;
                byte msgSize = 2;
                return new MidiMessage(Status = status, MsgSize = msgSize, Data = new List<Byte> {Pitch, Velocity});
            }
        }
    }
}