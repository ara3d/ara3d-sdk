using System.Data;
using Ara3D.Collections;

namespace Ara3D.DataTable
{
    public class DataRecordAdapter : IDataRecord
    {
        public IDataTable Table { get; }
        public IDataRow Row { get; }
        
        public DataRecordAdapter(IDataTable table, IDataRow row)
        {
            Table = table;
            Row = row;
        }

        public bool GetBoolean(int i) => (bool)Row[i];
        public byte GetByte(int i) => (byte)Row[i]; 
        public long GetBytes(int i, long fieldOffset, byte[]? buffer, int bufferoffset, int length) => throw new NotImplementedException();
        public char GetChar(int i) => (char)Row[i];
        public long GetChars(int i, long fieldoffset, char[]? buffer, int bufferoffset, int length) => throw new NotImplementedException();
        public IDataReader GetData(int i) => throw new NotImplementedException();
        public string GetDataTypeName(int i) => GetFieldType(i).Name;
        public DateTime GetDateTime(int i) => (DateTime)Row[i];
        public decimal GetDecimal(int i) => (Decimal)Row[i];
        public double GetDouble(int i) => (double)Row[i];
        public Type GetFieldType(int i) => Table.Columns[i].Descriptor.Type;
        public float GetFloat(int i) => (float)Row[i];
        public Guid GetGuid(int i) => (Guid)Row[i];
        public short GetInt16(int i) => (short)Row[i];
        public int GetInt32(int i) => (int)Row[i];
        public long GetInt64(int i) => (long)Row[i];
        public string GetName(int i) => Table.Columns[i].Descriptor.Name;
        public int GetOrdinal(string name) => Table.Columns.IndexOf(c => c.Descriptor.Name == name);
        public string GetString(int i) => (string)Row[i];
        public object GetValue(int i) => this[i];
        public int GetValues(object[] values)
        {
            var n = Math.Min(values.Length, FieldCount);
            for (var i = 0; i < n; i++)
                values[i] = Row[i];
            return n;
        }
        public bool IsDBNull(int i) => Row[i] == null;
        public int FieldCount => Table.Columns.Count;
        public object this[int i] => Row[i];
        public object this[string name] => Row[GetOrdinal(name)];
    }
}
