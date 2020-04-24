using System;
using System.Collections.Generic;
using System.Text;
using uMIDI.Common;

namespace uMIDI_decoder.Models
{
    public class TrackMetaEventInfo
    {
        public IMessage trackOrMetaEvent { get; set; }
        public int eventSize { get; set; }
    }
}
