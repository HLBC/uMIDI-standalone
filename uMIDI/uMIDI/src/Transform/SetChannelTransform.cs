using System;
using uMIDI.IO;

namespace uMIDI.Transform
{
    public class SetChannelTransform : ITransform
    {
        public SetChannelTransform()
        {
        }

        public MidiStream MidiStream => throw new NotImplementedException();

        public Region Apply(Region region)
        {
            throw new NotImplementedException();
        }
    }
}
