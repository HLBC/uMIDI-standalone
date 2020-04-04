using System;
namespace uMIDI_decoder.Utility
{
    public class BitByBit
    {
        private BitByBit() { }

        public static bool IsFrontBitsOn(byte b, int numOfBits)
        {
            if (0 < numOfBits && numOfBits <= 8)
                return 0xFF == (b | ClearFront(0xFF, numOfBits));
            else
                return false;
        }

        public static long ConcatIgnoreFront(long num, byte toConcat, int numOfBits)
        {
            if (0 < numOfBits && numOfBits <= 8)
                return (num << (8 - numOfBits)) + ClearFront(toConcat, numOfBits);
            else if (8 == numOfBits)
                return num;
            else
                throw new ArgumentException("Number of bits out of range [0,8].");
        }

        public static byte ClearFront(byte b, int numOfBits)
        {
            return (byte)((byte)(b << numOfBits) >> numOfBits);
        }

        public static byte ClearBack(byte b, int numOfBits)
        {
            return (byte)((byte)(b >> numOfBits) << numOfBits);
        }
    }
}
