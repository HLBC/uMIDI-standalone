using System;
using System.Collections.Generic;
using uMIDI.Common;
using uMIDI.IO;

namespace uMIDI.Transform
{
    public interface ITransform
    {
        public MidiStream MidiStream { get; }
        public LinkedList<IMessage> Apply(LinkedList<IMessage> buffer);
    }
}
