namespace uMidi.Instrument
{
    public interface IInstrument
    {
        void Process(byte[] midi);
    }
}