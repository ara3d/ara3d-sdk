using System;
using System.Collections;
using System.ComponentModel;
using System.Data;

namespace Ara3D.IO.VIM
{
    // For reference see:
    // https://referencesource.microsoft.com/#system.data/system/data/DataRowView.cs

    public class VimRow : ICustomTypeDescriptor, IEditableObject, INotifyPropertyChanged, IEnumerator
    {
        public VimRow(VimTable data, int row = -1)
            => (Table, RowIndex) = (data, row);

        public VimTable Table { get; }

        public int Count => Table.Columns.Count;
        public int RowIndex { get; private set; }

        public AttributeCollection GetAttributes()
            => new(null);

        public string GetClassName()
            => Table.Name;

        public string GetComponentName()
            => null;

        public TypeConverter GetConverter()
            => null;

        public EventDescriptor GetDefaultEvent()
            => null;

        public PropertyDescriptor GetDefaultProperty()
            => null;

        public object GetEditor(Type editorBaseType)
            => null;

        public EventDescriptorCollection GetEvents()
            => new(null);

        public EventDescriptorCollection GetEvents(Attribute[] attributes)
            => new(null);

        public PropertyDescriptorCollection GetProperties()
            => GetProperties(null);

        public PropertyDescriptorCollection GetProperties(Attribute[] attributes)
            => Table.GetItemProperties(null);

        public object GetPropertyOwner(PropertyDescriptor pd)
            => this;

        public object this[int index]
            => Table.GetValue(RowIndex, index);

        public object this[string columnName]
        {
            get => Table.GetValue(RowIndex, columnName);
            set => throw new ReadOnlyException();
        }

        public string GetString(string columnName)
            => Table.Document.GetString((int)this[columnName]);

        public void BeginEdit()
            => throw new ReadOnlyException();

        public void CancelEdit()
            => throw new ReadOnlyException();

        public void EndEdit()
            => throw new ReadOnlyException();

        public event PropertyChangedEventHandler PropertyChanged;

        public bool MoveNext()
            // Prevent incrementing if already too lat
            => RowIndex < Table.Count && RowIndex++ < Table.Count;

        public void Reset()
            => RowIndex = 0;

        public object Current
            => this;
    }
}