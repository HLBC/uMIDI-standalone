namespace uMIDI.Common
{
    public struct MidiMessage
    {
        public byte Status;
        public byte MsgSize;
        public byte[] Data;
        public long TimeDelta;
    }

    public interface IMessage
    {
        public MidiMessage Message { get; }

        public long TimeDelta { get; }
    }

    public class NoteOffMessage : IMessage
    {
        public Note Note { get; }

        public long TimeDelta { get; }

        public NoteOffMessage(Note note, long time)
        {
            Note = note;
            TimeDelta = time;
        }

        public MidiMessage Message
        {
            get
            {
                return new MidiMessage
                {
                    Status = (byte)(0x80 + Note.Channel),
                    MsgSize = 2,
                    Data = new byte[] { Note.Pitch, Note.Velocity },
                    TimeDelta = TimeDelta
                };
            }
        }
    }

    public class NoteOnMessage : IMessage
    {
        public Note Note { get; }

        public long TimeDelta { get; }

        public NoteOnMessage(Note note, long time)
        {
            Note = note;
            TimeDelta = time;
        }

        public MidiMessage Message
        {
            get
            {
                return new MidiMessage
                {
                    Status = (byte)(0x90 + Note.Channel),
                    MsgSize = 2,
                    Data = new byte[] { Note.Pitch, Note.Velocity },
                    TimeDelta = TimeDelta
                };
            }
        }
    }

    public class TimingTickMessage : IMessage
    {
        public long TimeDelta { get; }

        public TimingTickMessage(long time)
        {
            TimeDelta = time;
        }

        public MidiMessage Message
        {
            get
            {
                return new MidiMessage
                {
                    Status = 0xf8,
                    MsgSize = 0,
                    Data = new byte[0],
                    TimeDelta = TimeDelta
                };
            }
        }
    }
}
