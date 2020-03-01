namespace uMidi.Common
{
    // Decodes given midi data into uMidi structure
    public class File
    {
        private byte[] midi; // placeholder

        internal byte[] Midi() 
        {
            return midi;
        }

        // Filename is the complete path to a given midi file
        File(string filename)
        {
            midi = System.IO.File.ReadAllBytes(filename);
        }

        File(byte[] midi)
        {
            this.midi = midi;
        }
    }
}