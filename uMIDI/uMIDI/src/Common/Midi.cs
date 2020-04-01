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
        public MidiMessage Message { get; }
    }

    public class NoteOffMessage : IMessage
    {
        public Note Note { get; }

        public NoteOffMessage(Note note)
        {
            Note = note;
        }

        public MidiMessage Message
        {
            get
            {
                return new MidiMessage
                {
                    Status = (byte)(0x80 + Note.Channel),
                    MsgSize = 2,
                    Data = new byte[] { Note.Pitch, Note.Velocity }
                };
            }
        }
    }

    public class NoteOnMessage : IMessage
    {
        public Note Note { get; }

        public NoteOnMessage(Note note)
        {
            Note = note;
        }

        public MidiMessage Message
        {
            get
            {
                return new MidiMessage
                {
                    Status = (byte)(0x90 + Note.Channel),
                    MsgSize = 2,
                    Data = new byte[] { Note.Pitch, Note.Velocity }
                };
            }
        }
    }

    public class TimingTickMessage : IMessage
    {
        public MidiMessage Message
        {
            get
            {
                return new MidiMessage
                {
                    Status = 0xf8,
                    MsgSize = 0,
                    Data = new byte[0]
                };
            }
        }
    }
}
