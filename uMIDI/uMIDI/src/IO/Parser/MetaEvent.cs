using System;
using System.Collections.Generic;
using System.Text;

namespace uMIDI_decoder
{
    public class MetaEvent : Event
    {
        private String _eventType;
        private String _eventText;
        private int _deltaTime;

        public String EventType
        {
            get { return _eventType; }
            set { _eventType = value; }
        }

        public String EventText
        {
            get { return _eventText; }
            set { _eventText = value; }
        }

        public int DeltaTime
        {
            get { return _deltaTime; }
            set { _deltaTime = value; }
        }
    }
}
