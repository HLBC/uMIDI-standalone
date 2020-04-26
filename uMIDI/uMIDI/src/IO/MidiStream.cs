using System.Collections.Generic;
using uMIDI.Common;
using uMIDI.Transform;

namespace uMIDI.IO
{
    public class MidiStream
    {
        public MidiStreamState State { get; }
        private ISet<IMidiInstrument> instruments;
        private LinkedList<AbstractMessage> buffer;
        private int bufferCount;

        public MidiStream()
        {
            State = new MidiStreamState();
            instruments = new HashSet<IMidiInstrument>();
            bufferCount = 0;
        }

        public void AddInstrument(IMidiInstrument instrument)
        {
            instruments.Add(instrument);
        }

        public void PushBuffer(AbstractMessage[] messages)
        {
            // Add messages to buffer
            foreach (AbstractMessage message in messages)
            {
                buffer.AddLast(message);
                bufferCount++;
            }
        }

        public void PushBuffer(LinkedList<AbstractMessage> messages)
        {
            // Add messages to buffer
            foreach (AbstractMessage message in messages)
            {
                buffer.AddLast(message);
                bufferCount++;
            }
        }

        public double MillisecondsPerTick()
        {
            // TODO
            return 0;
        }

        public void Update()
        {
            // Call ProcessMidi and clear buffer
            AbstractMessage[] bufferArr = new AbstractMessage[bufferCount];
            LinkedListNode<AbstractMessage> msg = buffer.First;
            for (int i = 0; i < bufferCount; i++)
            {
                bufferArr[i] = msg.Value;
                msg = msg.Next;
            }

            foreach (IMidiInstrument inst in instruments)
            {
                inst.ProcessMidi(bufferArr);
            }

            buffer = new LinkedList<AbstractMessage>();
        }
    }
}
