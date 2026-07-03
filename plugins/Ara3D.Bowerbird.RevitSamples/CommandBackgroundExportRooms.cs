using Ara3D.Utils;
using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Architecture;
using Autodesk.Revit.UI;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;

namespace Ara3D.Bowerbird.RevitSamples
{
    public class CommandBackgroundExportRooms : NamedCommand
    {
        public BosBackgroundExporterForm BosForm;
        public List<long> Ids = new();
        public int QueueSize; 
        public override string Name => "Background Export Rooms";
        public RoomUpdater RoomUpdater;

        public static Guid AddInGuid = new ("8F4295CE-00A0-4E39-A64E-412DD8F2814B");

        public override void Execute(object arg)
        {
            BosForm = new BosBackgroundExporterForm();
            BosForm.FormClosing += BosFormOnFormClosing;
            BosForm.Show();

            var uiapp = arg as UIApplication; 
            Doc = uiapp?.ActiveUIDocument?.Document;
            Dir.Create();
            Ids.Clear();
            RoomUpdater = new RoomUpdater(new AddInId(AddInGuid), OnChangeElements);
            RoomUpdater.RegisterForRoomChanges();
            Processor = new BackgroundProcessor<long>(ProcessElementById, uiapp);
            Processor.ExecuteNextIdleImmediately = false;
            Processor.DoWorkDuringIdle = true;
            Processor.OnHeartbeat += ProcessorOnHeartbeat;
            Processor.OnIdle += ProcessorOnIdle;
            BosForm.OnReset += BosFormOnOnReset;
        }

        public void OnChangeElements(IEnumerable<ElementId> ids)
        {
            EnqueueRooms();
        }

        private void BosFormOnFormClosing(object sender, FormClosingEventArgs e)
        {
            Processor.Dispose();
            BosForm = null;
        }

        private void BosFormOnOnReset(object sender, EventArgs e)
        {
            EnqueueRooms();
        }

        public void EnqueueRooms()
        {
            EnqueueWork(Doc.GetRooms().Select(r => r.Id));
        }

        public void EnqueueWork(IEnumerable<ElementId> elements)
        {
            var tmp = elements.Select(e => e.Value).ToList();
            Processor.EnqueueWork(tmp);
            BosForm?.SetQueueSize(QueueSize += tmp.Count);
        }

        private void ProcessorOnHeartbeat(object sender, EventArgs e)
        {
            BosForm?.SetHeartBeat();
        }

        private void ProcessorOnIdle(object sender, EventArgs e)
        {
            BosForm?.SetIdle();
        }

        public void ProcessElementById(long id)
        {
            using var el = Doc.GetElement(new ElementId(id));
            if (el is Room room)
            {
                BosForm?.SetId(id.ToString());
                BosForm?.SetLastProcess();
                BosForm?.SetQueueSize(--QueueSize);

                var rd = room.ToImdf();
                var json = JsonConvert.SerializeObject(rd, Formatting.Indented);
                var fileName = GetFileName(el);
                var filePath = Dir.RelativeFile(fileName);
                filePath.WriteAllText(json);
            }
        }

        public static string GetFileName(Element e)
            //=> $"{e.Name.ToIdentifier()}({e.Id}){DateTime.Now.ToTimeStamp()}.json";
            => $"{e.Name.ToIdentifier()}({e.Id}).json";

        public class ElementData
        {
            public long Id { get; set; }
            public string Name { get; set; }
        }

        public Document Doc;

        public BackgroundProcessor<long> Processor;

        public DirectoryPath Dir => @"C:\Users\cdigg\data\temp";
    }
}
