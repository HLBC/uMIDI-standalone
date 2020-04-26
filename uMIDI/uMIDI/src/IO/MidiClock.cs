using System;
namespace uMIDI.IO
{
    public class MidiClock
    {
        public long CurrentTick { get; private set; }

        public int TicksPerBeat { get; set; }

        public long CurrentBeat
        {
            get
            {
                return CurrentTick / TicksPerBeat;
            }
        }

        public MidiClock(int ticksPerBeat)
        {
            CurrentTick = 0;
            TicksPerBeat = ticksPerBeat;
        }

        public void Advance(long amount)
        {
            if (amount < 0)
            {
                throw new ArgumentException(
                    "Advance() must have positive argument");
            }
            CurrentTick += amount;
        }
    }
}
