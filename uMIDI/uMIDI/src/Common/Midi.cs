using System.Linq;

namespace uMIDI.Common
{
    public struct MidiMessage
    {
        public byte Status;
        public byte[] Data;
        public long TimeDelta;

        public byte[] ToBytes()
        {
            // TODO return TimeDelta as well?
            byte[] arr = new byte[Data.Length + 1];
            arr[0] = Status;
            Data.CopyTo(arr, 1);
            return arr;
        }
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
                    Data = new byte[0],
                    TimeDelta = TimeDelta
                };
            }
        }
    }
}
