using System;
using System.Collections.Generic;
using System.Linq;
using uMIDI_decoder.Utility;
using uMIDI_decoder.Models;

namespace uMIDI_decoder
{
    public class MidiParser
    {
        #region --Constants--

        private static readonly List<String> headerChunk = new List<String> { "4d", "54", "68", "64" };
        private static readonly List<String> trackChunk = new List<String> { "4d", "54", "72", "6b" };
        private static readonly IDictionary<int, String> formatMap = new Dictionary<int, String> { { 0, "single track file format" }, { 1, "multiple track file format" }, { 2, "multiple song file format" } };
        private static readonly IDictionary<String, int> headerPartsIndexMap = new Dictionary<String, int> { { "format", 0 }, { "numberOfTracks", 2 }, { "unitForDeltaTiming", 4 } };
        private static readonly IDictionary<String, int> headerPartsSizeMap = new Dictionary<String, int> { { "format", 2 }, { "numberOfTracks", 2 }, { "unitForDeltaTiming", 2 } };
        private static readonly IDictionary<byte, string> metaEventCodeDictionary = new Dictionary<byte, string> { { 0x00, "sequence number" }, { 0x01, "text event" }, { 0x02, "copyright notice" }, { 0x03, "sequence or track name" }, { 0x04, "instrument name" }, { 0x05, "lyric text" }, { 0x06, "marker text" }, { 0x07, "cue point" }, { 0x20, "MIDI channel prefix assignment" }, { 0x2F, "end of track" }, { 0x51, "tempo setting" }, { 0x54, "SMPTE offset" }, { 0x58, "time signature" }, { 0x59, "key signature" }, { 0x7F, "sequencer specific event" } };
        private static readonly IDictionary<String, String> trackEventStatusDictionary = new Dictionary<String, String> { { "8", "note off" }, { "9", "note on" } };
        private const int chunkBodyLengthSize = 4;

        #endregion

        #region --Functions--


        #region Temporary fix to using strings over bytes
        private static List<byte> Strings2Bytes(List<string> strings)
        {
            CheckSizes(strings, 2);

            List<byte> bytes = new List<byte>(strings.Count);

            foreach (string s in strings) bytes.Add(HexByte.Paired(s[0], s[1]).Value());

            return bytes;
        }

        private static void CheckSizes(List<string> strings, int numOfChars)
        {
            foreach (string s in strings)
                if (s.Length < numOfChars)
                    throw new ArgumentException("Not all strings are at least " + numOfChars + " long. (" + s + ")");
        }
        #endregion

        public List<Event> DecodeMidi(List<String> midiFile)
        {
            List<Event> decodedEventList = new List<Event>();
            int chunkSize = 1;

            if (midiFile.Count() < 4)
            {
                return decodedEventList;         //ERROR: If the code reaches here, there were extra bytes left over that weren't covered by any type of chunk.
            }

            if (IsHeaderChunk(midiFile.GetRange(0, 4)))
            {
                int bodySize = FindChunkBodyLength(midiFile.GetRange(4, 4));
                chunkSize = bodySize + headerChunk.Count() + chunkBodyLengthSize;
                decodedEventList.Add(ProcessHeaderChunk(midiFile.GetRange(headerChunk.Count() + chunkBodyLengthSize, bodySize)));
            }
            else if (IsTrackChunk(midiFile.GetRange(0, 4)))
            {
                int bodySize = FindChunkBodyLength(midiFile.GetRange(4, 4));
                chunkSize = bodySize + trackChunk.Count() + chunkBodyLengthSize;
                decodedEventList = decodedEventList.Concat(ProcessTrackChunk(midiFile.GetRange(trackChunk.Count() + chunkBodyLengthSize, bodySize))).ToList();
            }
            
            if (midiFile.Count() > chunkSize) {
                return decodedEventList.Concat(DecodeMidi(midiFile.GetRange(chunkSize, decodedEventList.Count() - 1))).ToList();
            } 
            else
            {
                return decodedEventList;
            }
        }

        /// <summary>
        /// Finds and converts the chunk's body's length as a decimal number of bytes.
        /// </summary>
        /// <param name="chunkSize">A <see cref="List{T}"/> of four hexadecimal bytes.</param>
        /// <returns>Length of the chunk's body's length.</returns>
        private int FindChunkBodyLength(List<String> chunkSize)
        {
            // receive 4 bytes of hexadecimal and turn into decimal to figure out how big the header chunk is then return this
            int size =
                Convert.ToInt32(chunkSize[0], 16) * 16777216 +
                Convert.ToInt32(chunkSize[1], 16) * 65536 +
                Convert.ToInt32(chunkSize[2], 16) * 256 +
                Convert.ToInt32(chunkSize[3], 16);
            return size;
        }

        /// <summary>
        /// Receives four hexadecimal byte <see cref="List{T}"/> and checks if it is a chunk identifier for a header.
        /// </summary>
        /// <param name="chunkIdentifier">A <see cref="List{T}"/> of four hexadecimal bytes.</param>
        /// <returns>true if header chunk; else false.</returns>
        private bool IsHeaderChunk(List<String> chunkIdentifier)
        {
            return chunkIdentifier.SequenceEqual(headerChunk);
        }

        /// <summary>
        /// Converts from the hexadecimal bytes from the header chunk into a <see cref="Event"/>.
        /// </summary>
        /// <param name="headerChunkBody">A <see cref="List{T}"/> of hexadecimal bytes that represent the header's body.</param>
        /// <returns>A <see cref="List"/> containing a single <see cref="Event"/>.</returns>
        private Event ProcessHeaderChunk(List<String> headerChunkBody)
        {
            return new HeaderEvent()
            {
                Format = FindFormat(headerChunkBody.GetRange(headerPartsIndexMap["format"], headerPartsSizeMap["format"])),
                NumberOfTracks = FindNumberOfTracks(headerChunkBody.GetRange(headerPartsIndexMap["numberOfTracks"], headerPartsSizeMap["numberOfTracks"])),
                UnitForDeltaTiming = FindUnitForDeltaTiming(headerChunkBody.GetRange(headerPartsIndexMap["unitForDeltaTiming"], headerPartsSizeMap["unitForDeltaTiming"]))
            };
        }

        /// <summary>
        /// Finds the song format of the file from the header body.
        /// </summary>
        /// <param name="formatInfo">A <see cref="List{T}"/> of two hexadecimal bytes.</param>
        /// <returns>A <see cref="String"/> describing the format of the file.</returns>
        private String FindFormat(List<String> formatInfo)
        {
            int formatKey = Convert.ToInt32(formatInfo[0], 16) * 256 + Convert.ToInt32(formatInfo[1], 16);
            string formatValue;
            
            if (formatMap.TryGetValue(formatKey, out formatValue))
            {
                return formatValue;
            } 
            else
            {
                return String.Empty;        //ERROR: If the code reaches here, there has been an error.
            }
        }

        /// <summary>
        /// Finds the number of tracks in the file from the header body.
        /// </summary>
        /// <param name="numberOfTracksInfo">A <see cref="List{T}"/> of two hexadecimal bytes.</param>
        /// <returns>The number of tracks related to this header.</returns>
        private int FindNumberOfTracks(List<String> numberOfTracksInfo)
        {
            return Convert.ToInt32(numberOfTracksInfo[0], 16) * 256 + Convert.ToInt32(numberOfTracksInfo[1], 16);
        }

        /// <summary>
        /// Finds the unit for delta timing (if positive, it is ticks per beat and if negative, delta times are in SMPTE compatible units).
        /// </summary>
        /// <param name="unitForDeltaTimingInfo">A <see cref="List{T}"/> of two hexadecimal bytes.</param>
        /// <returns>The unit for delta timing</returns>
        private int FindUnitForDeltaTiming(List<String> unitForDeltaTimingInfo)
        {
            return Convert.ToInt32(unitForDeltaTimingInfo[0], 16) * 256 + Convert.ToInt32(unitForDeltaTimingInfo[1], 16);
        }

        /// <summary>
        /// Receives four hexadecimal byte <see cref="List{T}"/> and checks if it is a chunk identifier for a track.
        /// </summary>
        /// <param name="chunkIdentifier">A <see cref="List{T}"/> of four hexadecimal bytes.</param>
        /// <returns>true if track chunk; else false.</returns>
        private bool IsTrackChunk(List<String> chunkIdentifier)
        {
            return chunkIdentifier.SequenceEqual(trackChunk);
        }

        /// <summary>
        /// Converts from the hexadecimal bytes from the track chunk into a <see cref="List"/> of <see cref="Event"/>s.
        /// </summary>
        /// <param name="trackChunk">A <see cref="List"/> of hexadecimal bytes that represent the track chunk.</param>
        /// <returns>A <see cref="List"/> of <see cref="Event"/>s that represent the information from the track chunk.</returns>
        private List<Event> ProcessTrackChunk(List<String> trackChun)
        {
            List<byte> trackChunk = Strings2Bytes(trackChun);
            List<Event> decodedEventList = new List<Event>();
            bool endOfTrack = false;

            while (!endOfTrack)
            {
                VariableChunkSize<int> timeChunk = FindDeltaTime(trackChunk);
                trackChunk = trackChunk.GetRange(timeChunk.chunkSize, trackChunk.Count() - timeChunk.chunkSize);

                int currentEventSize = 0;
                byte nextByte = trackChunk.First();
                if (nextByte == 0xFF)
                {
                    TrackMetaEventInfo metaEventInfo = ProcessMetaEvent(trackChunk, timeChunk.data);
                    currentEventSize = metaEventInfo.eventSize;
                    decodedEventList.Add(metaEventInfo.trackOrMetaEvent);
                    if (trackChunk[1] == 0x2F)
                    {
                        endOfTrack = true;
                    }
                }
                else
                {
                    TrackMetaEventInfo trackEventInfo = ProcessTrackEvent(trackChunk, timeChunk.data);
                    currentEventSize = trackEventInfo.eventSize;
                    decodedEventList.Add(trackEventInfo.trackOrMetaEvent);
                }

                trackChunk = trackChunk.GetRange(currentEventSize, trackChunk.Count() - currentEventSize);
            }

            return decodedEventList;
        }

        /*
         * Dynamically finds the variable-sized time chunk and aggregates their
         * values.
         * Note: the back 7-bits of each byte correspond to the value, while the
         *       first bit notifies if there is another byte to follow.
         */
        private static VariableChunkSize<int> FindDeltaTime(List<byte> trackChunk)
        {
            VariableChunkSize<int> timeChunk;
            timeChunk.chunkSize = 1;
            timeChunk.data = 0;

            while (true)
            {
                byte next = trackChunk[timeChunk.chunkSize - 1];
                timeChunk.data = (int)BitByBit.ConcatIgnoreFront(timeChunk.data, next, 1);

                if (!BitByBit.IsFrontBitsOn(next, 1))
                    break;
                timeChunk.chunkSize++;
            }

            return timeChunk;
        }

        private struct VariableChunkSize<T>
        {
            internal int chunkSize;
            internal T data;
        }

        /// <summary>
        /// Converts from the hexadecimal bytes of a meta event into a <see cref="TrackMetaEventInfo"/>.
        /// </summary>
        /// <param name="trackChunk">A <see cref="List"/> of hexadecimal bytes that represent the track chunk.</param>
        /// <param name="deltaTime">The time between the previous event and the current event.</param>
        /// <returns>A <see cref="TrackMetaEventInfo"/> containing <see cref="Event"/> information and the number of bytes the meta event was.</returns>
        private TrackMetaEventInfo ProcessMetaEvent(List<byte> trackChunk, int deltaTime)
        {
            int eventSize = trackChunk[2];
            MetaEvent metaEvent = new MetaEvent
            {
                EventType = metaEventCodeDictionary[trackChunk[1]],
                // TODO: EventText is leftover from trackChunk being a list of strings. Don't know what to do yet.
                EventText = string.Join("", trackChunk.GetRange(3, eventSize)),
                DeltaTime = deltaTime
            };

            return new TrackMetaEventInfo
            {
                eventSize = eventSize,
                trackOrMetaEvent = metaEvent
            };
        }

        //TODO: have some conditionals on the trackEventType
        /// <summary>
        /// Converts from the hexadecimal bytes of a trac event into a <see cref="TrackMetaEventInfo"/>.
        /// </summary>
        /// <param name="trackChunk">A <see cref="List"/> of hexadecimal bytes that represent the track chunk.</param>
        /// <param name="deltaTime">The time between the previous event and the current event.</param>
        /// <returns>A <see cref="TrackMetaEventInfo"/> containing <see cref="Event"/> information and the number of bytes the meta event was.</returns>
        private TrackMetaEventInfo ProcessTrackEvent(List<byte> trackChunk, int deltaTime)
        {
            String trackEventType = trackEventStatusDictionary[trackChunk[0].ToString()];
            
            TrackEvent trackEvent = new TrackEvent
            {
                DeltaTime = deltaTime,
                EventType = trackEventType,
                NoteCode = trackChunk[1],
                NoteVelocity = trackChunk[2],
                // TODO: Should channel still be a string? probably not right?
                Channel = BitByBit.Nib2Hex(BitByBit.RightNib(trackChunk[0]))
            };

            return new TrackMetaEventInfo
            {
                eventSize = 3,
                trackOrMetaEvent = trackEvent
            };
        }
        #endregion
    }
}
