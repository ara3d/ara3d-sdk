using System;
using System.Runtime.CompilerServices;

namespace Ara3D.Utils
{
    public static class ByteUtils
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static byte GetByte0(this uint x)
            => (byte)(x & 0xFFu);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static byte GetByte1(this uint x)
            => (byte)(x >> 8 & 0xFFu);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static byte GetByte2(this uint x) 
            => (byte)(x >> 16 & 0xFFu);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static byte GetByte3(this uint x) 
            => (byte)(x >> 24 & 0xFFu);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ushort GetLow(this uint x) 
            => (ushort)(x & 0xFFFFu);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ushort GetHigh(this uint x) 
            => (ushort)(x >> 16 & 0xFFFFu);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static uint SetByte0(this uint x, byte b) 
            => (0xFFFFFF00 & x) | b;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static uint SetByte1(this uint x, byte b) 
            => (0xFFFF00FF & x) | (uint)(b << 8);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static uint SetByte2(this uint x, byte b) 
            => (0xFF00FFFF & x) | (uint)(b << 16);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static uint SetByte3(this uint x, byte b) 
            => (0x00FFFFFF & x) | (uint)(b << 24);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static uint SetBytes(this uint x, byte b0, byte b1, byte b2, byte b3) 
            => b0 | (uint)b1 << 8 | (uint)b2 << 16 | (uint)b3 << 24;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static uint SetLow(this uint x, ushort low) 
            => (0xFFFF0000 & x) | low;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static uint SetHigh(this uint x, ushort high) 
            => (0x0000FFFF & x) | (uint)high << 16;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static byte NormalizedToByte(this float v) 
            => (byte)(Math.Min(Math.Max(v, 0f), 1f) * 255f + 0.5f);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float ToNormalizedFloat(this byte b) 
            => b / 255f;
    }
}
