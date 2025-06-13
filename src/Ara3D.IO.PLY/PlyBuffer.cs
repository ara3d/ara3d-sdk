using Ara3D.Memory;

namespace Ara3D.IO.PLY;
public unsafe class UInt8Buffer : PlyBuffer
{
    public UInt8Buffer(int count, string name) : base(count, 1, name) { }
    public byte* Pointer => Bytes.Bytes.Begin;
    public override int GetInt(int n) => Pointer[n];
    public override void LoadValue(string s) => Pointer[Count++] = byte.Parse(s);
    public override void LoadValue(System.IO.BinaryReader br) => Pointer[Count++] = br.ReadByte();
}

public unsafe class Int8Buffer : PlyBuffer
{
    public Int8Buffer(int count, string name) : base(count, 1, name) { }
    public sbyte* Pointer => (sbyte*)Bytes.Bytes.Begin;
    public override int GetInt(int n) => Pointer[n];
    public override void LoadValue(string s) => Pointer[Count++] = sbyte.Parse(s);
    public override void LoadValue(System.IO.BinaryReader br) => Pointer[Count++] = br.ReadSByte();
}

public unsafe class Int16Buffer : PlyBuffer
{
    public Int16Buffer(int count, string name) : base(count, 2, name) { }
    public short* Pointer => (short*)Bytes.Bytes.Begin;
    public override int GetInt(int n) => Pointer[n];
    public override void LoadValue(string s) => Pointer[Count++] = short.Parse(s);
    public override void LoadValue(System.IO.BinaryReader br) => Pointer[Count++] = br.ReadInt16();
}

public unsafe class UInt16Buffer : PlyBuffer
{
    public UInt16Buffer(int count, string name) : base(count, 2, name) { }
    public ushort* Pointer => (ushort*)Bytes.Bytes.Begin;
    public override int GetInt(int n) => Pointer[n];
    public override void LoadValue(string s) => Pointer[Count++] = ushort.Parse(s);
    public override void LoadValue(System.IO.BinaryReader br) => Pointer[Count++] = br.ReadUInt16();
}

public unsafe class UInt32Buffer : PlyBuffer
{
    public UInt32Buffer(int count, string name) : base(count, 4, name) { }
    public uint* Pointer => (uint*)Bytes.Bytes.Begin;
    public override int GetInt(int n) => (int)Pointer[n];
    public override void LoadValue(string s) => Pointer[Count++] = uint.Parse(s);
    public override void LoadValue(System.IO.BinaryReader br) => Pointer[Count++] = br.ReadUInt32();
}

public unsafe class Int32Buffer : PlyBuffer
{
    public Int32Buffer(int count, string name) : base(count, 4, name) { }
    public int* Pointer => (int*)Bytes.Bytes.Begin;
    public override int GetInt(int n) => Pointer[n];
    public override void LoadValue(string s) => Pointer[Count++] = int.Parse(s);
    public override void LoadValue(System.IO.BinaryReader br) => Pointer[Count++] = br.ReadInt32();
}

public unsafe class SingleBuffer : PlyBuffer
{
    public SingleBuffer(int count, string name) : base(count, 4, name) { }
    public float* Pointer => (float*)Bytes.Bytes.Begin;
    public override int GetInt(int n) => (int)Pointer[n];
    public override double GetDouble(int n) => Pointer[n];
    public override void LoadValue(string s) => Pointer[Count++] = float.Parse(s);
    public override void LoadValue(System.IO.BinaryReader br) => Pointer[Count++] = br.ReadSingle();
}

public unsafe class DoubleBuffer : PlyBuffer
{
    public DoubleBuffer(int count, string name) : base(count, 8, name) { }
    public double* Pointer => (double*)Bytes.Bytes.Begin;
    public override int GetInt(int n) => (int)Pointer[n];
    public override double GetDouble(int n) => Pointer[n];
    public override void LoadValue(string s) => Pointer[Count++] = double.Parse(s);
    public override void LoadValue(System.IO.BinaryReader br) => Pointer[Count++] = br.ReadDouble();
}

public class ListBuffer : IPlyBuffer
{
    public readonly List<PlyBuffer> Buffers = new List<PlyBuffer>();
    public PlyBuffer Current { get; private set; }
    public ListBuffer(PlyBuffer buffer) => AddBuffer(buffer);
    private void AddBuffer(PlyBuffer buffer) => Buffers.Add(Current = buffer);
    public string Name => Current.Name;
    public int Count { get; set; }

    public void LoadValue(string s)
    {
        Current.LoadValue(s);
        if (Current.IsFull)
            AddBuffer(Current.Clone());
        Count++;
    }

    public void LoadValue(System.IO.BinaryReader br)
    {
        Current.LoadValue(br);
        if (Current.IsFull)
            AddBuffer(Current.Clone());
        Count++;
    }

    public int Cap => Current.Capacity;
    public int GetInt(int index) => Buffers[index / Cap].GetInt(index % Cap);
    public double GetDouble(int index) => Buffers[index / Cap].GetInt(index % Cap);
}
public abstract class PlyBuffer : IPlyBuffer
{
    public string Name { get; }
    public int Count { get; set; }
    public abstract void LoadValue(string s);
    public abstract void LoadValue(System.IO.BinaryReader br);
    public abstract int GetInt(int index);
    public virtual double GetDouble(int index) => GetInt(index);
    public IBuffer<byte> Bytes;
    public int Capacity => Bytes.Count / ElementSize;
    public readonly int ElementSize;
    public bool IsFull => Count == Capacity;
    public IEnumerable<int> GetInts() => Enumerable.Range(0, Count).Select(GetInt);
    public int RecentInt => GetInt(Count - 1);

    protected PlyBuffer(int count, int elementSize, string name)
    {
        ElementSize = elementSize;
        Bytes = new UnmanagedList<byte>(count * elementSize);
        Name = name;
    }

    public PlyBuffer Clone()
    {
        var b = (PlyBuffer)MemberwiseClone();
        b.Bytes = new UnmanagedList<byte>(Bytes.Count);
        b.Count = 0;
        return b;
    }
}