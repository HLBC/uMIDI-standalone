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
        public MidiClock Clock { get => MidiStream.State.Clock; }

        public MetaMidiStream()
        {
            MetaState = new MetaState();
            MidiStream = new MidiStream();
            Transforms = new List<ITransform>();
        }

        public MetaMidiStream(int ticksPerBeat)
        {
            MetaState = new MetaState();
            MidiStream = new MidiStream(ticksPerBeat);
            Transforms = new List<ITransform>();
        }

        public MetaMidiStream(int ticksPerBeat, int beatsPerMeasure,
            int subdivision, int bpm, KeySignature key)
        {
            MetaState = new MetaState(beatsPerMeasure, subdivision, bpm, key);
            MidiStream = new MidiStream(ticksPerBeat);
            Transforms = new List<ITransform>();
        }

        public void PushBuffer(IMessage[] messages)
        {
            // Convert to region
            Region region = Region.Messages2Region(messages, MetaState);

            foreach (ITransform transform in Transforms)
            {
                transform.Apply(region);
            }

            (IMessage[] newBuffer, MetaState newState) = Region.Region2Messages(
                region);
            MetaState = newState;

            MidiStream.PushBuffer(newBuffer);
        }
    }
}
