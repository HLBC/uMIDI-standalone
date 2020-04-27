using System;
using System.Collections.Generic;
using uMIDI.Common;
using uMIDI.Transform;

namespace uMIDI.IO
{
    public class MetaMidiStream
    {
        public MetaState MetaState { get; private set; }
        public MidiStream MidiStream { get; }
        public List<ITransform> Transforms { get; set; }

        public MetaMidiStream(int ticksPerBeat, int beatsPerMeasure,
            int subdivision, int bpm)
        {
            MetaState = new MetaState(beatsPerMeasure, subdivision, bpm);
            MidiStream = new MidiStream(ticksPerBeat);
            Transforms = new List<ITransform>();
        }

        public void PushBuffer(IMessage[] messages)
        {
            // Add messages to buffer, applying transformations
            LinkedList<IMessage> buffer = new LinkedList<IMessage>();

            foreach (IMessage message in messages)
            {
                buffer.AddLast(message);
            }

            // Convert to region
            Region region = Region.Messages2Region(buffer, MetaState);

            foreach (ITransform transform in Transforms)
            {
                region = transform.Apply(region);
            }

            (IMessage[] newBuffer, MetaState newState) = Region.Region2Messages(
                region);
            MetaState = newState;

            MidiStream.PushBuffer(newBuffer);
        }
    }
}
