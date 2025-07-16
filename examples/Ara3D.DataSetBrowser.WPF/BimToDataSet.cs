using System;
using System.Collections.Generic;
using System.Linq;
using Ara3D.DataTable;
using Ara3D.PropKit;
using BIMOpenSchema;

namespace Ara3D.DataSetBrowser.WPF
{
    public static class BimToDataSet
    {
        public static ReadOnlyDataTable CreateDataTable<T>(string name, IReadOnlyList<T> values)
        {
            var props = typeof(T).GetPropProvider();

            if (typeof(T).IsPrimitive || typeof(T) == typeof(string))
                return new ReadOnlyDataTable(name, [new ReadOnlyDataColumn<T>(0, values)]);

            var columns = props.Accessors.Select(
                (acc, i) => new DataColumnFromAccessorAndList<T>(i, acc, values))
                .ToList();
            return new ReadOnlyDataTable(name, columns);
        }

        public static IDataSet ToDataSet(this BIMData self)
            => new ReadOnlyDataSet([
                CreateDataTable(nameof(self.Points), self.Points),
                CreateDataTable(nameof(self.Strings), self.Strings),
                CreateDataTable(nameof(self.Descriptors), self.Descriptors),
                CreateDataTable(nameof(self.Documents), self.Documents),
                CreateDataTable(nameof(self.Entities), self.Entities),
                CreateDataTable(nameof(self.DoubleParameters), self.DoubleParameters),
                CreateDataTable(nameof(self.IntegerParameters), self.IntegerParameters),
                CreateDataTable(nameof(self.StringParameters), self.StringParameters),
                CreateDataTable(nameof(self.EntityParameters), self.EntityParameters),
                CreateDataTable(nameof(self.PointParameters), self.PointParameters),
            ]);
    }
}
