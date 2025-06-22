using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Linq;
using Ara3D.Memory;

namespace Ara3D.IO.VIM
{
    public class VimTable : 
        IBindingListView,
        ISupportInitializeNotification,
        ITypedList
    {
        public VimTable(VimDocument document, SerializableEntityTable table)
        {
            (Document, Table) = (document, table);
            var buffers = table.DataColumns.Concat(table.StringColumns).Concat(table.IndexColumns).ToArray();
            Count = (int)(buffers.FirstOrDefault()?.ElementCount() ?? 0);
            if (buffers.Any(b => b.ElementCount() != Count))
                throw new Exception("Not all columns are the same length");
            Columns = buffers.Select((c, i) => new VimColumn(this, c, i)).ToArray();
            ColumnLookup = Columns.Select((column, index) => (column, index)).ToDictionary(pair => pair.column.Name, pair => pair.index);
            Schema = new DataTable { TableName = Name };
            foreach (var c in Columns)
                Schema.Columns.Add(c.Name, c.ColumnType);
        }
    
        public VimDocument Document { get; }
        public DataTable Schema;
        public string Name => Table.Name.GetSimplifiedTableName();
        public SerializableEntityTable Table { get; }
        public int Count { get; }

        public IReadOnlyDictionary<string, int> ColumnLookup { get; }
        public IReadOnlyList<VimColumn> Columns { get; }

        public VimRow GetRow(int row)
            => new(this, row);

        public object GetValue(int row, int col)
            => Columns[col][row];

        public object GetValue(int row, string columnName)
            => GetValue(row, GetColumnIndex(columnName));

        public VimColumn GetColumn(string columnName)
        {
            var index = GetColumnIndex(columnName);
            return index >= 0 ? Columns[index] : null;
        }

        public int GetColumnIndex(string columnName)
            => ColumnLookup[columnName];

        public IEnumerator GetEnumerator()
            => new VimRow(this);

        public void CopyTo(Array array, int index)
        {
            for (var i = 0; i < Count; i++)
                array.SetValue(GetRow(i), i + index);
        }

        public bool IsSynchronized 
            => true;

        public object SyncRoot 
            => Table;

        public int Add(object value)
            => throw new NotImplementedException();

        public void Clear()
            => throw new NotImplementedException();

        public bool Contains(object value)
            => throw new NotImplementedException();

        public int IndexOf(object value)
            => value is VimRow vtr ? vtr.RowIndex : -1;

        public void Insert(int index, object value)
            => throw new NotImplementedException();

        public void Remove(object value)
            => throw new NotImplementedException();

        public void RemoveAt(int index)
            => throw new NotImplementedException();

        public bool IsFixedSize
            => true;

        public bool IsReadOnly
            => true;

        public object this[int index]
        {
            get => GetRow(index);
            set => throw new ReadOnlyException();
        }

        public void AddIndex(PropertyDescriptor property)
            => throw new NotImplementedException();

        public object AddNew()
            => throw new NotImplementedException();

        public void ApplySort(PropertyDescriptor property, ListSortDirection direction)
            => throw new NotImplementedException();

        public int Find(PropertyDescriptor property, object key)
            => throw new NotImplementedException();
   
        public void RemoveIndex(PropertyDescriptor property)
            => throw new NotImplementedException();

        public void RemoveSort()
            => throw new NotImplementedException();

        public bool AllowEdit => false;
        public bool AllowNew => false;
        public bool AllowRemove => false;

        public bool IsSorted => false;

        public ListSortDirection SortDirection => ListSortDirection.Ascending;
        public PropertyDescriptor SortProperty => null;

        public bool SupportsChangeNotification => false;
        public bool SupportsSearching => false;
        public bool SupportsSorting => false;
    
        public event ListChangedEventHandler ListChanged;
    
        public void ApplySort(ListSortDescriptionCollection sorts)
        { }

        public void RemoveFilter()
            => throw new NotImplementedException();

        public string Filter { get; set; }
        public ListSortDescriptionCollection SortDescriptions { get; } = new();
        public bool SupportsAdvancedSorting => false;
        public bool SupportsFiltering => false;

        public void BeginInit()
        {
        }

        public void EndInit()
        {
            IsInitialized = true;
            Initialized?.Invoke(this, EventArgs.Empty);
        }

        public bool IsInitialized { get; private set; }

        public event EventHandler Initialized;

        public PropertyDescriptorCollection GetItemProperties(PropertyDescriptor[] listAccessors)
            => new(Columns.Cast<PropertyDescriptor>().ToArray(), true);

        public string GetListName(PropertyDescriptor[] listAccessors)
            => Name;

        public string GetString(int stringIndex)
            => Document.Document.StringTable.ElementAtOrDefault(stringIndex) ?? string.Empty;
    }
}