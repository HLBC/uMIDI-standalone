namespace uMIDI.Common
{
    public struct BeatClockTime
    {
        public uint Measure { get; set; }
        public uint Beat { get; set; }
        public double Subdivision
    }

    public struct Note
    {
        public byte Velocity { get; set; }
        public byte Pitch { get; set; }
        public uint StartTick { get; set; }
        public uint EndTick { get; set; }
    }
}