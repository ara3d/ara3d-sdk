﻿using System.Linq;

namespace Ara3D.Studio.Samples
{
    public class GeometryStats : IModelModifier
    {
        public class MeshStats
        {
            public float Ratio { get; set; }
            public float BoundingVolume { get; set; }
            public float MinBindingSide { get; set; }
            public float MaxBoundingSide { get; set; }
            public float SurfaceArea { get; set; }
            public int NumTriangles { get; set; }
            public int NumPoints { get; set; }
        }

        public static MeshStats GetMeshStats(TriangleMesh3D mesh)
        {
            var bounds = mesh.Bounds;
            var extent = bounds.Size;
            return new MeshStats
            {
                NumPoints = mesh.Points.Count,
                BoundingVolume = extent.X * extent.Y * extent.Z,
                NumTriangles = mesh.Triangles.Count,
                MinBindingSide = extent.MinComponent,
                MaxBoundingSide = extent.MaxComponent,
                Ratio = extent.MinComponent / extent.MaxComponent,
        		SurfaceArea = mesh.Triangles.Aggregate(0f, (sum, t) => sum + t.Area)
            };
        }

        public Model3D Eval(Model3D model, EvalContext context)
        {
            var table = new DataTableBuilder("Meshes");
            var stats = model.Meshes.Select(GetMeshStats);
            table.AddColumnsFromFieldsAndProperties(stats);
            return model.MergeTable(table);
        }
    }
}
