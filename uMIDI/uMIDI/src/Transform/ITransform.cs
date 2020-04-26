using System;
using System.Collections.Generic;
using uMIDI.Common;
using uMIDI.IO;

namespace uMIDI.Transform
{
    public interface ITransform
    {
        MidiStream MidiStream { get; }
        LinkedList<AbstractMessage> Apply(LinkedList<AbstractMessage> buffer);
    }
}
