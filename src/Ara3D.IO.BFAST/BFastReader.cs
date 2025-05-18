using System;
using System.Collections.Generic;
using Ara3D.Memory;
using Ara3D.MemoryMappedFiles;

namespace Ara3D.IO.BFAST
{
    public class BFastReader
    {
        public BFastPreamble Preamble { get; }
        public BFastRange[] Ranges { get; }
        public string[] BufferNames { get; } = [];
        public MemoryMappedView View { get; }
        
        public static void Read(MemoryMappedView view, Action<string, MemoryMappedView, int> onBuffer)
        {
            new BFastReader(view).Read(onBuffer);
        }

        public static BFastBuffers Read(string filePath)
        {
            var r = new List<NamedAlignedMemory>();
            Read(filePath, (name, view, index) =>
            {
                var memory = view.ReadBytes();
                r.Add(new(memory, name));
            });
            return new BFastBuffers(r);
        }

        public void Read(Action<string, MemoryMappedView, int> onBuffer)
        {
            for (var i = 0; i < BufferNames.Length; ++i)
            {
                var (name, range) = GetNameAndRange(i);
                using var subView = View.CreateSubView(range.Begin, range.Count);
                onBuffer(name, subView, i);
            }
        }

        public static void Read(string filePath, Action<string, MemoryMappedView, int> onBuffer)
            => MemoryMappedView.ReadFile(filePath, view => Read(view, onBuffer));

        public (string, BFastRange) GetNameAndRange(int index)
            => (BufferNames[index], Ranges[index + 1]);

        public BFastReader(MemoryMappedView view)   
        {
            View = view;
            view.Accessor.Read(0, out BFastPreamble preamble);
            Preamble = preamble.Validate();
                
            var offset = BFastPreamble.Size;
            Ranges = new BFastRange[preamble.NumArrays];
            view.Accessor.ReadArray(offset, Ranges, 0, (int)preamble.NumArrays);

            var cnt = (int)Ranges[0].Count;
            if (cnt != Ranges[0].Count)
                throw new Exception($"Buffer is too big {Ranges[0].Count}");
            var bytes = new byte[cnt];

            if (cnt > 0)
            {
                view.Accessor.ReadArray(Ranges[0].Begin, bytes, 0, cnt);
                BufferNames = bytes.UnpackStrings();
                if (BufferNames.Length + 1 != Ranges.Length)
                    throw new Exception("Badly formed BFAST: number of buffer names does not match number of arrays");
            }
        }
    }
}