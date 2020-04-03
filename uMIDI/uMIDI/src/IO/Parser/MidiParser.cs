using System;
using System.Collections.Generic;
using System.Linq;
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
        private static readonly IDictionary<String, String> metaEventCodeDictionary = new Dictionary<String, String> { { "00", "sequence number" }, { "01", "text event" }, { "02", "copyright notice" }, { "03", "sequence or track name" }, { "04", "instrument name" }, { "05", "lyric text" }, { "06", "marker text" }, { "07", "cue point" }, { "20", "MIDI channel prefix assignment" }, { "2F", "end of track" }, { "51", "tempo setting" }, { "54", "SMPTE offset" }, { "58", "time signature" }, { "59", "key signature" }, { "7F", "sequencer specific event" } };
        private static readonly IDictionary<String, String> trackEventStatusDictionary = new Dictionary<String, String> { { "8", "note off" }, { "9", "note on" } };
        private const int chunkBodyLengthSize = 4;

        #endregion

        #region --Functions--

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
        private List<Event> ProcessTrackChunk(List<String> trackChunk)
        {
            List<Event> decodedEventList = new List<Event>();
            bool endOfTrack = false;

            while (!endOfTrack)
            {
                IDictionary<String, int> deltaTimeDictionary = FindDeltaTime(trackChunk);
                trackChunk = trackChunk.GetRange(deltaTimeDictionary["timeBytes"], trackChunk.Count() - deltaTimeDictionary["timeBytes"]);

                int currentEventSize = 0;
                String nextByte = trackChunk.First();
                if (nextByte == "FF")
                {
                    TrackMetaEventInfo metaEventInfo = ProcessMetaEvent(trackChunk, deltaTimeDictionary["deltaTime"]);
                    currentEventSize = metaEventInfo.eventSize;
                    decodedEventList.Add(metaEventInfo.trackOrMetaEvent);
                    if (trackChunk[1] == "2F")
                    {
                        endOfTrack = true;
                    }
                }
                else
                {
                    TrackMetaEventInfo trackEventInfo = ProcessTrackEvent(trackChunk, deltaTimeDictionary["deltaTime"]);
                    currentEventSize = trackEventInfo.eventSize;
                    decodedEventList.Add(trackEventInfo.trackOrMetaEvent);
                }

                trackChunk = trackChunk.GetRange(currentEventSize, trackChunk.Count() - currentEventSize);
            }

            return decodedEventList;
        }

        ///
        //TODO!!!
        private IDictionary<String, int> FindDeltaTime(List<String> trackChunk)
        {
            IDictionary<String, int> deltaTimeDictionary = new Dictionary<String, int>();
            // take next few bytes, convert to binary, figure out where it ends, convert to decimal, return
            // Create dictionary: "deltaTime" -> ..., "timeBytes" -> ...

            return deltaTimeDictionary;
        }

        /// <summary>
        /// Converts from the hexadecimal bytes of a meta event into a <see cref="TrackMetaEventInfo"/>.
        /// </summary>
        /// <param name="trackChunk">A <see cref="List"/> of hexadecimal bytes that represent the track chunk.</param>
        /// <param name="deltaTime">The time between the previous event and the current event.</param>
        /// <returns>A <see cref="TrackMetaEventInfo"/> containing <see cref="Event"/> information and the number of bytes the meta event was.</returns>
        private TrackMetaEventInfo ProcessMetaEvent(List<String> trackChunk, int deltaTime)
        {
            int eventSize = Convert.ToInt32(trackChunk[2], 16);
            MetaEvent metaEvent = new MetaEvent
            {
                EventType = metaEventCodeDictionary[trackChunk[1]],
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
        private TrackMetaEventInfo ProcessTrackEvent(List<String> trackChunk, int deltaTime)
        {
            String trackEventType = trackEventStatusDictionary[trackChunk[0].ToString()];
            
            TrackEvent trackEvent = new TrackEvent
            {
                DeltaTime = deltaTime,
                EventType = trackEventType,
                NoteCode = Convert.ToInt32(trackChunk[1], 16),
                NoteVelocity = Convert.ToInt32(trackChunk[2], 16),
                Channel = Convert.ToInt32(trackChunk[0][1].ToString(), 16)
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
