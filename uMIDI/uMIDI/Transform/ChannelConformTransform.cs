using uMidi;

namespace uMidi.Transform
{
    internal class ChannelConformTransform : ITransform
    {
        private int channel;

        public ChannelConformTransform(int channel)
        {
            this.channel = channel;
        }

        public File Apply(File midi)
        {
            // TODO: Change all channels in the midi file to "channel"
            return midi;
        }
    }
}