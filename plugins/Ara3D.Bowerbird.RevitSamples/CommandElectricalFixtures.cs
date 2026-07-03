using System.Linq;
using Autodesk.Revit.UI;

namespace Ara3D.Bowerbird.RevitSamples
{
    public class CommandElectricalFixtures : NamedCommand
    {
        public string Name => "Electrical Fixtures";

        public override void Execute(object arg)
        {
            var doc = (arg as UIApplication)?.ActiveUIDocument?.Document;
            var sockets = doc.GetSockets();
            var text = string.Join("\r\n", sockets.Select(s => s.GetRoomId().ToString()));
            TextDisplayForm.DisplayText(text);
        }
    }
}