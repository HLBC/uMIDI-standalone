using uMidi.Common;

namespace uMidi.Transform
{
    public interface ITransform 
    {
        File Apply(File midi);
    }
}