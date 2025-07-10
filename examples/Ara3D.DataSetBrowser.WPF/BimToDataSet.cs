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
        public static System.Type[] BIMSchemaTypes =
        [
            typeof(BIMData),
            typeof(ParameterData),
            typeof(PropDescriptor),
            typeof(Entity),
            typeof(Descriptor),
            typeof(ParameterInt),
            typeof(ParameterDouble),
            typeof(ParameterString),
            typeof(ParameterEntity),
            typeof(BoundsComponent),
            typeof(Point),
            typeof(LocationComponent),
            typeof(Level),
            typeof(LevelRelation),
            typeof(Room),
            typeof(Document),
            typeof(TypeRelation),
        ];

        public static ReadOnlyDataTable CreateDataTable<T>(string name, IReadOnlyList<T> values)
        {
            var props = typeof(T).GetPropProvider();
            var columns = props.Accessors.Select(
                (acc, i) => new DataColumnFromAccessorAndList<T>(i, acc, values))
                .ToList();
            return new ReadOnlyDataTable(name, columns);
        }

        public static IDataSet ToDataSet(this BIMData self)
            => new ReadOnlyDataSet([
                CreateDataTable(nameof(self.Points), self.Points),
                CreateDataTable(nameof(self.Bounds), self.Bounds),
                CreateDataTable(nameof(self.Descriptors), self.Descriptors),
                CreateDataTable(nameof(self.Documents), self.Documents),
                CreateDataTable(nameof(self.Entities), self.Entities),
                CreateDataTable(nameof(self.LevelRelations), self.LevelRelations),
                CreateDataTable(nameof(self.Levels), self.Levels),
                CreateDataTable(nameof(self.Locations), self.Locations),
                CreateDataTable(nameof(self.Rooms), self.Rooms),
                CreateDataTable(nameof(self.Types), self.Types),
                CreateDataTable(nameof(self.ParameterData.Integers), self.ParameterData.Integers),
                CreateDataTable(nameof(self.ParameterData.Doubles), self.ParameterData.Doubles),
                CreateDataTable(nameof(self.ParameterData.Entities), self.ParameterData.Entities),
                CreateDataTable(nameof(self.ParameterData.Strings), self.ParameterData.Strings)
            ]);
    }
}
