namespace uMidi.Transform
{
    // Utility class to provide static midi transformations
    public class StaticTransforms
    {
        private StaticTransforms() {} // Utility classes don't allow instantiation

        // Provides transformation that forces a midi file to a specific channel
        public static ITransform ConformToChannel(int channel)
        {
            return new ChannelConformTransform(channel);
        }
    }
}