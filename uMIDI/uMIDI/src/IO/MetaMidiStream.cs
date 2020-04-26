using System;
using System.Collections.Generic;
using uMIDI.Common;
using uMIDI.Transform;

namespace uMIDI.IO
{
    public class MetaMidiStream
    {
        public MetaState MetaState { get; }
        public MidiStream MidiStream { get; }
        public List<ITransform> Transforms { get; set; }

        public MetaMidiStream(int beatsPerMeasure, int subdivision, int bpm)
        {
            MetaState = new MetaState(beatsPerMeasure, subdivision, bpm);
            MidiStream = new MidiStream();
            Transforms = new List<ITransform>();
        }

        public void PushBuffer(AbstractMessage[] messages)
        {
            // Add messages to buffer, applying transformations
            LinkedList<AbstractMessage> buffer = new LinkedList<AbstractMessage>();

            foreach (AbstractMessage message in messages)
            {
                buffer.AddLast(message);
            }

            foreach (ITransform transform in Transforms)
            {
                buffer = transform.Apply(buffer);
            }

            MidiStream.PushBuffer(buffer);
        }
    }
}
