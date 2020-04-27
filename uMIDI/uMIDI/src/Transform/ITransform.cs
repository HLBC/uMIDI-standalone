using uMIDI.IO;

namespace uMIDI.Transform
{
    public interface ITransform
    {
        MidiStream MidiStream { get; }
        Region Apply(Region region);
    }
}
