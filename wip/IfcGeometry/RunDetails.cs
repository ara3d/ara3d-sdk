using System.Diagnostics;
using Ara3D.IfcLoader;
using Ara3D.Logging;
using Ara3D.Utils;

namespace Ara3D.IfcGeometry
{
    public class RunDetails
    {
        public RunDetails(FilePath filePath)
        {
            FilePath = filePath;
            StartingMemory = GC.GetTotalMemory(true);
            EndingMemory = StartingMemory;
        }

        public void Stop()
        {
            EndingMemory = GC.GetTotalMemory(true);
            Stopwatch.Stop();
        }

        public Stopwatch Stopwatch = Stopwatch.StartNew();
        public readonly FilePath FilePath;
        public int RunIndex { get; set; }
        public int ExpressIds { get; set; }
        public string FileName => FilePath.GetFileName();
        public long FileSize => FilePath.GetFileSize();
        public double FileSizeInMb => (double)FileSize / (1024 * 1024);
        public string FileSizeString => $"{FileSizeInMb:00.00}";
        public Exception? Exception { get; set; }
        public bool Success => Exception == null;
        public int Geometries { get; set; }
        public int IfcGraphNodes { get; set; }
        public int IfcGraphRelations { get; set; }
        public long StartingMemory { get; set; }
        public long EndingMemory { get; set; }
        public long MemoryConsumption => EndingMemory - StartingMemory;
        public string MemoryConsumptionString => PathUtil.BytesToString(MemoryConsumption);
        public TimeSpan Elapsed => Stopwatch.Elapsed; 
        public string ElapsedTimeString => Elapsed.ToFixedWidthTimeStamp();

        public string RowData()
            => $"{FileName}, " +
                $"{FileSizeString}MB, " +
                $"{Success}, " +
                $"{Exception?.Message}, " +
                $"{ElapsedTimeString}, " +
                $"{ExpressIds}, " +
                $"{Geometries}, " +
                $"{IfcGraphNodes}, " +
                $"{IfcGraphRelations}, " +
                $"{MemoryConsumptionString}, " +
                $"{RunIndex}";
        
        public string Header()
            => "File Name, " +
               "File size, " +
               "Success, " +
               "Error, " +
               "Elapsed time, " +
               "# Ids, " +
               "# Geometries, " +
               "# Graph nodes, " +
               "# Relations, " +
               "Memory consumption, " +
               "Run index";

        public static (RunDetails, IfcFile) LoadGraph(FilePath fp, bool includeGeometry, ILogger logger = null)
        {
            var r = new RunDetails(fp);
            var file = IfcFile.Load(fp, includeGeometry, logger);
            r.Stop();
            r.ExpressIds = file.Document.RawInstances.Length;
            r.IfcGraphNodes = file.Graph?.Nodes.Count ?? 0;
            r.IfcGraphRelations = file.Graph?.Relations.Count ?? 0;
            return (r, file);
        }
    }
}
