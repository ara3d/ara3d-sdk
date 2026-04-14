using System.Collections.Generic;
using System.Linq;
using Ara3D.DataTable;
using Ara3D.Geometry;
using Ara3D.Models;
using Ara3D.Utils;

namespace Ara3D.BimOpenSchema;

public static class BimGeometryExtensions
{
    public static int GetNumMaterials(this BimGeometry self) => self.MaterialRed.Length;
    public static int GetNumVertices(this BimGeometry self) => self.VertexX.Length;
    public static int GetNumFaces(this BimGeometry self) => self.GetNumIndices() / 3;
    public static int GetNumIndices(this BimGeometry self) => self.IndexBuffer.Length;
    public static int GetNumTransforms(this BimGeometry self) => self.TransformTX.Length;
    public static int GetNumMeshes(this BimGeometry self) => self.MeshIndexOffset.Length;
    public static int GetNumInstances(this BimGeometry self) => self.InstanceMeshIndex.Length;

    public static IReadOnlyList<Point3D> GetMeshPoints(this BimGeometry self, int meshIndex)
    {
        var r = new List<Point3D>();

        var vertexOffset = self.MeshVertexOffset[meshIndex];
        var nextVertexOffset = meshIndex < self.GetNumMeshes() - 1
            ? self.MeshVertexOffset[meshIndex + 1] 
            : self.GetNumVertices();
        var vertexCount = nextVertexOffset - vertexOffset;

        for (var i = 0; i < vertexCount; i++)
        {
            var vertexBufferOffset = i + vertexOffset;
            var x = self.VertexX[vertexBufferOffset] / BimGeometry.VertexMultiplier;
            var y = self.VertexY[vertexBufferOffset] / BimGeometry.VertexMultiplier;
            var z = self.VertexZ[vertexBufferOffset] / BimGeometry.VertexMultiplier;
            r.Add((x, y, z));
        }

        return r;
    }

    public static IReadOnlyList<Integer3> GetMeshFaceIndices(this BimGeometry self, int meshIndex)
    {
        var r = new List<Integer3>();

        var indexOffset = self.MeshIndexOffset[meshIndex];
        
        var nextIndexOffset = meshIndex < self.GetNumMeshes() - 1
            ? self.MeshIndexOffset[meshIndex + 1]
            : self.IndexBuffer.Length;

        var indexCount = nextIndexOffset - indexOffset;

        for (var i = 0; i < indexCount; i += 3)
        {
            var a = self.IndexBuffer[i + indexOffset + 0];
            var b = self.IndexBuffer[i + indexOffset + 1];
            var c = self.IndexBuffer[i + indexOffset + 2];
            r.Add((a, b, c));
        }

        return r;
    }

    public static TriangleMesh3D GetMesh(this BimGeometry self, int meshIndex)
        => (self.GetMeshPoints(meshIndex), self.GetMeshFaceIndices(meshIndex));

    public static Color GetColor(this BimGeometry self, int materialIndex)
        => new(
            self.MaterialRed[materialIndex].ToNormalizedFloat(), 
            self.MaterialGreen[materialIndex].ToNormalizedFloat(), 
            self.MaterialBlue[materialIndex].ToNormalizedFloat(), 
            self.MaterialAlpha[materialIndex].ToNormalizedFloat());

    public static Material GetMaterial(this BimGeometry self, int materialIndex, Material defaultMaterial)
        => materialIndex < 0 ? defaultMaterial : 
            new(self.GetColor(materialIndex), 
            self.MaterialMetallic[materialIndex].ToNormalizedFloat(),
            self.MaterialRoughness[materialIndex].ToNormalizedFloat());

    public static InstanceStruct GetInstanceStruct(this BimGeometry self, int instanceIndex)
        => new(
            self.InstanceEntityIndex[instanceIndex],    
            self.GetInstanceMatrix(instanceIndex), 
            self.InstanceMeshIndex[instanceIndex], 
            self.GetInstanceMaterial(instanceIndex),
            self.InstanceFlags[instanceIndex]);

    public static Material GetMaterial(this BimGeometry self, int materialIndex)
        => new(
            self.GetColor(materialIndex), 
            self.MaterialMetallic[materialIndex].ToNormalizedFloat(),
            self.MaterialRoughness[materialIndex].ToNormalizedFloat());

    public static Material GetInstanceMaterial(this BimGeometry self, int elementIndex)
        => self.GetMaterial(self.InstanceMaterialIndex[elementIndex]);

    public static Matrix4x4 GetInstanceMatrix(this BimGeometry self, int elementIndex)
        => new(self.GetTransformMatrix(self.InstanceTransformIndex[elementIndex]));

    public static Matrix4x4 GetTranslationMatrix(this BimGeometry self, int i)
        => Matrix4x4.CreateTranslation(self.GetTranslation(i));

    public static Vector3 GetTranslation(this BimGeometry self, int i)
        => new(self.TransformTX[i], self.TransformTY[i], self.TransformTZ[i]);

    public static Vector3 GetScale(this BimGeometry self, int i)
        => new(self.TransformSX[i], self.TransformSY[i], self.TransformSZ[i]);

    public static Matrix4x4 GetScaleMatrix(this BimGeometry self, int i)
        => Matrix4x4.CreateScale(self.TransformSX[i], self.TransformSY[i], self.TransformSZ[i]);

    public static Quaternion GetRotation(this BimGeometry self, int i)
        => new(self.TransformQX[i], self.TransformQY[i], self.TransformQZ[i], self.TransformQW[i]);

    public static Matrix4x4 GetRotationMatrix(this BimGeometry self, int i)
        => Matrix4x4.CreateFromQuaternion(self.GetRotation(i));

    public static Matrix4x4 GetTransformMatrix(this BimGeometry self, int i)
        => self.GetScaleMatrix(i) * self.GetRotationMatrix(i) * self.GetTranslationMatrix(i);

    public static Model3D ToModel3D(this BimGeometry self)
    {
        var meshes = self.GetNumMeshes().MapRange(i => self.GetMesh(i)).ToList();
        var instances = self.GetNumInstances().MapRange(i => self.GetInstanceStruct(i)).ToList();
        return new Model3D(meshes, instances);
    }

    public static MeshSliceStruct GetMeshSlice(this BimGeometry self, int i)
    {
        var currVertex = self.MeshVertexOffset[i];
        var currIndex = self.MeshIndexOffset[i];
        var nextVertex = i < self.GetNumMeshes() - 1 ? self.MeshVertexOffset[i + 1] : self.GetNumVertices();
        var nextIndex = i < self.GetNumMeshes() - 1 ? self.MeshIndexOffset[i + 1] : self.GetNumIndices();
        var indexCount = nextIndex - currIndex;
        var vertexCount = nextVertex - currVertex;
        return new MeshSliceStruct()
            { BaseVertex = currVertex, FirstIndex = (uint)currIndex, IndexCount = (uint)indexCount, VertexCount = vertexCount };
    }

    public static RenderModelData ToRenderModelData(this BimGeometry self)
        => CopyToRenderModelData(self, new RenderModelData(3));

    public static RenderModelData CopyToRenderModelData(this BimGeometry self, RenderModelData data)
    {
        data.VertexBuffer.SetCount(self.GetNumVertices() * 3);
        for (int i = 0, n = self.GetNumVertices(); i < n; i++)
        {
            data.VertexBuffer[i * 3] = self.VertexX[i] /  BimGeometry.VertexMultiplier;
            data.VertexBuffer[i * 3 + 1] = self.VertexY[i] / BimGeometry.VertexMultiplier;
            data.VertexBuffer[i * 3 + 2] = self.VertexZ[i] / BimGeometry.VertexMultiplier;
        }

        data.IndexBuffer.SetCount(self.GetNumIndices());
        for (int i = 0, n = self.GetNumIndices(); i < n; i++)
        {
            data.IndexBuffer[i] = (uint)self.IndexBuffer[i];
        }

        data.InstanceBuffer.SetCount(self.GetNumInstances());
        for (int i = 0, n = self.GetNumInstances(); i < n; i++)
        {
            data.InstanceBuffer[i] = GetInstanceStruct(self, i);
        }

        data.MeshSliceBuffer.SetCount(self.GetNumMeshes());
        for (int i = 0, n = self.GetNumMeshes(); i < n; i++)
        {
            data.MeshSliceBuffer[i] = GetMeshSlice(self, i);
        }

        data.ValidateMeshSlices();
        data.ComputeBounds();

        return data;
    }

    public static DataTableBuilder AddColumn<T>(this DataTableBuilder self, BimGeometry model, T[] data, string name)
    {
        self.AddColumn(data, name);
        return self;
    }

    public static IDataSet ToDataSet(this BimGeometry self)
    {
        var r = new DataSetBuilder();
        r.AddTable(BimGeometry.MaterialTableName)
            .AddColumn(self, self.MaterialRed, nameof(self.MaterialRed))
            .AddColumn(self, self.MaterialGreen, nameof(self.MaterialGreen))
            .AddColumn(self, self.MaterialBlue, nameof(self.MaterialBlue))
            .AddColumn(self, self.MaterialAlpha, nameof(self.MaterialAlpha))
            .AddColumn(self, self.MaterialMetallic, nameof(self.MaterialMetallic))
            .AddColumn(self, self.MaterialRoughness, nameof(self.MaterialRoughness));

        r.AddTable(BimGeometry.TransformTableName)
            .AddColumn(self, self.TransformTX, nameof(self.TransformTX))
            .AddColumn(self, self.TransformTY, nameof(self.TransformTY))
            .AddColumn(self, self.TransformTZ, nameof(self.TransformTZ))
            .AddColumn(self, self.TransformQX, nameof(self.TransformQX))
            .AddColumn(self, self.TransformQY, nameof(self.TransformQY))
            .AddColumn(self, self.TransformQZ, nameof(self.TransformQZ))
            .AddColumn(self, self.TransformQW, nameof(self.TransformQW))
            .AddColumn(self, self.TransformSX, nameof(self.TransformSX))
            .AddColumn(self, self.TransformSX, nameof(self.TransformSY))
            .AddColumn(self, self.TransformSX, nameof(self.TransformSZ));

        r.AddTable(BimGeometry.VertexTableName)
            .AddColumn(self, self.VertexX, nameof(self.VertexX))
            .AddColumn(self, self.VertexY, nameof(self.VertexY))
            .AddColumn(self, self.VertexZ, nameof(self.VertexZ));

        r.AddTable(BimGeometry.IndexTableName)
            .AddColumn(self, self.IndexBuffer, nameof(self.IndexBuffer));

        r.AddTable(BimGeometry.InstanceTableName)
            .AddColumn(self, self.InstanceEntityIndex, nameof(self.InstanceEntityIndex))
            .AddColumn(self, self.InstanceMaterialIndex, nameof(self.InstanceMaterialIndex))
            .AddColumn(self, self.InstanceMeshIndex, nameof(self.InstanceMeshIndex))
            .AddColumn(self, self.InstanceTransformIndex, nameof(self.InstanceTransformIndex))
            .AddColumn(self, self.InstanceFlags, nameof(self.InstanceFlags));

        r.AddTable(BimGeometry.MeshTableName)
            .AddColumn(self, self.MeshIndexOffset, nameof(self.MeshIndexOffset))
            .AddColumn(self, self.MeshVertexOffset, nameof(self.MeshVertexOffset));

        return r;
    }
    
    public static BimGeometry ToBimGeometry(this Model3D self)
    {
        var bgb = new BimGeometryBuilder();
        bgb.AddMeshes(self.Meshes);

        foreach (var inst in self.Instances)
        {
            var materialIndex = bgb.AddMaterial(inst.Material);
            var transformIndex = bgb.AddTransform(inst.Matrix4x4);
            bgb.AddInstance(inst.EntityIndex, materialIndex, inst.MeshIndex, transformIndex, inst.Flags);
        }

        return bgb.BuildModel();
    }

    public static IDataTable GetTable(this IDataSet self, BimGeometryTableName name)
        => self.GetTable(name.ToString());

    public static T[] ReadColumn<T>(this IDataSet set, BimGeometryTableName tableName, string columnName)
        => set.GetTable(tableName).GetColumn(columnName)?.GetTypedValues<T>();

    public static BimGeometry ToBimGeometry(this IDataSet self)
    {
        // TODO: this needs to be replaced by directly talking to the Parquet data columns.
        //Debugger.Break();

        var r = new BimGeometry();
        r.MaterialRed = ReadColumn<byte>(self, BimGeometryTableName.Materials, nameof(r.MaterialRed));
        r.MaterialGreen = ReadColumn<byte>(self, BimGeometryTableName.Materials, nameof(r.MaterialGreen));
        r.MaterialBlue = ReadColumn<byte>(self, BimGeometryTableName.Materials, nameof(r.MaterialBlue));
        r.MaterialAlpha = ReadColumn<byte>(self, BimGeometryTableName.Materials, nameof(r.MaterialAlpha));
        r.MaterialMetallic = ReadColumn<byte>(self, BimGeometryTableName.Materials, nameof(r.MaterialMetallic));
        r.MaterialRoughness = ReadColumn<byte>(self, BimGeometryTableName.Materials, nameof(r.MaterialRoughness));

        r.TransformTX = ReadColumn<float>(self, BimGeometryTableName.Transforms, nameof(r.TransformTX));
        r.TransformTY = ReadColumn<float>(self, BimGeometryTableName.Transforms, nameof(r.TransformTY));
        r.TransformTZ = ReadColumn<float>(self, BimGeometryTableName.Transforms, nameof(r.TransformTZ));
        r.TransformQX = ReadColumn<float>(self, BimGeometryTableName.Transforms, nameof(r.TransformQX));
        r.TransformQY = ReadColumn<float>(self, BimGeometryTableName.Transforms, nameof(r.TransformQY));
        r.TransformQZ = ReadColumn<float>(self, BimGeometryTableName.Transforms, nameof(r.TransformQZ));
        r.TransformQW = ReadColumn<float>(self, BimGeometryTableName.Transforms, nameof(r.TransformQW));
        r.TransformSX = ReadColumn<float>(self, BimGeometryTableName.Transforms, nameof(r.TransformSX));
        r.TransformSY = ReadColumn<float>(self, BimGeometryTableName.Transforms, nameof(r.TransformSY));
        r.TransformSZ = ReadColumn<float>(self, BimGeometryTableName.Transforms, nameof(r.TransformSZ));

        r.VertexX = ReadColumn<int>(self, BimGeometryTableName.VertexBuffer, nameof(r.VertexX));
        r.VertexY = ReadColumn<int>(self, BimGeometryTableName.VertexBuffer, nameof(r.VertexY));
        r.VertexZ = ReadColumn<int>(self, BimGeometryTableName.VertexBuffer, nameof(r.VertexZ));
        r.IndexBuffer = ReadColumn<int>(self, BimGeometryTableName.IndexBuffer, nameof(r.IndexBuffer));

        r.InstanceEntityIndex = ReadColumn<int>(self, BimGeometryTableName.Instances, nameof(r.InstanceEntityIndex));
        r.InstanceMaterialIndex = ReadColumn<int>(self, BimGeometryTableName.Instances, nameof(r.InstanceMaterialIndex));
        r.InstanceMeshIndex = ReadColumn<int>(self, BimGeometryTableName.Instances, nameof(r.InstanceMeshIndex));
        r.InstanceTransformIndex = ReadColumn<int>(self, BimGeometryTableName.Instances, nameof(r.InstanceTransformIndex));

        // Flags are not present on older files
        var numInstances = r.InstanceTransformIndex.Length;
        r.InstanceFlags = ReadColumn<byte>(self, BimGeometryTableName.Instances, nameof(r.InstanceFlags)) ?? new byte[numInstances];

        r.MeshIndexOffset = ReadColumn<int>(self, BimGeometryTableName.Meshes, nameof(r.MeshIndexOffset));
        r.MeshVertexOffset = ReadColumn<int>(self, BimGeometryTableName.Meshes, nameof(r.MeshVertexOffset));

        return r;
    }

    public static BimModel3D ToModel3D(this IBimData self, bool computeParametersAndRelations)
        => BimModel3D.Create(self, computeParametersAndRelations);

    public static Instance GetElement(this BimGeometry self, int i)
        => new(
            self.InstanceEntityIndex[i], 
            self.InstanceMaterialIndex[i], 
            self.InstanceMeshIndex[i],
            self.InstanceTransformIndex[i],
            self.InstanceFlags[i]); 
}