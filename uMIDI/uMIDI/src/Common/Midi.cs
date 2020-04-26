using System;
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

    public abstract class AbstractMessage
    {
        public abstract MidiMessage Message { get; }

        public abstract long TimeDelta { get; }
    }

    public class NoteOffMessage : AbstractMessage
    {
        public Note Note { get; }

        public override long TimeDelta { get; }

        public NoteOffMessage(Note note, long timeDelta)
        {
            Note = note;
            TimeDelta = timeDelta;
        }

        public override MidiMessage Message
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

    public class NoteOnMessage : AbstractMessage
    {
        public Note Note { get; }

        public override long TimeDelta { get; }

        public NoteOnMessage(Note note, long timeDelta)
        {
            Note = note;
            TimeDelta = timeDelta;
        }

        public override MidiMessage Message
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
}
