using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Ara3D.DataTable;
using Ara3D.Models;

namespace Ara3D.BimOpenSchema;

public static class BimDataExtension
{
    public const EntityIndex InvalidEntityIndex = (EntityIndex)(-1);
    public const ParameterIndex InvalidParameterIndex = (ParameterIndex)(-1);
    public const StringIndex InvalidStringIndex = (StringIndex)(-1);
    public const DocumentIndex InvalidDocumentIndex = (DocumentIndex)(-1);
    public const DescriptorIndex InvalidDescriptorIndex = (DescriptorIndex)(-1);

    //==

    public static string Get(this IBimData self, StringIndex index) 
        => self.Strings[(int)index];

    public static Entity? Get(this IBimData self, EntityIndex index) 
        => index < 0 ? null : self.Entities[(int)index];

    public static Document? Get(this IBimData self, DocumentIndex index) 
        => index < 0 ? null : self.Documents[(int)index];

    public static Point Get(this IBimData self, PointIndex index) 
        => index < 0 ? default : self.Points[(int)index];

    public static float Get(this IBimData self, NumberIndex index)
        => index < 0 ? default : self.Numbers[(int)index];

    public static Parameter Get(this IBimData self, ParameterIndex index)
        => index < 0 ? default : self.Parameters[(int)index];
    
    public static EntityRelation Get(this IBimData self, RelationIndex index) 
        => index < 0 ? default : self.Relations[(int)index];

    public static ParameterDescriptor? Get(this IBimData self, DescriptorIndex index) 
        => index < 0 ? null : self.Descriptors[(int)index];

    public static string GetEntityName(this IBimData self, EntityIndex index)
        => index >= 0 ? self.GetEntityName(self.Get(index)) : "null";

    public static string GetEntityName(this IBimData self, Entity? e)
        => e != null ? self.Get(e.Value.Name) : "null";

    public static string GetCategoryName(this IBimData self, EntityIndex index)
        => self.GetCategoryName(self.Get(index));

    public static string GetCategoryName(this IBimData self, Entity? e)
        => e != null ? self.GetEntityName(self.Get(e.Value.Category)) : "null";

    public static string GetEntityLabel(this IBimData self, EntityIndex index)
        => $"{self.GetEntityName(index)}[{index}]";

    public static IEnumerable<EntityIndex> EntityIndices(this IBimData self) 
        => Enumerable.Range(0, self.Entities.Length).Select(i => (EntityIndex)i);

    public static IEnumerable<DocumentIndex> DocumentIndices(this IBimData self)
        => Enumerable.Range(0, self.Documents.Length).Select(i => (DocumentIndex)i);

    public static IEnumerable<DescriptorIndex> DescriptorIndices(this IBimData self)
        => Enumerable.Range(0, self.Descriptors.Length).Select(i => (DescriptorIndex)i);

    public static IEnumerable<StringIndex> StringIndices(this IBimData self)
        => Enumerable.Range(0, self.Strings.Length).Select(i => (StringIndex)i);

    public static IEnumerable<PointIndex> PointIndices(this IBimData self)
        => Enumerable.Range(0, self.Points.Length).Select(i => (PointIndex)i);

    public static IDataSet ToDataSet(this IBimData self)
        => new ReadOnlyDataSet([
            self.Diagnostics.ToDataTable(nameof(self.Diagnostics)),
            self.Points.ToDataTable(nameof(self.Points)),
            self.Strings.ToDataTable(nameof(self.Strings)),
            self.Descriptors.ToDataTable(nameof(self.Descriptors)),
            self.Documents.ToDataTable(nameof(self.Documents)),
            self.Entities.ToDataTable(nameof(self.Entities)),
            self.Relations.ToDataTable(nameof(self.Relations)),
            self.Parameters.ToDataTable(nameof(self.Parameters)),
            self.Numbers.ToDataTable(nameof(self.Numbers)),
        ]);

    public static long GetNumParameters(this IBimData self)
        => self.Parameters.Length;

    public static T[] ReadTable<T>(this IDataSet set, Func<IDataRow, T> f, string name)
    {
        var table = set.GetTable(name);
        if (table == null)
        {
            Debug.WriteLine($"Could not find table {name}");
            return null;
        }

        var i = 0;
        var list = new T[table.Rows.Count];
        foreach (var row in table.Rows)
            list[i++] = f(row);
        return list;
    }

    public static Diagnostic ToDiagnostic(IDataRow row)
        => new((DiagnosticType)row[0], (DocumentIndex)row[1], (EntityIndex)row[2], (StringIndex)row[3]);

    public static Point ToPoint(IDataRow row)
        => new((float)row[0], (float)row[1], (float)row[2]);

    public static float ToNumber(IDataRow row)
        => (float)row[0];

    public static string ToString(IDataRow row)
        => new((string)row[0]);

    public static Parameter ToParameter(IDataRow row)
        => new((EntityIndex)row[0], (DescriptorIndex)row[1], (int)row[2]);

    public static EntityRelation ToRelation(IDataRow row)
        => new((EntityIndex)row[0], (EntityIndex)row[1], (RelationType)row[2]);

    public static ParameterDescriptor ToDescriptor(IDataRow row)
        => new((StringIndex)row[0], (StringIndex)row[1], (StringIndex)row[2], (ParameterType)row[3]);

    public static Document ToDocument(IDataRow row)
        => new((StringIndex)row[0], (StringIndex)row[1]);

    public static Entity ToEntity(IDataRow row)
        => new((long)row[0], (StringIndex)row[1], (DocumentIndex)row[2], (StringIndex)row[3], (EntityIndex)row[4], (EntityIndex)row[5]);

    public static BimData ToBimData(this IDataSet set)
    {
        var r = new BimData();
        r.Diagnostics = ReadTable(set, ToDiagnostic, nameof(r.Diagnostics));
        r.Points = ReadTable(set, ToPoint, nameof(r.Points));
        r.Parameters = ReadTable(set, ToParameter, nameof(r.Parameters));
        r.Numbers = ReadTable(set, ToNumber, nameof(r.Numbers));
        r.Relations = ReadTable(set, ToRelation, nameof(r.Relations));
        r.Strings = ReadTable(set, ToString, nameof(r.Strings));
        r.Descriptors = ReadTable(set, ToDescriptor, nameof(r.Descriptors));
        r.Documents = ReadTable(set, ToDocument, nameof(r.Documents));
        r.Entities = ReadTable(set, ToEntity, nameof(r.Entities));
        return r;
    }

    public static int ToInt(this StringIndex self) => (int)self;
    public static int ToInt(this EntityIndex self) => (int)self;
    public static int ToInt(this DocumentIndex self) => (int)self;
    public static int ToInt(this RelationIndex self) => (int)self;
    public static int ToInt(this PointIndex self) => (int)self;
    public static int ToInt(this DescriptorIndex self) => (int)self;

    public static IEnumerable<EntityIndex> GetCategories(this IBimData self)
        => self.Entities.Select(e => e.Category).Distinct();
    
    public static IEnumerable<string> GetCategoryNames(this IBimData self)
        => self.GetCategories().Select(self.GetEntityName).OrderBy(x => x);

    public static IEnumerable<EntityIndex> GetTypes(this IBimData self)
        => self.Entities.Select(e => e.Type).Distinct();

    public static IEnumerable<string> GetTypeNames(this IBimData self)
        => self.GetTypes().Select(self.GetEntityName).OrderBy(x => x);

    public static IEnumerable<string> GetDescriptorNames(this IBimData self)
        => self.Descriptors.Select(x => self.Get(x.Name)).OrderBy(x => x);

    public static string GetDiagnosticString(this IBimData self, Diagnostic d)
        => $"[{d.Type}] {d.Message}";

    public static IEnumerable<string> GetDiagnosticStrings(this IBimData self)
        => self.Diagnostics.Select(self.GetDiagnosticString);

    //==
    // Entity and EntityIndex helpers 

    public static string Name(this IBimData self, Entity? entity)
        => entity.HasValue ? self.Get(entity.Value.Name) : "";

    public static string Name(this IBimData self, EntityIndex index)
        => self.Name(self.Get(index));

    public static EntityIndex CategoryIndex(this IBimData self, EntityIndex index)
        => self.Get(index)?.Category ?? InvalidEntityIndex;

    public static Entity? Category(this IBimData self, EntityIndex index)
        => self.Get(self.CategoryIndex(index));

    public static string CategoryName(this IBimData self, EntityIndex index)
        => self.Name(self.Category(index));

    public static EntityIndex TypeIndex(this IBimData self, Entity? entity)
        => entity.HasValue ? self.TypeIndex(entity.Value) : InvalidEntityIndex;

    public static EntityIndex TypeIndex(this IBimData self, EntityIndex index)
        => self.Get(index)?.Type ?? InvalidEntityIndex;

    public static Entity? Type(this IBimData self, EntityIndex index)
        => self.Get(self.TypeIndex(index));

    public static Entity? Type(this IBimData self, Entity? entity)
        => entity.HasValue ? self.Type(entity.Value) : null;

    public static string TypeName(this IBimData self, EntityIndex index)
        => self.Name(self.Type(index));

    public static int DocumentIndex(this IBimData self, EntityIndex index)
        => (int?)self.Get(index)?.Document ?? -1;

    //==
    // InstanceStruct helpers

    public static Entity? Entity(this IBimData self, InstanceStruct inst)
        => self.Get((EntityIndex)inst.EntityIndex);

    public static string Name(this IBimData self, InstanceStruct inst)
        => self.Name(self.Entity(inst));

    public static Entity? Category(this IBimData self, InstanceStruct inst)
        => self.Category((EntityIndex)inst.EntityIndex);

    public static string CategoryName(this IBimData self, InstanceStruct inst)
        => self.Name(self.Category(inst));

    public static Entity? Type(this IBimData self, InstanceStruct inst)
        => self.Type(self.Entity(inst));

    public static string TypeName(this IBimData self, InstanceStruct inst)
        => self.Name(self.Type(inst));

    public static DocumentIndex DocumentIndex(this IBimData self, InstanceStruct inst)
        => self.Entity(inst)?.Document ?? (DocumentIndex)(-1);

    public static Document? Document(this IBimData self, InstanceStruct inst)
        => self.Get(self.DocumentIndex(inst));

    public static StringIndex DocumentTitleIndex(this IBimData self, InstanceStruct inst)
        => self.Document(inst)?.Title ?? (StringIndex)(-1);

    public static StringIndex DocumentPathIndex(this IBimData self, InstanceStruct inst)
        => self.Document(inst)?.Path ?? (StringIndex)(-1);

    //==
    // Parameter helpers

    public static ParameterDescriptor? Descriptor(this IBimData self, Parameter p)
        => self.Get(p.Descriptor);

    public static string ParameterName(this IBimData self, Parameter p)
        => self.Get(self.Descriptor(p)?.Name ?? InvalidStringIndex);
}