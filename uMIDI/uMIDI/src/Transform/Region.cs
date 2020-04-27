using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using uMIDI.Common;
using uMIDI.IO;

/// <summary>
/// Abstractions on top of <see cref="uMIDI.Common"/> to make transformations
/// more intuitive.
/// </summary>
namespace uMIDI.Transform
{
    /// <summary>
    /// A collection of <see cref="TimedNote"/>s, sorted by start time.
    /// </summary>
    public class Region : IEquatable<Region>, ICollection<TimedNote>
    {
        // Sorted by start time
        private List<TimedNote> _notes;

        public MetaState State { get; set; }

        public int Count => _notes.Count;

        public bool IsReadOnly => false;

        public Region(MetaState state)
        {
            _notes = new List<TimedNote>();
            State = state;
        }

        public Region(List<TimedNote> notes, MetaState state)
        {
            _notes = notes.OrderBy(o => o.StartTime).ToList();
            State = state;
        }

        public TimedNote[] NotesOnAt(long time)
        {
            List<TimedNote> notes = new List<TimedNote>(_notes.Capacity);

            foreach(TimedNote note in _notes)
            {
                if (note.StartTime <= time && note.EndTime >= time)
                {
                    notes.Add(note);
                }
            }

            return notes.ToArray();
        }

        public static Region Messages2Region(LinkedList<IMessage> buffer,
            MetaState state)
        {
            List<TimedNote> notes = new List<TimedNote>(buffer.Count);
            Dictionary<byte, TimedNote> notesOn
                = new Dictionary<byte, TimedNote>();
            long currentTime = 0;

            foreach (IMessage message in buffer)
            {
                currentTime += message.TimeDelta;
                if (message is NoteOnMessage noteOnMessage)
                {
                    TimedNote newNote = new TimedNote(
                        noteOnMessage.Note.Channel,
                        noteOnMessage.Note.Pitch,
                        noteOnMessage.Note.Velocity,
                        currentTime,
                        null
                        );
                    notes.Add(newNote);
                    notesOn.Add(newNote.Pitch, newNote);
                }
                else if (message is NoteOffMessage noteOffMessage)
                {
                    if (notesOn.ContainsKey(noteOffMessage.Note.Pitch))
                    {
                        notesOn.GetValueOrDefault(noteOffMessage.Note.Pitch)
                            .EndTime = currentTime;
                    }
                    else
                    {
                        TimedNote newNote = new TimedNote(
                            noteOffMessage.Note.Channel,
                            noteOffMessage.Note.Pitch,
                            noteOffMessage.Note.Velocity,
                            null,
                            currentTime
                            );
                        notes.Add(newNote);
                    }
                }
            }

            return new Region(notes, state);
        }

        public static (IMessage[], MetaState) Region2Messages(Region region)
        {
            LinkedList<IMessage> messages = new LinkedList<IMessage>();
            Dictionary<long, TimedNote> startTimes =
                new Dictionary<long, TimedNote>();
            Dictionary<long, TimedNote> endTimes =
                new Dictionary<long, TimedNote>();

            // Find start times and end times
            foreach (TimedNote note in region)
            {
                if (note.StartTime.HasValue)
                    startTimes.Add(note.StartTime.Value, note);
                if (note.EndTime.HasValue)
                    startTimes.Add(note.EndTime.Value, note);
            }

            long end = startTimes.Keys.Union(endTimes.Keys).Max();

            long lastEvent = 0;
            for (long i = 0; i < end; i++)
            {
                if (startTimes.Keys.Contains(i))
                {
                    TimedNote note = startTimes.GetValueOrDefault(i);
                    messages.AddLast(new NoteOnMessage(
                        note,
                        i - lastEvent
                        ));
                    lastEvent = i;
                }
                if (endTimes.Keys.Contains(i))
                {
                    TimedNote note = endTimes.GetValueOrDefault(i);
                    messages.AddLast(new NoteOffMessage(
                        note,
                        i - lastEvent
                        ));
                    lastEvent = i;
                }
            }

            return (messages.ToArray(), region.State);
        }

        public void Add(TimedNote note)
        {
            int index = 0;
            if (note.StartTime.HasValue)
            {
                long startTime = note.StartTime.Value;
                while (startTime < _notes[index].StartTime
                    && index < _notes.Count)
                {
                    index++;
                }
                _notes.Insert(index, note);
            }
            else
            {
                _notes.Insert(0, note);
            }
        }

        public void Clear()
        {
            _notes = new List<TimedNote>();
        }

        public bool Contains(TimedNote note)
        {
            return _notes.Contains(note);
        }

        public void CopyTo(TimedNote[] array, int arrayIndex)
        {
            _notes.CopyTo(array, arrayIndex);
        }

        public bool Equals([AllowNull] Region other)
        {
            return Enumerable.SequenceEqual(_notes, other._notes);
        }

        public IEnumerator<TimedNote> GetEnumerator()
        {
            return _notes.GetEnumerator();
        }

        public bool Remove(TimedNote item)
        {
            return _notes.Remove(item);
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return _notes.GetEnumerator();
        }
    }

    /// <summary>
    /// Abstraction upon <see cref="Note"/> which adds a start and end time
    /// to a note.
    /// </summary>
    public class TimedNote : Note, IEquatable<TimedNote>
    {
        // Absolute start time of note. null value denotes note is already being
        // played at instantiation.
        public long? StartTime { get; set; }
        // Absolute end time of note. null value denotes note is not released.
        public long? EndTime { get; set; }

        public TimedNote(byte channel, byte pitch, byte velocity,
            long? startTime, long? endTime) : base(channel, pitch, velocity)
        {
            StartTime = startTime;
            EndTime = endTime;
        }

        public bool Equals([AllowNull] TimedNote other)
        {
            return base.Equals(other) && StartTime == other.StartTime
                && EndTime == other.EndTime;
        }

        public override string ToString()
        {
            return String.Format("(Note {2} {0} ({1}) {3} Start: {4} End: {5})",
                Pitch, Name(), Channel, Velocity, StartTime, EndTime);
        }
    }
}
