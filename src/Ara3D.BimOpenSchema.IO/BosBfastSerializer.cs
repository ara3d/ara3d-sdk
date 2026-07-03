using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using Ara3D.Collections;
using Ara3D.IO.BFAST;
using Ara3D.Memory;
using Ara3D.Utils;

namespace Ara3D.BimOpenSchema.IO;


// TODO: move this somewhere
public static unsafe class BosBfastSerializer
{
    public static ReadOnlySpan<byte> ToByteSpan<T>(this ReadOnlySpan<T> span) where T : unmanaged
        => MemoryMarshal.AsBytes(span);

    public static T[] ToArray<T>(this ReadOnlySpan<byte> span) where T : unmanaged
        => MemoryMarshal.Cast<byte, T>(span).ToArray();

    public static ReadOnlySpan<byte> GetSpan(this IBimData bimData, int index)
        => index == 0 ? LinqArray.AsSpan(bimData.Entities).ToByteSpan() :
           index == 1 ? LinqArray.AsSpan(bimData.Descriptors).ToByteSpan() :
           index == 2 ? LinqArray.AsSpan(bimData.Parameters).ToByteSpan() :
           index == 3 ? LinqArray.AsSpan(bimData.Documents).ToByteSpan() :
           index == 4 ? bimData.Strings.PackStrings() :
           index == 5 ? LinqArray.AsSpan(bimData.Numbers).ToByteSpan() :
           index == 6 ? LinqArray.AsSpan(bimData.Points).ToByteSpan() :
           index == 7 ? LinqArray.AsSpan(bimData.Relations).ToByteSpan() :
           index == 8 ? LinqArray.AsSpan(bimData.Diagnostics).ToByteSpan() :
           throw new Exception($"Unrecognized buffer index: {index}");

    public static void LoadData(this BimData bimData, int index, ReadOnlySpan<byte> span)
    {
        switch (index)
        {
            case 0:
                bimData.Entities = span.ToArray<Entity>();
                break;
            case 1:
                bimData.Descriptors = span.ToArray<ParameterDescriptor>();
                break;
            case 2:
                bimData.Parameters = span.ToArray<Parameter>();
                break;
            case 3:
                bimData.Documents = span.ToArray<Document>();
                break;
            case 4:
                bimData.Strings = span.UnpackStrings();
                break;
            case 5:
                bimData.Numbers = span.ToArray<float>();
                break;
            case 6:
                bimData.Points = span.ToArray<Point>();
                break;
            case 7:
                bimData.Relations = span.ToArray<EntityRelation>();
                break;
            case 8:
                bimData.Diagnostics = span.ToArray<Diagnostic>();
                break;
            default:
                throw new Exception($"Unrecognized buffer index: {index}");
        }
    }

    public static string[] GetBufferNames()
        => [ 
            nameof(IBimData.Entities), 
            nameof(IBimData.Descriptors), 
            nameof(IBimData.Parameters), 
            nameof(IBimData.Documents), 
            nameof(IBimData.Strings), 
            nameof(IBimData.Numbers), 
            nameof(IBimData.Points), 
            nameof(IBimData.Relations), 
            nameof(IBimData.Diagnostics)
        ];

    public static void Write(this IBimData bimData, FilePath filePath)
    {
        var sizes = Enumerable.Range(0, 9).Select(i => (long)bimData.GetSpan(i).Length).ToArray();

        long OnBuffer(Stream stream, int index, string name, long bytesToWrite)
        {
            var span = bimData.GetSpan(index);
            var size = span.Length;
            Debug.Assert(bytesToWrite == size);
            while (true)
            {
                var tmp = Math.Min(size, int.MaxValue);
                stream.Write(span);
                size -= tmp;
                if (size <= 0)
                    break;
            }

            stream.Flush();
            return bytesToWrite;
        }

        BFast.Write(filePath, GetBufferNames(), sizes, OnBuffer);
    }

    public static void AddRange<T>(this UnmanagedList<T> self, byte* ptr, long count)
        where T : unmanaged
    {
        var byteSlice = new ByteSlice(ptr, count);
        self.AddRange(byteSlice.AsReadOnlySpan<T>());
    }

    public static BimData Load(FilePath fp)
    {
        var r = new BimData();

        void OnView(string name, MemoryMappedView view, int index)
        {
            byte* srcPointer = null;
            view.Accessor.SafeMemoryMappedViewHandle.AcquirePointer(ref srcPointer);
            try
            {
                srcPointer += view.Accessor.PointerOffset;
                var span = new Span<byte>(srcPointer, (int)view.Size);
                r.LoadData(index, span);
            }
            finally
            {
                view.Accessor.SafeMemoryMappedViewHandle.ReleasePointer();
            }
        }

        BFastReader.Read(fp, OnView);
        return r;
    }
}