namespace uMIDI.Common
{
    public class Note
    {
        public byte Channel { get; set; }
        public byte Pitch { get; set; }
        public byte Velocity { get; set; }

        public string PitchName
        {
            get
            {
                switch (Pitch % 12)
                {
                    case 0:
                        return "C";
                    case 1:
                        return "Db";
                    case 2:
                        return "D";
                    case 3:
                        return "Eb";
                    case 4:
                        return "E";
                    case 5:
                        return "F";
                    // TODO finish this
                }
            }
        }

        public Note(byte channel, byte pitch, byte velocity)
        {
            Channel = channel;
            Pitch = pitch;
            Velocity = velocity;
        }
        
        public Note Transpose(int interval)
        {
            int newPitch = Pitch + interval;
            if (newPitch < 0 || newPitch > 127)
            {
                throw new IllegalArgumentException("Resulting pitch is too high/low!");
            }
            return new Note(Channel, Pitch + interval, Velocity);
        }
    }

    public enum Tonic
    {
        C = 0,
        Db = 1,
        D = 2,
        Eb = 3,
        E = 4,
        F = 5,
        Gb = 6,
        G = 7,
        Ab = 8,
        A = 9,
        Bb = 10,
        B = 11
    }
}