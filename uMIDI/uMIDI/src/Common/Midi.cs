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

    public interface IMessage
    {
        public MidiMessage Message { get; }

        public long TimeDelta { get; }

        public static IMessage ToIMessage(MidiMessage msg, long timeDelta)
        {
            if (0x80 <= msg.Status && msg.Status < 0x90)
            {
                return new NoteOffMessage(new Note(
                    (byte)(msg.Status % 0x10),
                    msg.Data[0],
                    msg.Data[1],
                    timeDelta
                    ));
            }
            else if (0x90 <= msg.Status && msg.Status < 0xa0)
            {
                return new NoteOnMessage(new Note(
                    (byte)(msg.Status % 0x10),
                    msg.Data[0],
                    msg.Data[1],
                    timeDelta
                    ));
            }
            else if (msg.Status == 0xff)
            {
                throw new ArgumentException("This is a meta event. Please " +
                    "use the IMetaMessage.ToIMetaMessage() static method.");
            }
            else
            {
                throw new ArgumentException("MidiMessage not recognized");
            }
        }
    }

    public class NoteOffMessage : IMessage
    {
        public Note Note { get; }

        public long TimeDelta {
            get => Note.Time;
        }

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
                    Data = new byte[] { Note.Pitch, Note.Velocity },
                    TimeDelta = TimeDelta
                };
            }
        }
    }

    public class NoteOnMessage : IMessage
    {
        public Note Note { get; }

        public long TimeDelta { get => Note.Time; }

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
                    Data = new byte[] { Note.Pitch, Note.Velocity },
                    TimeDelta = TimeDelta
                };
            }
        }
    }
}
