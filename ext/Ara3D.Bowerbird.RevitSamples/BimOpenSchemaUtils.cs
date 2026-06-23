using Ara3D.Utils;
using System.IO;
using System.IO.Compression;
using Ara3D.BimOpenSchema;
using Ara3D.BimOpenSchema.IO;
using Ara3D.Collections;
using Ara3D.Logging;
using Autodesk.Revit.DB;
using Parquet;
using Document = Autodesk.Revit.DB.Document;
using FilePath = Ara3D.Utils.FilePath;
using Material = Ara3D.Models.Material;

namespace Ara3D.Bowerbird.RevitSamples
{
    public static class BimOpenSchemaUtils
    {
        public static bool IsVis(Geometry g)
        {
            var demoPhase = g.Element.DemolishedPhaseId;
            if (demoPhase != ElementId.InvalidElementId)
                return false;
            var cat = g.Element.Category;
            if (cat == null) 
                return false;
            if (cat.CategoryType != CategoryType.Model)
                return false;
            if (g.Element is SpatialElement)
                return false;
            return true;
        }

        public static Material DefaultMaterial = new Material(new(0.8f, 0.8f, 0.8f, 1), 0.05f, 0.3f);

        public static void BuildGeometry(this BosMeshGatherer meshGatherer, BimGeometryBuilder builder)
        {
            var meshOffset = builder.Meshes.Count;
            builder.Meshes.AddRange(meshGatherer.MeshList.Select(m => m.ToAra3D()));

            var geometries = meshGatherer.Geometries;
            foreach (var g in geometries)
            {
                if (g == null)
                    continue;

                var isHidden = IsVis(g) ? (byte)0 : (byte)1;
                var defaultMatIndex = builder.AddMaterial(g.DefaultMaterial ?? DefaultMaterial);
                foreach (var part in g.Parts)
                {
                    var matIndex = part.Material == null    
                        ? defaultMatIndex 
                        : builder.AddMaterial(part.Material.Value);

                    var transformIndex = builder.AddTransform(part.Transform.ToAra3D());
                    builder.AddInstance((int)g.EntityIndex, matIndex, part.MeshIndex + meshOffset, transformIndex, isHidden);
                }
            }
        }

        public static FilePath ExportBimOpenSchema(this BosRevitBuilder bosRevitBuilder, BimOpenSchemaExportSettings settings, ILogger logger)
        {
            logger.Log($"Creating BIM data");
            var dataSet = bosRevitBuilder.BimDataBuilder.Build().ToDataSet();

            logger.Log($"Creating FileStream");
            var fp = bosRevitBuilder.InputPath.ChangeDirectoryAndExt(settings.Folder, "bos");
            var fs = new FileStream(fp, FileMode.Create, FileAccess.Write, FileShare.None);

            logger.Log($"Creating Zip Archive");
            using var zip = new ZipArchive(fs, ZipArchiveMode.Create, leaveOpen: false);

            var parquetCompressionMethod = CompressionMethod.Brotli;
            var parquetCompressionLevel = CompressionLevel.Optimal;
            var zipCompressionLevel = CompressionLevel.Fastest;

            logger.Log($"Writing non-geometry data to Zip file {fp}");
            dataSet.WriteParquetToZip(zip,
                parquetCompressionMethod,
                parquetCompressionLevel,
                zipCompressionLevel);

            if (settings.IncludeGeometry)
            {
                logger.Log($"Writing BIM geometry");
                var bg = bosRevitBuilder.GeometryBuilder.BuildModel();
                bg.WriteParquetToZip(zip, parquetCompressionMethod, parquetCompressionLevel, zipCompressionLevel);
            }

            logger.Log($"Finished writing to {fp}");
            return fp;
        }

        public static FilePath ExportBimOpenSchema(this Document currentDoc, BimOpenSchemaExportSettings settings, ILogger logger)
        {
            logger.Log($"Exporting BIM Open Schema Parquet Files");

            var options = new Options()
            {
                ComputeReferences = false,
                DetailLevel = (ViewDetailLevel)settings.DetailLevel,
            };

            logger.Log($"Create Revit builder and gather links");
            var bosRevitBuilder = new BosRevitBuilder(options, settings, currentDoc, e => true);
            return ExportBimOpenSchema(bosRevitBuilder, settings, logger);
        }

        public static void BuildGeometry(this BosRevitBuilder self)
            => self.MeshGatherer.BuildGeometry(self.GeometryBuilder);
    }
}
