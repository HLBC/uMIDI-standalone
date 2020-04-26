using System;
using System.Collections.Generic;
using System.Text;
using uMIDI.Common;

namespace uMIDI.IO
{
    public class TrackMetaEventInfo
    {
        public AbstractMessage trackOrMetaEvent { get; set; }
        public int eventSize { get; set; }
    }
}
