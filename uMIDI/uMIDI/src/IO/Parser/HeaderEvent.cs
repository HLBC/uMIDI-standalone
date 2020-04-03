using System;
using System.Collections.Generic;
using System.Text;

namespace uMIDI_decoder
{
    public class HeaderEvent : Event
    {
        #region --Fields--

        private String _format;
        private int _numberOfTracks;
        private int _unitForDeltaTiming;

        #endregion

        #region -- Variables--

       public String Format
        {
            get { return _format; }
            set { _format = value; }
        }

        public int NumberOfTracks
        {
            get { return _numberOfTracks; }
            set { _numberOfTracks = value; }
        }

        public int UnitForDeltaTiming
        {
            get { return _unitForDeltaTiming; }
            set { _unitForDeltaTiming = value; }
        }

        #endregion
    }
}
