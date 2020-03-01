using uMidi;

namespace uMidi.Transform
{
    public interface ITransform 
    {
        File Apply(File midi);
    }
}