using System.Collections.Generic;
using uMidi.Instrument;
using uMidi.Transform;
using uMidi.Common;

namespace uMidi
{
    // Streams midi files through transforms to be played on instruments
    public class Stream
    {
        private List<ITransform> transforms;
        private List<IInstrument> instruments;

        public Stream() : this(new List<ITransform>(), new List<IInstrument>()) {}

        private Stream(List<ITransform> transforms, List<IInstrument> instruments)
        {
            this.transforms = transforms;
            this.instruments = instruments;
        }

        public void AddTransform(ITransform transform)
        {
            transforms.Add(transform);
        }

        public void BindInstrument(IInstrument instrument)
        {
            instruments.Add(instrument);
        }

        public void Play(File midi)
        {
            // TODO: Figure out how we will keep track of time in the system
            // Stream midi file through transforms
            // Output transformed stream to instruments (in "realtime")
        }

        // Performs a shallow copy
        public Stream Copy()
        {
            return new Stream(new List<ITransform>(this.transforms), new List<IInstrument>(this.instruments));
        }

        // Performs a shallow copy on the given Stream
        public static Stream Copy(Stream toCopy)
        {
            return toCopy.Copy();
        }
    }
}