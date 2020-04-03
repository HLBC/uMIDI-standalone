using System;
using System.Collections.Generic;
using System.Text;

namespace uMIDI_decoder.Models
{
    public class TrackMetaEventInfo
    {
        public Event trackOrMetaEvent { get; set; }
        public int eventSize { get; set; }
    }
}
