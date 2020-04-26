using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using uMIDI.Common;

/// <summary>
/// Abstractions on top of <see cref="uMIDI.Common"/> to make transformations
/// more intuitive.
/// </summary>
namespace uMIDI.Transform.Region
{
    /// <summary>
    /// A collection of <see cref="TimedNote"/>s.
    /// </summary>
    public class Region : IEquatable<TimedNote>, ICollection<TimedNote>
    {
        private List<TimedNote> _notes;

        public int Count => _notes.Count;

        public bool IsReadOnly => false;

        public void Add(TimedNote item)
        {
            throw new NotImplementedException();
        }

        public void Clear()
        {
            throw new NotImplementedException();
        }

        public bool Contains(TimedNote item)
        {
            throw new NotImplementedException();
        }

        public void CopyTo(TimedNote[] array, int arrayIndex)
        {
            throw new NotImplementedException();
        }

        public bool Equals([AllowNull] TimedNote other)
        {
            throw new NotImplementedException();
        }

        public IEnumerator<TimedNote> GetEnumerator()
        {
            throw new NotImplementedException();
        }

        public bool Remove(TimedNote item)
        {
            throw new NotImplementedException();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            throw new NotImplementedException();
        }
    }

    /// <summary>
    /// Abstraction upon <see cref="Note"/> which adds a start and end time
    /// to a note.
    /// </summary>
    public class TimedNote : Note, IEquatable<TimedNote>
    {
        // Absolute start time of note. -1 denotes note is already being played
        // at instantiation.
        public long StartTime { get; set; }
        // Absolute end time of note. -1 denotes note is not released.
        public long EndTime { get; set; }

        public TimedNote(byte channel, byte pitch, byte velocity,
            long startTime, long endTime) : base(channel, pitch, velocity)
        {
            StartTime = startTime;
            EndTime = endTime;
        }

        public bool Equals([AllowNull] TimedNote other)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Converts to a <see cref="NoteOnMessage"/> and
        /// <see cref="NoteOffMessage"/>. Note that the time deltas for each
        /// message must be passed in, as other events can influence the exact
        /// values (such as multiple notes being held at once).
        /// </summary>
        /// <param name="onTimeDelta"></param>
        /// <param name="offTimeDelta"></param>
        /// <returns></returns>
        public (NoteOnMessage, NoteOffMessage) ToMessages(long onTimeDelta,
            long offTimeDelta)
        {
            return (
                new NoteOnMessage(this, onTimeDelta),
                new NoteOffMessage(this, offTimeDelta)
                );
        }
    }
}
