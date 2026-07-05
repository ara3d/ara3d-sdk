using System.Collections;
using System.Diagnostics;

namespace Ara3D.Bowerbird.RevitSamples;

/// <summary>
/// A dynamically sized list of 4‑bit values (nibbles), packed two per byte internally.
/// Supports indexing, appending single nibbles, bytes, and 32‑bit unsigned ints.
/// </summary>
public sealed class NibbleList : IReadOnlyList<byte>
{
    // Internal storage: each byte holds two nibbles: low (bits 0–3), high (bits 4–7).
    private readonly List<byte> _data = new List<byte>();
    private int _count;

    /// <summary>
    /// Number of nibbles in the list.
    /// </summary>
    public int Count => _count;

    /// <summary>
    /// Gets the nibble at the given index (0..15).
    /// </summary>
    public byte this[int index]
    {
        get
        {
            if ((uint)index >= (uint)_count)
                throw new ArgumentOutOfRangeException(nameof(index));
            var byteIndex = index >> 1;
            var isHigh = (index & 1) == 1;
            var b = _data[byteIndex];
            return isHigh ? (byte)(b >> 4) : (byte)(b & 0x0F);
        }
    }

    /// <summary
    /// Appends a single nibble (0–15).
    /// </summary>
    public void AddNibble(byte nibble)
    {
        Debug.Assert(nibble < 16, "Nibble value must be 0..15");
        var byteIndex = _count >> 1;
        var isHigh = (_count & 1) == 1;
        if (!isHigh)
        {
            // low nibble of a new byte
            _data.Add((byte)(nibble & 0x0F));
        }
        else
        {
            // high nibble of existing last byte
            _data[byteIndex] |= (byte)((nibble & 0x0F) << 4);
        }
        _count++;
    }

    /// <summary>
    /// Splits a byte into two nibbles (low first, then high) and appends them.
    /// </summary>
    public void AddByte(byte value)
    {
        AddNibble((byte)(value & 0x0F));       // low nibble
        AddNibble((byte)((value >> 4) & 0x0F)); // high nibble
    }

    /// <summary>
    /// Splits a 32‑bit unsigned integer into eight nibbles (lowest first) and appends them.
    /// </summary>
    public void AddUShort(ushort value)
    {
        for (var shift = 0; shift < 16; shift += 4)
            AddNibble((byte)((value >> shift) & 0x0F));
    }

    /// <summary>
    /// Splits a 32‑bit unsigned integer into eight nibbles (lowest first) and appends them.
    /// </summary>
    public void AddUInt(uint value)
    {
        for (var shift = 0; shift < 32; shift += 4)
            AddNibble((byte)((value >> shift) & 0x0F));
    }

    /// <summary>
    /// Returns the packed byte array (each byte contains two nibbles: low then high).
    /// </summary>
    public byte[] ToPackedArray()
    {
        // _data already stores in correct format.  No extra padding needed,
        // since if Count is odd, high nibble of last byte remains zero.
        return _data.ToArray();
    }

    /// <inheritdoc/> (enumerates nibbles)
    public IEnumerator<byte> GetEnumerator()
    {
        for (var i = 0; i < _count; i++)
            yield return this[i];
    }

    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
}