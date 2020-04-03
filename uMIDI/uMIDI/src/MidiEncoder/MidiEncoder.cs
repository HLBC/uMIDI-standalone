using System;

namespace uMIDI.Encoder
{
    public class MidiEncoder
    {
        List<IList<IMessage>> messages;

        private void readFromBytes(byte[] bytes)
        {
            MidiFile midifile = new MidiFile(bytes, false);
            MidiEventCollection events = midifile.Events;
        }

        private List<IList<IMessage>> parseMidiEvents(MidiEventCollection events)
        {
            int currentTrack = 0;
            List<IList<IMessage>> overallMessages;
            foreach (IList<MidiEvent> track in events)
            {
                IList<IMessage> trackMessages = new List<IMessage>();
                foreach (MidiEvent midiEvent in track)
                {
                    IMessage message = getMessage(midiEvent);
                    if (message != null)
                        trackMessages.add(message);
                }
                overallMessages.Add(trackMessages);
            }
        }

        public IMessage getMessage(MidiEvent midiEvent)
        {

            switch (midiEvent.CommandCode)
            {
                case MidiCommandCode.NoteOn:
                    NoteEvent noteEvent = (NoteEvent)midiEvent;
                    Note note = new Note(noteEvent.channel, noteEvent.NoteNumber, noteEvent.Velocity);
                    NoteOnMessage message = new NoteOnMessage(note);
                    return message;

                case MidiCommandCode.NoteOff:
                    NoteEvent noteEvent = (NoteEvent)midiEvent;
                    Note note = new Note(noteEvent.channel, noteEvent.NoteNumber, noteEvent.Velocity);
                    NoteOffMessage message = new NoteOffMessage(note);
                    trackMessages.add(message);
                    return message;

                case MidiCommandCode.TimingClock:
                    NoteOffMessage message = new NoteOffMessage(midiEvent.AbsoluteTime);
                    trackMessages.add(message);
                    return message;

                case MidiCommandCode.MetaEvent:
                    MetaEvent metaEvent = (MetaEvent)midiEvent;
                    return getMessage(metaEvent);

                default:
                    return null; // TODO implement remaining IMessage types

            }
        }

        public IMessage getMessage(MetaEvent metaEvent)
        {

            switch (metaEvent.MetaEventType)
            {
                case SetTempo:
                    TempoEvent tempoEvent = (TempoEvent)metaEvent;
                    TempoMetaMessage message = TempoMetaMessage(tempoEvent.Tempo, tempoEvent.AbsoluteTime);
                    return message;

                case TimeSignature:
                    TimeSignatureEvent timeSignatureEvent = (TimeSignatureEvent)metaEvent;
                    TimeSignatureMetaMessage message = TimeSignatureMetaMessage(timeSignatureEvent.Numerator, timeSignatureEvent.Numerator, timeSignatureEvent.AbsoluteTime);
                    return message;

                case KeySignature:
                    KeySignatureEvent keySignatureEvent = (KeySignatureEvent)metaEvent;
                    TimeSignatureMetaMessage message = TimeSignatureMetaMessage((sbyte)keySignatureEvent.SharpsFlats, (byte)keySignatureEvent.MajorMinor, timeSignatureEvent.AbsoluteTime);
                    return message;

                default:
                    return null; // TODO remaining MetaMessage types

            }
        }
    }
}



