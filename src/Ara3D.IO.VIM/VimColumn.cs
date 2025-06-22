using System;
using System.ComponentModel;
using Ara3D.Memory;

namespace Ara3D.IO.VIM
{
    public class VimColumn : PropertyDescriptor
    {
        public VimTable Table { get; }
        public ITypedNamedBuffer Buffer { get; }
        public int ColumnIndex { get; }
        public Type ColumnType { get; }
        public string RelatedTableName { get; }

        public VimColumn(VimTable table, ITypedNamedBuffer buffer, int index)
            : base(buffer.Name.GetColumnNameFromBufferName(), null)
        {
            Table = table;
            Buffer = buffer;
            ColumnIndex = index;

            ColumnType = buffer.GetTypePrefix() switch
            {
                VimConstants.IntColumnNameTypePrefix => typeof(int),
                VimConstants.ByteColumnNameTypePrefix => typeof(byte),
                VimConstants.FloatColumnNameTypePrefix => typeof(float),
                VimConstants.DoubleColumnNameTypePrefix => typeof(double),
                VimConstants.StringColumnNameTypePrefix => typeof(string),
                VimConstants.IndexColumnNameTypePrefix => typeof(int),             
                VimConstants.LongColumnNameTypePrefix => typeof(long),
                _ => throw new ArgumentException($"Unrecognized VIM column type {buffer.Name}")
            };

            RelatedTableName = buffer.Name.GetRelatedTableName();
        }

        public VimTable GetRelatedTable()
            => Table.Document.FindTable(RelatedTableName);

        public override bool CanResetValue(object component) 
            => false;

        public override object GetValue(object component)
        {
            if (component is VimRow vtr)
            {
                if (vtr.RowIndex < 0 || vtr.RowIndex >= Count)
                    throw new Exception("Row index out of range");
                if (ColumnType == typeof(string))
                {
                    var span = Buffer.Bytes.AsSpan<int>();
                    var stringIndex = span[vtr.RowIndex];
                    return Table.GetString(stringIndex);
                }

                return Buffer.GetElement(vtr.RowIndex);
            }
            throw new ArgumentException("Incorrect component type", nameof(component));
        }

        public override void ResetValue(object component)
            => throw new NotImplementedException();

        public override void SetValue(object component, object value)
            => throw new NotImplementedException();

        public override bool ShouldSerializeValue(object component)
            => false;

        public override Type ComponentType 
            => typeof(VimRow);

        public override bool IsReadOnly 
            => true;

        public override Type PropertyType
            => ColumnType;

        public long Count 
            => Buffer.ElementCount();

        public object this[int n]
            => Buffer.GetElement(n);
    }
}