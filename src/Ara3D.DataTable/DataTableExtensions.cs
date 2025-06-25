using System.Diagnostics;
using Ara3D.Collections;
using Ara3D.PropKit;

namespace Ara3D.DataTable;

public static class DataTableExtensions
{
    public static DataTableBuilder AddColumnsFromFieldsAndProperties<T>(this DataTableBuilder self, IEnumerable<T> values)
    {
        var propSet = typeof(T).GetPropProvider();
        var descriptors = propSet.GetDescriptors();
        var columns = descriptors.Count.Select(_ => new List<object>()).ToList();

        foreach (var value in values)
        {
            var row = propSet.GetPropValues(value);
            for (var i = 0; i < row.Count; i++)
            {
                var propVal = row[i];
                Debug.Assert(propVal.Descriptor.Name.Equals(descriptors[i].Name));
                columns[i].Add(propVal.Value);
            }
        }

        for (var i=0; i < columns.Count; i++)
        {
            self.AddColumn(columns[i], descriptors[i].Name, descriptors[i].Type);
        }

        return self;
    }
}