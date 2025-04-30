using System.Runtime.InteropServices;

namespace Ara3D.BFAST
{
    /// <summary>
    /// This contains the BFAST data loaded or written from disk. 
    /// </summary>
    public class BFastHeader
    {
        public BFastPreamble Preamble = new BFastPreamble();
        public BFastRange[] Ranges;
        public string[] Names;
    }

    /// <summary>
    /// Constants.
    /// </summary>
    public static class Constants
    {
        public const long Magic = 0xBFA5;

        // https://en.wikipedia.org/wiki/Endianness
        public const long SameEndian = Magic;
        public const long SwappedEndian = 0xA5BFL << 48;

        /// <summary>
        /// Data arrays are aligned to 64 bytes, so that they can be cast directly to AVX-512 registers.
        /// This is useful for efficiently working with floating point data. 
        /// </summary>
        public const long ALIGNMENT = 64;
    }

    /// <summary>
    /// This tells us where a particular array begins and ends in relation to the beginning of a file.
    /// * Begin must be less than or equal to End.
    /// * Begin must be greater than or equal to DataStart
    /// * End must be less than or equal to DataEnd
    /// </summary>
    [StructLayout(LayoutKind.Explicit, Pack = 8, Size = 16)]
    public struct BFastRange
    {
        [FieldOffset(0)] public long Begin;
        [FieldOffset(8)] public long End;

        public long Count => End - Begin;
        public static long Size = 16;
    }

    /// <summary>
    /// The header contains a magic number, the begin and end indices of data, and the number of arrays.
    /// </summary>
    [StructLayout(LayoutKind.Explicit, Pack = 8, Size = 32)]
    public struct BFastPreamble
    {
        [FieldOffset(0)] public long Magic;         // Either Constants.SameEndian or Constants.SwappedEndian depending on endianess of writer compared to reader. 
        [FieldOffset(8)] public long DataStart;     // <= file size and >= ArrayRangesEnd and >= FileHeader.ByteCount
        [FieldOffset(16)] public long DataEnd;      // >= DataStart and <= file size
        [FieldOffset(24)] public long NumArrays;    // number of arrays 

        /// <summary>
        /// This is where the array ranges are finished. 
        /// Must be less than or equal to DataStart.
        /// Must be greater than or equal to FileHeader.ByteCount
        /// </summary>
        public long RangesEnd => Size + NumArrays * 16;

        /// <summary>
        /// The size of the FileHeader structure 
        /// </summary>
        public static long Size = 32;

        /// <summary>
        /// Returns true if the producer of the BFast file has the same endianness as the current library
        /// </summary>
        public bool SameEndian => Magic == Constants.SameEndian;
    };
}
