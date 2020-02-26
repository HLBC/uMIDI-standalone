namespace uMIDI.Common
{
    public class Region
    {
        public Time Start { get; }

        public Time End { get; }

        public Region(Time start, Time end)
        {
            Start = start;
            End = end;
        }
    }
}
