using System;
using System.Collections.Generic;
using System.Text;

namespace uMIDI_decoder
{
    public class TrackEvent : Event
    {
        public String EventType { get; set; }
        public int Channel { get; set; }
        public int NoteVelocity  { get; set; }
        public int NoteCode { get; set; }
        public int DeltaTime { get; set; }
    }
}
