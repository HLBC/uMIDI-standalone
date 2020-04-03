using System;
using uMIDI.Common;
using NAudio.Midi;
using System.Collections.Generic;


namespace uMIDI.Encoder
{
    public class MidiEncoder
    {
        public static List<List<IMessage>> readFromBytes(byte[] bytes)
        {
            MidiFile midifile = new MidiFile(bytes, false);
            MidiEventCollection events = midifile.Events;
            messages = parseMidiEvents(events);
        }

        private static List<List<IMessage>> parseMidiEvents(MidiEventCollection events)
        {
            int currentTrack = 0;
            List<List<IMessage>> overallMessages = new List<List<IMessage>>();
            foreach (List<MidiEvent> track in events)
            {
                List<IMessage> trackMessages = new List<IMessage>();
                foreach (MidiEvent midiEvent in track)
                {
                    IMessage message = getMessage(midiEvent);
                    if (message != null)
                        trackMessages.Add(message);
                }
                overallMessages.Add(trackMessages);
            }
            return overallMessages;
        }

        private static IMessage getMessage(MidiEvent midiEvent)
        {

            IMessage message;

            switch (midiEvent.CommandCode)
            {
                case MidiCommandCode.NoteOn:
                    NoteEvent noteEvent = (NoteEvent)midiEvent;
                    Note note = new Note((byte) noteEvent.Channel, (byte) noteEvent.NoteNumber, (byte) noteEvent.Velocity, noteEvent.AbsoluteTime);
                    message = new NoteOnMessage(note, midiEvent.AbsoluteTime);
                    return message;

                case MidiCommandCode.NoteOff:
                    NoteEvent noteOffEvent = (NoteEvent)midiEvent;
                    Note noteOff = new Note((byte) noteOffEvent.Channel, (byte) noteOffEvent.NoteNumber, (byte) noteOffEvent.Velocity, noteOffEvent.AbsoluteTime);
                    message = new NoteOffMessage(noteOff, midiEvent.AbsoluteTime);
                    return message;

                case MidiCommandCode.TimingClock:
                    message = new TimingTickMessage(midiEvent.AbsoluteTime);
                    return message;

                case MidiCommandCode.MetaEvent:
                    MetaEvent metaEvent = (MetaEvent)midiEvent;
                    return getMessage(metaEvent);

                default:
                    return null; // TODO implement remaining IMessage types

            }
        }

        private static IMessage getMessage(MetaEvent metaEvent)
        {
            IMessage message;

            switch (metaEvent.MetaEventType)
            {
                case MetaEventType.SetTempo:
                    TempoEvent tempoEvent = (TempoEvent)metaEvent;
                    message = new TempoMetaMessage(tempoEvent.Tempo, tempoEvent.AbsoluteTime);
                    return message;

                case MetaEventType.TimeSignature:
                    TimeSignatureEvent timeSignatureEvent = (TimeSignatureEvent) metaEvent;
                    message = new TimeSignatureMetaMessage(timeSignatureEvent.Numerator, timeSignatureEvent.Numerator, timeSignatureEvent.AbsoluteTime);
                    return message;

                case MetaEventType.KeySignature:
                    KeySignatureEvent keySignatureEvent = (KeySignatureEvent)metaEvent;
                    message = new TimeSignatureMetaMessage((sbyte)keySignatureEvent.SharpsFlats, (byte)keySignatureEvent.MajorMinor, keySignatureEvent.AbsoluteTime);
                    return message;

                default:
                    return null; // TODO remaining MetaMessage types

            }
        }
    }
}



